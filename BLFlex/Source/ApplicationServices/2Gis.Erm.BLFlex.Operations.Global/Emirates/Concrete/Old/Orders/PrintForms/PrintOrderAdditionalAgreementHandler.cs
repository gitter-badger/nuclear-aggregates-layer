using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Security.API.UserContext;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderAdditionalAgreementHandler : RequestHandler<PrintOrderAdditionalAgreementRequest, Response>, IEmiratesAdapted
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintOrderAdditionalAgreementHandler(ISubRequestProcessor requestProcessor, IUserContext userContext, IFinder finder)
        {
            _finder = finder;
            _userContext = userContext;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintOrderAdditionalAgreementRequest request)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                                   .Select(order => new
                                       {
                                           WorkflowStep = order.WorkflowStepId,
                                           OrderNumber = order.Number,
                                           order.IsTerminated,
                                           order.RejectionDate,
                                           CurrencyISOCode = order.Currency.ISOCode,
                                           order.BranchOfficeOrganizationUnitId,
                                           order.LegalPersonProfileId,
                                       })
                                   .Single();

            if (orderInfo.LegalPersonProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            if (!orderInfo.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);
            }

            if (orderInfo.WorkflowStep != OrderState.OnTermination && orderInfo.WorkflowStep != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            if (orderInfo.RejectionDate == null)
            {
                throw new NotificationException(BLResources.OrderRejectDateFieldIsNotFilled);
            }

            TemplateCode templateCode;
            string template;
            object printData;
            switch (request.PrintType)
            {
                case PrintAdditionalAgreementTarget.Order:
                    templateCode = TemplateCode.AdditionalAgreementLegalPerson;
                    template = "{0} - Termination of the Quotation.docx";
                    printData = GetOrderTerminationData(request.OrderId, orderInfo.LegalPersonProfileId.Value);
                    break;
                case PrintAdditionalAgreementTarget.Bargain:
                    templateCode = TemplateCode.BargainAdditionalAgreementLegalPerson;
                    template = "{0} - Termination of the Contract.docx";
                    printData = GetBargainTerminationData(request.OrderId, orderInfo.LegalPersonProfileId.Value);
                    break;
                default:
                    throw new ArgumentException("request");
            }

            var printRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = orderInfo.CurrencyISOCode,
                    BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId,
                    TemplateCode = templateCode,
                    FileName = string.Format(template, orderInfo.OrderNumber),
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        private object GetBargainTerminationData(long orderId, long legalPersonProfileId)
        {
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId));
            var data = _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => new
                          {
                              Bargain = new
                              {
                                  order.Bargain.Number,
                                  order.Bargain.SignedOn,
                                  order.Bargain.ClosedOn,
                              },

                              BranchOfficeOrganizationUnit = new
                              {
                                  order.BranchOfficeOrganizationUnit.ShortLegalName,
                                  order.BranchOfficeOrganizationUnit.PostalAddress,
                                  order.BranchOfficeOrganizationUnit.PhoneNumber,
                                  order.BranchOfficeOrganizationUnit.PaymentEssentialElements,
                                  order.BranchOfficeOrganizationUnit.ChiefNameInNominative,
                              },

                              BranchOffice = new
                              {
                                  order.BranchOfficeOrganizationUnit.BranchOffice.LegalAddress,
                              },

                              LegalPerson = new
                              {
                                  order.LegalPerson.LegalName,
                                  order.LegalPerson.LegalAddress,
                              },
                          })
                          .Single();

            return new
            {
                DateToday = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo),
                data.Bargain,
                data.BranchOfficeOrganizationUnit,
                data.BranchOffice,
                data.LegalPerson,

                Profile = new
                {
                    legalPersonProfile.PostAddress,
                    legalPersonProfile.Parts.OfType<EmiratesLegalPersonProfilePart>().Single().Phone,
                    legalPersonProfile.BankName,
                    legalPersonProfile.SWIFT,
                    legalPersonProfile.IBAN,
                    legalPersonProfile.PaymentEssentialElements,
                    legalPersonProfile.ChiefNameInNominative,
                }
            };
        }

        private object GetOrderTerminationData(long orderId, long legalPersonProfileId)
        {
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId));
            var data = _finder.Find(Specs.Find.ById<Order>(orderId))
                              .Select(order => new
                                  {
                                      Order = new
                                          {
                                              order.Number,
                                              order.SignupDate,
                                              order.RejectionDate
                                          },

                                      Bargain = order.Bargain == null
                                                    ? null
                                                    : new
                                                        {
                                                            order.Bargain.Number,
                                                            order.Bargain.SignedOn,
                                                        },

                                      BranchOfficeOrganizationUnit = new
                                          {
                                              order.BranchOfficeOrganizationUnit.ShortLegalName,
                                              order.BranchOfficeOrganizationUnit.PostalAddress,
                                              order.BranchOfficeOrganizationUnit.PhoneNumber,
                                              order.BranchOfficeOrganizationUnit.PaymentEssentialElements,
                                              order.BranchOfficeOrganizationUnit.ChiefNameInNominative,
                                          },

                                      BranchOffice = new
                                          {
                                              order.BranchOfficeOrganizationUnit.BranchOffice.LegalAddress,
                                          },

                                      LegalPerson = new
                                          {
                                              order.LegalPerson.LegalName,
                                              order.LegalPerson.LegalAddress,
                                          },
                                  })
                              .Single();

            var bargain = data.Bargain != null
                              ? new PrintData
                                  {
                                      { "Number", data.Bargain.Number },
                                      { "SignedOn", data.Bargain.SignedOn },
                                  }
                              : null;

            return new PrintData
                {
                    { "DateToday", TimeZoneInfo.ConvertTime(DateTime.UtcNow, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo) },

                    { "Order.Number", data.Order.Number },
                    { "Order.SignupDate", data.Order.SignupDate },
                    { "Order.RejectionDate", data.Order.RejectionDate },

                    { "UseBargain", bargain != null },
                    { "Bargain", bargain },

                    { "BranchOfficeOrganizationUnit.ShortLegalName", data.BranchOfficeOrganizationUnit.ShortLegalName },
                    { "BranchOfficeOrganizationUnit.PostalAddress", data.BranchOfficeOrganizationUnit.PostalAddress },
                    { "BranchOfficeOrganizationUnit.PhoneNumber", data.BranchOfficeOrganizationUnit.PhoneNumber },
                    { "BranchOfficeOrganizationUnit.PaymentEssentialElements", data.BranchOfficeOrganizationUnit.PaymentEssentialElements },
                    { "BranchOfficeOrganizationUnit.ChiefNameInNominative", data.BranchOfficeOrganizationUnit.ChiefNameInNominative },

                    { "BranchOffice.LegalAddress", data.BranchOffice.LegalAddress },

                    { "LegalPerson.LegalName", data.LegalPerson.LegalName },
                    { "LegalPerson.LegalAddress", data.LegalPerson.LegalAddress },

                    { "Profile.PostAddress", legalPersonProfile.PostAddress },
                    { "Profile.Phone", legalPersonProfile.Parts.OfType<EmiratesLegalPersonProfilePart>().Single().Phone },
                    { "Profile.BankName", legalPersonProfile.BankName },
                    { "Profile.SWIFT", legalPersonProfile.SWIFT },
                    { "Profile.IBAN", legalPersonProfile.IBAN },
                    { "Profile.PaymentEssentialElements", legalPersonProfile.PaymentEssentialElements },
                    { "Profile.ChiefNameInNominative", legalPersonProfile.ChiefNameInNominative },
                };
        }
    }
}