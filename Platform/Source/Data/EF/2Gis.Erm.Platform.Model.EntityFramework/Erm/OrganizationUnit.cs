//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed partial class OrganizationUnit : 
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
            this.CategoryOrganizationUnits = new HashSet<CategoryOrganizationUnit>();
            this.Territories = new HashSet<Territory>();
            this.Prices = new HashSet<Price>();
            this.UserTerritoriesOrganizationUnits = new HashSet<UserTerritoriesOrganizationUnits>();
            this.ReleaseInfos = new HashSet<ReleaseInfo>();
            this.OrdersByDestination = new HashSet<Order>();
            this.OrdersBySource = new HashSet<Order>();
            this.BranchOfficeOrganizationUnits = new HashSet<BranchOfficeOrganizationUnit>();
            this.Firms = new HashSet<Firm>();
            this.WithdrawalInfos = new HashSet<WithdrawalInfo>();
            this.LocalMessages = new HashSet<LocalMessage>();
            this.Operations = new HashSet<Operation>();
            this.ThemeOrganizationUnits = new HashSet<ThemeOrganizationUnit>();
            this.Projects = new HashSet<Project>();
            this.OrderProcessingRequests = new HashSet<OrderProcessingRequest>();
        }
        public long Id { get; set; }
        public System.Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public Nullable<int> DgppId { get; set; }
        public System.DateTime FirstEmitDate { get; set; }
        public long CountryId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public string Code { get; set; }
        public string SyncCode1C { get; set; }
        public long TimeZoneId { get; set; }
        public string ElectronicMedia { get; set; }
        public Nullable<System.DateTime> ErmLaunchDate { get; set; }
        public Nullable<System.DateTime> InfoRussiaLaunchDate { get; set; }
    
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
    
    	override public int GetHashCode()
    	{
    		return Id.GetHashCode();
    	}
    }
}
