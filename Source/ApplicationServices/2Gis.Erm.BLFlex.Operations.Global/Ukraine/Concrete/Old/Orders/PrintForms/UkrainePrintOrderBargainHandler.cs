using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
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

        public UkrainePrintOrderBargainHandler(IFinder finder,
                                               ISubRequestProcessor requestProcessor,
                                               ILegalPersonReadModel legalPersonReadModel,
                                               IBranchOfficeReadModel branchOfficeReadModel,
                                               IFormatterFactory formatterFactory)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _ukrainePrintHelper = new UkrainePrintHelper(formatterFactory);
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                                   .Select(order => new
                           {
                               order.Bargain,
                               LegalPersonId = order.LegalPersonId.Value,
                               OrganizationUnitName = order.DestOrganizationUnit.Name,
                                           order.BranchOfficeOrganizationUnitId,
                               order.BranchOfficeOrganizationUnit.BranchOfficeId,
                               CurrencyIsoCode = order.Currency.ISOCode,
                               LegalPersonType = (LegalPersonType)order.LegalPerson.LegalPersonTypeEnum,
                           })
                       .Single();

            var profile = _legalPersonReadModel.GetLegalPersonProfile(request.LegalPersonProfileId.Value);
            var legalPerson = _legalPersonReadModel.GetLegalPerson(orderInfo.LegalPersonId);
            var branchOffice = _branchOfficeReadModel.GetBranchOffice(orderInfo.BranchOfficeId);
            var branchOfficeOrganizationUnit = orderInfo.BranchOfficeOrganizationUnitId.HasValue
                ? _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(orderInfo.BranchOfficeOrganizationUnitId.Value))
                : null;

            var printData = new PrintData
                {
                    { "Bargain", GetBargainFields(orderInfo.Bargain) },
                    { "Profile", UkrainePrintHelper.LegalPersonProfileFields(profile) },
                    { "LegalPerson", UkrainePrintHelper.LegalPersonFields(legalPerson) },
                    { "BranchOffice", UkrainePrintHelper.BranchOfficeFields(branchOffice) },
                    { "BranchOfficeOrganizationUnit", UkrainePrintHelper.BranchOfficeOrganizationUnitFields(branchOfficeOrganizationUnit) },
                    { "OperatesOnTheBasisInGenitive", _ukrainePrintHelper.GetOperatesOnTheBasisInGenitive(profile) },
                    { "OrganizationUnitName", orderInfo.OrganizationUnitName },
                };

            return
                _requestProcessor.HandleSubRequest(
                    new PrintDocumentRequest
                    {
                        CurrencyIsoCode = orderInfo.CurrencyIsoCode,
                        BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
                        TemplateCode = GetTemplateCode(orderInfo.LegalPersonType),
                        FileName = orderInfo.Bargain.Number,
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

        private static TemplateCode GetTemplateCode(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return TemplateCode.BargainLegalPerson;

                case LegalPersonType.Businessman:
                    return TemplateCode.BargainBusinessman;

                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }
    }
}
