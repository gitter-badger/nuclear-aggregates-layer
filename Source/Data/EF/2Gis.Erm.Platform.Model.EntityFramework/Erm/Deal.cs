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
    public sealed partial class Deal : 
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
    
        public Deal()
        {
            this.Orders = new HashSet<Order>();
            this.AfterSaleServiceActivities = new HashSet<AfterSaleServiceActivity>();
        }
        public long Id { get; set; }
        public System.Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public Nullable<long> MainFirmId { get; set; }
        public long ClientId { get; set; }
        public long CurrencyId { get; set; }
        public int StartReason { get; set; }
        public int CloseReason { get; set; }
        public string CloseReasonOther { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }
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
    	long? ICuratedEntity.OldOwnerCode { get { return _oldOwnerCode; } }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public int DealStage { get; set; }
    
        public Currency Currency { get; set; }
        public ICollection<Order> Orders { get; set; }
        public Client Client { get; set; }
        public Firm Firm { get; set; }
        public ICollection<AfterSaleServiceActivity> AfterSaleServiceActivities { get; set; }
    
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
