using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia
{
    public sealed class FirmViewModel : EntityViewModelBase<Firm>, INameAspect, IClosedForAscertainmentAspect, IRussiaAdapted
    {
        public Guid ReplicationCode { get; set; }

        public long? DgppId { get; set; }

        [DisplayNameLocalized("FirmName")]
        public string Name { get; set; }

        [DisplayNameLocalized("IsHiddenFemale")]
        [YesNoRadio]
        public bool ClosedForAscertainment { get; set; }

        public int PromisingScore { get; set; }

        public string Comment { get; set; }

        public UsingOtherMediaOption UsingOtherMedia { get; set; }

        public MarketType MarketType { get; set; }

        public ProductType ProductType { get; set; }

        [DisplayNameLocalized("FirmBudgetType")]
        public BudgetType BudgetType { get; set; }

        public Geolocation Geolocation { get; set; }

        public InCityBranchesAmount InCityBranchesAmount { get; set; }

        public OutCityBranchesAmount OutCityBranchesAmount { get; set; }

        public StaffAmount StaffAmount { get; set; }

        // Дата взятия из резерва
        public DateTime? LastQualifyTime { get; set; }

        // Дата возврата в резерв
        public DateTime? LastDisqualifyTime { get; set; }

        // Клиент
        [Dependency(DependencyType.Transfer, "ClientReplicationCode", "(this.item && this.item.data)?this.item.data.ReplicationCode:undefined;")]
        public LookupField Client { get; set; }

        public Guid? ClientReplicationCode { get; set; }

        public string ClientName { get; set; }

        // Территория
        [RequiredLocalized]
        public LookupField Territory { get; set; }

        // Город
        public LookupField OrganizationUnit { get; set; }

        // Куратор
        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }
        
        public override bool IsSecurityRoot
        {
            get
            {
                return true;
            }
        }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (FirmDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            ClosedForAscertainment = modelDto.ClosedForAscertainment;
            ProductType = modelDto.ProductType;
            MarketType = modelDto.MarketType;
            UsingOtherMedia = modelDto.UsingOtherMedia;
            BudgetType = modelDto.BudgetType;
            Geolocation = modelDto.Geolocation;
            InCityBranchesAmount = modelDto.InCityBranchesAmount;
            OutCityBranchesAmount = modelDto.OutCityBranchesAmount;
            StaffAmount = modelDto.StaffAmount;
            PromisingScore = modelDto.PromisingScore;
            ReplicationCode = modelDto.ReplicationCode;
            Comment = modelDto.Comment;

            LastQualifyTime = modelDto.LastQualifyTime;
            LastDisqualifyTime = modelDto.LastDisqualifyTime;

            Client = LookupField.FromReference(modelDto.ClientRef);
            ClientReplicationCode = modelDto.ClientReplicationCode;

            Territory = LookupField.FromReference(modelDto.TerritoryRef);
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);

            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new FirmDomainEntityDto
            {
                Id = Id,
                Name = Name,
                ClosedForAscertainment = ClosedForAscertainment,
                ProductType = ProductType,
                MarketType = MarketType,
                UsingOtherMedia = UsingOtherMedia,
                BudgetType = BudgetType,
                Geolocation = Geolocation,
                InCityBranchesAmount = InCityBranchesAmount,
                OutCityBranchesAmount = OutCityBranchesAmount,
                StaffAmount = StaffAmount,
                PromisingScore = PromisingScore,
                ReplicationCode = ReplicationCode,
                Comment = Comment,

                LastQualifyTime = LastQualifyTime,
                LastDisqualifyTime = LastDisqualifyTime,

                ClientRef = Client.ToReference(),

                TerritoryRef = Territory.ToReference(),

                OwnerRef = Owner.ToReference(),

                Timestamp = Timestamp
            };

            if (Client.Key != null)
            {
                dto.ClientReplicationCode = ClientReplicationCode.Value;
            }

            return dto;
        }
    }
}