using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel
{
    public static partial class OrderSpecs
    {
        public static class Orders
        {
        public static class Find
        {
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
            /// Отразмещавшиеся заказы по городу.
            /// </summary>
                /// <param name="sourceOrganizationUnitId"></param>
            /// <returns></returns>
                public static FindSpecification<Order> CompletelyReleasedByOrganizationUnit(long sourceOrganizationUnitId)
            {
                // кол-во неактивных блокировок по заказу = кол-ву выпусков факт в заказе (Orders.ReleaseCountFact):
                    return new FindSpecification<Order>(x => x.SourceOrganizationUnitId == sourceOrganizationUnitId &&
                                                             !x.IsDeleted && x.Locks.Count(l => !l.IsDeleted && !l.IsActive) == x.ReleaseCountFact);
            }

            public static FindSpecification<Order> ForRelease(long destinationOrganizationUnitId, TimePeriod period)
            {
                return new FindSpecification<Order>(o =>
                                                        o.BeginDistributionDate <= period.Start
                    && o.EndDistributionDateFact >= period.End
                    && o.DestOrganizationUnitId == destinationOrganizationUnitId
                    && (o.WorkflowStepId == (int)OrderState.Approved || o.WorkflowStepId == (int)OrderState.OnTermination));
            }

                public static FindSpecification<Order> AllForReleaseByPeriodExceptOrganizationUnit(long excludedOrdersOrganizationUnitId, TimePeriod period)
            {
                return new FindSpecification<Order>(o =>
                                                        !o.IsDeleted
                   && o.IsActive
                   && o.BeginDistributionDate <= period.Start
                   && o.EndDistributionDateFact >= period.End
                   && o.DestOrganizationUnitId != excludedOrdersOrganizationUnitId
                   && (o.WorkflowStepId == (int)OrderState.Approved || o.WorkflowStepId == (int)OrderState.OnTermination));
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

            public static FindSpecification<Order> ActiveOrdersForLegalPerson(long legalPersonId)
            {
                return new FindSpecification<Order>(x => !x.IsDeleted && x.IsActive &&
                                                         x.LegalPersonId == legalPersonId);
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
                                                                               IsSelectedToWhiteList = materials.FirstOrDefault().Advertisement.IsSelectedToWhiteList,
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
                                                                                                   ExportCode = advElelement.AdsTemplatesAdsElementTemplate.ExportCode
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
                                                                                where (!material.AdvertisementId.HasValue || !material.Advertisement.IsDeleted) &&
                                                                                       material.PositionId == childMasterRelation.ChildPositionId
                                                                                group material by material.AdvertisementId
                                                                                into materials
                                                                                select new AdvertisingMaterialInfo
                                                                                    {
                                                                                        Id = materials.Key,
                                                                                        IsSelectedToWhiteList = materials.FirstOrDefault().Advertisement.IsSelectedToWhiteList,
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
                                                                                                            ExportCode = advElelement.AdsTemplatesAdsElementTemplate.ExportCode
                                                                                                        })
                                                                                                    .Distinct()
                                                                                    }
            }
                    });
            }
            }
        }
    }
}
