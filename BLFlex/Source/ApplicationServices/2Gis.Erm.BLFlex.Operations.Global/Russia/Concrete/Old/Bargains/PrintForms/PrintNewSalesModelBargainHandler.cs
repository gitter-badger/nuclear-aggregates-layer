using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Bargains.PrintForms
{
    public sealed class PrintNewSalesModelBargainHandler : RequestHandler<PrintNewSalesModelBargainRequest, Response>, IRussiaAdapted
    {
        private readonly IBargainPrintFormReadModel _readModel;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IBargainPrintFormDataExtractor _dataExtractor;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IOrderReadModel _orderReadModel;

        public PrintNewSalesModelBargainHandler(IBargainPrintFormReadModel readModel,
                                                IBranchOfficeReadModel branchOfficeReadModel,
                                                ILegalPersonReadModel legalPersonReadModel,
                                                IBargainPrintFormDataExtractor dataExtractor,
                                                ISubRequestProcessor requestProcessor,
                                                IOrderReadModel orderReadModel)
        {
            _readModel = readModel;
            _dataExtractor = dataExtractor;
            _requestProcessor = requestProcessor;
            _orderReadModel = orderReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override Response Handle(PrintNewSalesModelBargainRequest request)
        {
            var bargainId = request.BargainId ?? _orderReadModel.GetBargainIdByOrder(request.OrderId.Value);
            var relations = _readModel.GetBargainRelationsDto(bargainId.Value);

            if (relations.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException(BLResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            var legalPersonProfileId = request.LegalPersonProfileId ?? _orderReadModel.GetLegalPersonProfileIdByOrder(request.OrderId.Value);
            if (legalPersonProfileId == null)
            {
                throw new LegalPersonProfileMustBeSpecifiedException();
            }

            var printData = GetPrintData(relations, bargainId.Value, legalPersonProfileId.Value);

            var printRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = relations.CurrencyIsoCode,
                    BranchOfficeOrganizationUnitId = relations.BranchOfficeOrganizationUnitId,
                    TemplateCode = TemplateCode.BargainNewSalesModel,
                    FileName = relations.BargainNumber,
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        private PrintData GetPrintData(BargainRelationsDto relations, long bargainId, long legalPersonProfileId)
        {
            var legalPerson = _legalPersonReadModel.GetLegalPerson(relations.LegalPersonId.Value);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(legalPersonProfileId);
            var boou = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(relations.BranchOfficeOrganizationUnitId.Value);

            var bargainQuery = _readModel.GetBargainQuery(bargainId);
            var branchOfficeQuery = _readModel.GetBranchOfficeQuery(bargainId);

            return PrintData.Concat(_dataExtractor.GetBargain(bargainQuery), 
                                    _dataExtractor.GetLegalPersonProfile(profile),
                                    _dataExtractor.GetLegalPerson(legalPerson),
                                    _dataExtractor.GetBranchOfficeOrganizationUnit(boou),
                                    _dataExtractor.GetBranchOffice(branchOfficeQuery),
                                    _dataExtractor.GetUngroupedFields(bargainQuery));
        }
    }
}
