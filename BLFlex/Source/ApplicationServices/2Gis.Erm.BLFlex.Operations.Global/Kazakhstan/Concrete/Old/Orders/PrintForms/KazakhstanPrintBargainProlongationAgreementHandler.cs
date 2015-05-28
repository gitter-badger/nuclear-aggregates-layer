using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Security.API.UserContext;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.Orders.PrintForms
{
    public sealed class KazakhstanPrintBargainProlongationAgreementHandler : RequestHandler<PrintBargainProlongationAgreementRequest, Response>, IKazakhstanAdapted
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILocalizationSettings _localizationSettings;

        public KazakhstanPrintBargainProlongationAgreementHandler(
            IFinder finder, 
            IUserContext userContext, 
            ISubRequestProcessor requestProcessor, 
            ILocalizationSettings localizationSettings)
        {
            _finder = finder;
            _userContext = userContext;
            _localizationSettings = localizationSettings;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintBargainProlongationAgreementRequest request)
        {
            var bargain = _finder.Find(Specs.Find.ById<Bargain>(request.BargainId)).One();
            var branchOfficeOrganizationUnit = _finder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(bargain.ExecutorBranchOfficeId)).One();
            var legalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(bargain.CustomerLegalPersonId)).One();
            var legalPersonProfile = _finder.Find(Specs.Find.ById<LegalPersonProfile>(request.LegalPersonProfileId)).One();
            var branchOffice = _finder.Find(Specs.Find.ById<BranchOffice>(branchOfficeOrganizationUnit.BranchOfficeId)).One();

            var printData = new PrintData
                {
                    { "CurrentDate", DateTimeOffset.UtcNow.ToOffset(_userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.BaseUtcOffset).Date },
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
                    TemplateCode = TemplateCode.BargainProlongationAgreement,
                    FileName = "доп. соглашение к договору " + bargain.Number,
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }
    }
}
