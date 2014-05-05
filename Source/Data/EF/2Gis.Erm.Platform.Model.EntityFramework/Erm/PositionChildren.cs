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
    public sealed partial class PositionChildren : 
        IEntity, 
        IEntityKey, 
        ICuratedEntity, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IStateTrackingEntity
    {
        private long _ownerCode;
        private long? _oldOwnerCode;
    
        public long Id { get; set; }
        public long MasterPositionId { get; set; }
        public long ChildPositionId { get; set; }
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
        public bool IsDeleted { get; set; }
        public byte[] Timestamp { get; set; }
        public bool IsActive { get; set; }
    
        public Position ChildPosition { get; set; }
        public Position MasterPosition { get; set; }
    
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
