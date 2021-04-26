using System;

namespace Detached.PatchTypes.Annotations
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class UsePatchAttribute : Attribute
    {
    }
}