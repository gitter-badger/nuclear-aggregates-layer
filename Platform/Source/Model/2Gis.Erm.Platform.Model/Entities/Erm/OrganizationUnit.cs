using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class OrganizationUnit :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IReplicableEntity,
        IStateTrackingEntity
    {
        public OrganizationUnit()
        {
            CategoryOrganizationUnits = new HashSet<CategoryOrganizationUnit>();
            Territories = new HashSet<Territory>();
            Prices = new HashSet<Price>();
            UserTerritoriesOrganizationUnits = new HashSet<UserTerritoriesOrganizationUnits>();
            ReleaseInfos = new HashSet<ReleaseInfo>();
            OrdersByDestination = new HashSet<Order>();
            OrdersBySource = new HashSet<Order>();
            BranchOfficeOrganizationUnits = new HashSet<BranchOfficeOrganizationUnit>();
            Firms = new HashSet<Firm>();
            WithdrawalInfos = new HashSet<WithdrawalInfo>();
            LocalMessages = new HashSet<LocalMessage>();
            Operations = new HashSet<Operation>();
            ThemeOrganizationUnits = new HashSet<ThemeOrganizationUnit>();
            Projects = new HashSet<Project>();
            OrderProcessingRequests = new HashSet<OrderProcessingRequest>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public int? DgppId { get; set; }
        public DateTime FirstEmitDate { get; set; }
        public long CountryId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public string Code { get; set; }
        public string SyncCode1C { get; set; }
        public long TimeZoneId { get; set; }
        public string ElectronicMedia { get; set; }
        public DateTime? ErmLaunchDate { get; set; }
        public DateTime? InfoRussiaLaunchDate { get; set; }

        public ICollection<CategoryOrganizationUnit> CategoryOrganizationUnits { get; set; }
        public ICollection<Territory> Territories { get; set; }
        public Country Country { get; set; }
        public ICollection<Price> Prices { get; set; }
        public ICollection<UserTerritoriesOrganizationUnits> UserTerritoriesOrganizationUnits { get; set; }
        public ICollection<ReleaseInfo> ReleaseInfos { get; set; }
        public ICollection<Order> OrdersByDestination { get; set; }
        public ICollection<Order> OrdersBySource { get; set; }
        public ICollection<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits { get; set; }
        public ICollection<Firm> Firms { get; set; }
        public ICollection<WithdrawalInfo> WithdrawalInfos { get; set; }
        public ICollection<LocalMessage> LocalMessages { get; set; }
        public ICollection<Operation> Operations { get; set; }
        public ICollection<ThemeOrganizationUnit> ThemeOrganizationUnits { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<OrderProcessingRequest> OrderProcessingRequests { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var entityKey = obj as IEntityKey;
            if (entityKey != null)
            {
                return Id == entityKey.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}