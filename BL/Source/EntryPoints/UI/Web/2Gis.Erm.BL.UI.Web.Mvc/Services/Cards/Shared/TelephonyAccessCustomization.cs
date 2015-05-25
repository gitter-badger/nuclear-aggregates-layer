using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Aspects;

using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    public sealed class TelephonyAccessCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;        

        public TelephonyAccessCustomization(IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _userContext = userContext;
            _functionalAccessService = functionalAccessService;            
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var privilege = _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.TelephonyAccess, _userContext.Identity.Code);
            var haveTelephonyAspet = (IHaveTelephonyAccessAspect)viewModel;
            if (haveTelephonyAspet != null)
            {
                haveTelephonyAspet.HaveTelephonyAccess = privilege;
            }
        }
    }
}
