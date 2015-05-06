using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic
{
    public sealed class UkraineOrderPrintFormDataExtractor : IUkraineOrderPrintFormDataExtractor
    {
        private readonly PrintOrderHelper _printOrderHelper;
        private readonly UkrainePrintHelper _ukrainePrintHelper;

        public UkraineOrderPrintFormDataExtractor(IFormatterFactory formatterFactory, ISecurityServiceUserIdentifier userIdentifierService)
        {
            _printOrderHelper = new PrintOrderHelper(formatterFactory, userIdentifierService);
            _ukrainePrintHelper = new UkrainePrintHelper(formatterFactory);
        }

        public PrintData GetPaymentSchedule(IQueryable<Bill> query)
        {
            return _printOrderHelper.GetPaymentSchedule(query);
        }

        public PrintData GetLegalPersonProfile(LegalPersonProfile legalPersonProfile)
        {
            return new PrintData
                {
                    { "Profile", UkrainePrintHelper.LegalPersonProfileFields(legalPersonProfile) },
                    { "OperatesOnTheBasis", _ukrainePrintHelper.GetOperatesOnTheBasisInGenitive(legalPersonProfile) }
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

        public PrintData GetFirmAddresses(IQueryable<FirmAddress> query, IDictionary<long, IEnumerable<FirmContact>> contacts)
        {
            return _printOrderHelper.GetFirmAddresses(query, contacts, CultureInfo.CurrentCulture);
        }

        public PrintData GetBranchOfficeData(BranchOffice branchOffice)
        {
            return new PrintData
                {
                    { "BranchOffice", UkrainePrintHelper.BranchOfficeFields(branchOffice)},
                };
        }

        public PrintData GetBranchOfficeOrganizationUnit(BranchOfficeOrganizationUnit boou)
        {
            return new PrintData
                {
                    { "BranchOfficeOrganizationUnit", UkrainePrintHelper.BranchOfficeOrganizationUnitFields(boou) },
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
                    { "LegalPerson", UkrainePrintHelper.LegalPersonFields(legalPerson) },
                };
        }

        public PrintData GetUngrouppedFields(IQueryable<Order> query)
        {
            var categories = _printOrderHelper.GetCategories(query);

            var stuff = query
                .Select(order => new
                    {
                        order.DestOrganizationUnit.ElectronicMedia,
                        SourceElectronicMedia = order.SourceOrganizationUnit.ElectronicMedia,
                        FirmName = order.Firm.Name,
                        BargainNumber = order.Bargain.Number,
                        BeginDistributionDate = order.BeginDistributionDate,
                        order.SignupDate,
                        BargainExists = order.Bargain != null,
                    })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "UseBargainNumberExists", x.BargainExists && !string.IsNullOrWhiteSpace(x.BargainNumber) },
                        { "AdvMatherialsDeadline", PrintOrderHelper.GetAdvMatherialsDeadline(x.BeginDistributionDate, x.SignupDate) },
                        { "ElectronicMedia", x.ElectronicMedia },
                        { "SourceElectronicMedia", x.SourceElectronicMedia },
                        { "BargainNumber", x.BargainNumber },
                        { "Firm.Name", x.FirmName },
                    })
                .Single();

            return PrintData.Concat(categories, stuff);
        }
    }
}
