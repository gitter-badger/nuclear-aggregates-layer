using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Deal :
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

        public Deal()
        {
            Orders = new HashSet<Order>();
            AfterSaleServiceActivities = new HashSet<AfterSaleServiceActivity>();
            FirmDeals = new HashSet<FirmDeal>();
            LegalPersonDeals = new HashSet<LegalPersonDeal>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public long? MainFirmId { get; set; }
        public long ClientId { get; set; }
        public long CurrencyId { get; set; }
        public ReasonForNewDeal StartReason { get; set; }
        public CloseDealReason CloseReason { get; set; }
        public string CloseReasonOther { get; set; }
        public DateTime? CloseDate { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

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
        public DealStage DealStage { get; set; }
        public long? BargainId { get; set; }
        public DateTime? AdvertisingCampaignBeginDate { get; set; }
        public DateTime? AdvertisingCampaignEndDate { get; set; }
        public string AdvertisingCampaignGoalText { get; set; }
        public AdvertisingCampaignGoals? AdvertisingCampaignGoals { get; set; }
        public int? PaymentFormat { get; set; }
        public decimal? AgencyFee { get; set; }

        public Currency Currency { get; set; }
        public ICollection<Order> Orders { get; set; }
        public Client Client { get; set; }
        public Firm Firm { get; set; }
        public ICollection<AfterSaleServiceActivity> AfterSaleServiceActivities { get; set; }
        public Bargain Bargain { get; set; }
        public ICollection<FirmDeal> FirmDeals { get; set; }
        public ICollection<LegalPersonDeal> LegalPersonDeals { get; set; }

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