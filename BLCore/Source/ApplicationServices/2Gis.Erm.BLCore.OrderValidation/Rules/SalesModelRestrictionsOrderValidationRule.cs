using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.Exceptions;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class SalesModelRestrictionsOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private const int CategoryLevelToCheck = 3;
        private readonly IFinder _finder;

        public SalesModelRestrictionsOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var organizationUnitSelectSpec =
                new SelectSpecification<OrganizationUnit, OrganizationUnitDto>(
                    x => new OrganizationUnitDto
                             {
                                 Id = x.Id,
                                 Name = x.Name
                             });

            var destOrganizationUnitFindSpec = ruleContext.ValidationParams.IsMassValidation
                                                   ? Specs.Find.ById<OrganizationUnit>(ruleContext.ValidationParams.Mass.OrganizationUnitId)
                                                   : OrganizationUnitSpecs.Find.DestOrganizationUnitByOrder(ruleContext.ValidationParams.Single.OrderId) &&
                                                     Specs.Find.ActiveAndNotDeleted<OrganizationUnit>();

            var destOrganizationUnit = _finder.Find(organizationUnitSelectSpec, destOrganizationUnitFindSpec).SingleOrDefault();
            if (destOrganizationUnit == null)
            {
                throw new EntityNotFoundException(typeof(OrganizationUnit));
            }

            var projectInfo = _finder.Find(ProjectSpecs.Find.ByOrganizationUnit(destOrganizationUnit.Id) && Specs.Find.Active<Project>())
                                     .Select(x => new
                                                      {
                                                          x.Id,
                                                          Name = x.DisplayName
                                                      })
                                     .SingleOrDefault();

            if (projectInfo == null)
            {
                throw new OrganizationUnitHasNoProjectsException(string.Format(BLResources.DestOrganizationUnitHasNoProject, destOrganizationUnit.Name));
            }

            var badAdvertisemements =
                _finder.Find(ruleContext.OrdersFilterPredicate)
                       .SelectMany(order => order.OrderPositions)
                       .Where(orderPosition => orderPosition.IsActive
                                               && !orderPosition.IsDeleted)
                       .SelectMany(orderPosition =>
                                   orderPosition.OrderPositionAdvertisements.Where(opa =>
                                                                                   opa.CategoryId.HasValue
                                                                                   && opa.Category.Level == CategoryLevelToCheck
                                                                                   && !opa.Category.SalesModelRestrictions.Any(sr =>
                                                                                                                               sr.SalesModel == opa.Position.SalesModel &&
                                                                                                                               sr.ProjectId == projectInfo.Id))
                                                .Select(advertisement => new
                                                                             {
                                                                                 OrderPositionId = orderPosition.Id,
                                                                                 OrderPositionName = advertisement.Position.Name,
                                                                                 OrderId = orderPosition.Order.Id,
                                                                                 OrderNumber = orderPosition.Order.Number,
                                                                                 CategoryId = advertisement.CategoryId.Value,
                                                                                 CategoryName = advertisement.Category.Name,
                                                                             }))
                       .ToArray();

            return badAdvertisemements.Select(x => new OrderValidationMessage
                                                       {
                                                           Type = MessageType.Error,
                                                           OrderId = x.OrderId,
                                                           OrderNumber = x.OrderNumber,
                                                           MessageText = string.Format(BLResources.CategoryIsRestrictedForSpecifiedSalesModelError,
                                                                                       GenerateDescription(ruleContext.ValidationParams.IsMassValidation,
                                                                                                           EntityName.OrderPosition,
                                                                                                           x.OrderPositionName,
                                                                                                           x.OrderPositionId),
                                                                                       GenerateDescription(ruleContext.ValidationParams.IsMassValidation,
                                                                                                           EntityName.Category,
                                                                                                           x.CategoryName,
                                                                                                           x.CategoryId),
                                                                                       projectInfo.Name)
                                                       });
        }

        private sealed class OrganizationUnitDto
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}