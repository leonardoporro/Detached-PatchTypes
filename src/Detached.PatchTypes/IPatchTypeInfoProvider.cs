using System;

namespace Detached.PatchTypes
{
    public interface IPatchTypeInfoProvider
    {
        bool ShouldPatch(Type type);
    }
}