using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Firm :
        IEntity, 
        IEntityKey, 
        ICuratedEntity, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IReplicableEntity, 
        IStateTrackingEntity
    {
        private long _ownerCode;
        private long? _oldOwnerCode;
    
        public Firm()
        {
            Advertisements = new HashSet<Advertisement>();
            Clients = new HashSet<Client>();
            Deals = new HashSet<Deal>();
            Orders = new HashSet<Order>();
            FirmAddresses = new HashSet<FirmAddress>();
            OrderProcessingRequests = new HashSet<OrderProcessingRequest>();
            FirmDeals = new HashSet<FirmDeal>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public int PromisingScore { get; set; }
        public UsingOtherMediaOption UsingOtherMedia { get; set; }
        public ProductType ProductType { get; set; }
        public MarketType MarketType { get; set; }
        public long OrganizationUnitId { get; set; }
        public long TerritoryId { get; set; }
        public long? ClientId { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public DateTime? LastQualifyTime { get; set; }
        public DateTime? LastDisqualifyTime { get; set; }
        public string Information { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public long OwnerCode 
    	{
            get { return _ownerCode; }
    				 
    		set
    		{
    			_oldOwnerCode = _ownerCode;
    			_ownerCode = value;
    		} 
        }

        long? ICuratedEntity.OldOwnerCode
        {
            get { return _oldOwnerCode; }
    	}

        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public BudgetType BudgetType { get; set; }
        public Geolocation Geolocation { get; set; }
        public InCityBranchesAmount InCityBranchesAmount { get; set; }
        public OutCityBranchesAmount OutCityBranchesAmount { get; set; }
        public StaffAmount StaffAmount { get; set; }
    
        public ICollection<Advertisement> Advertisements { get; set; }
        public ICollection<Client> Clients { get; set; }
        public Client Client { get; set; }
        public ICollection<Deal> Deals { get; set; }
        public ICollection<Order> Orders { get; set; }
        public OrganizationUnit OrganizationUnit { get; set; }
        public ICollection<FirmAddress> FirmAddresses { get; set; }
        public Territory Territory { get; set; }
        public ICollection<OrderProcessingRequest> OrderProcessingRequests { get; set; }
        public ICollection<FirmDeal> FirmDeals { get; set; }
    
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
