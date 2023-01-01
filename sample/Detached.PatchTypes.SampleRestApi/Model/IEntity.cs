namespace Detached.PatchTypes.SampleRestApi.Model
{
    public interface IEntity : IHasId, IHasAuditory, IHasTracking
    {
        string Name { get; set; }
    }
}
