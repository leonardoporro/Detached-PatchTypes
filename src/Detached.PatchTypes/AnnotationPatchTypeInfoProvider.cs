using Detached.PatchTypes.Annotations;
using System;
using System.Reflection;

namespace Detached.PatchTypes
{
    public class AnnotationPatchTypeInfoProvider : IPatchTypeInfoProvider
    {
        public bool ShouldPatch(Type type)
        {
            return !typeof(IPatch).IsAssignableFrom(type) && type.GetCustomAttribute<UsePatchAttribute>() != null;
        }
    }
}