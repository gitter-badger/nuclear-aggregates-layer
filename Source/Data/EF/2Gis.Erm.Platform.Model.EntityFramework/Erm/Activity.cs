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
using DoubleGis.Erm.Model.Entities.Interfaces;
using DoubleGis.Erm.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Model.Entities.Erm
{
    public sealed partial class Activity : IEntityKey, 
        ICuratedEntity, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IStateTrackingEntity
    {
        private long? _oldOwnerCode;
        long? ICuratedEntity.OldOwnerCode { get { return _oldOwnerCode; } }
    
        public Activity()
        {
            this.ActivityExtensions = new HashSet<ActivityExtension>();
        }
    
        public long Id { get; set; }
        public int Type { get; set; }
        public Nullable<long> ClientId { get; set; }
        public Nullable<long> ContactId { get; set; }
        public Nullable<long> DealId { get; set; }
        public Nullable<long> FirmId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
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
        public System.DateTime CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
    
        public Client Client { get; set; }
        public Contact Contact { get; set; }
        public Deal Deal { get; set; }
        public Firm Firm { get; set; }
        public ICollection<ActivityExtension> ActivityExtensions { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm


