﻿using System;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Old.AfterSaleServices;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Old.AfterSaleService
{
    public class CreateAfterSaleServiceActivitiesHandlerTest :
        UseModelEntityHandlerTestBase<OrganizationUnit, CreateAfterSaleServiceActivitiesRequest, CreateAfterSaleServiceActivitiesResponse>
    {
        private readonly TimePeriod _period;
        public CreateAfterSaleServiceActivitiesHandlerTest(IPublicService publicService, IAppropriateEntityProvider<OrganizationUnit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
            var now = DateTime.UtcNow;
            _period = new TimePeriod(now.GetFirstDateOfMonth(), now.GetLastDateOfMonth());
        }

        protected override FindSpecification<OrganizationUnit> ModelEntitySpec
        {
            get
            {
                var restrictedTypes = new[] { (int)OrderType.SelfAds, (int)OrderType.None, (int)OrderType.ProductAdsService };
                var allowedStates = new[] { (int)OrderState.Approved, (int)OrderState.OnTermination };
                return base.ModelEntitySpec &&
                       new FindSpecification<OrganizationUnit>(
                           ou =>
                           ou.OrdersBySource.Any(
                               o =>
                               o.IsActive && !o.IsDeleted && allowedStates.Contains(o.WorkflowStepId) && !restrictedTypes.Contains(o.OrderType) &&
                               o.Locks.Any(l => l.IsActive && !l.IsDeleted && l.PeriodStartDate == _period.Start && l.PeriodEndDate == _period.End)));
            }
        }

        protected override bool TryCreateRequest(OrganizationUnit modelEntity, out CreateAfterSaleServiceActivitiesRequest request)
        {
            request = new CreateAfterSaleServiceActivitiesRequest(modelEntity.Id, _period);
            return true;
        }

        protected override OrdinaryTestResult AssertResponse(CreateAfterSaleServiceActivitiesResponse response)
        {
            return Result.When(response).Then(r => r.CreatedActivities.Should().NotBeEmpty());
        }
    }
}