using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    internal class AnotherTestEntity : IEntityKey
    {
        public string DataOne { get; set; }
        public string DataTwo { get; set; }
        public long Id { get; set; }
    }
}