using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Order;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.Orders.PrintForms
{
    // FIXME {a.rechkalov, 10.11.2014}: IPrintValidationOperationService
    public sealed class KazakhstanPrintOrderBargainHandler : RequestHandler<PrintOrderBargainRequest, Response>, IKazakhstanAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILocalizationSettings _localizationSettings;
        private readonly IPrintValidationOperationService _validationService;

        public KazakhstanPrintOrderBargainHandler(
            IFinder finder,
            ISubRequestProcessor requestProcessor,
            ILocalizationSettings localizationSettings,
            IPrintValidationOperationService validationService)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _localizationSettings = localizationSettings;
            _validationService = validationService;
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            var legalPersonProfileId = request.LegalPersonProfileId 
                ?? _finder.Find(Specs.Find.ById<Order>(request.OrderId.Value)).Select(order => order.LegalPersonProfileId).SingleOrDefault();

            var bargainSpecification = request.BargainId.HasValue
                                           ? Specs.Find.ById<Bargain>(request.BargainId.Value)
                                           : OrderSpecs.Bargains.Find.ByOrder(request.OrderId.Value);
            var bargain = _finder.FindOne(bargainSpecification);
            var branchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(bargain.ExecutorBranchOfficeId));
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(bargain.CustomerLegalPersonId));
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId.Value));
            var branchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(branchOfficeOrganizationUnit.BranchOfficeId));
            var organizationUnit = _finder.FindOne(Specs.Find.ById<OrganizationUnit>(branchOfficeOrganizationUnit.OrganizationUnitId));

            var printData = new PrintData
                {
                    { "OrganizationUnitName", organizationUnit.Name },
                    { "BranchOfficeOrganizationUnit", PrintHelper.BranchOfficeOrganizationUnitFields(branchOfficeOrganizationUnit) },
                    { "BranchOffice", PrintHelper.BranchOfficeFields(branchOffice) },
                    { "LegalPerson", PrintHelper.LegalPersonFields(legalPerson) },
                    { "LegalPersonProfile", PrintHelper.LegalPersonProfileFields(legalPersonProfile) },
                    { "AuthorityDocument", PrintHelper.GetAuthorityDocumentDescription(legalPersonProfile, _localizationSettings.ApplicationCulture) },
                    { "Bargain", PrintHelper.BargainFields(bargain) },
                };

            printData = PrintData.Concat(printData, PrintHelper.BargainFlagFields(bargain), PrintHelper.LegalPersonFlagFields(legalPerson));

            var printRequest = new PrintDocumentRequest
                {
                    BranchOfficeOrganizationUnitId = branchOfficeOrganizationUnit.Id,
                    TemplateCode = (BargainKind)bargain.BargainKind == BargainKind.Client ? TemplateCode.ClientBargain : TemplateCode.AgentBargain,
                    FileName = bargain.Number,
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }
    }
}