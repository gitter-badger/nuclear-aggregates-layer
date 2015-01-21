using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeContributionType : EntityTypeBase<EntityTypeContributionType>
    {
        public override int Id
        {
            get { return EntityTypeIds.ContributionType; }
        }

        public override string Description
        {
            get { return "ContributionType"; }
        }
    }
}