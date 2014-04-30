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
    public sealed partial class Price : 
        IEntity, 
        IEntityKey, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IStateTrackingEntity
    {
        public Price()
        {
            this.LockDetails = new HashSet<LockDetail>();
            this.DeniedPositions = new HashSet<DeniedPosition>();
            this.PricePositions = new HashSet<PricePosition>();
        }
        public long Id { get; set; }
        public Nullable<long> DgppId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime PublishDate { get; set; }
        public System.DateTime BeginDate { get; set; }
        public bool IsPublished { get; set; }
        public long OrganizationUnitId { get; set; }
        public long CurrencyId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
    
        public Currency Currency { get; set; }
        public ICollection<LockDetail> LockDetails { get; set; }
        public OrganizationUnit OrganizationUnit { get; set; }
        public ICollection<DeniedPosition> DeniedPositions { get; set; }
        public ICollection<PricePosition> PricePositions { get; set; }
    
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
