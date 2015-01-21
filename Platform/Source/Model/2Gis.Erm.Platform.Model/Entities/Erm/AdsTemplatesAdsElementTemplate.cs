using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class AdsTemplatesAdsElementTemplate :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IStateTrackingEntity
    {
        public AdsTemplatesAdsElementTemplate()
        {
            AdvertisementElements = new HashSet<AdvertisementElement>();
        }

        public long Id { get; set; }
        public long AdsTemplateId { get; set; }
        public long AdsElementTemplateId { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public int ExportCode { get; set; }

        public AdvertisementElementTemplate AdvertisementElementTemplate { get; set; }
        public AdvertisementTemplate AdvertisementTemplate { get; set; }
        public ICollection<AdvertisementElement> AdvertisementElements { get; set; }

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