using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Order;
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
    public class UkrainePrintLetterOfGuaranteeHandler : RequestHandler<PrintLetterOfGuaranteeRequest, Response>, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IPrintValidationOperationService _validationService;

        public UkrainePrintLetterOfGuaranteeHandler(ISubRequestProcessor requestProcessor, IFinder finder, IPrintValidationOperationService validationService)
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
                    FileName = string.Format("{0}-Гарантийное письмо", orderInfo.OrderNumber),
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
                                      LegalPersonName = order.LegalPerson.LegalName,
                                      BranchOfficeOrganizationUnitName = order.BranchOfficeOrganizationUnit.ShortLegalName,
                                  })
                              .Single();

            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(data.ProfileId.Value));

            return new PrintData
                {
                    { "BranchOfficeOrganizationUnitName", data.BranchOfficeOrganizationUnitName },
                    { "LegalPersonName", data.LegalPersonName },
                    { "Order", GetOrderData(data.Order) },
                    { "Profile", GetProfileData(legalPersonProfile) }
                };
        }

        private PrintData GetOrderData(Order order)
        {
            return new PrintData
                {
                    { "Number", order.Number },
                    { "SignupDate", order.SignupDate },
                };
        }

        private PrintData GetProfileData(LegalPersonProfile profile)
        {
            return new PrintData
                {
                    { "PositionInNominative", profile.PositionInNominative },
                    { "ChiefNameInNominative", profile.ChiefNameInNominative },
                };
        }
    }
}