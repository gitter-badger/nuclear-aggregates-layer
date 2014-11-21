using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Generic.List
{
    public sealed class EmiratesListLegalPersonDto : IOperationSpecificEntityDto, IEmiratesAdapted
    {
        public long Id { get; set; }
        public string LegalName { get; set; }
        public string CommercialLicense { get; set; }
        public string LegalAddress { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
