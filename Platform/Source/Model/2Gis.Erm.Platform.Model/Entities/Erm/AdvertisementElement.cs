using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class AdvertisementElement :
        IEntity,
        IEntityKey,
        ICuratedEntity,
        IAuditableEntity,
        IDeletableEntity,
        IEntityFileOptional,
        IStateTrackingEntity
    {
        private long _ownerCode;
        private long? _oldOwnerCode;

        public AdvertisementElement()
        {
            AdvertisementElementDenialReasons = new HashSet<AdvertisementElementDenialReason>();
        }

        public long Id { get; set; }
        public long AdvertisementId { get; set; }
        public long AdvertisementElementTemplateId { get; set; }
        public string Text { get; set; }
        public long? FileId { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public FasComment? FasCommentType { get; set; }
        public bool IsDeleted { get; set; }

        public long OwnerCode
        {
            get { return _ownerCode; }

            set
            {
                _oldOwnerCode = _ownerCode;
                _ownerCode = value;
            }
        }

        long? ICuratedEntity.OldOwnerCode
        {
            get { return _oldOwnerCode; }
        }

        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public long AdsTemplatesAdsElementTemplatesId { get; set; }
        public long? DgppId { get; set; }

        public Advertisement Advertisement { get; set; }
        public AdvertisementElementTemplate AdvertisementElementTemplate { get; set; }
        public File File { get; set; }
        public AdsTemplatesAdsElementTemplate AdsTemplatesAdsElementTemplate { get; set; }
        public ICollection<AdvertisementElementDenialReason> AdvertisementElementDenialReasons { get; set; }
        public AdvertisementElementStatus AdvertisementElementStatus { get; set; }

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