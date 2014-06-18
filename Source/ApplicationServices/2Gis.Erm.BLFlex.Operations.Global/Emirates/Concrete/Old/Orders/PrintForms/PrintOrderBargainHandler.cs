using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderBargainHandler : RequestHandler<PrintOrderBargainRequest, Response>, IEmiratesAdapted
    {
        private static readonly IEnumerable<Tuple<string, TemplateCode>> DocumentVariants = new List<Tuple<string, TemplateCode>>
            {
                Tuple.Create("en", TemplateCode.BargainLegalPerson),
                Tuple.Create("en-AE", TemplateCode.BargainLegalPersonAlternativeLanguage)
            };

        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintOrderBargainHandler(IFinder finder, ISubRequestProcessor requestProcessor)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            var orderInfo =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                       .Select(order => new
                           {
                               CurrencyIsoCode = order.Currency.ISOCode,
                               order.BranchOfficeOrganizationUnitId,
                               order.BargainId,
                               order.LegalPersonProfileId,
                               BargainNumber = order.Bargain.Number,
                           })
                       .SingleOrDefault();

            if (orderInfo == null)
            {
                throw new EntityNotFoundException(typeof(Order), request.OrderId);
            }

            if (orderInfo.BargainId == null)
            {
                throw new EntityNotLinkedException(typeof(Order), request.OrderId, typeof(Bargain));
            }

            if (orderInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new EntityNotLinkedException(typeof(Order), request.OrderId, typeof(BranchOfficeOrganizationUnit));
            }

            var legalPersonProfileId = request.LegalPersonProfileId.HasValue ? request.LegalPersonProfileId.Value : orderInfo.LegalPersonProfileId.Value;
            var printdata = GetPrintData(request.OrderId, legalPersonProfileId);
            var streamDictionary = DocumentVariants.Select(variant => PrintDocument(printdata,
                                                                                    orderInfo.CurrencyIsoCode,
                                                                                    orderInfo.BranchOfficeOrganizationUnitId.Value,
                                                                                    variant.Item2,
                                                                                    orderInfo.BargainNumber,
                                                                                    variant.Item1))
                                                   .ToDictionary(response => response.FileName, response => response.Stream)
                                                   .ZipStreamDictionary();

            return new StreamResponse
            {
                Stream = streamDictionary,
                ContentType = MediaTypeNames.Application.Zip,
                FileName = string.Format("{0}.zip", orderInfo.BargainNumber)
            };
        }

        private object GetPrintData(long orderId, long legalPersonProfileId)
        {
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId));
            var data = _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => new
                              {
                                  Bargain = new
                                      {
                                          order.Bargain.Number,
                                          order.Bargain.SignedOn,
                                      },

                                  BranchOfficeOrganizationUnit = new
                                      {
                                          order.BranchOfficeOrganizationUnit.ShortLegalName,
                                          order.BranchOfficeOrganizationUnit.PositionInNominative,
                                          order.BranchOfficeOrganizationUnit.ChiefNameInNominative,
                                          order.BranchOfficeOrganizationUnit.PostalAddress,
                                          order.BranchOfficeOrganizationUnit.PhoneNumber,
                                          order.BranchOfficeOrganizationUnit.PaymentEssentialElements,
                                      },

                                  BranchOffice = new
                                      {
                                          order.BranchOfficeOrganizationUnit.BranchOffice.Inn,
                                          order.BranchOfficeOrganizationUnit.BranchOffice.LegalAddress,
                                      },

                                  LegalPerson = new
                                      {
                                          order.LegalPerson.LegalName,
                                          order.LegalPerson.LegalAddress,
                                          order.LegalPerson.Inn,
                                      },
                              })
                          .Single();

            return new 
            {
                data.Bargain,
                data.BranchOfficeOrganizationUnit,
                data.BranchOffice,
                data.LegalPerson,

                Profile = new
                {
                    legalPersonProfile.PositionInNominative,
                    legalPersonProfile.ChiefNameInNominative,
                    legalPersonProfile.PostAddress,
                    legalPersonProfile.BankName,
                    legalPersonProfile.SWIFT,
                    legalPersonProfile.IBAN,
                    legalPersonProfile.AdditionalPaymentElements,
                    legalPersonProfile.Parts.OfType<EmiratesLegalPersonProfilePart>().Single().Phone,
                }
            };
        }

        private StreamResponse PrintDocument(object printData, short currencyIsoCode, long boouId, TemplateCode templateCode, string bargainNumber, string documentSuffix)
        {
            var request = new PrintDocumentRequest
                {
                    CurrencyIsoCode = currencyIsoCode,
                    BranchOfficeOrganizationUnitId = boouId,
                    TemplateCode = templateCode,
                    FileName = bargainNumber + "." + documentSuffix,
                    PrintData = printData
                };

            return (StreamResponse)_requestProcessor.HandleSubRequest(request, Context);
        }
    }
}
