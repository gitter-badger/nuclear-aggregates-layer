using System;

using DoubleGis.Erm.BL.API.Operations.Concrete.Order;
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
    // FIXME {a.rechkalov, 10.11.2014}: IPrintValidationOperationService
    public sealed class PrintOrderBargainHandler : RequestHandler<PrintOrderBargainRequest, Response>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        private readonly IBargainPrintFormReadModel _readModel;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IBargainPrintFormDataExtractor _dataExtractor;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IOrderReadModel _orderReadModel;
        private readonly MultiCulturePrintHelper _printHelper;
        private readonly IPrintValidationOperationService _validationService;

        public PrintOrderBargainHandler(IBargainPrintFormReadModel readModel,
                                        IBranchOfficeReadModel branchOfficeReadModel,
                                        ILegalPersonReadModel legalPersonReadModel,
                                        IBargainPrintFormDataExtractor dataExtractor,
                                        ISubRequestProcessor requestProcessor,
                                        IFormatterFactory formatterFactory,
                                        IOrderReadModel orderReadModel,
                                        IPrintValidationOperationService validationService)
        {
            _readModel = readModel;
            _dataExtractor = dataExtractor;
            _requestProcessor = requestProcessor;
            _orderReadModel = orderReadModel;
            _validationService = validationService;
            _branchOfficeReadModel = branchOfficeReadModel;
            _legalPersonReadModel = legalPersonReadModel;
            _printHelper = new MultiCulturePrintHelper(formatterFactory);
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            long bargainId;
            long legalPersonProfileId;
            if (request.BargainId.HasValue && request.LegalPersonProfileId.HasValue)
            {
                bargainId = request.BargainId.Value;
                legalPersonProfileId = request.LegalPersonProfileId.Value;
            }
            else if (request.OrderId.HasValue)
            {
                _validationService.ValidateOrderForBargain(request.OrderId.Value);
                bargainId = _orderReadModel.GetOrderBargainId(request.OrderId.Value);
                legalPersonProfileId = _orderReadModel.GetOrderLegalPersonProfileId(request.OrderId.Value);
            }
            else
            {
                // ReSharper disable once LocalizableElement
                throw new ArgumentException("Запрос дожен содержать BargainId & LegalPersonProfileId или OrderId", "request");
            }

            var relations = _readModel.GetBargainRelationsDto(bargainId);
            var printData = GetPrintData(relations, bargainId, legalPersonProfileId);

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

        private PrintData GetPrintData(BargainRelationsDto relations, long bargainId, long legalPersonProfileId)
        {
            var legalPerson = _legalPersonReadModel.GetLegalPerson(relations.LegalPersonId.Value);
            var profile = _legalPersonReadModel.GetLegalPersonProfile(legalPersonProfileId);
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
