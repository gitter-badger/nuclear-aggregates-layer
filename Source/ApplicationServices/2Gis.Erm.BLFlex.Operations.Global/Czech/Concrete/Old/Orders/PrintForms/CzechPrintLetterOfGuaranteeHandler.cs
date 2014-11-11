using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using BLResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    public class CzechPrintLetterOfGuaranteeHandler : RequestHandler<PrintLetterOfGuaranteeRequest, Response>, ICzechAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IPrintValidationOperationService _validationService;

        public CzechPrintLetterOfGuaranteeHandler(ISubRequestProcessor requestProcessor, IFinder finder, IPrintValidationOperationService validationService)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
            _validationService = validationService;
        }

        protected override Response Handle(PrintLetterOfGuaranteeRequest request)
        {
            _validationService.ValidateOrder(request.OrderId);
            var orderInfo =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                    .Select(order => new
                            {
                                OrderNumber = order.Number,
                                order.BranchOfficeOrganizationUnitId,
                            })
                    .Single();

            var printRequest = new PrintDocumentRequest
                {
                    TemplateCode = TemplateCode.LetterOfGuarantee,
                    FileName = string.Format("{0}-Cestne prohlaseni", orderInfo.OrderNumber),
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
                    PrintData = GetPrintData(request.OrderId)
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        protected PrintData GetPrintData(long orderId)
        {
            var data = _finder.Find(Specs.Find.ById<Order>(orderId))
                              .Select(order => new
                                  {
                                      Order = order,
                                      ProfileId = order.LegalPersonProfileId,
                                      order.LegalPersonId,
                                      order.BranchOfficeOrganizationUnit.BranchOfficeId,
                                  })
                              .Single();

            var branchOfficeOrganizationUnit = _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.ByOrderId(orderId));
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(data.LegalPersonId.Value));
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(data.ProfileId.Value));

            return new PrintData
                {
                    { "BranchOffice", CzechPrintHelper.BranchOfficeFields(_finder.FindOne(Specs.Find.ById<BranchOffice>(data.BranchOfficeId))) },
                    { "BranchOfficeOrganizationUnit", CzechPrintHelper.BranchOfficeOrganizationUnitFields(branchOfficeOrganizationUnit) },
                    { "LegalPerson", CzechPrintHelper.LegalPersonFields(legalPerson) },
                    { "Order", CzechPrintHelper.OrderFieldsForLetterOfGuarantee(data.Order) },
                    { "Profile", CzechPrintHelper.LegalPersonProfileFields(legalPersonProfile) },
                    { "LegalAddressPrefix", GetLegalAddressPrefix((LegalPersonType)legalPerson.LegalPersonTypeEnum) },
                    { "PersonPrefix", GetPersonPrefix((LegalPersonType)legalPerson.LegalPersonTypeEnum) },
                    { "OperatesOnTheBasis", GetOperatesOnTheBasisString(legalPersonProfile) },
                };
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