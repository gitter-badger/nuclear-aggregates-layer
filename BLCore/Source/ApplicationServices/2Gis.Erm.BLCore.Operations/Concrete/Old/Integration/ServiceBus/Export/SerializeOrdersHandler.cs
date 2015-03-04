using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeOrdersHandler : SerializeObjectsHandler<Order, ExportFlowOrdersOrder>
    {
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public SerializeOrdersHandler(ISecurityServiceUserIdentifier securityServiceUserIdentifier,
                                      IExportRepository<Order> exportOperationsRepository,
                                      ITracer tracer)
            : base(exportOperationsRepository, tracer)
        {
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        protected override ISelectSpecification<Order, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<Order, IExportableEntityDto>(x => new OrderDto
            {
                Id = x.Id,

                // order
                Order = x,
                FirmId = x.Firm.Id,
                SourceOrganizationUnitDgppId = x.SourceOrganizationUnit.DgppId.Value,
                DestOrganizationUnitDgppId = x.DestOrganizationUnit.DgppId.Value,

                LegalEntityCode = x.LegalPersonId,
                LegalEntityBranchCode = x.BranchOfficeOrganizationUnitId,

                // Не фильтруем FirmAddresses по IsActive/IsDeleted, т.к. ERM не является мастер-системой для них
                FirmAddressDgppIds = new long[0].Union(x.Firm.FirmAddresses.Select(z => z.Id)),

                PayablePlan = x.PayablePlan,

                OrderPositionDtos = x.OrderPositions.Where(z => z.IsActive && !z.IsDeleted)
                .Select(z => new
                {
                    OrderPosition = z,

                    // учитываем и обычные и пакетные позиции
                    z.PricePosition.Position,
                    ChildPositions = new[] { z.PricePosition.Position }
                                    .Union(z.PricePosition.Position.ChildPositions.Where(p => p.IsActive && !p.IsDeleted).Select(p => p.ChildPosition))
                                    .Where(p => !p.IsComposite),
                }).Select(z => new
                {
                    z.OrderPosition,
                    z.Position,

                    ChildPositionDtos = z.ChildPositions.Select(p => new OrderPositionDto
                    {
                        // у Export и у ERM немного разные понимания слова category
                        // в ERM category объединяет позиции в категории
                        // в Export сategory это наоборот разделение мета-позиций по категориям
                        // отсюда путаница в понимании атрибутов ProductCode и CategoryCode
                        ProductCode = p.PositionCategory.ExportCode,
                        CategoryCode = p.ExportCode,
                        PlatformCode = p.Platform.DgppId,
                        OrderPositionId = z.OrderPosition.Id,

                        ObjectLinkDtos = z.OrderPosition.OrderPositionAdvertisements.Where(q => q.PositionId == p.Id).Select(q => new ObjectLinkDto
                        {
                            RubricCode = q.Category.Id,
                            CardCode = q.FirmAddress.Id,
                            FirmCode = x.Firm.Id,
                            ThemeCode = q.ThemeId,
                            AdvMaterialCode = q.AdvertisementId
                        })
                    })
                })
                .SelectMany(z => z.ChildPositionDtos)
                .Where(z => z.ObjectLinkDtos.Any()),
            });
        }

        protected override string GetError(IExportableEntityDto entityDto)
        {
            var orderDto = (OrderDto)entityDto;

            // невозможно выгрузить заказ у которого нет фирмы (xsd валидация)
            if (orderDto.FirmId == null)
            {
                return "Заказ не имеет связи с фирмой";
            }

            // невозможно выгрузить заказ у фирмы которого нет адресов (xsd валидация)
            if (!orderDto.FirmAddressDgppIds.Any())
            {
                return "Фирма не имеет ни одного адреса";
            }

            // невозможно выгрузить заказ, у которого не задан Стабильный идентификатор юридического лица клиента (xsd валидация)
            if (orderDto.LegalEntityCode == null)
            {
                return "Не задан стабильный идентификатор юридического лица клиента";
            }

            // невозможно выгрузить заказ, у которого нет юридического лица отделения организации (xsd валидация)
            if (orderDto.LegalEntityBranchCode == null)
            {
                return "Заказ не имеет юридического лица отделения организации";
            }

            return null;
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entiryDto)
        {
            var orderDto = (OrderDto)entiryDto;
            var order = orderDto.Order;

            // todo: optimize
            var curator = _securityServiceUserIdentifier.GetUserInfo(order.OwnerCode).Account;

            if (orderDto.FirmId == null || orderDto.LegalEntityCode == null || orderDto.LegalEntityBranchCode == null)
            {
                throw new BusinessLogicException("При выгрузке заказов в шину интеграции возникла невозможная ситуация");
            }

            var orderElement = new XElement("Order",
                new XAttribute("Code", order.Id),
                new XAttribute("Number", order.Number),
                new XAttribute("FirmCode", orderDto.FirmId.Value),
                new XAttribute("BranchSrcCode", orderDto.SourceOrganizationUnitDgppId),
                new XAttribute("BranchDestCode", orderDto.DestOrganizationUnitDgppId),
                new XAttribute("LegalEntityCode", orderDto.LegalEntityCode.Value),
                new XAttribute("LegalEntityBranchCode", orderDto.LegalEntityBranchCode.Value),
                new XAttribute("CreatedDate", order.CreatedOn),
                new XAttribute("StartDate", order.BeginDistributionDate),
                new XAttribute("EndDate", order.EndDistributionDateFact),
                new XAttribute("PayablePlan", orderDto.PayablePlan),
                new XAttribute("Status", order.WorkflowStepId),
                new XAttribute("Curator", curator));

            if (order.ApprovalDate != null)
            {
                orderElement.Add(new XAttribute("ApprovedDate", order.ApprovalDate.Value));
            }

            if (!order.IsActive)
            {
                orderElement.Add(new XAttribute("IsHidden", true));
            }

            if (order.IsDeleted)
            {
                orderElement.Add(new XAttribute("IsDeleted", true));
            }

            var promotionalCardsElement = GetPromotionalCardsElement(orderDto);
            orderElement.Add(promotionalCardsElement);

            // если заказ не имеет ни одной позиции, то не пишем в xml ничего
            if (orderDto.OrderPositionDtos.Any())
            {
                var positionsElement = GetPositionsElement(orderDto);
                orderElement.Add(positionsElement);
            }

            return orderElement;
        }
        
        private static XElement GetPromotionalCardsElement(OrderDto orderDto)
        {
            var promotionalCardsElement = new XElement("PromotionalCards");

            foreach (var firmAddressDgppId in orderDto.FirmAddressDgppIds)
            {
                var promotionalCardElement = new XElement("PromotionalCard",
                    new XAttribute("Code", firmAddressDgppId));
                promotionalCardsElement.Add(promotionalCardElement);
            }

            return promotionalCardsElement;
        }

        private static XElement GetPositionsElement(OrderDto orderDto)
        {
            var positionsElement = new XElement("Positions");

            foreach (var orderPositionDto in orderDto.OrderPositionDtos)
            {
                var positionElement = new XElement("Position",
                                                   new XAttribute("OrderPositionCode", orderPositionDto.OrderPositionId),
                                                   new XAttribute("ProductCode", orderPositionDto.ProductCode),
                                                   new XAttribute("CategoryCode", orderPositionDto.CategoryCode),
                                                   new XAttribute("PlatformCode", orderPositionDto.PlatformCode));

                var objectLinksElement = GetObjectLinksElement(orderPositionDto);
                positionElement.Add(objectLinksElement);

                positionsElement.Add(positionElement);
            }

            return positionsElement;
        }

        private static XElement GetObjectLinksElement(OrderPositionDto orderPositionDto)
        {
            var objectLinksElement = new XElement("ObjectLinks");

            foreach (var objectLinkDto in orderPositionDto.ObjectLinkDtos)
            {
                var objectLinkElement = new XElement("ObjectLink");

                if (objectLinkDto.RubricCode != null)
                {
                    objectLinkElement.Add(new XAttribute("RubricCode", objectLinkDto.RubricCode.Value));
                }

                if (objectLinkDto.CardCode != null)
                {
                    objectLinkElement.Add(new XAttribute("CardCode", objectLinkDto.CardCode.Value));
                }

                if (objectLinkDto.ThemeCode != null)
                {
                    objectLinkElement.Add(new XAttribute("ThemeCode", objectLinkDto.ThemeCode.Value));
                }

                // todo: узнать, бывают ли заказы, в которых есть одновременно ThemeCode & FirmCode
                if (objectLinkDto.RubricCode == null && objectLinkDto.CardCode == null && objectLinkDto.FirmCode != null)
                {
                    objectLinkElement.Add(new XAttribute("FirmCode", objectLinkDto.FirmCode.Value));
                }

                if (objectLinkDto.AdvMaterialCode != null)
                {
                    objectLinkElement.Add(new XAttribute("AdvMaterialCode", objectLinkDto.AdvMaterialCode.Value));
                }

                objectLinksElement.Add(objectLinkElement);
            }

            return objectLinksElement;
        }

        #region nested types

        public sealed class OrderDto : IExportableEntityDto
        {
            public long Id { get; set; }

            public Order Order { get; set; }
            public long? FirmId { get; set; }
            public decimal PayablePlan { get; set; }
            public int SourceOrganizationUnitDgppId { get; set; }
            public int DestOrganizationUnitDgppId { get; set; }
            public long? LegalEntityCode { get; set; }
            public long? LegalEntityBranchCode { get; set; }

            public IEnumerable<long> FirmAddressDgppIds { get; set; }
            public IEnumerable<OrderPositionDto> OrderPositionDtos { get; set; }
        }

        public sealed class OrderPositionDto
        {
            public long OrderPositionId { get; set; }
            public int ProductCode { get; set; }
            public int CategoryCode { get; set; }
            public long PlatformCode { get; set; }

            public IEnumerable<ObjectLinkDto> ObjectLinkDtos { get; set; }
        }

        public sealed class ObjectLinkDto
        {
            public long? RubricCode { get; set; }
            public long? CardCode { get; set; }
            public long? FirmCode { get; set; }
            public long? ThemeCode { get; set; }
            public long? AdvMaterialCode { get; set; }
        }

        #endregion
    }
}
