﻿using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid
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

        protected override EntityViewSet SecureViewsToolbarsInternal(EntityViewSet gridViewSettings,
                                                                     long? parentEntityId,
                                                                     EntityName parentEntityName,
                                                                     string parentEntityState)
        {
            if (parentEntityId == null || parentEntityName != EntityName.LegalPerson)
            {
                return gridViewSettings;
            }

            var parentLegalPerson = _legalPersonRepository.FindLegalPerson(parentEntityId.Value);

            if (!_entityAccessService.HasEntityAccess(EntityAccessTypes.Update,
                                                      EntityName.LegalPerson,
                                                      _userContext.Identity.Code,
                                                      parentLegalPerson.Id,
                                                      parentLegalPerson.OwnerCode,
                                                      null))
            {
                gridViewSettings.DataViews.DisableButtons("MakeMain");
            }

            return gridViewSettings;
        }
    }
}
