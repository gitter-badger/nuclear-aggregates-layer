using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Grid
{
    public class LegalPersonProfileGridViewService : GenericEntityGridViewService<LegalPersonProfile>
    {
        private readonly ISecurityServiceEntityAccessInternal _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly ILegalPersonRepository _legalPersonRepository;

        public LegalPersonProfileGridViewService(IUIConfigurationService configurationService,
                                                 ISecurityServiceEntityAccessInternal entityAccessService,
                                                 ISecurityServiceFunctionalAccess functionalAccessService,
                                                 IUserContext userContext,
                                                 ILegalPersonRepository legalPersonRepository)
            : base(configurationService, entityAccessService, functionalAccessService, userContext)
        {
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _legalPersonRepository = legalPersonRepository;
        }

        protected override EntityViewSet SecureViewsToolbarsInternal(EntityViewSet gridViewSettings, long? parentEntityId, IEntityType parentEntityName, string parentEntityState)
        {
            if (parentEntityId == null || !parentEntityName.Equals(EntityType.Instance.LegalPerson()))
            {
                return gridViewSettings;
            }

            var parentLegalPerson = _legalPersonRepository.FindLegalPerson(parentEntityId.Value);

            if (!_entityAccessService.HasEntityAccess(EntityAccessTypes.Update,
                                                      EntityType.Instance.LegalPerson(),
                                                      _userContext.Identity.Code,
                                                      parentLegalPerson.Id,
                                                      parentLegalPerson.OwnerCode,
                                                      null))
            {
                gridViewSettings.DataViews.DisableButtons("MakeMain");
            }

            if (!parentLegalPerson.IsActive || parentLegalPerson.IsDeleted)
            {
                gridViewSettings.DataViews.DisableButtons("Create");
            }

            return gridViewSettings;
        }
    }
}
