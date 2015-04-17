using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class ClientGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string MainAddress { get; set; }
        public string TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public string OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public bool IsAdvertisingAgency { get; set; }

        public string MainFirmId { get; set; }
        public string MainFirmName { get; set; }
        public string MainPhoneNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastQualifyTime { get; set; }
        public DateTime? LastDisqualifyTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public InformationSource InformationSourceEnum { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}