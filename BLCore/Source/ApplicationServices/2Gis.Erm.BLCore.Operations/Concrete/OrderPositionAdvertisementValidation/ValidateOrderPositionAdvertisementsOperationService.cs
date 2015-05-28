using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation;
using DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositionAdvertisementValidation.Rules;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositionAdvertisementValidation
{
    // FIXME {all, 01.12.2014}: в operation service используется finder, те же вопросы к AdverisementValidationRule - скорее всего их resolve вынести в factory, а ещё лучше подтягивать через сonstructor injection
    public class ValidateOrderPositionAdvertisementsOperationService : IValidateOrderPositionAdvertisementsOperationService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IFinder _finder;

        public ValidateOrderPositionAdvertisementsOperationService(IOperationScopeFactory operationScopeFactory, IFinder finder)
        {
            _operationScopeFactory = operationScopeFactory;
            _finder = finder;
        }

        public IReadOnlyCollection<OrderPositionAdvertisementValidationError> Validate(long orderPositionId, IEnumerable<AdvertisementDescriptor> advertisements)
        {
            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ValidateOrderPositionAdvertisementsIdentity>())
            {
                string error;
                if (!IsValidForRateType(orderPositionId, advertisements, out error))
                {
                    return new[]
                        {
                            new OrderPositionAdvertisementValidationError
                                {
                                    Advertisement = null,
                                    ErrorMessage = error,
                                    Rule = OrderPositionAdvertisementValidationRule.None
                                }
                        };
                }

                var rules = GetValidationRules();
                var validationErrors = new List<OrderPositionAdvertisementValidationError>();

                // TODO {all, 16.12.2013}: Тут можно бы распараллелить. 
                foreach (var advertisement in advertisements)
                {
                    foreach (var rule in rules)
                    {
                        rule.Validate(advertisement, validationErrors);
                    }
                }

                operationScope.Complete();
                return validationErrors;
            }
        }

        private bool IsValidForRateType(long orderPositionId, IEnumerable<AdvertisementDescriptor> advertisements, out string error)
        {
            error = null;
            var boundCategoryChildPositionIds =
                _finder.Find(Specs.Find.ById<OrderPosition>(orderPositionId) &&
                             Specs.Find.Custom<OrderPosition>(
                                                              x => x.PricePosition.RateType == PricePositionRateType.BoundCategory && x.PricePosition.Position.IsComposite))
                       .Map(q => q.SelectMany(
                                              x =>
                                              x.PricePosition.Position.ChildPositions.Where(cp => cp.IsActive && !cp.IsDeleted)
                                               .Select(cp => cp.ChildPosition)
                                               .Where(cpp => cpp.IsActive && !cpp.IsDeleted)
                                               .Select(cpp => cpp.Id)))
                       .Many();

            if (!boundCategoryChildPositionIds.Any())
            {
                return true;
            }

            var adsValidForBoundCategoryRateType = boundCategoryChildPositionIds.All(id => advertisements.Any(adv => adv.PositionId == id));
            if (adsValidForBoundCategoryRateType)
            {
                return true;
            }

            error = string.Format(BLResources.CategoryShouldBeSpecifiedForAllSubpositions,
                                  PricePositionRateType.BoundCategory.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));

            return false;
        }

        private IAdvertisementValidationRule[] GetValidationRules()
        {
            return new IAdvertisementValidationRule[]
                {
                    new RequiredAdvertisementOrderPositionAdvertisementValidationRule(_finder),
                    new IsTemplatePublishedOrderPositionAdvertisementValidationRule(_finder),
                    new CorrectAddressOrderPositionAdvertisementValidationRule(_finder),
                    new AdvertisementTemplateMatchesPositionTemplateOrderPositionAdvertisementValidationRule(_finder)
                };
        }
    }
}
