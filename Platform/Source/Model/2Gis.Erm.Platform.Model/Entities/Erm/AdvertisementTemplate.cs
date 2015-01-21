using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class AdvertisementTemplate :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IStateTrackingEntity
    {
        public AdvertisementTemplate()
        {
            Advertisements = new HashSet<Advertisement>();
            Positions = new HashSet<Position>();
            AdsTemplatesAdsElementTemplates = new HashSet<AdsTemplatesAdsElementTemplate>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public bool IsAllowedToWhiteList { get; set; }
        public bool IsAdvertisementRequired { get; set; }
        public long? DummyAdvertisementId { get; set; }
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

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}