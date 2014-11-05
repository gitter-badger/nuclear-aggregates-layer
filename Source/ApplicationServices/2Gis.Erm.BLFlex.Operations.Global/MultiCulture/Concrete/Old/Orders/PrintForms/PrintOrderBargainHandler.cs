using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderBargainHandler : RequestHandler<PrintOrderBargainRequest, Response>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        private readonly IBargainPrintFormReadModel _readModel;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IBargainPrintFormDataExtractor _dataExtractor;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IOrderReadModel _orderReadModel;
        private readonly MultiCulturePrintHelper _printHelper;

        public PrintOrderBargainHandler(IBargainPrintFormReadModel readModel,
                                        IBranchOfficeReadModel branchOfficeReadModel,
                                        ILegalPersonReadModel legalPersonReadModel,
                                        IBargainPrintFormDataExtractor dataExtractor,
                                        ISubRequestProcessor requestProcessor,
                                        IFormatterFactory formatterFactory,
                                        IOrderReadModel orderReadModel)
        {
            _readModel = readModel;
            _dataExtractor = dataExtractor;
            _requestProcessor = requestProcessor;
            _orderReadModel = orderReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _legalPersonReadModel = legalPersonReadModel;
            _printHelper = new MultiCulturePrintHelper(formatterFactory);
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            // checkme: ������ ��� ������?
            var bargainId = request.BargainId ?? _orderReadModel.GetBargainIdByOrder(request.OrderId.Value);

            if (bargainId == null)
            {
                throw new EntityNotLinkedException(typeof(Order), request.OrderId.Value, typeof(Bargain));
            }

            var relations = _readModel.GetBargainRelationsDto(bargainId.Value);
            var printData = GetPrintData(request, relations, bargainId.Value);

            if (relations.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException(BLResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            var printRequest = new PrintDocumentRequest
                                   {
                                       BranchOfficeOrganizationUnitId = relations.BranchOfficeOrganizationUnitId,
                                       TemplateCode = relations.BargainKind == BargainKind.Client ? TemplateCode.ClientBargain : TemplateCode.AgentBargain,
                                       FileName = relations.BargainNumber,
                                       PrintData = printData
                                   };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        private PrintData GetPrintData(PrintOrderBargainRequest request, BargainRelationsDto relations, long bargainId)
            {
            var profileId = request.LegalPersonProfileId;

            var legalPerson = _legalPersonReadModel.GetLegalPerson(relations.LegalPersonId.Value);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(profileId);
            var boou = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(relations.BranchOfficeOrganizationUnitId.Value);

            var bargainQuery = _readModel.GetBargainQuery(bargainId);
            var branchOfficeQuery = _readModel.GetBranchOfficeQuery(bargainId);

            var printData = PrintData.Concat(_dataExtractor.GetBargain(bargainQuery),
                                             _dataExtractor.GetLegalPersonProfile(profile),
                                             _dataExtractor.GetLegalPerson(legalPerson),
                                             _dataExtractor.GetBranchOfficeOrganizationUnit(boou),
                                             _dataExtractor.GetBranchOffice(branchOfficeQuery),
                                             _dataExtractor.GetUngroupedFields(bargainQuery));

            printData.GetObject("LegalPersonProfile")
                     .Add("OperatesOnTheBasisInGenitive", _printHelper.GetOperatesOnTheBasisInGenitive(profile, (LegalPersonType)legalPerson.LegalPersonTypeEnum));

            return printData;
        }
    }
}
