using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    public sealed class CzechPrintLetterOfGuaranteeHandler : RequestHandler<PrintLetterOfGuaranteeRequest, Response>, ICzechAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;

        public CzechPrintLetterOfGuaranteeHandler(ISubRequestProcessor requestProcessor, IFinder finder)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
        }

        protected override Response Handle(PrintLetterOfGuaranteeRequest request)
        {
            var orderInfo =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                    .Select(order => new
                            {
                                Order = order,
                                Profile = order.LegalPerson.LegalPersonProfiles
                                    .FirstOrDefault(y => y.Id == request.LegalPersonProfileId),
                                MainProfile = order.LegalPerson.LegalPersonProfiles
                                    .FirstOrDefault(y => y.Id == request.LegalPersonProfileId && y.IsMainProfile),
                                order.LegalPerson,
                                order.BranchOfficeOrganizationUnit,
                                order.BranchOfficeOrganizationUnitId,
                                order.BranchOfficeOrganizationUnit.BranchOffice
                            })
                    .Single();

            var legalPersonType = (LegalPersonType)orderInfo.LegalPerson.LegalPersonTypeEnum;
            var profile = orderInfo.MainProfile ?? orderInfo.Profile;

            var printData = new
                            {
                                orderInfo.Order,
                                orderInfo.Profile,
                                orderInfo.LegalPerson,
                                orderInfo.BranchOfficeOrganizationUnit,
                                orderInfo.BranchOfficeOrganizationUnitId,
                                orderInfo.BranchOffice,
                                PersonPrefix = GetPersonPrefix(legalPersonType),
                                LegalAddressPrefix = GetLegalAddressPrefix(legalPersonType),
                                OperatesOnTheBasis = GetOperatesOnTheBasisString(profile),
                            };
            return
                _requestProcessor.HandleSubRequest(
                    new PrintDocumentRequest
                        {
                            TemplateCode = TemplateCode.LetterOfGuarantee,
                            FileName = string.Format("{0}-Cestne prohlaseni", printData.Order.Number),
                            BranchOfficeOrganizationUnitId = printData.BranchOfficeOrganizationUnitId,
                            PrintData = printData
                        },
                    Context);
        }

        private static string GetPersonPrefix(LegalPersonType legalPersonType)
        {
            return legalPersonType == LegalPersonType.Businessman
                ? BLResources.CzechPrintLetterOfGuaranteeHandler_PersonPrefixBusinessman
                : BLResources.CzechPrintLetterOfGuaranteeHandler_PersonPrefixLegalPerson;
        }

        private static string GetLegalAddressPrefix(LegalPersonType legalPersonType)
        {
            return legalPersonType == LegalPersonType.Businessman
                ? BLResources.CzechPrintLetterOfGuaranteeHandler_LegalAddressPrefixBusinessman
                : BLResources.CzechPrintLetterOfGuaranteeHandler_LegalAddressPrefixLegalPerson;
        }

        private static string GetOperatesOnTheBasisString(LegalPersonProfile profile)
        {
            if (profile.OperatesOnTheBasisInGenitive != (int)OperatesOnTheBasisType.Warranty)
            {
                return string.Empty;
            }

            if (profile.WarrantyBeginDate == null)
            {
                return string.Empty;
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                BLResources.CzechPrintLetterOfGuaranteeHandler_OperatesOnTheBasisStringTemplate,
                ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                profile.WarrantyBeginDate.Value.ToShortDateString());
        }
    }
}