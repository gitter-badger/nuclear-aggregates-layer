﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Old.Bills
{
    public sealed class PrintBillHandler : RequestHandler<PrintBillRequest, Response>, IEmiratesAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintBillHandler(ISubRequestProcessor requestProcessor, IFinder finder)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintBillRequest request)
        {
            var billInfo = _finder.Find(Specs.Find.ById<Bill>(request.Id))
                                  .Select(bill => new
                                      {
                                          bill.OrderId,
                                          bill.BillNumber,
                                          bill.Order.BranchOfficeOrganizationUnitId,
                                          CurrencyISOCode = bill.Order.Currency.ISOCode
                                      })
                                  .SingleOrDefault();

            if (billInfo == null)
            {
                throw new EntityNotFoundException(typeof(Bill), request.Id);
            }

            var printRequest = new PrintDocumentRequest()
                {
                    CurrencyIsoCode = billInfo.CurrencyISOCode,
                    FileName = billInfo.BillNumber,
                    BranchOfficeOrganizationUnitId = billInfo.BranchOfficeOrganizationUnitId,
                    PrintData = GetPrintData(request.Id, request.LegalPersonProfileId),
                    TemplateCode = TemplateCode.BillLegalPerson,
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        private object GetPrintData(long billId, long? profileId)
        {
            var printData = _finder.Find(Specs.Find.ById<Bill>(billId))
                       .Select(bill => new
                       {
                           Bill = new
                           {
                               bill.BillNumber,
                               bill.BillDate,
                               bill.PayablePlan,
                               bill.PaymentDatePlan,
                               bill.BeginDistributionDate,
                               bill.EndDistributionDate,
                           },

                           Order = new
                           {
                               bill.Order.Number,
                               bill.Order.SignupDate,
                           },

                           BranchOfficeOrganizationUnit = new
                           {
                               bill.Order.BranchOfficeOrganizationUnit.ShortLegalName,
                               bill.Order.BranchOfficeOrganizationUnit.PostalAddress,
                               bill.Order.BranchOfficeOrganizationUnit.PhoneNumber,
                               bill.Order.BranchOfficeOrganizationUnit.PaymentEssentialElements,
                               bill.Order.BranchOfficeOrganizationUnit.ChiefNameInNominative,
                           },

                           BranchOffice = new
                           {
                               bill.Order.BranchOfficeOrganizationUnit.BranchOffice.LegalAddress,
                           },

                           LegalPerson = new
                           {
                               bill.Order.LegalPerson.LegalName,
                               bill.Order.LegalPerson.LegalAddress,
                           },

                           bill.Order.LegalPersonProfileId
                       })
                       .Single();

            var profile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(profileId.HasValue ? profileId.Value : printData.LegalPersonProfileId.Value));

            return new
            {
                printData.Bill,
                printData.Order,
                printData.BranchOfficeOrganizationUnit,
                printData.BranchOffice,
                printData.LegalPerson,

                Profile = new
                {
                    profile.PostAddress,
                    profile.Parts.OfType<EmiratesLegalPersonProfilePart>().Single().Phone,
                    profile.BankName,
                    profile.SWIFT,
                    profile.IBAN,
                    profile.AdditionalPaymentElements,
                },
            };
        }
    }
}
