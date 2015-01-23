using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    // TODO {y.baranihin, 18.09.2014}: Все I*Adapted собрались тут? Тогда можно, как в тетрисе, строчку стирать.
    // COMMENT {a.rechkalov, 19.09.2014}: При текущей реализации IAdapted на ViewModel - IAdapted View'хи. В данном случае вьюхи различны.
    public sealed class MultiCultureDealViewModel : EntityViewModelBase<Deal>, IRussiaAdapted, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
        [Dependency(DependencyType.Required, "Name", "this.value != 0")]
        public override long Id { get; set; }

        public Guid ReplicationCode { get; set; }

        [StringLengthLocalized(300)]
        [DisplayNameLocalized("DealName")]
        public string Name { get; set; }

        public DealStage DealStage { get; set; }

        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        public DateTime? CloseDate { get; set; }

        [RequiredLocalized]
        public ReasonForNewDeal StartReason { get; set; }

        public CloseDealReason CloseReason { get; set; }

        [StringLengthLocalized(256)]
        public string CloseReasonOther { get; set; }

        public LookupField Currency { get; set; }

        public override byte[] Timestamp { get; set; }

        // Куратор
        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        // Клиент
        [RequiredLocalized]
        [Dependency(DependencyType.Transfer, "ClientReplicationCode", "(this.item && this.item.data)?this.item.data.ReplicationCode:undefined;")]
        [Dependency(DependencyType.Disable, "MainFirm", "this.getValue()==undefined")]
        public LookupField Client { get; set; }
        
        public Guid? ClientReplicationCode { get; set; }
        
        public string ClientName { get; set; }

        // Фирма
        public LookupField MainFirm { get; set; }

        public bool IncreaseSalesGoal { get; set; }

        public bool AttractAudienceToSiteGoal { get; set; }

        public bool IncreasePhoneCallsGoal { get; set; }

        public bool IncreaseBrandAwarenessGoal { get; set; }

        [StringLengthLocalized(512)]
        public string AdvertisingCampaignGoalText { get; set; }

        public LookupField Bargain { get; set; }

        public DateTime? AdvertisingCampaignBeginDate { get; set; }
        
        public DateTime? AdvertisingCampaignEndDate { get; set; }

        public PaymentFormat? PaymentFormat { get; set; }

        [CustomClientValidation("validateAgencyFeePercent",
                                ErrorMessageResourceType = typeof(BLResources),
                                ErrorMessageResourceName = "AgencyFeePercentMustBeBetweenZeroAndOneHundred")]
        public decimal? AgencyFee { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (DealDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            ReplicationCode = modelDto.ReplicationCode;

            Name = modelDto.Name;
            ClientReplicationCode = modelDto.ClientReplicationCode;
            CloseDate = modelDto.CloseDate;
            CloseReasonOther = modelDto.CloseReasonOther;
            Comment = modelDto.Comment;
            CreatedOn = modelDto.CreatedOn;

            CloseReason = modelDto.CloseReason;
            DealStage = modelDto.DealStage;
            StartReason = modelDto.StartReason;

            MainFirm = LookupField.FromReference(modelDto.MainFirmRef);
            Client = LookupField.FromReference(modelDto.ClientRef);
            Currency = LookupField.FromReference(modelDto.CurrencyRef);

            IncreaseSalesGoal = modelDto.AdvertisingCampaignGoals.HasValue &&
                                modelDto.AdvertisingCampaignGoals.Value.HasFlag(AdvertisingCampaignGoals.IncreaseSales);
            AttractAudienceToSiteGoal = modelDto.AdvertisingCampaignGoals.HasValue &&
                                        modelDto.AdvertisingCampaignGoals.Value.HasFlag(AdvertisingCampaignGoals.AttractAudienceToSite);
            IncreasePhoneCallsGoal = modelDto.AdvertisingCampaignGoals.HasValue &&
                                     modelDto.AdvertisingCampaignGoals.Value.HasFlag(AdvertisingCampaignGoals.IncreasePhoneCalls);
            IncreaseBrandAwarenessGoal = modelDto.AdvertisingCampaignGoals.HasValue &&
                                         modelDto.AdvertisingCampaignGoals.Value.HasFlag(AdvertisingCampaignGoals.IncreaseBrandAwareness);
            AdvertisingCampaignGoalText = modelDto.AdvertisingCampaignGoalText;
            Bargain = LookupField.FromReference(modelDto.BargainRef);
            AdvertisingCampaignBeginDate = modelDto.AdvertisingCampaignBeginDate;
            AdvertisingCampaignEndDate = modelDto.AdvertisingCampaignEndDate;
            PaymentFormat = modelDto.PaymentFormat;
            AgencyFee = modelDto.AgencyFee;

            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new DealDomainEntityDto
                {
                    Id = Id,
                    ReplicationCode = ReplicationCode,

                    Name = Name,
                    ClientReplicationCode = ClientReplicationCode.HasValue ? ClientReplicationCode.Value : Guid.Empty,
                    CloseDate = CloseDate,
                    CloseReasonOther = CloseReasonOther,
                    Comment = Comment,
                    CreatedOn = CreatedOn,
                    CloseReason = CloseReason,
                    DealStage = DealStage,
                    StartReason = StartReason,
                    MainFirmRef = MainFirm.ToReference(),
                    CurrencyRef = Currency.ToReference(),
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp,
                    AdvertisingCampaignGoalText = AdvertisingCampaignGoalText,
                    BargainRef = Bargain.ToReference(),
                    AdvertisingCampaignBeginDate = AdvertisingCampaignBeginDate,
                    AdvertisingCampaignEndDate = AdvertisingCampaignEndDate,
                    PaymentFormat = PaymentFormat,
                    AgencyFee = AgencyFee,
                };

            if (Bargain.Key != null)
            {
                dto.BargainRef = Bargain.ToReference();
            }

            if (Client.Key != null)
            {
                dto.ClientRef = Client.ToReference();
            }

            if (IncreaseSalesGoal)
            {
                dto.AdvertisingCampaignGoals = dto.AdvertisingCampaignGoals.HasValue
                                                   ? dto.AdvertisingCampaignGoals | AdvertisingCampaignGoals.IncreaseSales
                                                   : AdvertisingCampaignGoals.IncreaseSales;
            }

            if (AttractAudienceToSiteGoal)
            {
                dto.AdvertisingCampaignGoals = dto.AdvertisingCampaignGoals.HasValue
                                                   ? dto.AdvertisingCampaignGoals | AdvertisingCampaignGoals.AttractAudienceToSite
                                                   : AdvertisingCampaignGoals.AttractAudienceToSite;
            }

            if (IncreasePhoneCallsGoal)
            {
                dto.AdvertisingCampaignGoals = dto.AdvertisingCampaignGoals.HasValue
                                                   ? dto.AdvertisingCampaignGoals | AdvertisingCampaignGoals.IncreasePhoneCalls
                                                   : AdvertisingCampaignGoals.IncreasePhoneCalls;
            }

            if (IncreaseBrandAwarenessGoal)
            {
                dto.AdvertisingCampaignGoals = dto.AdvertisingCampaignGoals.HasValue
                                                   ? dto.AdvertisingCampaignGoals | AdvertisingCampaignGoals.IncreaseBrandAwareness
                                                   : AdvertisingCampaignGoals.IncreaseBrandAwareness;
            }

            return dto;
        }
    }
}