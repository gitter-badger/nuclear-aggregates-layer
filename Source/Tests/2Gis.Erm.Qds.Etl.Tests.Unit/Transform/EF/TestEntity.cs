using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    public class TestEntity : IEntityKey, IEntity
    {
        public string PropertyOne { get; set; }
        public string PropertyTwo { get; set; }
        public long Id { get; set; }
    }
}