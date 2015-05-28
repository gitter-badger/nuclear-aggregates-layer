using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Concrete.Old.Orders.PrintForms
{
    public sealed class ChilePrintLetterOfGuaranteeHandler : RequestHandler<PrintLetterOfGuaranteeRequest, Response>, IChileAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IFormatter _shortDateFormatter;

        public ChilePrintLetterOfGuaranteeHandler(ILegalPersonReadModel legalPersonReadModel,
                                                  IFormatterFactory formatterFactory,
                                                  ISubRequestProcessor requestProcessor,
                                                  IFinder finder)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
            _legalPersonReadModel = legalPersonReadModel;
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        protected override Response Handle(PrintLetterOfGuaranteeRequest request)
        {
            var order = _finder.FindObsolete(Specs.Find.ById<Order>(request.OrderId)).Single();

            if (order == null)
            {
                throw new NotificationException(BLResources.OrderNotFound);
            }

            if (!order.LegalPersonId.HasValue)
            {
                throw new NotificationException(BLResources.LegalPersonNotFound);
            }

            if (order.LegalPersonProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            var legalPerson = _legalPersonReadModel.GetLegalPerson(order.LegalPersonId.Value);
            var legalPersonProfile = _legalPersonReadModel.GetLegalPersonProfile(order.LegalPersonProfileId.Value);
            var legalPersonProfilePart = legalPersonProfile.Parts.OfType<ChileLegalPersonProfilePart>().Single();

            var printData = new 
            {
                Order = new 
                {
                    order.SignupDate,
                    order.Number,
                    order.BeginDistributionDate,
                    order.EndDistributionDatePlan,
                    order.PayablePlan,
                },

                LegalPersonProfile = new 
                {
                    legalPersonProfile.ChiefNameInNominative,
                    legalPersonProfile.PositionInNominative,
                    legalPersonProfilePart.RepresentativeRut,
                    RepresentativeAuthorityDocumentIssuedOn = _shortDateFormatter.Format(legalPersonProfilePart.RepresentativeAuthorityDocumentIssuedOn),
                    legalPersonProfilePart.RepresentativeAuthorityDocumentIssuedBy,
                    OperatesOnTheBasisInGenitive = LocalizeOperatesOnTheBasisInGenitive(legalPersonProfile.OperatesOnTheBasisInGenitive)
                },

                LegalPerson = new 
                {
                    legalPerson.LegalName,
                    legalPerson.Inn,
                    legalPerson.LegalAddress,
                },
            };

            var printDocumentRequest = new PrintDocumentRequest
                {
                    TemplateCode = TemplateCode.LetterOfGuarantee,
                    FileName = string.Format("{0}-CARTA_DE_COMPROMISO", order.Number),
                    BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId,
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printDocumentRequest, Context);
        }

        private static string LocalizeOperatesOnTheBasisInGenitive(OperatesOnTheBasisType? operatesOnTheBasisInGenitive)
        {
            return (operatesOnTheBasisInGenitive.HasValue
                        ? operatesOnTheBasisInGenitive.Value
                        : OperatesOnTheBasisType.Undefined)
                .ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture);
        }
    }
}