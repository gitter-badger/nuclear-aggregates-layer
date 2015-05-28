using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.Orders.PrintForms
{
    public sealed class KazakhstanPrintOrderBargainHandler : RequestHandler<PrintOrderBargainRequest, Response>, IKazakhstanAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILocalizationSettings _localizationSettings;

        public KazakhstanPrintOrderBargainHandler(
            IFinder finder, 
            ISubRequestProcessor requestProcessor, 
            ILocalizationSettings localizationSettings)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _localizationSettings = localizationSettings;
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            var legalPersonProfileId = request.LegalPersonProfileId 
                ?? _finder.Find(Specs.Find.ById<Order>(request.OrderId.Value)).Map(q => q.Select(order => order.LegalPersonProfileId)).One();

            if (legalPersonProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            var bargainSpecification = request.BargainId.HasValue
                                           ? Specs.Find.ById<Bargain>(request.BargainId.Value)
                                           : OrderSpecs.Bargains.Find.ByOrder(request.OrderId.Value);
            var bargain = _finder.Find(bargainSpecification).One();
            var branchOfficeOrganizationUnit = _finder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(bargain.ExecutorBranchOfficeId)).One();
            var legalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(bargain.CustomerLegalPersonId)).One();
            var legalPersonProfile = _finder.Find(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId.Value)).One();
            var branchOffice = _finder.Find(Specs.Find.ById<BranchOffice>(branchOfficeOrganizationUnit.BranchOfficeId)).One();

            var printData = new PrintData
                {
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