using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Activity
{
    public sealed class Activity : IBaseEntity, IEntityKey
        , ICuratedEntity // TODO {s.pomadin, 11.08.2014}: it's a temporary solution as this fake is used to check condition for any activity still
    {
        public long Id { get; set; }

        public long OwnerCode { get; set; }
        public long? OldOwnerCode { get; private set; }
    }
}