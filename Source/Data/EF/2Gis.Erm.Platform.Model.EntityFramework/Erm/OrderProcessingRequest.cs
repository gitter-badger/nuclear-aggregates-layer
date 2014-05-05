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
    public sealed partial class OrderProcessingRequest : 
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
    
        public OrderProcessingRequest()
        {
            this.OrderProcessingRequestMessages = new HashSet<OrderProcessingRequestMessage>();
        }
        public long Id { get; set; }
        public System.Guid ReplicationCode { get; set; }
        public string Title { get; set; }
        public int RequestType { get; set; }
        public System.DateTime DueDate { get; set; }
        public Nullable<long> BaseOrderId { get; set; }
        public Nullable<long> RenewedOrderId { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public System.DateTime BeginDistributionDate { get; set; }
        public long FirmId { get; set; }
        public long LegalPersonProfileId { get; set; }
        public long LegalPersonId { get; set; }
        public string Description { get; set; }
        public int State { get; set; }
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
        public short ReleaseCountPlan { get; set; }
    
        public LegalPerson LegalPerson { get; set; }
        public Order BaseOrder { get; set; }
        public LegalPersonProfile LegalPersonProfile { get; set; }
        public Firm Firm { get; set; }
        public Order RenewedOrder { get; set; }
        public OrganizationUnit SourceOrganizationUnit { get; set; }
        public ICollection<OrderProcessingRequestMessage> OrderProcessingRequestMessages { get; set; }
    
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
