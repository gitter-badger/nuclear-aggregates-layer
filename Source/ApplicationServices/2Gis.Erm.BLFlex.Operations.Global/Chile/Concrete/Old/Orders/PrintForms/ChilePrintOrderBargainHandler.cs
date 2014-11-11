using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Concrete.Old.Orders.PrintForms
{
    public sealed class ChilePrintOrderBargainHandler : RequestHandler<PrintOrderBargainRequest, Response>, IChileAdapted
    {
        private readonly IFinder _finder;
        private readonly IBankReadModel _bankReadModel;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly IFormatter _shortDateFormatter;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPrintValidationOperationService _validationService;

        public ChilePrintOrderBargainHandler(IFinder finder,
                                             IBankReadModel bankReadModel,
                                             ISubRequestProcessor requestProcessor,
                                             ILegalPersonReadModel legalPersonReadModel,
                                             IBranchOfficeReadModel branchOfficeReadModel,
                                             IFormatterFactory formatterFactory,
                                             IOrderReadModel orderReadModel,
                                             IPrintValidationOperationService validationService)
        {
            _finder = finder;
            _bankReadModel = bankReadModel;
            _requestProcessor = requestProcessor;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _orderReadModel = orderReadModel;
            _validationService = validationService;
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            long bargainId;
            long legalPersonProfileId;
            if (request.BargainId.HasValue && request.LegalPersonProfileId.HasValue)
            {
                bargainId = request.BargainId.Value;
                legalPersonProfileId = request.LegalPersonProfileId.Value;
            }
            else if (request.OrderId.HasValue)
            {
                _validationService.ValidateOrderForBargain(request.OrderId.Value);
                bargainId = _orderReadModel.GetOrderBargainId(request.OrderId.Value);
                legalPersonProfileId = _orderReadModel.GetOrderLegalPersonProfileId(request.OrderId.Value);
            }
            else
            {
                // ReSharper disable once LocalizableElement
                throw new ArgumentException("Запрос дожен содержать BargainId & LegalPersonProfileId или OrderId", "request");
            }

            var bargainData = _finder.Find(Specs.Find.ById<Bargain>(bargainId))
                                     .Select(x => new
                                                      {
                                                          LegalPersonId = x.CustomerLegalPersonId,
                                                          BranchOfficeOrganizationUnitId = x.ExecutorBranchOfficeId,

                                                          BargainSignedOn = x.SignedOn,
                                                          BargainNumber = x.Number,
                                                          BranchOfficeName = x.BranchOfficeOrganizationUnit.BranchOffice.Name,
                                                          BranchOfficeLegalAddress = x.BranchOfficeOrganizationUnit.BranchOffice.LegalAddress,
                                                          BranchOfficeInn = x.BranchOfficeOrganizationUnit.BranchOffice.Inn,
                                                      })
                                     .Single();

            var boou = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(bargainData.BranchOfficeOrganizationUnitId);
            var boouPart = boou.Parts.OfType<ChileBranchOfficeOrganizationUnitPart>().Single();
            var legalPerson = _legalPersonReadModel.GetLegalPerson(bargainData.LegalPersonId);
            var legalPersonProfile = _legalPersonReadModel.GetLegalPersonProfile(legalPersonProfileId);
            var legalPersonProfilePart = legalPersonProfile.Parts.OfType<ChileLegalPersonProfilePart>().Single();

            var bankName = legalPersonProfilePart.BankId.HasValue ? _bankReadModel.GetBank(legalPersonProfilePart.BankId.Value).Name : string.Empty;

            var printData = new 
            {
                Bargain = new 
                {
                    Number = bargainData.BargainNumber,
                    SignedOn = bargainData.BargainSignedOn,   
                },

                BranchOffice = new
                {
                    bargainData.BranchOfficeName,
                    bargainData.BranchOfficeLegalAddress,
                    bargainData.BranchOfficeInn,
                },

                BranchOfficeOrganizationUnit = new 
                {
                    boou.ChiefNameInNominative,
                    boou.PositionInNominative,
                    boou.PaymentEssentialElements,
                    boouPart.RepresentativeRut,
                },

                LegalPerson = new 
                {
                    legalPerson.LegalName,
                    legalPerson.LegalAddress,
                    legalPerson.Inn,
                },

                LegalPersonProfile = new 
                {
                    legalPersonProfile.ChiefNameInNominative,
                    legalPersonProfile.PositionInNominative,
                    legalPersonProfile.AccountNumber,
                    legalPersonProfile.PaymentEssentialElements,
                    legalPersonProfilePart.RepresentativeRut,
                    legalPersonProfilePart.RepresentativeAuthorityDocumentIssuedBy,
                    RepresentativeAuthorityDocumentIssuedOn = _shortDateFormatter.Format(legalPersonProfilePart.RepresentativeAuthorityDocumentIssuedOn),
                    AccountType = LocalizeAccountType(legalPersonProfilePart.AccountType),
                    OperatesOnTheBasisInGenitive = LocalizeOperatesOnTheBasisInGenitive(legalPersonProfile.OperatesOnTheBasisInGenitive),
                    BankName = bankName
                },
            };

            var printDocumentRequest = new PrintDocumentRequest
                {
                    BranchOfficeOrganizationUnitId = bargainData.BranchOfficeOrganizationUnitId,
                    TemplateCode = TemplateCode.ClientBargain,
                    FileName = bargainData.BargainNumber,
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printDocumentRequest, Context);
        }

        private static string LocalizeAccountType(AccountType accountType)
        {
            return accountType.ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture);
        }

        private static string LocalizeOperatesOnTheBasisInGenitive(int? operatesOnTheBasisInGenitive)
        {
            return (operatesOnTheBasisInGenitive.HasValue
                        ? (OperatesOnTheBasisType)operatesOnTheBasisInGenitive.Value
                        : OperatesOnTheBasisType.Undefined)
                .ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture);
        }
    }
}
