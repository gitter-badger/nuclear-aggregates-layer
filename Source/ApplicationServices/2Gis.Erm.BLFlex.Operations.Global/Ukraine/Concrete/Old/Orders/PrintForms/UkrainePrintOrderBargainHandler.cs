using System;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Order;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete.Old.Orders.PrintForms
{
    public sealed class UkrainePrintOrderBargainHandler : RequestHandler<PrintOrderBargainRequest, Response>, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly UkrainePrintHelper _ukrainePrintHelper;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPrintValidationOperationService _validationService;

        public UkrainePrintOrderBargainHandler(IFinder finder,
                                               ISubRequestProcessor requestProcessor,
                                               ILegalPersonReadModel legalPersonReadModel,
                                               IBranchOfficeReadModel branchOfficeReadModel,
                                               IFormatterFactory formatterFactory,
                                               IOrderReadModel orderReadModel,
                                               IPrintValidationOperationService validationService)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _orderReadModel = orderReadModel;
            _validationService = validationService;
            _ukrainePrintHelper = new UkrainePrintHelper(formatterFactory);
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

            var bargainInfo = _finder.Find(Specs.Find.ById<Bargain>(bargainId))
                                     .Select(x => new
                                                      {
                                                          Bargain = x,
                                                          LegalPersonId = x.CustomerLegalPersonId,
                                                          OrganizationUnitName = x.BranchOfficeOrganizationUnit.OrganizationUnit.Name,
                                                          BranchOfficeOrganizationUnitId = x.ExecutorBranchOfficeId,
                                                          x.BranchOfficeOrganizationUnit.BranchOfficeId,
                                                          LegalPersonType = (LegalPersonType)x.LegalPerson.LegalPersonTypeEnum,
                                                      })
                                     .Single();

            var profile = _legalPersonReadModel.GetLegalPersonProfile(legalPersonProfileId);
            var legalPerson = _legalPersonReadModel.GetLegalPerson(bargainInfo.LegalPersonId);
            var branchOffice = _branchOfficeReadModel.GetBranchOffice(bargainInfo.BranchOfficeId);
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(bargainInfo.BranchOfficeOrganizationUnitId));

            var printData = new PrintData
                                {
                                    { "Bargain", GetBargainFields(bargainInfo.Bargain) },
                                    { "Profile", UkrainePrintHelper.LegalPersonProfileFields(profile) },
                                    { "LegalPerson", UkrainePrintHelper.LegalPersonFields(legalPerson) },
                                    { "BranchOffice", UkrainePrintHelper.BranchOfficeFields(branchOffice) },
                                    { "BranchOfficeOrganizationUnit", UkrainePrintHelper.BranchOfficeOrganizationUnitFields(branchOfficeOrganizationUnit) },
                                    { "OperatesOnTheBasisInGenitive", _ukrainePrintHelper.GetOperatesOnTheBasisInGenitive(profile) },
                                    { "OrganizationUnitName", bargainInfo.OrganizationUnitName },
                                };

            return
                _requestProcessor.HandleSubRequest(
                                                   new PrintDocumentRequest
                                                       {
                                                           BranchOfficeOrganizationUnitId = bargainInfo.BranchOfficeOrganizationUnitId,
                                                           TemplateCode = TemplateCode.ClientBargain,
                                                           FileName = bargainInfo.Bargain.Number,
                                                           PrintData = printData
                                                       },
                                                   Context);
        }

        private PrintData GetBargainFields(Bargain bargain)
        {
            return new PrintData
                {
                    { "Number", bargain.Number },
                    { "SignedOn", bargain.SignedOn },
                };
        }
    }
}
