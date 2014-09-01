using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintNewSalesModelBargainHandler : RequestHandler<PrintNewSalesModelBargainRequest, Response>, IRussiaAdapted
    {
        private readonly IBargainPrintFormReadModel _readModel;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IBargainPrintFormDataExtractor _dataExtractor;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public PrintNewSalesModelBargainHandler(IBargainPrintFormReadModel readModel,
                                                IBranchOfficeReadModel branchOfficeReadModel,
                                                ILegalPersonReadModel legalPersonReadModel,
                                                IBargainPrintFormDataExtractor dataExtractor,
                                                ISubRequestProcessor requestProcessor)
        {
            _readModel = readModel;
            _dataExtractor = dataExtractor;
            _requestProcessor = requestProcessor;
            _branchOfficeReadModel = branchOfficeReadModel;
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override Response Handle(PrintNewSalesModelBargainRequest request)
        {
            var relations = _readModel.GetBargainRelationsDto(request.OrderId);

            if (relations.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException(BLFlexResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            if (relations.LegalPersonProfileId == null)
            {
                throw new LegalPersonProfileMustBeSpecifiedException();
            }
            
            var printData = GetPrintData(request, relations);


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

        private PrintData GetPrintData(PrintNewSalesModelBargainRequest request, BargainRelationsDto relations)
        {
            var legalPerson = _legalPersonReadModel.GetLegalPerson(relations.LegalPersonId.Value);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(relations.LegalPersonProfileId.Value);
            var boou = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(relations.BranchOfficeOrganizationUnitId.Value);

            var bargainQuery = _readModel.GetBargainQuery(request.OrderId);
            var branchOfficeQuery = _readModel.GetBranchOfficeQuery(request.OrderId);
            var orderQuery = _readModel.GetOrderQuery(request.OrderId);

            return PrintData.Concat(_dataExtractor.GetBargain(bargainQuery), 
                                    _dataExtractor.GetLegalPersonProfile(profile),
                                    _dataExtractor.GetLegalPerson(legalPerson),
                                    _dataExtractor.GetBranchOfficeOrganizationUnit(boou),
                                    _dataExtractor.GetBranchOffice(branchOfficeQuery),
                                    _dataExtractor.GetUngroupedFields(orderQuery));
        }
    }
}
