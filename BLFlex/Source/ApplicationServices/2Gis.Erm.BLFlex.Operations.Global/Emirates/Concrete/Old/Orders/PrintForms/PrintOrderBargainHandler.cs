using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
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
                Tuple.Create("en", TemplateCode.ClientBargain),
                Tuple.Create("en-AE", TemplateCode.ClientBargainAlternativeLanguage)
            };

        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IOrderReadModel _orderReadModel;

        public PrintOrderBargainHandler(IFinder finder, ISubRequestProcessor requestProcessor, IOrderReadModel orderReadModel)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _orderReadModel = orderReadModel;
        }

        protected override Response Handle(PrintOrderBargainRequest request)
        {
            var bargainId = request.BargainId ?? _orderReadModel.GetBargainIdByOrder(request.OrderId.Value);
            var legalPersonProfileId = request.LegalPersonProfileId ?? _orderReadModel.GetLegalPersonProfileIdByOrder(request.OrderId.Value);

            if (bargainId == null)
            {
                throw new EntityNotLinkedException(typeof(Order), request.OrderId.Value, typeof(Bargain));
            }

            if (legalPersonProfileId == null)
            {
                throw new FieldNotSpecifiedException(BLResources.LegalPersonProfileMustBeSpecified);
            }

            var bargainInfo =
                _finder.Find(Specs.Find.ById<Bargain>(bargainId.Value))
                       .Select(x => new
            {
                               BranchOfficeOrganizationUnitId = x.ExecutorBranchOfficeId,
                               BargainNumber = x.Number,
                           })
                       .SingleOrDefault();

            if (bargainInfo == null)
            {
                throw new EntityNotFoundException(typeof(Bargain), bargainId.Value);
            }

            var printdata = GetPrintData(bargainId.Value, legalPersonProfileId.Value);
            var streamDictionary = DocumentVariants.Select(variant => PrintDocument(printdata,
                                                                                    bargainInfo.BranchOfficeOrganizationUnitId,
                                                                                    variant.Item2,
                                                                                    bargainInfo.BargainNumber,
                                                                                    variant.Item1))
                                                   .ToDictionary(response => response.FileName, response => response.Stream)
                                                   .ZipStreamDictionary();

            return new StreamResponse
            {
                Stream = streamDictionary,
                ContentType = MediaTypeNames.Application.Zip,
                FileName = string.Format("{0}.zip", bargainInfo.BargainNumber)
            };
        }

        private object GetPrintData(long bargainId, long legalPersonProfileId)
        {
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId));
            var data = _finder.Find(Specs.Find.ById<Bargain>(bargainId))
                          .Select(x => new
                              {
                                  Bargain = new
                                      {
                                          x.Number,
                                          x.SignedOn,
                                      },

                                  BranchOfficeOrganizationUnit = new
                                      {
                                          x.BranchOfficeOrganizationUnit.ShortLegalName,
                                          x.BranchOfficeOrganizationUnit.PositionInNominative,
                                          x.BranchOfficeOrganizationUnit.ChiefNameInNominative,
                                          x.BranchOfficeOrganizationUnit.PostalAddress,
                                          x.BranchOfficeOrganizationUnit.PhoneNumber,
                                          x.BranchOfficeOrganizationUnit.PaymentEssentialElements,
                                      },

                                  BranchOffice = new
                                      {
                                          x.BranchOfficeOrganizationUnit.BranchOffice.Inn,
                                          x.BranchOfficeOrganizationUnit.BranchOffice.LegalAddress,
                                      },

                                  LegalPerson = new
                                      {
                                          x.LegalPerson.LegalName,
                                          x.LegalPerson.LegalAddress,
                                          x.LegalPerson.Inn,
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
                    legalPersonProfile.PaymentEssentialElements,
                    legalPersonProfile.Parts.OfType<EmiratesLegalPersonProfilePart>().Single().Phone,
                }
            };
        }

        private StreamResponse PrintDocument(object printData, long boouId, TemplateCode templateCode, string bargainNumber, string documentSuffix)
        {
            var request = new PrintDocumentRequest
                {
                    BranchOfficeOrganizationUnitId = boouId,
                    TemplateCode = templateCode,
                    FileName = bargainNumber + "." + documentSuffix,
                    PrintData = printData
                };

            return (StreamResponse)_requestProcessor.HandleSubRequest(request, Context);
        }
    }
}
