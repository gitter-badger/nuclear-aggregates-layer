﻿using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
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

        public ChilePrintOrderBargainHandler(IFinder finder,
                                             IBankReadModel bankReadModel,
                                             ISubRequestProcessor requestProcessor,
                                             ILegalPersonReadModel legalPersonReadModel,
                                             IBranchOfficeReadModel branchOfficeReadModel)
        {
            _finder = finder;
            _bankReadModel = bankReadModel;
            _requestProcessor = requestProcessor;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            var orderData =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                       .Select(order => new
                           {
                               order.LegalPersonId,
                               order.LegalPersonProfileId,
                               order.BranchOfficeOrganizationUnitId,

                               BargainSignedOn = order.Bargain.SignedOn,
                               BargainNumber = order.Bargain.Number,
                               CurrencyIsoCode = order.Currency.ISOCode,
                               order.BranchOfficeOrganizationUnit.BranchOffice,
                           })
                       .Single();

            if (orderData == null)
            {
                throw new NotificationException(BLResources.OrderNotFound);
            }

            if (!orderData.LegalPersonId.HasValue)
            {
                throw new NotificationException(BLResources.LegalPersonNotFound);
            }

            if (!orderData.LegalPersonProfileId.HasValue)
            {
                throw new NotificationException(BLResources.LegalPersonProfileMissing);
            }

            if (!orderData.BranchOfficeOrganizationUnitId.HasValue)
            {
                throw new NotificationException(BLResources.BranchOfficeOrganizationUnitNotFound);
            }
            
            var boou = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(orderData.BranchOfficeOrganizationUnitId.Value);
            var boouPart = boou.Parts.OfType<BranchOfficeOrganizationUnitPart>().Single();
            var legalPerson = _legalPersonReadModel.GetLegalPerson(orderData.LegalPersonId.Value);
            var legalPersonProfile = _legalPersonReadModel.GetLegalPersonProfile(orderData.LegalPersonProfileId.Value);
            var legalPersonProfilePart = legalPersonProfile.Parts.OfType<LegalPersonProfilePart>().Single();

            var bankName = legalPersonProfilePart.BankId.HasValue ? _bankReadModel.GetBank(legalPersonProfilePart.BankId.Value).Name : string.Empty;

            var printData = new 
            {
                Bargain = new 
                {
                    Number = orderData.BargainNumber,
                    SignedOn = orderData.BargainSignedOn,   
                },

                BranchOffice = new
                {
                    orderData.BranchOffice.Name,
                    orderData.BranchOffice.LegalAddress,
                    orderData.BranchOffice.Inn,
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
                    legalPersonProfilePart.RepresentativeAuthorityDocumentIssuedOn,
                    AccountType = LocalizeAccountType(legalPersonProfilePart.AccountType),
                    OperatesOnTheBasisInGenitive = LocalizeOperatesOnTheBasisInGenitive(legalPersonProfile.OperatesOnTheBasisInGenitive),
                    BankName = bankName
                },
            };

            var printDocumentRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = orderData.CurrencyIsoCode,
                    BranchOfficeOrganizationUnitId = orderData.BranchOfficeOrganizationUnitId,
                    TemplateCode = TemplateCode.BargainLegalPerson,
                    FileName = orderData.BargainNumber,
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
