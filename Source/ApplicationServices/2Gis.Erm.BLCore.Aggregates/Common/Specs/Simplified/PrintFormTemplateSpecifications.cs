using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified
{
    public class PrintFormTemplateSpecifications
    {
        public static class Find
        {
            public static FindSpecification<PrintFormTemplate> ById(long id)
            {
                return new FindSpecification<PrintFormTemplate>(x => x.Id == id);
            }
        }
    }
}
