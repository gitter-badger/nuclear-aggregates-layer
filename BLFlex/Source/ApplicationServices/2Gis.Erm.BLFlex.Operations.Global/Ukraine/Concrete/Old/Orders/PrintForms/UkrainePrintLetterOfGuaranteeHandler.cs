using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Readings;

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
                _finder.FindObsolete(Specs.Find.ById<Order>(request.OrderId))
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
            var data = _finder.FindObsolete(Specs.Find.ById<Order>(orderId))
                              .Select(order => new
                                  {
                                      Order = order,
                                      ProfileId = order.LegalPersonProfileId,
                                      LegalPersonName = order.LegalPerson.LegalName,
                                      BranchOfficeOrganizationUnitName = order.BranchOfficeOrganizationUnit.ShortLegalName,
                                  })
                              .Single();

            if (data.ProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            var legalPersonProfile = _finder.Find(Specs.Find.ById<LegalPersonProfile>(data.ProfileId.Value)).One();

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