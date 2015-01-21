using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCountry : EntityTypeBase<EntityTypeCountry>
    {
        public override int Id
        {
            get { return EntityTypeIds.Country; }
        }

        public override string Description
        {
            get { return "Country"; }
        }
    }
}