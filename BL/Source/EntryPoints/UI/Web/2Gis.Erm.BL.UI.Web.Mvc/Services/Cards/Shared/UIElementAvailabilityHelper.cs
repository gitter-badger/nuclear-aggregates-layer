using System;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions;

using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    public sealed class UIElementAvailabilityHelper
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        public UIElementAvailabilityHelper(ISecurityServiceFunctionalAccess functionalAccessService, ISecurityServiceEntityAccess entityAccessService, IUserContext userContext)
        {
            _functionalAccessService = functionalAccessService;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
        }

        public bool IsUIElementDisabled(UIElementMetadata element, IEntityViewModelBase model)
        {
            if (element.Uses<LockOnNewCardFeature>() && model.IsNew)
            {
                return true;
            }

            if (element.Uses<LockOnInactiveCardFeature>() && model.ViewConfig.ReadOnly)
            {
                return true;
            }

            if (
                element.Features<SecuredByFunctionalPrivelegeFeature>()
                       .Any(feature => !_functionalAccessService.HasFunctionalPrivilegeGranted(feature.Privilege, _userContext.Identity.Code)))
            {
                return true;
            }

            var modelOwnerCode = model.Owner != null && model.Owner.Key.HasValue
                                     ? model.Owner.Key.Value
                                     : _userContext.Identity.Code;

            foreach (var feature in element.Features<SecuredByEntityPrivelegeFeature>())
            {
                if (feature.Entity == model.ViewConfig.EntityName)
                {
                    if (!_entityAccessService.HasEntityAccess(feature.Privilege, feature.Entity, _userContext.Identity.Code, model.Id, modelOwnerCode, null))
                    {
                        return true;
                    }
                }
                else
                {
                    if (!_entityAccessService.HasEntityAccess(feature.Privilege, feature.Entity, _userContext.Identity.Code, null, _userContext.Identity.Code, null))
                    {
                        return true;
                    }
                }
            }

            foreach (var feature in element.Features<IDisableExpressionFeature>())
            {
                bool expressionResult;

                if (!feature.Expression.TryExecuteAspectLambda((IAspect)model, out expressionResult))
                {
                    throw new InvalidOperationException(string.Format("Unable to execute disable expression for {0} element with {1} viewmodel", element.Identity, model.GetType()));
                }

                if (expressionResult)
                {
                    return true;
                }
            }

            foreach (var feature in element.Features<DisableExpressionsFeature>())
            {
                bool expressionResult;

                if (!feature.Expressions.TryExecuteAspectBoolLambdas((IAspect)model, feature.LogicalOperation, out expressionResult))
                {
                    throw new InvalidOperationException(string.Format("Unable to execute disable expressions for {0} element with {1} viewmodel", element.Identity, model.GetType()));
                }

                if (expressionResult)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsUIElementInvisible<TAspect>(UIElementMetadata element, TAspect aspect)
            where TAspect : IAspect
        {
            foreach (var feature in element.Features<IHideExpressionFeature>())
            {
                bool expressionResult;
                if (!feature.Expression.TryExecuteAspectLambda(aspect, out expressionResult))
                {
                    throw new InvalidOperationException(string.Format("Unable to execute disable expression for {0} element with {1} viewmodel", element.Identity, aspect.GetType()));
                }

                if (expressionResult)
                {
                    return true;
                }
            }

            foreach (var feature in element.Features<HideExpressionsFeature>())
            {
                bool expressionResult;

                if (!feature.Expressions.TryExecuteAspectBoolLambdas(aspect, feature.LogicalOperation, out expressionResult))
                {
                    throw new InvalidOperationException(string.Format("Unable to execute disable expressions for {0} element with {1} viewmodel", element.Identity, aspect.GetType()));
                }

                if (expressionResult)
                {
                    return true;
                }
            }

            return false;
        }
    }
}