using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
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
using NuClear.Storage.Futures.Queryable;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete.Old.Orders.PrintForms
{
    public sealed class UkrainePrintOrderTerminationNoticeHandler : RequestHandler<PrintOrderTerminationNoticeRequest, Response>, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public UkrainePrintOrderTerminationNoticeHandler(ISubRequestProcessor requestProcessor,
                                                         IFinder finder,
                                                         ILegalPersonReadModel legalPersonReadModel)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override Response Handle(PrintOrderTerminationNoticeRequest request)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                                   .Map(q => q.Select(order => new
                                       {
                                           OrderState = order.WorkflowStepId,
                                           order.IsTerminated,
                                           Order = order,
                                           order.EndDistributionDateFact,
                                           LegalPersonId = order.LegalPersonId.Value,
                                           order.BranchOfficeOrganizationUnitId,
                                           CurrencyISOCode = order.Currency.ISOCode,
                                           LegalPersonType = order.LegalPerson.LegalPersonTypeEnum,
                                           order.LegalPersonProfileId,
                                       }))
                                   .Many()
                                   .Select(x => new
                                       {
                                           x.Order,
                                           x.IsTerminated,
                                           x.OrderState,
                                           TerminationDate = x.EndDistributionDateFact.AddDays(1),
                                           x.LegalPersonId,
                                           x.BranchOfficeOrganizationUnitId,
                                           x.CurrencyISOCode,
                                           x.LegalPersonType,
                                           x.LegalPersonProfileId,
                                       })
                                   .Single();

            if (orderInfo.LegalPersonProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            if (!orderInfo.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);
            }

            if (orderInfo.OrderState != OrderState.OnTermination && orderInfo.OrderState != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            var legalPerson = _legalPersonReadModel.GetLegalPerson(orderInfo.LegalPersonId);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(orderInfo.LegalPersonProfileId.Value);
            var branchOfficeOrganizationUnit = orderInfo.BranchOfficeOrganizationUnitId.HasValue
                ? _finder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(orderInfo.BranchOfficeOrganizationUnitId.Value)).One()
                : null;

            var printData = new PrintData
                {
                    { "LegalPerson", UkrainePrintHelper.LegalPersonFields(legalPerson) },
                    { "Profile", UkrainePrintHelper.LegalPersonProfileFields(profile) },
                    { "Order", UkrainePrintHelper.OrderFields(orderInfo.Order) },
                    { "BranchOfficeOrganizationUnit", UkrainePrintHelper.BranchOfficeOrganizationUnitFields(branchOfficeOrganizationUnit) },
                    { "TerminationDate", orderInfo.TerminationDate },
                };

            return _requestProcessor.HandleSubRequest(
                new PrintDocumentRequest
                    {
                        CurrencyIsoCode = orderInfo.CurrencyISOCode,
                        TemplateCode = GetTemplateCode(orderInfo.LegalPersonType),
                        FileName = string.Format(BLResources.PrintTerminationNoticeFileNameFormat, orderInfo.Order.Number),
                        BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
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

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }
    }
}