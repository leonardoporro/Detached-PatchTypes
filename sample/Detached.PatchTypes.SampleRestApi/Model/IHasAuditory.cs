using System;

namespace Detached.PatchTypes.SampleRestApi.Model
{
    public interface IHasAuditory
    {
        DateTime ModifiedOn { get; set; }

        string ModifiedBy { get; set; }
    }
}
