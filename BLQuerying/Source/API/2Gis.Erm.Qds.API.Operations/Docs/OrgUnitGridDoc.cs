using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class OrgUnitGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public int? DgppId { get; set; }
        public string ReplicationCode { get; set; }
        public string Name { get; set; }
        public DateTime FirstEmitDate { get; set; }
        public DateTime? ErmLaunchDate { get; set; }
        public DateTime? InfoRussiaLaunchDate { get; set; }
        public bool ErmLaunched { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // relations
        public string CountryId { get; set; }
        public string CountryName { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}