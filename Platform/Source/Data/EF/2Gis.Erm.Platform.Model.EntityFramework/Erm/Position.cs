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
    public sealed partial class Position : 
        IEntity, 
        IEntityKey, 
        IAuditableEntity, 
        IDeletableEntity, 
        IDeactivatableEntity, 
        IReplicableEntity, 
        IStateTrackingEntity
    {
        public Position()
        {
            this.MasterPositions = new HashSet<PositionChildren>();
            this.ChildPositions = new HashSet<PositionChildren>();
            this.AssociatedPositions = new HashSet<AssociatedPosition>();
            this.DeniedPositions = new HashSet<DeniedPosition>();
            this.PricePositions = new HashSet<PricePosition>();
            this.OrderPositionAdvertisements = new HashSet<OrderPositionAdvertisement>();
            this.ReleasesWithdrawalsPositions = new HashSet<ReleasesWithdrawalsPosition>();
        }
        public long Id { get; set; }
        public System.Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public bool IsComposite { get; set; }
        public int CalculationMethodEnum { get; set; }
        public int BindingObjectTypeEnum { get; set; }
        public int AccountingMethodEnum { get; set; }
        public long PlatformId { get; set; }
        public long CategoryId { get; set; }
        public Nullable<long> AdvertisementTemplateId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public Nullable<long> DgppId { get; set; }
        public int ExportCode { get; set; }
        public bool IsControlledByAmount { get; set; }
        public bool RestrictChildPositionPlatforms { get; set; }
    
        public PositionCategory PositionCategory { get; set; }
        public ICollection<PositionChildren> MasterPositions { get; set; }
        public ICollection<PositionChildren> ChildPositions { get; set; }
        public Platform Platform { get; set; }
        public AdvertisementTemplate AdvertisementTemplate { get; set; }
        public ICollection<AssociatedPosition> AssociatedPositions { get; set; }
        public ICollection<DeniedPosition> DeniedPositions { get; set; }
        public ICollection<PricePosition> PricePositions { get; set; }
        public ICollection<OrderPositionAdvertisement> OrderPositionAdvertisements { get; set; }
        public ICollection<ReleasesWithdrawalsPosition> ReleasesWithdrawalsPositions { get; set; }
    
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
