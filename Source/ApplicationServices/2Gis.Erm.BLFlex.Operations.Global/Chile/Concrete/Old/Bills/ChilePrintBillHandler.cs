﻿using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
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

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Concrete.Old.Bills
{
    public sealed class ChilePrintBillHandler : RequestHandler<PrintBillRequest, Response>, IChileAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public ChilePrintBillHandler(
            ILegalPersonReadModel legalPersonReadModel,
            ISubRequestProcessor requestProcessor, 
            IFinder finder)
        {
            _legalPersonReadModel = legalPersonReadModel;
            _requestProcessor = requestProcessor;
            _finder = finder;
        }

        protected override Response Handle(PrintBillRequest request)
        {
            var billInfo = _finder.Find(Specs.Find.ById<Bill>(request.Id))
                                  .Select(bill => new
                                      {
                                          Bill = bill,
                                          Bargain = bill.Order.Bargain,
                                          LegalPersonId = bill.Order.LegalPersonId,
                                          LegalPersonProfileId = bill.Order.LegalPersonProfileId,
                                          bill.Order.BranchOfficeOrganizationUnitId,
                                          BranchOfficeOrganizationUnitVatRate = bill.Order.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,

                                          Order = new
                                              {
                                                  bill.Order.Number,
                                                  bill.Order.SignupDate,
                                                  bill.Order.PaymentMethod,
                                                  bill.Order.DiscountPercent,
                                              },

                                          CurrencyISOCode = bill.Order.Currency.ISOCode
                                      })
                                  .SingleOrDefault();

            if (billInfo == null)
            {
                throw new NotificationException(BLResources.SpecifiedBillNotFound);
            }

            if (!billInfo.LegalPersonId.HasValue)
            {
                throw new NotificationException(BLResources.LegalPersonNotFound);
            }

            if (!billInfo.LegalPersonProfileId.HasValue)
            {
                throw new NotificationException(BLResources.LegalPersonProfileMissing);
            }

            var legalPerson = _legalPersonReadModel.GetLegalPerson(billInfo.LegalPersonId.Value);
            var legalPersonPart = legalPerson.Parts.OfType<LegalPersonPart>().Single();

            var legalPersonProfile = _legalPersonReadModel.GetLegalPersonProfile(billInfo.LegalPersonProfileId.Value);
            var legalPersonProfilePart = legalPersonProfile.Parts.OfType<LegalPersonProfilePart>().Single();

            var communeRef = _legalPersonReadModel.GetCommuneReference(legalPerson.Id);

            var printData2 = new
                {
                    Order = new
                        {
                            billInfo.Order.Number,
                            billInfo.Order.SignupDate,
                            billInfo.Order.PaymentMethod,
                            billInfo.BranchOfficeOrganizationUnitVatRate,
                            billInfo.Order.DiscountPercent
                        },

                    LegalPerson = new
                        {
                            legalPerson.LegalName,
                            legalPerson.LegalAddress,
                            legalPerson.Inn, // TODO {all, 26.02.2014}: Rut vs Inn
                            legalPersonPart.OperationsKind,
                            legalPersonProfile.Phone,

                            Commune = communeRef.Name,
                            PaymentMethod = LocalizePaymentMethod(legalPersonProfile.PaymentMethod),
                        },

                    Bill = new
                        {
                            billInfo.Bill.BillNumber,
                            billInfo.Bill.PaymentDatePlan,
                            billInfo.Bill.BeginDistributionDate,
                            billInfo.Bill.EndDistributionDate,
                            billInfo.Bill.PayablePlan,
                            billInfo.Bill.VatPlan,
                            PayableWithoutVatPlan = billInfo.Bill.PayablePlan - billInfo.Bill.VatPlan,
                        },

                    RelatedBargainInfo = FormatRelatedBargainInfo(billInfo.Bargain),
                };

            var printDocumentRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = billInfo.CurrencyISOCode,
                    BranchOfficeOrganizationUnitId = billInfo.BranchOfficeOrganizationUnitId,
                    FileName = printData2.Bill.BillNumber,
                    PrintData = printData2,
                    TemplateCode = TemplateCode.BillLegalPerson
                };

            return _requestProcessor.HandleSubRequest(printDocumentRequest, Context);
        }

        private static string LocalizePaymentMethod(int? paymentMethod)
        {
            return (paymentMethod.HasValue
                        ? (PaymentMethod)paymentMethod.Value
                        : PaymentMethod.Undefined)
                .ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture);
        }

        private static string FormatRelatedBargainInfo(Bargain bargain)
        {
            return (bargain == null)
                ? null
                : string.Format(BLResources.RelatedToBargainInfoTemplate,
                                       bargain.Number,
                                       bargain.CreatedOn);
        }
    }
}
