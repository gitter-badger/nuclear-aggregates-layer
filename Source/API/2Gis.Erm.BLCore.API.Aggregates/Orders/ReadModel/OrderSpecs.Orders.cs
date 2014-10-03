using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel
{
    public static partial class OrderSpecs
    {
        public static class Orders
        {
            public static class Find
            {
                public static FindSpecification<Order> ReleasableStatuses
                {
                    get
                    {
                        return
                            new FindSpecification<Order>(o => o.WorkflowStepId == (int)OrderState.Approved || o.WorkflowStepId == (int)OrderState.OnTermination);
                    }
                }

                public static FindSpecification<Order> HasLocks(long orderId)
                {
                    return new FindSpecification<Order>(x => x.Id == orderId && x.Locks.Any(y => !y.IsDeleted));
                }

                public static FindSpecification<Order> ForDeal(long dealId)
                {
                    return new FindSpecification<Order>(x => x.DealId == dealId);
                }

                public static FindSpecification<Order> NotRejected()
                {
                    return new FindSpecification<Order>(x => x.WorkflowStepId != (int)OrderState.Rejected && !x.IsDeleted);
                }

                public static FindSpecification<Order> NotInArchive()
                {
                    return new FindSpecification<Order>(x => x.WorkflowStepId != (int)OrderState.Archive);
                }

                public static FindSpecification<Order> HasLegalPerson()
                {
                    return new FindSpecification<Order>(x => x.LegalPersonId.HasValue);
                }

                /// <summary>
                ///     Отразмещавшиеся заказы по городу.
                /// </summary>
                /// <param name="sourceOrganizationUnitId"></param>
                /// <returns></returns>
                public static FindSpecification<Order> CompletelyReleasedByOrganizationUnit(long sourceOrganizationUnitId)
                {
                    // кол-во неактивных блокировок по заказу = кол-ву выпусков факт в заказе (Orders.ReleaseCountFact):
                    return new FindSpecification<Order>(x => x.SourceOrganizationUnitId == sourceOrganizationUnitId &&
                                                             !x.IsDeleted && x.Locks.Count(l => !l.IsDeleted && !l.IsActive) == x.ReleaseCountFact);
                }

                public static FindSpecification<Order> ByAccount(long accountId)
                {
                    return
                        new FindSpecification<Order>(
                            o => o.BranchOfficeOrganizationUnit.Accounts.Any(a => a.Id == accountId && a.LegalPersonId == o.LegalPersonId));
                }

                public static FindSpecification<Order> ByPeriod(TimePeriod period)
                {
                    return new FindSpecification<Order>(o => o.BeginDistributionDate <= period.Start && o.EndDistributionDateFact >= period.End);
                }

                public static FindSpecification<Order> WithStatuses(params OrderState[] statuses)
                {
                    return new FindSpecification<Order>(o => statuses.Cast<int>().Contains(o.WorkflowStepId));
                }

                public static FindSpecification<Order> ForRelease(long destinationOrganizationUnitId, TimePeriod period)
                {
                    return ByPeriod(period)
                           && ReleasableStatuses
                           && new FindSpecification<Order>(o => o.DestOrganizationUnitId == destinationOrganizationUnitId);
                }

                public static FindSpecification<Order> AllForReleaseByPeriodExceptOrganizationUnit(long excludedOrdersOrganizationUnitId, TimePeriod period)
                {
                    return ByPeriod(period)
                           && ReleasableStatuses
                           && new FindSpecification<Order>(o => o.DestOrganizationUnitId != excludedOrdersOrganizationUnitId);
                }

                public static FindSpecification<Order> ActiveOrdersForFirm(long firmId)
                {
                    return new FindSpecification<Order>(x => !x.IsDeleted && x.IsActive &&
                                                             x.FirmId == firmId &&
                                                             x.WorkflowStepId != (int)OrderState.Archive);
                }

                public static FindSpecification<Order> ActiveOrdersForClient(long clientId)
                {
                    return new FindSpecification<Order>(x => !x.IsDeleted && x.IsActive &&
                                                             x.Firm.ClientId == clientId &&
                                                             x.WorkflowStepId != (int)OrderState.Archive);
                }

                public static FindSpecification<Order> ForLegalPerson(long legalPersonId)
                {
                    return new FindSpecification<Order>(x => x.LegalPersonId == legalPersonId);
                }

                public static FindSpecification<Order> BySourceOrganizationUnit(long sourceOrganizationUnitId)
                {
                    return new FindSpecification<Order>(x => x.SourceOrganizationUnitId == sourceOrganizationUnitId);
                }

                public static FindSpecification<Order> ByEndDistributionDateFact(DateTime endDistributionDate)
                {
                    return new FindSpecification<Order>(x => x.EndDistributionDateFact == endDistributionDate);
                }

                public static FindSpecification<Order> ForOrganizationUnitsPair(long sourceOrganizationUnitId, long destOrganizationUnitId)
                {
                    return new FindSpecification<Order>(x => x.SourceOrganizationUnitId == sourceOrganizationUnitId &&
                                                             x.DestOrganizationUnitId == destOrganizationUnitId);
                }
            }

            public static class Select
            {
                public static ISelectSpecification<Order, OrderInfo> OrderInfosForRelease()
                {
                    return new SelectSpecification<Order, OrderInfo>(
                        o => new OrderInfo
                            {
                                ApproverId = o.InspectorCode,
                                CuratorId = o.OwnerCode,
                                StableFirmId = o.Firm.Id,
                                CreatedOn = o.CreatedOn,
                                BeginDistributionDate = o.BeginDistributionDate,
                                EndDistributionDate = o.EndDistributionDateFact,
                                Status = o.WorkflowStepId,
                                Id = o.Id,
                                Number = o.Number,
                                DestOrganizationUnitId = o.DestOrganizationUnitId,
                                Positions = from orderPosition in o.OrderPositions.Where(x => !x.PricePosition.Position.IsComposite)
                                            where orderPosition.IsActive && !orderPosition.IsDeleted
                                            select new OrderPositionInfo
                                                {
                                                    Id = orderPosition.Id,
                                                    PlatformId = orderPosition.PricePosition.Position.Platform.DgppId,
                                                    ProductType = orderPosition.PricePosition.Position.PositionCategory.ExportCode,
                                                    ProductCategory = orderPosition.PricePosition.Position.ExportCode,
                                                    AdvertisingMaterials = from material in orderPosition.OrderPositionAdvertisements
                                                                           where !material.AdvertisementId.HasValue || !material.Advertisement.IsDeleted
                                                                           group material by material.AdvertisementId
                                                                           into materials
                                                                           select new AdvertisingMaterialInfo
                                                                               {
                                                                                   Id = materials.Key,
                                                                                   IsSelectedToWhiteList =
                                                                                       materials.FirstOrDefault().Advertisement.IsSelectedToWhiteList,
                                                                                   StableAddrIds = from mat in materials
                                                                                                   where mat.FirmAddressId != null && mat.CategoryId == null
                                                                                                   select mat.FirmAddress.Id,
                                                                                   StableRubrIds = from mat in materials
                                                                                                   where mat.FirmAddressId == null && mat.CategoryId != null
                                                                                                   select mat.Category.Id,
                                                                                   RubrInAddrIds = from mat in materials
                                                                                                   where mat.FirmAddressId != null && mat.CategoryId != null
                                                                                                   select new CategoryInAddressElementInfo
                                                                                                       {
                                                                                                           FirmAddressId = mat.FirmAddress.Id,
                                                                                                           CategoryId = mat.Category.Id
                                                                                                       },
                                                                                   ThemeIds = from mat in materials
                                                                                              where mat.ThemeId != null
                                                                                              select mat.ThemeId.Value,
                                                                                   Elements = (from mat in materials
                                                                                               from advElelement in mat.Advertisement.AdvertisementElements
                                                                                               where !advElelement.IsDeleted
                                                                                               select new AdvertisingElementInfo
                                                                                                   {
                                                                                                       Id = advElelement.Id,
                                                                                                       Text = advElelement.Text,
                                                                                                       FileId = advElelement.FileId,
                                                                                                       BeginDate = advElelement.BeginDate,
                                                                                                       EndDate = advElelement.EndDate,
                                                                                                       Name = advElelement.AdvertisementElementTemplate.Name,
                                                                                                       ExportCode =
                                                                                                           advElelement.AdsTemplatesAdsElementTemplate
                                                                                                                       .ExportCode
                                                                                                   })
                                                                               .Distinct()
                                                                               }
                                                },
                                CompositePositions = from orderPosition in o.OrderPositions.Where(x => x.PricePosition.Position.IsComposite)
                                                     from childMasterRelation in orderPosition.PricePosition.Position.ChildPositions
                                                     where orderPosition.IsActive && !orderPosition.IsDeleted
                                                     select new OrderPositionInfo
                                                         {
                                                             Id = orderPosition.Id,
                                                             PlatformId = childMasterRelation.ChildPosition.Platform.DgppId,
                                                             ProductType = childMasterRelation.ChildPosition.PositionCategory.ExportCode,
                                                             ProductCategory = childMasterRelation.ChildPosition.ExportCode,
                                                             AdvertisingMaterials = from material in orderPosition.OrderPositionAdvertisements
                                                                                    where
                                                                                        (!material.AdvertisementId.HasValue || !material.Advertisement.IsDeleted) &&
                                                                                        material.PositionId == childMasterRelation.ChildPositionId
                                                                                    group material by material.AdvertisementId
                                                                                    into materials
                                                                                    select new AdvertisingMaterialInfo
                                                                                        {
                                                                                            Id = materials.Key,
                                                                                            IsSelectedToWhiteList =
                                                                                                materials.FirstOrDefault().Advertisement.IsSelectedToWhiteList,
                                                                                            StableAddrIds = from mat in materials
                                                                                                            where
                                                                                                                mat.FirmAddressId != null &&
                                                                                                                mat.CategoryId == null
                                                                                                            select mat.FirmAddress.Id,
                                                                                            StableRubrIds = from mat in materials
                                                                                                            where
                                                                                                                mat.FirmAddressId == null &&
                                                                                                                mat.CategoryId != null
                                                                                                            select mat.Category.Id,
                                                                                            RubrInAddrIds = from mat in materials
                                                                                                            where
                                                                                                                mat.FirmAddressId != null &&
                                                                                                                mat.CategoryId != null
                                                                                                            select new CategoryInAddressElementInfo
                                                                                                                {
                                                                                                                    FirmAddressId = mat.FirmAddress.Id,
                                                                                                                    CategoryId = mat.Category.Id
                                                                                                                },
                                                                                            ThemeIds = from mat in materials
                                                                                                       where mat.ThemeId != null
                                                                                                       select mat.ThemeId.Value,
                                                                                            Elements = (from mat in materials
                                                                                                        from advElelement in
                                                                                                            mat.Advertisement.AdvertisementElements
                                                                                                        where !advElelement.IsDeleted
                                                                                                        select new AdvertisingElementInfo
                                                                                                            {
                                                                                                                Id = advElelement.Id,
                                                                                                                Text = advElelement.Text,
                                                                                                                FileId = advElelement.FileId,
                                                                                                                BeginDate = advElelement.BeginDate,
                                                                                                                EndDate = advElelement.EndDate,
                                                                                                                Name =
                                                                                                                    advElelement.AdvertisementElementTemplate
                                                                                                                                .Name,
                                                                                                                ExportCode =
                                                                                                                    advElelement.AdsTemplatesAdsElementTemplate
                                                                                                                                .ExportCode
                                                                                                            })
                                                                                        .Distinct()
                                                                                        }
                                                         }
                            });
                }

                public static ISelectSpecification<Order, OrderDomainEntityDto> OrderDomainEntityDto()
                {
                    return new SelectSpecification<Order, OrderDomainEntityDto>(x => new OrderDomainEntityDto
                    {
                        Id = x.Id,
                        OrderNumber = x.Number,
                        RegionalNumber = x.RegionalNumber,
                        FirmRef = new EntityReference { Id = x.FirmId, Name = x.Firm.Name },
                        ClientRef = new EntityReference
                        {
                            Id = x.Deal != null ? x.Deal.ClientId : x.Firm.ClientId,
                            Name = (x.Deal != null) ? x.Deal.Client.Name : (x.Firm.Client != null ? x.Firm.Client.Name : null)
                        },
                        DgppId = x.DgppId,
                        HasAnyOrderPosition = x.OrderPositions.Any(op => op.IsActive && !op.IsDeleted),
                        HasDestOrganizationUnitPublishedPrice = x.DestOrganizationUnit.Prices
                                                                 .Any(price => price.IsPublished && price.IsActive &&
                                                                               !price.IsDeleted && price.BeginDate <= x.BeginDistributionDate),
                        SourceOrganizationUnitRef = new EntityReference { Id = x.SourceOrganizationUnitId, Name = x.SourceOrganizationUnit.Name },
                        DestOrganizationUnitRef = new EntityReference { Id = x.DestOrganizationUnitId, Name = x.DestOrganizationUnit.Name },
                        BranchOfficeOrganizationUnitRef = new EntityReference { Id = x.BranchOfficeOrganizationUnitId, Name = x.BranchOfficeOrganizationUnit.ShortLegalName },
                        LegalPersonRef = new EntityReference { Id = x.LegalPersonId, Name = x.LegalPerson.LegalName },
                        LegalPersonProfileRef = new EntityReference { Id = x.LegalPersonProfileId, Name = x.LegalPersonProfile.Name },
                        DealRef = new EntityReference { Id = x.DealId, Name = x.Deal.Name },
                        DealCurrencyId = x.Deal.CurrencyId,
                        CurrencyRef = new EntityReference { Id = x.CurrencyId, Name = x.Currency.Name },
                        BeginDistributionDate = x.BeginDistributionDate,
                        EndDistributionDatePlan = x.EndDistributionDatePlan,
                        EndDistributionDateFact = x.EndDistributionDateFact,
                        BeginReleaseNumber = x.BeginReleaseNumber,
                        EndReleaseNumberPlan = x.EndReleaseNumberPlan,
                        EndReleaseNumberFact = x.EndReleaseNumberFact,
                        SignupDate = x.SignupDate,
                        ReleaseCountPlan = x.ReleaseCountPlan,
                        ReleaseCountFact = x.ReleaseCountFact,
                        PreviousWorkflowStepId = (OrderState)x.WorkflowStepId,
                        WorkflowStepId = (OrderState)x.WorkflowStepId,
                        PayablePlan = x.PayablePlan,
                        PayableFact = x.PayableFact,
                        PayablePrice = x.PayablePrice,
                        VatPlan = x.VatPlan,
                        AmountToWithdraw = x.AmountToWithdraw,
                        AmountWithdrawn = x.AmountWithdrawn,
                        DiscountSum = x.DiscountSum,
                        DiscountPercent = x.DiscountPercent,
                        DiscountReasonEnum = (OrderDiscountReason)x.DiscountReasonEnum,
                        DiscountComment = x.DiscountComment,
                        DiscountPercentChecked = x.OrderPositions
                                                  .Where(y => !y.IsDeleted && y.IsActive)
                                                  .All(y => y.CalculateDiscountViaPercent),
                        Comment = x.Comment,
                        IsTerminated = x.IsTerminated,
                        TerminationReason = (OrderTerminationReason)x.TerminationReason,
                        OrderType = (OrderType)x.OrderType,
                        InspectorRef = new EntityReference { Id = x.InspectorCode, Name = null },
                        BargainRef = new EntityReference { Id = x.BargainId, Name = x.Bargain.Number },
                        Platform = x.Platform == null ? string.Empty : x.Platform.Name,
                        PlatformRef = new EntityReference { Id = x.PlatformId, Name = x.Platform == null ? string.Empty : x.Platform.Name },
                        HasDocumentsDebt = (DocumentsDebt)x.HasDocumentsDebt,
                        DocumentsComment = x.DocumentsComment,
                        AccountRef = new EntityReference { Id = x.AccountId, Name = null },
                        OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                        PaymentMethod = (PaymentMethod)x.PaymentMethod,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                        ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                        CreatedOn = x.CreatedOn,
                        ModifiedOn = x.ModifiedOn,
                        Timestamp = x.Timestamp
                    });
                }
            }
        }
    }
}