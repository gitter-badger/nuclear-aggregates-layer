using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete.Old.Orders.PrintForms
{
    public class UkrainePrintOrderAdditionalAgreementHandler : RequestHandler<PrintOrderAdditionalAgreementRequest, Response>, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModelReadModel;
        private readonly UkrainePrintHelper _ukrainePrintHelper;

        public UkrainePrintOrderAdditionalAgreementHandler(ISubRequestProcessor requestProcessor,
                                                           IFinder finder,
                                                           ILegalPersonReadModel legalPersonReadModel,
                                                           IBranchOfficeReadModel branchOfficeReadModelReadModel,
                                                           IFormatterFactory formatterFactory)
        {
            _finder = finder;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModelReadModel = branchOfficeReadModelReadModel;
            _requestProcessor = requestProcessor;
            _ukrainePrintHelper = new UkrainePrintHelper(formatterFactory);
        }

        protected override Response Handle(PrintOrderAdditionalAgreementRequest request)
        {
            var orderInfoValidation =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                    .Select(order => new { WorkflowStep = order.WorkflowStepId, order.IsTerminated, order.RejectionDate })
                    .Single();

            if (!orderInfoValidation.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);
            }

            if (orderInfoValidation.WorkflowStep != OrderState.OnTermination && orderInfoValidation.WorkflowStep != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            if (orderInfoValidation.RejectionDate == null)
            {
                throw new NotificationException(BLResources.OrderRejectDateFieldIsNotFilled);
            }

            var orderInfo =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                       .Select(order => new
                           {
                               Order = order,
                               LegalPersonId = order.LegalPersonId.Value,
                               order.BranchOfficeOrganizationUnitId,
                               order.BranchOfficeOrganizationUnit.BranchOfficeId,
                               order.Bargain,
                               order.RejectionDate,
                               OrganizationUnitName = order.DestOrganizationUnit.Name,
                               OrderNumber = order.Number,
                               CurrencyISOCode = order.Currency.ISOCode,
                               LegalPersonType = order.LegalPerson.LegalPersonTypeEnum,
                               order.LegalPersonProfileId,
                           })
                       .Single();

            if (orderInfo.LegalPersonProfileId == null)
            {
                throw new FieldNotSpecifiedException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            var profile = _legalPersonReadModel.GetLegalPersonProfile(orderInfo.LegalPersonProfileId.Value);
            var legalPerson = _legalPersonReadModel.GetLegalPerson(orderInfo.LegalPersonId);
            var branchOffice = _branchOfficeReadModelReadModel.GetBranchOffice(orderInfo.BranchOfficeId);
            var branchOfficeOrganizationUnit = orderInfo.BranchOfficeOrganizationUnitId.HasValue
                ? _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(orderInfo.BranchOfficeOrganizationUnitId.Value))
                : null;

            var printData = new PrintData
                {
                    { "BranchOffice", UkrainePrintHelper.BranchOfficeFields(branchOffice) },
                    { "BranchOfficeOrganizationUnit", UkrainePrintHelper.BranchOfficeOrganizationUnitFields(branchOfficeOrganizationUnit) },
                    { "LegalPerson", UkrainePrintHelper.LegalPersonFields(legalPerson) },
                    { "Profile", UkrainePrintHelper.LegalPersonProfileFields(profile) },
                    { "Order", UkrainePrintHelper.OrderFields(orderInfo.Order) },
                    { "OperatesOnTheBasisInGenitive", _ukrainePrintHelper.GetOperatesOnTheBasisInGenitive(profile) },
                    { "RelatedBargainInfo", _ukrainePrintHelper.GetRelatedBargainInfo(orderInfo.Bargain) },
                    { "NextReleaseDate", orderInfo.RejectionDate.Value.AddMonths(1).GetFirstDateOfMonth() },
                    { "OrganizationUnitName", orderInfo.OrganizationUnitName },
                };

            var printRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = orderInfo.CurrencyISOCode,
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
                    TemplateCode = GetTemplateCode(orderInfo.LegalPersonType),
                    FileName = string.Format(BLResources.PrintAdditionalAgreementFileNameFormat, orderInfo.OrderNumber),
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.AdditionalAgreementLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.AdditionalAgreementBusinessman;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }
    }
}