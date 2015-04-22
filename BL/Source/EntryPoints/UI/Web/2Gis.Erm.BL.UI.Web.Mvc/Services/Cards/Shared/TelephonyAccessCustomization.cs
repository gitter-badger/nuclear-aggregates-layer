using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    public sealed class TelephonyAccessCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;        

        public TelephonyAccessCustomization(IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            this._userContext = userContext;
            this._functionalAccessService = functionalAccessService;            
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var privilege = this._functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.TelephonyAccess, this._userContext.Identity.Code);
            viewModel.SetPropertyValue("HaveTelephonyAccess", privilege);
        }
    }
}
