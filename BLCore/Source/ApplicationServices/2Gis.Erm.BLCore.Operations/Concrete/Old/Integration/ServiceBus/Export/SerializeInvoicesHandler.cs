using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Shared;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeInvoicesHandler : SerializeObjectsHandler<Order, ExportFlowOrdersInvoice>
    {
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public SerializeInvoicesHandler(IExportRepository<Order> exportOperationsRepository,
                                        ITracer tracer,
                                        ISecurityServiceUserIdentifier securityServiceUserIdentifier)
            : base(exportOperationsRepository, tracer)
        {
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        protected override ISelectSpecification<Order, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<Order, IExportableEntityDto>(x => new InvoiceDto
            {
                Id = x.Id,
                SalesModels = x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted)
                                              .Select(y => y.PricePosition.Position.SalesModel)
                                              .Distinct(),
                Number = x.Number,
                FirmCode = x.FirmId,
                FirmName = x.Firm.Name,
                BranchSrcCode = x.SourceOrganizationUnit.DgppId.Value,
                BranchDestCode = x.DestOrganizationUnit.DgppId.Value,
                LegalEntityCode = x.LegalPersonId,
                LegalEntityBranchCode = x.BranchOfficeOrganizationUnitId,
                CreatedDate = x.CreatedOn,
                ApprovedDate = x.ApprovalDate,
                StartDate = x.BeginDistributionDate,
                EndDate = x.EndDistributionDateFact,
                EndDatePlan = x.EndDistributionDatePlan,
                SignupDate = x.SignupDate,
                Status = x.WorkflowStepId,
                OrderType = x.OrderType,
                OwnerCode = x.OwnerCode,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                InvoiceItems =
                    x.OrderPositions
                     .Where(z => z.IsActive && !z.IsDeleted)
                     .Select(z => new
                     {
                         OrderPosition = z,

                         // учитываем и обычные и пакетные позиции
                         z.PricePosition.Position,
                         ChildPositions = new[] { z.PricePosition.Position }
                                  .Union(z.PricePosition.Position.ChildPositions
                                          .Where(p => p.IsActive && !p.IsDeleted)
                                          .Select(p => p.ChildPosition))
                                  .Where(p => !p.IsComposite),
                     })
                     .Select(z => new InvoiceItemDto
                     {
                         OrderPositionCode = z.OrderPosition.Id,
                         PriceListPositionCode = z.OrderPosition.PricePositionId,
                         NomenclatureElementCode = z.Position.Id,
                         PricePerUnitWithVat = z.OrderPosition.PricePerUnitWithVat,
                         Amount = z.OrderPosition.Amount,
                         DiscountPercent = z.OrderPosition.DiscountPercent,
                         CategoryRate = z.OrderPosition.CategoryRate,
                         ReleasesWithdrawals = z.OrderPosition.ReleasesWithdrawals.Select(r =>
                             new ReleasesWithdrawalDto
                                 {
                                     ReleaseBeginDate = r.ReleaseBeginDate,
                                     ReleaseEndDate = r.ReleaseEndDate,
                                     AmountToWithdraw = z.OrderPosition.PricePosition.Cost * z.OrderPosition.Amount
                                 }),
                         SubInvoiceItems = z.ChildPositions
                                            .Select(p => new SubInvoiceItemDto
                                            {
                                                NomenclatureElementCode = p.Id,
                                                LinkObjects = z.OrderPosition.OrderPositionAdvertisements
                                                               .Where(q => q.PositionId == p.Id)
                                                               .Select(q => new LinkObjectDto
                                                               {
                                                                   IsLinkedWithExportableLinkingObject = q.CategoryId.HasValue || q.FirmAddressId.HasValue,
                                                                   RubricCode = q.Category.Id,
                                                                   CardCode = q.FirmAddressId
                                                               })
                                            })
                     })
            });
        }

        protected override string GetXsdSchemaContent(string schemaName)
        {
            return Properties.Resources.ResourceManager.GetString(schemaName);
        }

        protected override string GetError(IExportableEntityDto entityDto)
        {
            var invoiceDto = (InvoiceDto)entityDto;

            // невозможно выгрузить заказ у которого нет фирмы (xsd валидация)
            if (invoiceDto.FirmName == null)
            {
                return "Заказ не имеет связи с фирмой";
            }

            // невозможно выгрузить заказ, у которого не задан Стабильный идентификатор юридического лица клиента (xsd валидация)
            if (!invoiceDto.LegalEntityCode.HasValue)
            {
                return "Не задан стабильный идентификатор юридического лица клиента";
            }

            // невозможно выгрузить заказ, у которого нет юридического лица отделения организации (xsd валидация)
            if (!invoiceDto.LegalEntityBranchCode.HasValue)
            {
                return "Заказ не имеет юридического лица отделения организации";
            }

            return null;
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entityDto)
        {
            var invoiceDto = (InvoiceDto)entityDto;
            if (invoiceDto.FirmName == null || invoiceDto.LegalEntityCode == null || invoiceDto.LegalEntityBranchCode == null)
            {
                throw new BusinessLogicException("При выгрузке заказов в шину интеграции возникла невозможная ситуация");
            }

            var invoiceElement = new XElement("Invoice",
                                              new XAttribute("Code", invoiceDto.Id),
                                              new XAttribute("AdvModel",
                                                             (invoiceDto.SalesModels.Any()

                                                                  // Используется First, а не Single потому что сейчас есть заказы, которые нарушают правило
                                                                  // о том, что все позиции заказа должны быть одной модели продаж. Мы дадим таким заказам доразмещаться.
                                                                  // После того, как все такие заказы доразмещаются First() можно и нужно будет заменить на Single()
                                                                  ? invoiceDto.SalesModels.First()
                                                                  : SalesModel.None).ConvertToServiceBusSalesModel()),
                                              new XAttribute("Number", invoiceDto.Number),
                                              new XAttribute("FirmCode", invoiceDto.FirmCode),
                                              new XAttribute("FirmName", invoiceDto.FirmName),
                                              new XAttribute("BranchSrcCode", invoiceDto.BranchSrcCode),
                                              new XAttribute("BranchDestCode", invoiceDto.BranchDestCode),
                                              new XAttribute("LegalEntityCode", invoiceDto.LegalEntityCode.Value),
                                              new XAttribute("LegalEntityBranchCode", invoiceDto.LegalEntityBranchCode.Value),
                                              new XAttribute("CreatedDate", invoiceDto.CreatedDate),
                                              new XAttribute("StartDate", invoiceDto.StartDate),
                                              new XAttribute("EndDate", invoiceDto.EndDate),
                                              new XAttribute("EndDatePlan", invoiceDto.EndDatePlan),
                                              new XAttribute("SignupDate", invoiceDto.SignupDate),
                                              new XAttribute("Status", invoiceDto.Status),
                                              new XAttribute("OrderType", invoiceDto.OrderType),
                                              new XAttribute("UserCode", invoiceDto.UserCode));

            if (invoiceDto.ApprovedDate.HasValue)
            {
                invoiceElement.Add(new XAttribute("ApprovedDate", invoiceDto.ApprovedDate.Value));
            }

            if (!invoiceDto.IsActive)
            {
                invoiceElement.Add(new XAttribute("IsHidden", true));
            }

            invoiceElement.Add(GetInvoiceItemsElement(invoiceDto));

            return invoiceElement;
        }

        protected override IEnumerable<IExportableEntityDto> ProcessDtosAfterMaterialization(IEnumerable<IExportableEntityDto> entityDtos)
        {
            foreach (var entityDto in entityDtos.Cast<InvoiceDto>())
            {
                entityDto.UserCode = _securityServiceUserIdentifier.GetUserInfo(entityDto.OwnerCode).Account;
            }

            return entityDtos;
        }

        private static XElement GetInvoiceItemsElement(InvoiceDto invoiceDto)
        {
            var invoiceItemsElement = new XElement("InvoiceItems");
            foreach (var invoiceItem in invoiceDto.InvoiceItems)
            {
                var invoiceItemElement = new XElement("InvoiceItem",
                                                      new XAttribute("OrderPositionCode", invoiceItem.OrderPositionCode),
                                                      new XAttribute("PriceListPositionCode", invoiceItem.PriceListPositionCode),
                                                      new XAttribute("NomenclatureElementCode", invoiceItem.NomenclatureElementCode),
                                                      new XAttribute("PricePerUnitWithVat", invoiceItem.PricePerUnitWithVat),
                                                      new XAttribute("Amount", invoiceItem.Amount),
                                                      new XAttribute("DiscountPercent", invoiceItem.DiscountPercent),
                                                      new XAttribute("CategoryRate", invoiceItem.CategoryRate));
                invoiceItemElement.Add(GetSubInvoiceItemsElement(invoiceItem));
                invoiceItemElement.Add(GetReleasesWithdrawalsElement(invoiceItem));
                invoiceItemsElement.Add(invoiceItemElement);
            }

            return invoiceItemsElement;
        }

        private static XElement GetSubInvoiceItemsElement(InvoiceItemDto invoiceItemDto)
        {
            var subInvoiceItemsElement = new XElement("SubInvoiceItems");
            foreach (var subInvoiceItem in invoiceItemDto.SubInvoiceItems.Where(x => x.LinkObjects.Any()))
            {
                var subInvoiceItemElement = new XElement("SubInvoiceItem",
                                                         new XAttribute("NomenclatureElementCode", subInvoiceItem.NomenclatureElementCode));

                foreach (var linkObject in subInvoiceItem.LinkObjects.Where(lo => lo.IsLinkedWithExportableLinkingObject))
                {
                    subInvoiceItemElement.Add(GetLinkObjectElement(linkObject));
                }

                subInvoiceItemsElement.Add(subInvoiceItemElement);
            }

            return subInvoiceItemsElement;
        }

        private static XElement GetReleasesWithdrawalsElement(InvoiceItemDto invoiceItemDto)
        {
            var releasesWithdrawalsElement = new XElement("PlannedDebits");
            foreach (var releasesWithdrawalItem in invoiceItemDto.ReleasesWithdrawals)
            {
                var releasesWithdrawalItemElement = new XElement("PlannedDebit",
                                                                 new XAttribute("StartDate", releasesWithdrawalItem.ReleaseBeginDate),
                                                                 new XAttribute("EndDate", releasesWithdrawalItem.ReleaseEndDate),
                                                                 new XAttribute("Amount", releasesWithdrawalItem.AmountToWithdraw));

                releasesWithdrawalsElement.Add(releasesWithdrawalItemElement);
            }

            return releasesWithdrawalsElement;
        }

        private static XElement GetLinkObjectElement(LinkObjectDto linkObject)
        {
            var objectLinkElement = new XElement("LinkObject");

            if (linkObject.RubricCode.HasValue)
            {
                objectLinkElement.Add(new XAttribute("RubricCode", linkObject.RubricCode.Value));
            }

            if (linkObject.CardCode.HasValue)
            {
                objectLinkElement.Add(new XAttribute("CardCode", linkObject.CardCode.Value));
            }

            return objectLinkElement;
        }

        #region nested types

        public sealed class InvoiceDto : IExportableEntityDto
        {
            public long Id { get; set; }
            public string Number { get; set; }
            public long FirmCode { get; set; }
            public string FirmName { get; set; }
            public int BranchSrcCode { get; set; }
            public int BranchDestCode { get; set; }
            public long? LegalEntityCode { get; set; }
            public long? LegalEntityBranchCode { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? ApprovedDate { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime EndDatePlan { get; set; }
            public DateTime SignupDate { get; set; }
            public OrderState Status { get; set; }
            public OrderType OrderType { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public IEnumerable<InvoiceItemDto> InvoiceItems { get; set; }
            public long OwnerCode { get; set; }
            public string UserCode { get; set; }
            public IEnumerable<SalesModel> SalesModels { get; set; }
        }

        public sealed class InvoiceItemDto
        {
            public long OrderPositionCode { get; set; }
            public long PriceListPositionCode { get; set; }
            public long NomenclatureElementCode { get; set; }
            public decimal PricePerUnitWithVat { get; set; }
            public int Amount { get; set; }
            public decimal DiscountPercent { get; set; }
            public decimal CategoryRate { get; set; }
            public IEnumerable<SubInvoiceItemDto> SubInvoiceItems { get; set; }
            public IEnumerable<ReleasesWithdrawalDto> ReleasesWithdrawals { get; set; }
        }

        public sealed class SubInvoiceItemDto
        {
            public long NomenclatureElementCode { get; set; }
            public IEnumerable<LinkObjectDto> LinkObjects { get; set; }
        }

        public sealed class LinkObjectDto
        {
            public bool IsLinkedWithExportableLinkingObject { get; set; }
            public long? RubricCode { get; set; }
            public long? CardCode { get; set; }
        }

        public sealed class ReleasesWithdrawalDto
        {
            public DateTime ReleaseBeginDate { get; set; }
            public DateTime ReleaseEndDate { get; set; }
            public decimal AmountToWithdraw { get; set; }
        }

        #endregion
    }
}
