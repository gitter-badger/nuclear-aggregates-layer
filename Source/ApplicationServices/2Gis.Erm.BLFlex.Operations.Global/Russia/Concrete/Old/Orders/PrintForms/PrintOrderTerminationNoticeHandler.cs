using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderTerminationNoticeHandler : RequestHandler<PrintOrderTerminationNoticeRequest, Response>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _longDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintOrderTerminationNoticeHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
        }

        protected override Response Handle(PrintOrderTerminationNoticeRequest request)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                                 {
                                     OrderState = (OrderState)order.WorkflowStepId,
                                     order.IsTerminated
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

            var printData = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                                 {
                                     Order = order,
                                     order.Bargain,
                                     order.EndDistributionDateFact,
                                     order.LegalPerson,
                                     Profile = order.LegalPerson.LegalPersonProfiles.FirstOrDefault(y => request.LegalPersonProfileId.HasValue && y.Id == request.LegalPersonProfileId),
                                     order.BranchOfficeOrganizationUnit,
                                     CurrencyISOCode = order.Currency.ISOCode,
                                     LegalPersonType = (LegalPersonType)order.LegalPerson.LegalPersonTypeEnum,
                                     order.BranchOfficeOrganizationUnitId,
                                     order.BranchOfficeOrganizationUnit.BranchOffice
                                 })
                .AsEnumerable()
                .Select(x => new
                             {
                                 x.Order,
                                 RelatedBargainInfo = (x.Bargain != null) ?
                                    string.Format(BLResources.RelatedToBargainInfoTemplate, x.Bargain.Number, _longDateFormatter.Format(x.Bargain.CreatedOn)) 
                                    : null,
                                 TerminationDate = x.EndDistributionDateFact.AddDays(1),
                                 x.LegalPerson,
                                 x.Profile,
                                 x.BranchOfficeOrganizationUnit,
                                 x.CurrencyISOCode,
                                 x.LegalPersonType,
                                 x.BranchOfficeOrganizationUnitId,
                                 x.BranchOffice
                             })
                .Single();

            return _requestProcessor.HandleSubRequest(new PrintDocumentRequest
                                                      {
                                                          CurrencyIsoCode = printData.CurrencyISOCode,
                                                          TemplateCode = GetTemplateCode(printData.LegalPersonType),
                                                          FileName = string.Format(BLResources.PrintTerminationNoticeFileNameFormat, printData.Order.Number),
                                                          BranchOfficeOrganizationUnitId = printData.BranchOfficeOrganizationUnitId,
                                                          PrintData = printData
                                                      }, 
                                                      Context);
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.TerminationNoticeLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.TerminationNoticeBusinessman;

                case LegalPersonType.NaturalPerson:
                    return TemplateCode.TerminationNoticeNaturalPerson;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }
    }
}