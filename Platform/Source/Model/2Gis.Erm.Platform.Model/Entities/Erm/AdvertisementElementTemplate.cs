using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class AdvertisementElementTemplate :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IStateTrackingEntity
    {
        public AdvertisementElementTemplate()
        {
            AdsTemplatesAdsElementTemplates = new HashSet<AdsTemplatesAdsElementTemplate>();
            AdvertisementElements = new HashSet<AdvertisementElement>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int? TextLengthRestriction { get; set; }
        public byte? MaxSymbolsInWord { get; set; }
        public int? TextLineBreaksCountRestriction { get; set; }
        public bool FormattedText { get; set; }
        public int? FileSizeRestriction { get; set; }
        public string FileExtensionRestriction { get; set; }
        public int? FileNameLengthRestriction { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public AdvertisementElementRestrictionType RestrictionType { get; set; }
        public bool IsRequired { get; set; }
        public string ImageDimensionRestriction { get; set; }
        public bool IsAdvertisementLink { get; set; }
        public bool NeedsValidation { get; set; }

        public ICollection<AdsTemplatesAdsElementTemplate> AdsTemplatesAdsElementTemplates { get; set; }
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