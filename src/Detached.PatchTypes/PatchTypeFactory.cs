﻿using Detached.RuntimeTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using static Detached.RuntimeTypes.Expressions.ExtendedExpression;
using static System.Linq.Expressions.Expression;

namespace Detached.PatchTypes
{
    public static class PatchTypeFactory
    {
        readonly static ConcurrentDictionary<Type, Type> _proxyTypes
            = new ConcurrentDictionary<Type, Type>();

        readonly static ConcurrentDictionary<Type, object> _typedFactories
           = new ConcurrentDictionary<Type, object>();

        readonly static ConcurrentDictionary<Type, Func<object>> _factories
           = new ConcurrentDictionary<Type, Func<object>>();

        public static TModel Create<TModel>()
        {
            return ((Func<TModel>)_typedFactories.GetOrAdd(typeof(TModel), type =>
            {
                Type patchType = GetType(typeof(TModel));
                return Lambda<Func<TModel>>(Convert(Expression.New(patchType), type)).Compile();
            }))();
        }

        public static object Create(Type type)
        {
            return _factories.GetOrAdd(type, t =>
            {
                Type patchType = GetType(t);
                return Lambda<Func<object>>(Convert(Expression.New(patchType), typeof(object))).Compile();
            })();
        }

        public static Type GetType(Type type)
        {
            return _proxyTypes.GetOrAdd(type, CreateType);
        }

        static Type CreateType(Type type)
        {
            if (type.IsInterface)
            {
                return CreateInterfaceProxy(type);
            }
            else
            {
                return CreateClassProxy(type);
            }
        }

        static Type CreateInterfaceProxy(Type type)
        {
            string typeName = $"{type.FullName}_Patch{Guid.NewGuid().ToString().Replace("-", "")}";
            RuntimeTypeBuilder proxyBuilder = new RuntimeTypeBuilder(typeName);

            FieldBuilder modified = proxyBuilder.DefineField("_modified", typeof(HashSet<string>), FieldAttributes.Private);
            var modifiedField = Field(proxyBuilder.This, modified);

            List<Type> ifaceTypes = new List<Type>(type.GetInterfaces()) { type };

            foreach (Type ifaceType in ifaceTypes)
            {
                foreach (PropertyInfo propInfo in ifaceType.GetRuntimeProperties())
                {
                    if (!proxyBuilder.Properties.ContainsKey(propInfo.Name) && propInfo.CanRead && propInfo.CanWrite)
                    {
                        var fieldInfo = proxyBuilder.DefineField("_" + propInfo.Name, propInfo.PropertyType, FieldAttributes.Private);
                        var field = Expression.Field(proxyBuilder.This, fieldInfo);

                        var value = Expression.Parameter(propInfo.PropertyType, "value");

                        proxyBuilder.DefineProperty(propInfo.Name,
                            propInfo.PropertyType,
                            field,
                            value,
                            Block(typeof(void), new Expression[]
                            {
                                If(IsNull(modifiedField),
                                    Assign(modifiedField, New(typeof(HashSet<string>)))
                                ),
                                Call("Add", modifiedField, Constant(propInfo.Name)),
                                Assign(field, value)
                            }));
                    }
                }
            }

            proxyBuilder.DefineMethod("Reset", null,
                Block(
                    If(IsNotNull(modifiedField),
                        Call("Clear", modifiedField)
                    )
                )
            );

            var propNameParam = Parameter(typeof(string), "propName");
            proxyBuilder.DefineMethod("IsSet",
                new[] { propNameParam },
                Block(
                    Variable("result", Constant(false), out Expression result),
                    If(IsNotNull(modifiedField),
                        Assign(result, Call("Contains", modifiedField, propNameParam))
                    ),
                    Result(result)
                )
            );

            proxyBuilder.AutoImplementInterface(type);

            proxyBuilder.AutoImplementInterface(typeof(IPatch));

            return proxyBuilder.Create();
        }

        static Type CreateClassProxy(Type type)
        {
            if (type.GetConstructor(new Type[0]) == null)
                throw new PatchProxyTypeException($"Type {type} doesn't have an empty constructor.");

            RuntimeTypeBuilder proxyBuilder = new RuntimeTypeBuilder($"{type.FullName}_Patch{Guid.NewGuid().ToString().Replace("-", "")}", type);

            FieldBuilder modified = proxyBuilder.DefineField("_modified", typeof(HashSet<string>), FieldAttributes.Private);
            var modifiedField = Field(proxyBuilder.This, modified);

            foreach (PropertyInfo propInfo in type.GetRuntimeProperties())
            {
                if (propInfo.CanRead && propInfo.CanWrite && propInfo.GetSetMethod().IsVirtual)
                {
                    ParameterExpression valueExpr = Parameter(propInfo.PropertyType, "value");
                     
                    proxyBuilder.OverrideMethod(propInfo.GetSetMethod(),
                        new[] { valueExpr },
                        Block(
                            If(IsNull(modifiedField),
                                Assign(modifiedField, New(typeof(HashSet<string>)))
                            ),
                            Call("Add", modifiedField, Constant(propInfo.Name)),
                            Call(proxyBuilder.Base, propInfo.SetMethod, valueExpr)
                        )
                    );
                }
            }

            proxyBuilder.DefineMethod("Reset", null,
                Block(
                    If(IsNotNull(modifiedField),
                        Call("Clear", modifiedField)
                    )
                )
            );

            var propNameParam = Parameter(typeof(string), "propName");
            proxyBuilder.DefineMethod("IsSet",
               new[] { propNameParam },
               Block(
                   Variable("result", Constant(false), out Expression result),
                   If(IsNotNull(modifiedField),
                       Assign(result, Call("Contains", modifiedField, propNameParam))
                   ),
                   Result(result)
               )
           );

            proxyBuilder.AutoImplementInterface(typeof(IPatch));

            return proxyBuilder.Create();
        }
    }
}
