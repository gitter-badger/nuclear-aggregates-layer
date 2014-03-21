using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List
{
    public sealed class ChileListLegalPersonDto : IChileAdapted, IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string LegalName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public string Rut { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public string LegalAddress { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
