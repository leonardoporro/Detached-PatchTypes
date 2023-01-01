using Detached.PatchTypes;
using System;
using Xunit;

namespace Detached.Mappers.Tests.Patching
{
    public class InterfacePatchTests
    {
        [Fact]
        public void create_interface_proxy()
        {
            IEntity entity = PatchTypeFactory.Create<IEntity>(); 

            entity.Name = "newName";
            entity.ModifiedOn = DateTime.Now;

            IPatch entityChanges = (IPatch)entity;

            Assert.True(entityChanges.IsSet("Name"));
            Assert.False(entityChanges.IsSet("ModifiedOn"));

            entityChanges.Reset();

            Assert.False(entityChanges.IsSet("Name"));
            Assert.False(entityChanges.IsSet("ModifiedOn"));
        }
    }

    public interface IHasId<TId>
    {
        TId Id { get; set; }
    }

    public interface IHasId : IHasId<int>
    {
    }

    public interface IHasTracking : IPatch
    {
    }

    public interface IHasAuditory
    {
        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }

    public interface IEntity : IHasId, IHasTracking, IHasAuditory
    {
        public string Name { get; set; }
    }
}
