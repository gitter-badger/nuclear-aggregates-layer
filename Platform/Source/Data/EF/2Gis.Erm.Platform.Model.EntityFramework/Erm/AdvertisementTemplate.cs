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
    public sealed partial class AdvertisementTemplate : 
        IEntity, 
        IEntityKey, 
        IAuditableEntity, 
        IDeletableEntity, 
        IStateTrackingEntity
    {
        public AdvertisementTemplate()
        {
            this.Advertisements = new HashSet<Advertisement>();
            this.Positions = new HashSet<Position>();
            this.AdsTemplatesAdsElementTemplates = new HashSet<AdsTemplatesAdsElementTemplate>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public bool IsAllowedToWhiteList { get; set; }
        public bool IsAdvertisementRequired { get; set; }
        public Nullable<long> DummyAdvertisementId { get; set; }
        public bool IsPublished { get; set; }
    
        public ICollection<Advertisement> Advertisements { get; set; }
        public ICollection<Position> Positions { get; set; }
        public ICollection<AdsTemplatesAdsElementTemplate> AdsTemplatesAdsElementTemplates { get; set; }
    
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
