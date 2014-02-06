using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Generic.List
{
    public sealed class ChileListClientDto : IListItemEntityDto<Client>, IChileAdapted
    {
        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public long? MainFirmId { get; set; }
        public string MainFirmName { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public long TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public string MainPhoneNumber { get; set; }
        public string MainAddress { get; set; }
        public DateTime LastQualifyTime { get; set; }
        public DateTime? LastDisqualifyTime { get; set; }
    }
}