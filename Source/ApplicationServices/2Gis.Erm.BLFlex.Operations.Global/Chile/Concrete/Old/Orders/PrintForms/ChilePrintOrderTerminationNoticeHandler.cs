using System.Data.Objects;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Concrete.Old.Orders.PrintForms
{
    public sealed class ChilePrintOrderTerminationNoticeHandler : RequestHandler<PrintOrderTerminationNoticeRequest, Response>, IChileAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public ChilePrintOrderTerminationNoticeHandler(ILegalPersonReadModel legalPersonReadModel,
                                                       ISubRequestProcessor requestProcessor,
                                                       IFinder finder)
        {
            _legalPersonReadModel = legalPersonReadModel;
            _requestProcessor = requestProcessor;
            _finder = finder;
        }

        protected override Response Handle(PrintOrderTerminationNoticeRequest request)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                                   .Select(order => new
                                       {
                                           OrderState = (OrderState)order.WorkflowStepId, 
                                           order.IsTerminated, 
                                           CurrencyISOCode = order.Currency.ISOCode, 
                                           order.BranchOfficeOrganizationUnitId,
                                           order.LegalPersonProfileId
                                       })
                                   .Single();

            if (!orderInfo.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);
            }

            if (orderInfo.OrderState != OrderState.OnTermination && orderInfo.OrderState != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            var legalPersonProfileId = request.LegalPersonProfileId ?? orderInfo.LegalPersonProfileId;
            if (!legalPersonProfileId.HasValue)
            {
                throw new NotificationException(BLResources.LegalPersonProfileMissing);
            }

            var legalPersonProfile = _legalPersonReadModel.GetLegalPersonProfile(legalPersonProfileId.Value);

            var printData =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                       .Select(order => new
                           {
                               Order = new
                                   {
                                       order.RejectionDate, 
                                       order.Number,
                                       order.SignupDate, 
                                       TerminationDate = EntityFunctions.AddDays(order.EndDistributionDateFact, 1), 
                                   }, 
                               LegalPerson = new
                                   {
                                       order.LegalPerson.LegalName, 
                                   }, 
                               LegalPersonProfile = new
                                   {
                                       legalPersonProfile.ChiefNameInNominative, 
                                   },
                               BranchOffice = new
                                   {
                                       order.BranchOfficeOrganizationUnit.BranchOffice.Name, 
                                       order.BranchOfficeOrganizationUnit.BranchOffice.Inn, 
                                   }, 
                               Bargain = new
                                   {
                                       order.Bargain.Number, 
                                   }, 
                           })
                       .Single();

            var printDocumentRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = orderInfo.CurrencyISOCode, 
                    TemplateCode = TemplateCode.TerminationNoticeLegalPerson, 
                    FileName = string.Format(BLResources.PrintTerminationNoticeFileNameFormat, printData.Order.Number), 
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId, 
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printDocumentRequest, Context);
        }
    }
}