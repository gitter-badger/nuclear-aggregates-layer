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

using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.Orders.PrintForms
{
    public sealed class KazakhstanPrintOrderTerminationNoticeHandler : RequestHandler<PrintOrderTerminationNoticeRequest, Response>, IKazakhstanAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;

        public KazakhstanPrintOrderTerminationNoticeHandler(ISubRequestProcessor requestProcessor, IFinder finder)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
        }

        protected override Response Handle(PrintOrderTerminationNoticeRequest request)
        {
            var orderInfo = _finder.FindObsolete(Specs.Find.ById<Order>(request.OrderId))
                                   .Select(x => new { OrderState = (OrderState)x.WorkflowStepId, x.IsTerminated, x.Number, x.LegalPersonProfileId })
                                   .Single();

            if (!orderInfo.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);
            }

            if (orderInfo.OrderState != OrderState.OnTermination && orderInfo.OrderState != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            var order = _finder.Find(Specs.Find.ById<Order>(request.OrderId)).One();
            var legalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(order.LegalPersonId)).One();
            var profile = _finder.Find(Specs.Find.ById<LegalPersonProfile>(orderInfo.LegalPersonProfileId)).One();
            var bargain = _finder.Find(Specs.Find.ById<Bargain>(order.BargainId)).One();
            var branchOfficeOrganiationUnit = _finder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(order.BranchOfficeOrganizationUnitId)).One();

            var printData = new PrintData
                                {
                                    { "LegalPerson.LegalName", legalPerson.LegalName },
                                    { "LegalPersonProfile.ChiefNameInGenitive", profile.ChiefNameInGenitive },
                                    { "Order.TerminationDate", order.EndDistributionDateFact.AddDays(1) },
                                    { "Order.Number", order.Number },
                                    { "Order.CreatedOn", order.CreatedOn },
                                    { "BranchOfficeOrganizationUnit", PrintHelper.BranchOfficeOrganizationUnitFields(branchOfficeOrganiationUnit) },
                                };

            if (bargain != null)
            {
                printData.Add("UseBargain", true);
                printData.Add("Bargain", PrintHelper.BargainFields(bargain));
            }

            var printRequest = new PrintDocumentRequest
                {
                    BranchOfficeOrganizationUnitId = branchOfficeOrganiationUnit.Id,
                    TemplateCode = TemplateCode.TerminationNoticeLegalPerson,
                    FileName = string.Format(BLResources.PrintTerminationNoticeFileNameFormat, orderInfo.Number),
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }
    }
}
