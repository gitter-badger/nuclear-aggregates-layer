//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------

// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable ConvertNullableToShortForm

using System;
using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed partial class Firm : 
        IEntity, 
        IEntityKey, 
        ICuratedEntity, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IReplicableEntity, 
        IStateTrackingEntity
    {
        private long? _oldOwnerCode;
        long? ICuratedEntity.OldOwnerCode { get { return _oldOwnerCode; } }
    
        public Firm()
        {
            this.Advertisements = new HashSet<Advertisement>();
            this.Clients = new HashSet<Client>();
            this.Deals = new HashSet<Deal>();
            this.Orders = new HashSet<Order>();
            this.FirmAddresses = new HashSet<FirmAddress>();
            this.ActivityInstances = new HashSet<ActivityInstance>();
            this.OrderProcessingRequests = new HashSet<OrderProcessingRequest>();
        }
    
        public long Id { get; set; }
        public System.Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public int PromisingScore { get; set; }
        public int UsingOtherMedia { get; set; }
        public int ProductType { get; set; }
        public int MarketType { get; set; }
        public long OrganizationUnitId { get; set; }
        public long TerritoryId { get; set; }
        public Nullable<long> ClientId { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public Nullable<System.DateTime> LastQualifyTime { get; set; }
        public Nullable<System.DateTime> LastDisqualifyTime { get; set; }
        public string Information { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long OwnerCode 
    	{
    		get
    		{
    			return _ownerCode;
    		}
    				 
    		set
    		{
    			_oldOwnerCode = _ownerCode;
    			_ownerCode = value;
    		} 
    	}
    				
    	private long _ownerCode;
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public int BudgetType { get; set; }
        public int Geolocation { get; set; }
        public int InCityBranchesAmount { get; set; }
        public int OutCityBranchesAmount { get; set; }
        public int StaffAmount { get; set; }
    
        public ICollection<Advertisement> Advertisements { get; set; }
        public ICollection<Client> Clients { get; set; }
        public Client Client { get; set; }
        public ICollection<Deal> Deals { get; set; }
        public ICollection<Order> Orders { get; set; }
        public OrganizationUnit OrganizationUnit { get; set; }
        public ICollection<FirmAddress> FirmAddresses { get; set; }
        public Territory Territory { get; set; }
        public ICollection<ActivityInstance> ActivityInstances { get; set; }
        public ICollection<OrderProcessingRequest> OrderProcessingRequests { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm


