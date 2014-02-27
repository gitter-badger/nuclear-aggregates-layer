using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List
{
    public sealed class ChileListLegalPersonDto : IListItemEntityDto<LegalPerson>, IChileAdapted
    {
        public long Id { get; set; }
        public string LegalName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public string Rut { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public string LegalAddress { get; set; }
    }
}
