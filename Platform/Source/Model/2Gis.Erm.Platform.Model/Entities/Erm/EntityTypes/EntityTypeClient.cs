using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeClient : EntityTypeBase<EntityTypeClient>
    {
        public override int Id
        {
            get { return EntityTypeIds.Client; }
        }

        public override string Description
        {
            get { return "Client"; }
        }
    }
}