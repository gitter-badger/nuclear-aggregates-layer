using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListContactDto : IListItemEntityDto<Contact>
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public long ClientId { get; set; }
        public string Client { get; set; }
        public string JobTitle { get; set; }
        public string MainPhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string Website { get; set; }
        public string HomeEmail { get; set; }
        public string WorkEmail { get; set; }
        public long OwnerCode { get; set; }
        public string Owner { get; set; }
        public DateTime CreateDate { get; set; }
        public string WorkAddress { get; set; }
        public string AccountRole { get; set; }
    }
}