using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Dictionary
{
    public static class CategorySpecifications
    {
        public static class Find
        {
            public static FindSpecification<CategoryGroup> CategoryGroupById(long id)
            {
                return new FindSpecification<CategoryGroup>(x => x.Id == id);
            }
        }
    }
}
