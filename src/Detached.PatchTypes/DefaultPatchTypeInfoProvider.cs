using System;
using System.Collections.Generic;

namespace Detached.PatchTypes
{
    public class DefaultPatchTypeInfoProvider : IPatchTypeInfoProvider
    {
        public virtual HashSet<Type> Primitives { get; } = new HashSet<Type>
        {
            typeof(bool),
            typeof(string),
            typeof(char),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
            typeof(byte),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Nullable<>),
            typeof(List<>),
            typeof(IEnumerable<>),
            typeof(IList<>),
            typeof(ICollection<>),
            typeof(Dictionary<,>),
            typeof(IDictionary<,>),
        };

        public virtual bool ShouldPatch(Type type)
        {
            if (typeof(IPatch).IsAssignableFrom(type) && !type.IsInterface)
                return false; // do not patch types already patched
            else if (type.IsEnum)
                return false; // do not patch enums
            else if (type.IsGenericType && Primitives.Contains(type.GetGenericTypeDefinition()))
                return false; // do not patch any generic type containted in the primitive list
            else
                return !Primitives.Contains(type); // finally, do not patch any contained in the primitive list
        }
    }
}