using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.LegalPersonAggregate.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Concrete.Old.Bills
{
    public sealed class ChilePrintBillHandler : RequestHandler<PrintBillRequest, Response>, IChileAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IChileLegalPersonReadModel _chileLegalPersonReadModel;
        private readonly IFormatter _shortDateFormatter;

        public ChilePrintBillHandler(
            ILegalPersonReadModel legalPersonReadModel, 
            IChileLegalPersonReadModel chileLegalPersonReadModel, 
            ISubRequestProcessor requestProcessor, 
            IFormatterFactory formatterFactory, 
            IFinder finder)
        {
            _legalPersonReadModel = legalPersonReadModel;
            _chileLegalPersonReadModel = chileLegalPersonReadModel;
            _requestProcessor = requestProcessor;
            _finder = finder;
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        protected override Response Handle(PrintBillRequest request)
        {
            var billInfo = _finder.Find(Specs.Find.ById<Bill>(request.BillId))
                                  .Map(q => q.Select(bill => new
                                      {
                                          Bill = bill, 
                                          Bargain = bill.Order.Bargain, 
                                          LegalPersonId = bill.Order.LegalPersonId, 
                                          LegalPersonProfileId = bill.Order.LegalPersonProfileId, 
                                          bill.Order.BranchOfficeOrganizationUnitId, 
                                          BranchOfficeOrganizationUnitVatRate = (long?)bill.Order.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate, 

                                          Order = new
                                              {
                                                  bill.Order.Number, 
                                                  bill.Order.SignupDate, 
                                                  bill.Order.PaymentMethod, 
                                                  bill.Order.DiscountPercent, 
                                              }, 

                                          CurrencyISOCode = bill.Order.Currency.ISOCode
                                      }))
                                  .One();

            if (billInfo == null)
            {
                throw new NotificationException(BLResources.SpecifiedBillNotFound);
            }

            if (!billInfo.LegalPersonId.HasValue)
            {
                throw new NotificationException(BLResources.LegalPersonNotFound);
            }

            if (billInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new RequiredFieldIsEmptyException(string.Format(Resources.Server.Properties.BLResources.OrderFieldNotSpecified, MetadataResources.BranchOfficeOrganizationUnit));
            }

            if (billInfo.LegalPersonProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            var legalPerson = _legalPersonReadModel.GetLegalPerson(billInfo.LegalPersonId.Value);
            var legalPersonPart = legalPerson.Parts.OfType<ChileLegalPersonPart>().Single();

            var legalPersonProfile = _legalPersonReadModel.GetLegalPersonProfile(billInfo.LegalPersonProfileId.Value);

            var communeRef = _chileLegalPersonReadModel.GetCommuneReference(legalPerson.Id);

            var printData2 = new
                {
                    Order = new
                        {
                            billInfo.Order.Number, 
                            billInfo.Order.SignupDate, 
                            billInfo.Order.PaymentMethod, 
                            billInfo.BranchOfficeOrganizationUnitVatRate.Value, 
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
                            BillNumber = billInfo.Bill.Number, 
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

        private static string LocalizePaymentMethod(PaymentMethod? paymentMethod)
        {
            return (paymentMethod.HasValue
                        ? paymentMethod.Value
                        : PaymentMethod.Undefined)
                .ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture);
        }

        private string FormatRelatedBargainInfo(Bargain bargain)
        {
            return (bargain == null)
                       ? null
                       : string.Format(BLResources.RelatedToBargainInfoTemplate, 
                                       bargain.Number, 
                                       _shortDateFormatter.Format(bargain.CreatedOn));
        }
    }
}
