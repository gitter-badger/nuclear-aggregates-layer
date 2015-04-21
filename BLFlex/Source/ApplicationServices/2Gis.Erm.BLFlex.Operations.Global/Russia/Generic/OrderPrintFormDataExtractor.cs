using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;
using EnumResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.EnumResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic
{
    public sealed class OrderPrintFormDataExtractor : IOrderPrintFormDataExtractor
    {
        private readonly PrintOrderHelper _printOrderHelper;
        private readonly IFormatter _longDateFormatter;
        private readonly IFormatter _shortDateFormatter;

        public OrderPrintFormDataExtractor(IFormatterFactory formatterFactory, ISecurityServiceUserIdentifier userIdentifierService)
        {
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
            _shortDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.ShortDate, 0);
            _printOrderHelper = new PrintOrderHelper(formatterFactory, userIdentifierService);
        }

        public PrintData GetPaymentSchedule(IQueryable<Bill> query)
        {
            return _printOrderHelper.GetPaymentSchedule(query);
        }

        public PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile)
        {
            var profile = new PrintData
                {
                    { "ChiefNameInNominative", legalPersonProfile.ChiefNameInNominative },
                    { "EmailForAccountingDocuments", legalPersonProfile.EmailForAccountingDocuments },
                    { "Email", legalPersonProfile.Email },
                };

            return new PrintData
                {
                    { "Profile", profile },
                };
        }

        public PrintData GetBranchOffice(IQueryable<BranchOffice> query)
        {
            var branchOffice = query
                .Select(office => new
                {
                    office.Inn,
                    office.LegalAddress,
                })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "Inn", x.Inn },
                        { "LegalAddress", x.LegalAddress },
                    })
                .Single();

            return new PrintData
                {
                    { "BranchOffice", branchOffice },
                };
        }

        public PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou)
        {
            var branchOfficeOrganizationUnit = new PrintData
                {
                    { "ChiefNameInNominative", boou.ChiefNameInNominative },
                    { "Kpp", boou.Kpp },
                    { "PaymentEssentialElements", boou.PaymentEssentialElements },
                    { "ShortLegalName", boou.ShortLegalName },
                    { "ActualAddress", boou.ActualAddress },
                    { "Email", boou.Email },
                };

            return new PrintData
                {
                    { "BranchOfficeOrganizationUnit", branchOfficeOrganizationUnit },
                };
        }

        public PrintData GetOrder(IQueryable<Order> query)
        {
            var order = _printOrderHelper.GetOrder(query);

            return new PrintData 
            { 
                { "Order", order }, 
            };
        }

        public PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> query, SalesModel? salesModel)
        {
            return _printOrderHelper.GetOrderPositionsWithDetailedName(orderQuery, query, salesModel);
        }

        public PrintData GetUngrouppedFields(IQueryable<Order> query, BranchOfficeOrganizationUnit branchOfficeOrganizationUnit, LegalPerson legalPerson, LegalPersonProfile legalPersonProfile)
        {
            var data = query
                .Select(order => new
                {
                    order.BeginDistributionDate,
                    order.SignupDate,
                    order.LegalPersonId,
                    order.DestOrganizationUnit.ElectronicMedia,
                    Bargain = new[] { order.Bargain }.Where(b => b != null).Select(b => new { b.Number, b.CreatedOn }).FirstOrDefault(),
                    SourceElectronicMedia = order.SourceOrganizationUnit.ElectronicMedia,
                    Order = order,
                    TerminatedOrder = order.TechnicallyTerminatedOrder,
                    
                    order.PayablePlan,

                    discountSum = order.DiscountSum.HasValue ? order.DiscountSum.Value : 0,
                })
                .Single();

            var platforms = query.SelectMany(order => order.OrderPositions)
                                 .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                                 .Select(position => (PlatformEnum)position.PricePosition.Position.Platform.DgppId)
                                 .Distinct()
                                 .ToArray();

            return new PrintData
                {
                    { "AdvMatherialsDeadline", PrintOrderHelper.GetAdvMatherialsDeadline(data.BeginDistributionDate, data.SignupDate) },
                    { "BeginContractParagraph", GetBeginContractParagraph(branchOfficeOrganizationUnit, legalPerson, legalPersonProfile) },
                    { "ClientRequisitesParagraph", GetClientRequisitesParagraph(legalPerson, legalPersonProfile) },
                    { "ElectronicMedia", data.ElectronicMedia },
                    { "RelatedBargainInfo", data.Bargain != null ? GetRelatedBargainInfo(data.Bargain.Number, data.Bargain.CreatedOn) : null },
                    { "SourceElectronicMedia", data.SourceElectronicMedia },
                    { "TerminatedOrder", GetTerminatedOrder(data.TerminatedOrder) },
                    { "UseTechnicalTermination", data.TerminatedOrder != null },
                    { "DiscountSum", data.discountSum },
                    { "PriceWithoutDiscount", data.discountSum + data.PayablePlan },
                    { "UseAsteriskParagraph", platforms.Contains(PlatformEnum.Independent) || platforms.Contains(PlatformEnum.Api) },
                };
        }

        private string GetRelatedBargainInfo(string bargainNumber, DateTime createdOn)
        {
            return string.Format(BLCoreResources.RelatedToBargainInfoTemplate, bargainNumber, _longDateFormatter.Format(createdOn));
        }

        private string GetBeginContractParagraph(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit, LegalPerson legalPerson, LegalPersonProfile profile)
        {
            var operatesOnTheBasisInGenitive = string.Empty;
            switch (legalPerson.LegalPersonTypeEnum)
            {
                case LegalPersonType.NaturalPerson:

                    if (profile != null && profile.OperatesOnTheBasisInGenitive != null)
                    {
                        switch (profile.OperatesOnTheBasisInGenitive)
                        {
                            case OperatesOnTheBasisType.Undefined:
                                operatesOnTheBasisInGenitive = string.Empty;
                                break;
                            case OperatesOnTheBasisType.Warranty:
                                operatesOnTheBasisInGenitive = string.Format(
                                    BLCoreResources.OperatesOnBasisOfWarantyTemplateForNaturalPerson,
                                    profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                    profile.WarrantyNumber,
                                    _shortDateFormatter.Format(profile.WarrantyBeginDate));
                                break;
                            default:
                                throw new BusinessLogicDataException(legalPerson.LegalPersonTypeEnum, profile.OperatesOnTheBasisInGenitive.Value);
                        }
                    }

                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLFlexResources.PrintOrderHandler_BeginContractParagraph1,
                        branchOfficeOrganizationUnit.ShortLegalName,
                        branchOfficeOrganizationUnit.PositionInGenitive,
                        branchOfficeOrganizationUnit.ChiefNameInGenitive,
                        branchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive,
                        legalPerson.LegalName,
                        operatesOnTheBasisInGenitive);

                case LegalPersonType.Businessman:
                case LegalPersonType.LegalPerson:
                    {
                        if (profile != null && profile.OperatesOnTheBasisInGenitive != null)
                        {
                            switch (profile.OperatesOnTheBasisInGenitive)
                            {
                                case OperatesOnTheBasisType.Undefined:
                                    operatesOnTheBasisInGenitive =
                                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture);
                                    break;
                                case OperatesOnTheBasisType.Charter:
                                    operatesOnTheBasisInGenitive = string.Format(
                                        BLCoreResources.OperatesOnBasisOfCharterTemplate,
                                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                                    break;
                                case OperatesOnTheBasisType.Certificate:
                                    operatesOnTheBasisInGenitive = string.Format(
                                        BLCoreResources.OperatesOnBasisOfCertificateTemplate,
                                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                        profile.CertificateNumber,
                                        _shortDateFormatter.Format(profile.CertificateDate));
                                    break;
                                case OperatesOnTheBasisType.Warranty:
                                    operatesOnTheBasisInGenitive = string.Format(
                                        BLCoreResources.OperatesOnBasisOfWarantyTemplate,
                                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                        profile.WarrantyNumber,
                                        _shortDateFormatter.Format(profile.WarrantyBeginDate));
                                    break;
                                case OperatesOnTheBasisType.Bargain:
                                    operatesOnTheBasisInGenitive = string.Format(
                                        BLCoreResources.OperatesOnBasisOfBargainTemplate,
                                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                        profile.BargainNumber,
                                        _shortDateFormatter.Format(profile.BargainBeginDate));
                                    break;
                                case OperatesOnTheBasisType.FoundingBargain:
                                    operatesOnTheBasisInGenitive = string.Format(
                                        BLCoreResources.OperatesOnBasisOfFoundingBargainTemplate,
                                        profile.OperatesOnTheBasisInGenitive.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                                    break;
                                default:
                                    throw new BusinessLogicDataException(legalPerson.LegalPersonTypeEnum, profile.OperatesOnTheBasisInGenitive.Value);
                            }
                        }

                        return string.Format(
                            CultureInfo.CurrentCulture,
                            BLFlexResources.PrintOrderHandler_BeginContractParagraph2,
                            branchOfficeOrganizationUnit.ShortLegalName,
                            branchOfficeOrganizationUnit.PositionInGenitive,
                            branchOfficeOrganizationUnit.ChiefNameInGenitive,
                            branchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive,
                            legalPerson.LegalName,
                            profile != null ? profile.PositionInGenitive : string.Empty,
                            profile != null ? profile.ChiefNameInGenitive : string.Empty,
                            operatesOnTheBasisInGenitive);
                    }

                default:
                    throw new BusinessLogicDataException(legalPerson.LegalPersonTypeEnum);
            }
        }

        private static string GetClientRequisitesParagraph(LegalPerson legalPerson, LegalPersonProfile profile)
        {
            switch (legalPerson.LegalPersonTypeEnum)
            {
                case LegalPersonType.NaturalPerson:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLFlexResources.PrintOrderHandler_ClientRequisitesParagraph1,
                        legalPerson.LegalName,
                        legalPerson.PassportSeries,
                        legalPerson.PassportNumber,
                        legalPerson.PassportIssuedBy,
                        legalPerson.RegistrationAddress);
                case LegalPersonType.Businessman:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLFlexResources.PrintOrderHandler_ClientRequisitesParagraph2,
                        legalPerson.LegalName,
                        legalPerson.Inn,
                        legalPerson.LegalAddress,
                        profile != null ? profile.PaymentEssentialElements : string.Empty);
                case LegalPersonType.LegalPerson:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLFlexResources.PrintOrderHandler_ClientRequisitesParagraph3,
                        legalPerson.LegalName,
                        legalPerson.Inn,
                        legalPerson.Kpp,
                        legalPerson.LegalAddress,
                        profile != null ? profile.PaymentEssentialElements : string.Empty);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private PrintData GetTerminatedOrder(Order terminatedOrder)
        {
            return terminatedOrder == null
                       ? null
                       : new PrintData
                             {
                                 { "Number", terminatedOrder.Number },
                                 { "SignupDate", terminatedOrder.SignupDate },
                                 { "EndDistributionDateFact", terminatedOrder.EndDistributionDateFact },
                             };
        }

        private sealed class BusinessLogicDataException : BusinessLogicException
        {
            public BusinessLogicDataException(LegalPersonType legalPerson, OperatesOnTheBasisType operatesOnTheBasis)
                : base(GenerateMessage(legalPerson, operatesOnTheBasis))
            {
            }

            public BusinessLogicDataException(LegalPersonType legalPerson)
                : base(GenerateMessage(legalPerson))
            {
            }

            private static string GenerateMessage(LegalPersonType legalPerson)
            {
                return string.Format(BLFlexResources.CannotPrintOrderForLegalPerson,
                                     legalPerson.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
            }

            private static string GenerateMessage(LegalPersonType legalPerson, OperatesOnTheBasisType operatesOnTheBasis)
            {
                return string.Format(BLFlexResources.CannotPrintOrderForLegalPersonMainDocument,
                                     legalPerson.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                     operatesOnTheBasis.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture).ToLower());
            }
        }
    }
}
