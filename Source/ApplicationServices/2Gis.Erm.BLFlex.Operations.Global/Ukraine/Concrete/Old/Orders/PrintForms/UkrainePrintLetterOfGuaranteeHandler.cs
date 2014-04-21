using System.Linq;

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

        public UkrainePrintLetterOfGuaranteeHandler(ISubRequestProcessor requestProcessor, IFinder finder)
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
                               OrderNumber = order.Number,
                               order.BranchOfficeOrganizationUnitId,
                           })
                       .Single();

            var printRequest = new PrintDocumentRequest
                {
                    TemplateCode = TemplateCode.LetterOfGuarantee,
                    FileName = string.Format("{0}-Гарантийное письмо", orderInfo.OrderNumber),
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
                    PrintData = GetPrintData(request.OrderId, request.LegalPersonProfileId)
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        protected PrintData GetPrintData(long orderId, long? legalPersonProfileId)
        {
            return
                _finder.Find(Specs.Find.ById<Order>(orderId))
                       .Select(order => new
                           {
                               Order = order,
                               Profile = order.LegalPerson.LegalPersonProfiles
                                              .FirstOrDefault(y => y.Id == legalPersonProfileId),
                               order.LegalPerson,
                               order.BranchOfficeOrganizationUnit,
                           })
                       .AsEnumerable()
                       .Select(x => new PrintData
                           {
                               { "BranchOfficeOrganizationUnitName", x.BranchOfficeOrganizationUnit.ShortLegalName },
                               {
                                   "LegalPersonName", x.LegalPerson.LegalName
                               },
                               { "Order", GetOrderData(x.Order) },
                               { "Profile", GetProfileData(x.Profile) }
                           })
                       .Single();
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