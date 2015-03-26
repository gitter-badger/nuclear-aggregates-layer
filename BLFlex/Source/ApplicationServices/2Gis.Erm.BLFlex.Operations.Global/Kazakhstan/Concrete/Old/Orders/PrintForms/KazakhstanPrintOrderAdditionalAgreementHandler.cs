using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.Orders.PrintForms
{
    public class KazakhstanPrintOrderAdditionalAgreementHandler : RequestHandler<PrintOrderAdditionalAgreementRequest, Response>, IKazakhstanAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModelReadModel;
        private readonly IOrderPrintFormDataExtractor _orderPrintFormDataExtractor;
        private readonly ILocalizationSettings _localizationSettings;

        public KazakhstanPrintOrderAdditionalAgreementHandler(ISubRequestProcessor requestProcessor,
                                                              IFinder finder,
                                                              ILegalPersonReadModel legalPersonReadModel,
                                                              IBranchOfficeReadModel branchOfficeReadModelReadModel,
                                                              IOrderPrintFormDataExtractor orderPrintFormDataExtractor,
                                                              ILocalizationSettings localizationSettings)
        {
            _finder = finder;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModelReadModel = branchOfficeReadModelReadModel;
            _orderPrintFormDataExtractor = orderPrintFormDataExtractor;
            _localizationSettings = localizationSettings;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintOrderAdditionalAgreementRequest request)
        {
            var orderInfoValidation =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                    .Select(order => new { WorkflowStep = (OrderState)order.WorkflowStepId, order.IsTerminated, order.RejectionDate })
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
                               LegalPersonType = (LegalPersonType)order.LegalPerson.LegalPersonTypeEnum,
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
            var branchOfficeOrganizationUnit = _branchOfficeReadModelReadModel.GetBranchOfficeOrganizationUnit(orderInfo.BranchOfficeOrganizationUnitId.Value);
                
            var printData = new PrintData
                {
                    { "BranchOffice", PrintHelper.BranchOfficeFields(branchOffice) },
                    { "BranchOfficeOrganizationUnit", PrintHelper.BranchOfficeOrganizationUnitFields(branchOfficeOrganizationUnit) },
                    { "Order", PrintHelper.OrderFields(orderInfo.Order) },
                    { "AuthorityDocument", PrintHelper.GetAuthorityDocumentDescription(profile, _localizationSettings.ApplicationCulture) },
                    { "NextReleaseDate", orderInfo.RejectionDate.Value.GetNextMonthFirstDate() },
                    { "OrganizationUnitName", orderInfo.OrganizationUnitName },
                    { "Profile", PrintHelper.LegalPersonProfileFields(profile) },
                };

            printData = PrintData.Concat(printData,
                                         _orderPrintFormDataExtractor.GetBargain(orderInfo.Bargain),
                                         _orderPrintFormDataExtractor.GetLegalPersonData(legalPerson));

            var printRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = orderInfo.CurrencyISOCode,
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
                    TemplateCode = TemplateCode.AdditionalAgreementLegalPerson,
                    FileName = string.Format(BLResources.PrintAdditionalAgreementFileNameFormat, orderInfo.OrderNumber),
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }
    }
}