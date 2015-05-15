using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBranchOffice : EntityTypeBase<EntityTypeBranchOffice>
    {
        public override int Id
        {
            get { return EntityTypeIds.BranchOffice; }
        }

        public override string Description
        {
            get { return "BranchOffice"; }
        }
    }
}