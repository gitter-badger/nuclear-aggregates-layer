using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListClientDto : IListItemEntityDto<Client>
    {
        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public string MainAddress { get; set; }
        public long TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
    }
}