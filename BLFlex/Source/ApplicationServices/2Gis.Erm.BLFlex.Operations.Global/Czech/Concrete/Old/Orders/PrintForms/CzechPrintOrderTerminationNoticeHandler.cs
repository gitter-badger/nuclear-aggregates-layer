using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    public class CzechPrintOrderTerminationNoticeHandler : RequestHandler<PrintOrderTerminationNoticeRequest, Response>, ICzechAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;

        public CzechPrintOrderTerminationNoticeHandler(ISubRequestProcessor requestProcessor, IFinder finder)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
        }

        protected override Response Handle(PrintOrderTerminationNoticeRequest request)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                    {
                        OrderState = order.WorkflowStepId,
                        order.IsTerminated,
                        order.BranchOfficeOrganizationUnitId,
                        order.Number,
                        CurrencyISOCode = order.Currency.ISOCode,
                        LegalPersonType = order.LegalPerson.LegalPersonTypeEnum,
                    })
                .Single();

            if (!orderInfo.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);
            }

            if (orderInfo.OrderState != OrderState.OnTermination && orderInfo.OrderState != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            var printRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = orderInfo.CurrencyISOCode,
                    TemplateCode = GetTemplateCode(orderInfo.LegalPersonType, request.WithoutReason, request.TerminationBargain),
                    FileName = string.Format(BLResources.PrintTerminationNoticeFileNameFormat, orderInfo.Number),
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
                    PrintData = GetPrintData(request.OrderId)
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        protected PrintData GetPrintData(long orderId)
        {
            var data = _finder.Find(Specs.Find.ById<Order>(orderId))
                              .Select(order => new
                                  {
                                      Order = order,
                                      order.LegalPersonId,
                                      order.BranchOfficeOrganizationUnit.BranchOfficeId
                                  })
                              .Single();

            var branchOfficeOrganizationUnit = _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.ByOrderId(orderId));
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(data.LegalPersonId.Value));

            return new PrintData
                {
                    { "BranchOffice", CzechPrintHelper.BranchOfficeFields(_finder.FindOne(Specs.Find.ById<BranchOffice>(data.BranchOfficeId))) },
                    { "BranchOfficeOrganizationUnit", CzechPrintHelper.BranchOfficeOrganizationUnitFieldsForTerminationNotice(branchOfficeOrganizationUnit) },
                    { "LegalPerson", CzechPrintHelper.LegalPersonFields(legalPerson) },
                    { "Order", CzechPrintHelper.OrderFields(data.Order) }
                };
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType, bool withoutReason, bool terminationBargain)
        {
            return terminationBargain
                       ? GetTerminationBargainTemplateCode(legalPersonType, withoutReason)
                       : GetTerminationOrderTemplateCode(legalPersonType, withoutReason);
        }

        private static TemplateCode GetTerminationBargainTemplateCode(LegalPersonType legalPersonType, bool withoutReason)
        {
            if (withoutReason)
            {
                switch (legalPersonType)
                {
                    case LegalPersonType.LegalPerson:
                        return TemplateCode.TerminationNoticeBargainWithoutReasonLegalPerson;

                    case LegalPersonType.Businessman:
                        return TemplateCode.TerminationNoticeBargainWithoutReasonBusinessman;

                    default:
                        throw new ArgumentOutOfRangeException("legalPersonType");
                }
            }

            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.TerminationNoticeBargainLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.TerminationNoticeBargainBusinessman;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }

        private static TemplateCode GetTerminationOrderTemplateCode(LegalPersonType legalPersonType, bool withoutReason)
        {
            if (withoutReason)
            {
                switch (legalPersonType)
                {
                    case LegalPersonType.LegalPerson:
                        return TemplateCode.TerminationNoticeWithoutReasonLegalPerson;

                    case LegalPersonType.Businessman:
                        return TemplateCode.TerminationNoticeWithoutReasonBusinessman;

                    default:
                        throw new ArgumentOutOfRangeException("legalPersonType");
                }
            }

            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.TerminationNoticeLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.TerminationNoticeBusinessman;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }
    }
}