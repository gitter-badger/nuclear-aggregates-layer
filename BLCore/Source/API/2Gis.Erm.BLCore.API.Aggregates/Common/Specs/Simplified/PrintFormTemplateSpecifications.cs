using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified
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