using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListBargainDto : IListItemEntityDto<Bargain>
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CustomerLegalPersonId { get; set; }
        public long BranchOfficeId { get; set; }
        public string Number { get; set; }
        public string CustomerLegalPersonLegalName { get; set; }
        public string BranchOfficeName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long OrderCode { get; set; }
        public string LegalAddress { get; set; }
    }
}