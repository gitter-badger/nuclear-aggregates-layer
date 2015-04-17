using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Bargains.PrintForms
{
    public sealed class PrintBargainProlongationAgreementHandler : RequestHandler<PrintBargainProlongationAgreementRequest, Response>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly MultiCulturePrintHelper _printHelper;
        private readonly IFormatter _shortDateFormatter;

        public PrintBargainProlongationAgreementHandler(IFinder finder, ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory)
        {
            _finder = finder;
            _requestProcessor = requestProcessor;
            _printHelper = new MultiCulturePrintHelper(formatterFactory);
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
        }

        protected override Response Handle(PrintBargainProlongationAgreementRequest request)
        {
            var bargainInfo =
                _finder.Find(Specs.Find.ById<Bargain>(request.BargainId))
                       .Select(x => new
                           {
                               BranchOfficeOrganizationUnitId = x.ExecutorBranchOfficeId,
                               BargainNumber = x.Number,
                           })
                       .SingleOrDefault();

            if (bargainInfo == null)
            {
                throw new EntityNotFoundException(typeof(Bargain), request.BargainId);
            }

            var legalPersonProfileId = request.LegalPersonProfileId;
            var printdata = GetPrintData(request.BargainId, legalPersonProfileId);
            var printRequest = new PrintDocumentRequest
                {
                    BranchOfficeOrganizationUnitId = bargainInfo.BranchOfficeOrganizationUnitId,
                    TemplateCode = TemplateCode.BargainProlongationAgreement,
                    FileName = "доп. соглашение к договору " + bargainInfo.BargainNumber,
                    PrintData = printdata
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        private PrintData GetPrintData(long bargainId, long legalPersonProfileId)
        {
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId));
            var data = _finder.Find(Specs.Find.ById<Bargain>(bargainId))
                              .Select(x => new
                                  {
                                      OrganizationUnitName = x.BranchOfficeOrganizationUnit.OrganizationUnit.Name,

                                      Bargain = new
                                          {
                                              x.Number,
                                              x.SignedOn,
                                              EndDate = x.BargainEndDate
                                          },

                                      BranchOfficeOrganizationUnit = new
                                          {
                                              x.BranchOfficeOrganizationUnit.ApplicationCityName,
                                              x.BranchOfficeOrganizationUnit.ShortLegalName,
                                              x.BranchOfficeOrganizationUnit.PositionInNominative,
                                              x.BranchOfficeOrganizationUnit.ChiefNameInNominative,
                                              x.BranchOfficeOrganizationUnit.PositionInGenitive,
                                              x.BranchOfficeOrganizationUnit.ChiefNameInGenitive,
                                              x.BranchOfficeOrganizationUnit.Email,
                                              x.BranchOfficeOrganizationUnit.PaymentEssentialElements,
                                              x.BranchOfficeOrganizationUnit.Kpp,
                                              x.BranchOfficeOrganizationUnit.ActualAddress,
                                              x.BranchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive,
                                          },

                                      BranchOffice = new
                                          {
                                              x.BranchOfficeOrganizationUnit.BranchOffice.Inn,
                                              x.BranchOfficeOrganizationUnit.BranchOffice.LegalAddress,
                                          },

                                      LegalPerson = new
                                          {
                                              x.LegalPerson.LegalName,
                                              x.LegalPerson.ShortName,
                                              x.LegalPerson.PassportIssuedBy,
                                              x.LegalPerson.PassportNumber,
                                              x.LegalPerson.PassportSeries,
                                              x.LegalPerson.LegalAddress,
                                              x.LegalPerson.RegistrationAddress,
                                              x.LegalPerson.Inn,
                                              x.LegalPerson.Kpp,

                                              x.LegalPerson.LegalPersonTypeEnum
                                          },
                                  })
                              .Single();

            var legalPersonType = data.LegalPerson.LegalPersonTypeEnum;

            return new PrintData
                {
                    {
                        "Bargain", new PrintData
                            {
                                { "Number", data.Bargain.Number },
                                { "SignedOn", data.Bargain.SignedOn },
                                { "EndDate", data.Bargain.EndDate }
                            }
                    },
                    {
                        "BranchOfficeOrganizationUnit", new PrintData
                            {
                                { "ApplicationCityName", data.BranchOfficeOrganizationUnit.ApplicationCityName },
                                { "ShortLegalName", data.BranchOfficeOrganizationUnit.ShortLegalName },
                                { "PositionInNominative", data.BranchOfficeOrganizationUnit.PositionInNominative },
                                { "ChiefNameInNominative", data.BranchOfficeOrganizationUnit.ChiefNameInNominative },
                                { "PositionInGenitive", data.BranchOfficeOrganizationUnit.PositionInGenitive },
                                { "ChiefNameInGenitive", data.BranchOfficeOrganizationUnit.ChiefNameInGenitive },
                                { "Email", data.BranchOfficeOrganizationUnit.Email },
                                { "PaymentEssentialElements", data.BranchOfficeOrganizationUnit.PaymentEssentialElements },
                                { "Kpp", data.BranchOfficeOrganizationUnit.Kpp },
                                { "ActualAddress", data.BranchOfficeOrganizationUnit.ActualAddress },
                                { "OperatesOnTheBasisInGenitive", data.BranchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive },
                            }
                    },
                    {
                        "BranchOffice", new PrintData
                            {
                                { "Inn", data.BranchOffice.Inn },
                                { "LegalAddress", data.BranchOffice.LegalAddress },
                            }
                    },
                    {
                        "LegalPerson", new PrintData
                            {
                                { "LegalName", data.LegalPerson.LegalName },
                                { "ShortName", data.LegalPerson.ShortName },
                                { "PassportIssuedBy", data.LegalPerson.PassportIssuedBy },
                                { "PassportNumber", data.LegalPerson.PassportNumber },
                                { "PassportSeries", data.LegalPerson.PassportSeries },
                                { "LegalAddress", data.LegalPerson.LegalAddress },
                                { "RegistrationAddress", data.LegalPerson.RegistrationAddress },
                                { "Inn", data.LegalPerson.Inn },
                                { "Kpp", data.LegalPerson.Kpp },
                            }
                    },
                    {
                        "LegalPersonProfile", new PrintData
                            {
                                { "PositionInNominative", legalPersonProfile.PositionInNominative },
                                { "ChiefNameInNominative", legalPersonProfile.ChiefNameInNominative },
                                { "PositionInGenitive", legalPersonProfile.PositionInGenitive },
                                { "ChiefNameInGenitive", legalPersonProfile.ChiefNameInGenitive },
                                { "PostAddress", legalPersonProfile.PostAddress },
                                { "PaymentEssentialElements", legalPersonProfile.PaymentEssentialElements },
                                { "OperatesOnTheBasisInGenitive", _printHelper.GetOperatesOnTheBasisInGenitive(legalPersonProfile, legalPersonType) }
                            }
                    },
                    { "UseBusinessman", legalPersonType == LegalPersonType.Businessman },
                    { "UseLegalPerson", legalPersonType == LegalPersonType.LegalPerson },
                    { "UseNaturalPerson", legalPersonType == LegalPersonType.NaturalPerson },
                    { "UseEndlessBargain", data.Bargain.EndDate == null },
                    { "UseLimitedBargain", data.Bargain.EndDate != null },
                    { "CurrentDate", _shortDateFormatter.Format(DateTime.Today) },
                    { "OrganizationUnitName", data.OrganizationUnitName }
                };
        }
    }
}