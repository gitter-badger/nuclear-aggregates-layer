﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.PrintForms
{
    // FIXME {a.rechkalov, 10.11.2014}: move, IPrintValidationOperationService
    public sealed class PrintRegionalOrderTerminationNoticeHandler : RequestHandler<PrintRegionalOrderTerminationNoticeRequest, Response>
    {
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;

        public PrintRegionalOrderTerminationNoticeHandler(ISubRequestProcessor requestProcessor,
            ISecurityServiceFunctionalAccess securityServiceFunctionalAccess, 
            IFinder finder,
            IUserContext userContext)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
            _userContext = userContext;
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
        }

        protected override Response Handle(PrintRegionalOrderTerminationNoticeRequest request)
        {
            var hasPriviledge = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.MakeRegionalAdsDocs, _userContext.Identity.Code);
            if (!hasPriviledge)
            {
                throw new NotificationException(BLResources.AccessDenied);                
            }

            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                             {
                        WorkflowStep = (OrderState)order.WorkflowStepId,
                                 order.SourceOrganizationUnitId,
                                 order.DestOrganizationUnitId,
                                 order.IsTerminated
                             })
                .Single();

            if (!orderInfo.IsTerminated)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminated);                
            }

            if (orderInfo.WorkflowStep != OrderState.OnTermination && orderInfo.WorkflowStep != OrderState.Archive)
            {
                throw new NotificationException(BLResources.OrderShouldBeTerminatedOrArchive);
            }

            if (orderInfo.SourceOrganizationUnitId == orderInfo.DestOrganizationUnitId)
            {
                throw new NotificationException(BLResources.OrderSholdBeRegional);
            }

            var contributionTypeSpecificStrategies = GetContributionTypeSpecificStratagies(orderInfo.SourceOrganizationUnitId, orderInfo.DestOrganizationUnitId);

            var data = (from order in _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                             let sourceBOOU = order.SourceOrganizationUnit.BranchOfficeOrganizationUnits.FirstOrDefault(boou => boou.IsPrimaryForRegionalSales)
                             let destinationBOOU = order.DestOrganizationUnit.BranchOfficeOrganizationUnits.FirstOrDefault(boou => boou.IsPrimaryForRegionalSales)
                             select new
                                    {
                                        Order = order,
                                        SourceBranchOfficeOrganizationUnitId = sourceBOOU.Id,
                                        SourceBranchOfficeId = sourceBOOU.BranchOfficeId,
                                        DestinationBranchOfficeOrganizationUnitId = destinationBOOU.Id,
                                        VatRateForDestination = destinationBOOU.BranchOffice.BargainType.VatRate,
                                        VatRateForSource = sourceBOOU.BranchOffice.BargainType.VatRate,
                                        order.Bargain,
                                        order.EndDistributionDateFact,
                                        order.LegalPersonId,
                                        order.LegalPersonProfileId,
                                        order.Firm,
                                        CurrencyISOCode = order.Currency.ISOCode,
                                    })
                .Single();

            if (data.LegalPersonProfileId == null)
            {
                throw new RequiredFieldIsEmptyException(string.Format(BLResources.OrderFieldNotSpecified, MetadataResources.LegalPerson));
            }

            var sourceBranchOffice = _finder.FindOne(Specs.Find.ById<BranchOffice>(data.SourceBranchOfficeId));
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(data.LegalPersonId.Value));
            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(data.LegalPersonProfileId.Value));
            var sourceBranchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(data.SourceBranchOfficeOrganizationUnitId));
            var destinationBranchOfficeOrganizationUnit = _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(data.DestinationBranchOfficeOrganizationUnitId));

            var printData = new
                {
                    NotificationDate = DateTime.Now,
                    OrderProcessedPayableFact = contributionTypeSpecificStrategies.PayableFactEvaluator(data.Order,
                        GetValue(contributionTypeSpecificStrategies.UseSourceBranchVat ? data.VatRateForSource : data.VatRateForDestination)),
                    OrderProcessedNumber = contributionTypeSpecificStrategies.OrderNumberSelector(data.Order),
                    data.Order,
                    SourceBranchOfficeOrganizationUnit = sourceBranchOfficeOrganizationUnit,
                    DestinationBranchOfficeOrganizationUnit = destinationBranchOfficeOrganizationUnit,
                    SourceBranchOffice = sourceBranchOffice,
                    TerminationReleaseNumber = data.Order.EndReleaseNumberFact + 1,
                    TerminationDate = data.EndDistributionDateFact.AddDays(1),
                    LegalPerson = legalPerson,
                    LegalPersonProfile = legalPersonProfile,
                    data.Firm,
                    data.CurrencyISOCode,
                };

            var printRequest = new PrintDocumentRequest
                {
                    CurrencyIsoCode = printData.CurrencyISOCode,
                    BranchOfficeOrganizationUnitId = printData.Order.BranchOfficeOrganizationUnitId,
                    TemplateCode = contributionTypeSpecificStrategies.TemplateCode,
                    FileName = string.Format(BLResources.NotificationAboutRegionalTermination, printData.OrderProcessedNumber),
                    PrintData = printData
                };

            return _requestProcessor.HandleSubRequest(printRequest, Context);
        }

        private static decimal GetValue(decimal? val)
        {
            return val.HasValue ? val.Value : 0.0m;
        }

        private ContributionTypeEnum GetContributionType(long organizationUnitId)
        {
            var contributionType = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                .SelectMany(x => x.BranchOfficeOrganizationUnits)
                .Where(x => x.IsActive && !x.IsDeleted && x.IsPrimaryForRegionalSales)
                .Select(x => x.BranchOffice.ContributionTypeId)
                .SingleOrDefault();

            if (contributionType == null)
            {
                throw new NotificationException(string.Format(BLResources.ContributionTypeNotSpecifiedForOrgUnitWithId, organizationUnitId));                
            }

            return (ContributionTypeEnum)contributionType.Value;
        }

        private ContributionTypeSpecificStrategies GetContributionTypeSpecificStratagies(long sourceOrganizationUnitId, long destinationOrganizationUnitId)
        {
            var sourceContributionType = GetContributionType(sourceOrganizationUnitId);
            var destinationContributionType = GetContributionType(destinationOrganizationUnitId);

            ContributionTypeSpecificStrategies strategies;

            switch (destinationContributionType)
            {
                case ContributionTypeEnum.Branch:
                    {
                        switch (sourceContributionType)
                        {
                            // оформляем из филиала в филиал
                            case ContributionTypeEnum.Branch:
                                {
                                    strategies = new ContributionTypeSpecificStrategies
                                    {
                                        TemplateCode = TemplateCode.RegionalTerminationNoticeBranch2Branch,
                                        PayableFactEvaluator = ContributionTypeSpecificStrategies.PayableFactEvaluators.Branch2Branch,
                                        OrderNumberSelector = ContributionTypeSpecificStrategies.OrderNumberSelectors.Branch2Branch
                                    };
                                    break;
                                }

                            // оформляем из франчайзи в филиал
                            case ContributionTypeEnum.Franchisees:
                                {
                                    strategies = new ContributionTypeSpecificStrategies
                                    {
                                        TemplateCode = TemplateCode.RegionalTerminationNoticeNotBranch2Branch,
                                        PayableFactEvaluator = ContributionTypeSpecificStrategies.PayableFactEvaluators.NotBranch2BranchWithVat,
                                        OrderNumberSelector = ContributionTypeSpecificStrategies.OrderNumberSelectors.NotBranch2Branch
                                    };
                                    break;
                                }

                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    }

                // оформляем во франчайзи с коэффициентом 0.4
                case ContributionTypeEnum.Franchisees:
                    {
                        switch (sourceContributionType)
                        {
                            // оформляем из филиала во франчайзи
                            case ContributionTypeEnum.Branch:
                                {
                                    strategies = new ContributionTypeSpecificStrategies
                                    {
                                        TemplateCode = TemplateCode.RegionalTerminationNoticeNotBranch2Branch,
                                        PayableFactEvaluator = ContributionTypeSpecificStrategies.PayableFactEvaluators.Branch2NotBranch,
                                        OrderNumberSelector = ContributionTypeSpecificStrategies.OrderNumberSelectors.NotBranch2Branch,
                                        UseSourceBranchVat = true
                                    };
                                    break;
                                }

                            // оформляем из франчайзи во франчайзи
                            case ContributionTypeEnum.Franchisees:
                                {
                                    strategies = new ContributionTypeSpecificStrategies
                                    {
                                        TemplateCode = TemplateCode.RegionalTerminationNoticeNotBranch2Branch,
                                        PayableFactEvaluator = ContributionTypeSpecificStrategies.PayableFactEvaluators.NotBranch2NotBranch,
                                        OrderNumberSelector = ContributionTypeSpecificStrategies.OrderNumberSelectors.NotBranch2Branch
                                    };
                                    break;
                                }

                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return strategies;
        }

        private sealed class ContributionTypeSpecificStrategies
        {
            public TemplateCode TemplateCode { get; set; }
            public Func<Order, decimal, decimal> PayableFactEvaluator { get; set; }
            public Func<Order, string> OrderNumberSelector { get; set; }
            public bool UseSourceBranchVat { get; set; }

            public static class OrderNumberSelectors
            {
                public static readonly Func<Order, string> Branch2Branch = o => !string.IsNullOrEmpty(o.Number) ? o.Number : BLResources.OrderNumberIsEmpty;
                public static readonly Func<Order, string> NotBranch2Branch = o => !string.IsNullOrEmpty(o.RegionalNumber) ? o.RegionalNumber : BLResources.RegionalOrderNumberIsEmpty;
            }

            public static class PayableFactEvaluators
            {
                public static readonly Func<Order, decimal, decimal> Branch2Branch = (oe, vatRate) => 0;
                public static readonly Func<Order, decimal, decimal> NotBranch2NotBranch = (oe, vatRate) => Math.Round(oe.PayableFact * RegionalAdsRatio, 2, MidpointRounding.ToEven);
                public static readonly Func<Order, decimal, decimal> Branch2NotBranch = (oe, vatRate) => Math.Round(oe.PayableFact * RegionalAdsRatio / (1m + (vatRate / 100m)), 2, MidpointRounding.ToEven);
                public static readonly Func<Order, decimal, decimal> NotBranch2BranchWithVat = (oe, vatRate) => Math.Round(oe.PayableFact * RegionalAdsRatio * (1m + (vatRate / 100m)), 2, MidpointRounding.ToEven);

                private const decimal RegionalAdsRatio = 0.4m;
            }
        }
    }
}
