using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic
{
    public sealed class OrderPrintFormDataExtractor : IOrderPrintFormDataExtractor
    {
        private readonly PrintOrderHelper _printOrderHelper;

        public OrderPrintFormDataExtractor(IFormatterFactory formatterFactory, ISecurityServiceUserIdentifier userIdentifierService)
        {
            _printOrderHelper = new PrintOrderHelper(formatterFactory, userIdentifierService);
        }

        public PrintData GetPaymentSchedule(IQueryable<Bill> query)
        {
            return _printOrderHelper.GetPaymentSchedule(query);
        }

        public PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile)
        {
            return new PrintData
                {
                    { "Profile", PrintHelper.LegalPersonProfileFields(legalPersonProfile) },
                    { "UseOperatesOnTheBasis", legalPersonProfile.OperatesOnTheBasisInGenitive.HasValue && legalPersonProfile.OperatesOnTheBasisInGenitive != (int)OperatesOnTheBasisType.Undefined },
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

        public PrintData GetBargain(Bargain bargain)
        {
            if (bargain == null)
            {
                return new PrintData
                {
                    { "UseBargain", false },
                };
            }

            var bargainData = new PrintData
                {
                    { "Number", bargain.Number },
                    { "SignedOn", bargain.SignedOn },
                };

            return new PrintData
                {
                    { "UseBargain", true },
                    { "Bargain", bargainData },
                };
        }

        public PrintData GetBranchOfficeData(BranchOffice branchOffice)
        {
            return new PrintData
                {
                    { "BranchOffice", PrintHelper.BranchOfficeFields(branchOffice)},
                };
        }

        public PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou)
        {
            return new PrintData
                {
                    { "BranchOfficeOrganizationUnit", PrintHelper.BranchOfficeOrganizationUnitFields(boou) },
                };
        }

        public PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> orderPositionsQuery)
        {
            return _printOrderHelper.GetOrderPositions(orderQuery, orderPositionsQuery);
        }

        public PrintData GetLegalPersonData(LegalPerson legalPerson)
        {
            return new PrintData
                {
                    { "UseLegalPersonOrBusinessman", legalPerson.LegalPersonTypeEnum == LegalPersonType.Businessman || legalPerson.LegalPersonTypeEnum == LegalPersonType.LegalPerson },
                    { "UseNaturalPerson", legalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson },
                    { "LegalPerson", PrintHelper.LegalPersonFields(legalPerson) },
                };
        }

        public PrintData GetUngrouppedFields(IQueryable<Order> query)
        {
            var categories = _printOrderHelper.GetCategories(query);

            var stuff = query
                .Select(order => new
                                     {
                                         SourceElectronicMedia = order.SourceOrganizationUnit.ElectronicMedia,
                                         order.BeginDistributionDate,
                                         order.SignupDate,
                                         order.PaymentMethod,
                                         order.PayablePlan,
                                         DiscountSum = order.DiscountSum.HasValue ? order.DiscountSum.Value : 0,
                                     })
                .AsEnumerable()
                .Select(x => new PrintData
                                 {
                                     { "AdvMatherialsDeadline", PrintOrderHelper.GetAdvMaterialsDeadline(x.BeginDistributionDate, x.SignupDate) },
                                     { "SourceElectronicMedia", x.SourceElectronicMedia },
                                     {
                                         "PaymentMethod",
                                         x.PaymentMethod != 0
                                             ? ((PaymentMethod)x.PaymentMethod).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)
                                             : string.Empty
                                     },
                                     { "DiscountSum", x.DiscountSum },
                                     { "PriceWithoutDiscount", x.DiscountSum + x.PayablePlan },
                                 })
                .Single();

            return PrintData.Concat(categories, stuff);
        }
    }
}