using System;

namespace Detached.PatchTypes
{
    public class PatchProxyTypeException : Exception
    {
        public PatchProxyTypeException(string message)
            : base(message)
        {
        }
    }
}