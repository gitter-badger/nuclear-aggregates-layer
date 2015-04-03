using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Grid
{
    public class AccountDetailGridViewService : GenericEntityGridViewService<AccountDetail>
    {
        private readonly IPublicService _publicService;

        public AccountDetailGridViewService(
            IUIConfigurationService configurationService,
            ISecurityServiceEntityAccessInternal entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IPublicService publicService,
            IUserContext userContext)
        : base(
            configurationService,
            entityAccessService,
            functionalAccessService,
            userContext)
        {
            _publicService = publicService;
        }

        protected override EntityViewSet SecureViewsToolbarsInternal(EntityViewSet gridViewSettings, long? parentEntityId, IEntityType parentEntityName, string parentEntityState)
        {
            if (!parentEntityName.Equals(EntityType.Instance.Account()))
            {
                return gridViewSettings;
            }

            foreach (var dataViews in gridViewSettings.DataViews)
            {
                foreach (var toolbarItem in dataViews.ToolbarItems)
                {
                    if (toolbarItem.Disabled || toolbarItem.SecurityPrivelege == null)
                    {
                        continue;
                    }
                    if (toolbarItem.SecurityPrivelege.Value != (int)FunctionalPrivilegeName.CreateAccountDetails)
                    {
                        continue;
                    }
                    if (!parentEntityId.HasValue)
                    {
                        continue;
                    }

                    // disable button by custom code
                    var response = (ValidateCreateAccountDetailResponse)
                                   _publicService.Handle(new ValidateCreateAccountDetailRequest { AccountId = parentEntityId.Value });
                    if (!response.Validated)
                    {
                        toolbarItem.Disabled = true;
                    }
                }
            }

            return gridViewSettings;
        }
    }
}