using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms
{
    public class PrintDocumentRequest: Request
    {
        public object PrintData { get; set; }
        public TemplateCode TemplateCode { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
        public string FileName { get; set; }
        public short CurrencyIsoCode { get; set; }
    }
}
