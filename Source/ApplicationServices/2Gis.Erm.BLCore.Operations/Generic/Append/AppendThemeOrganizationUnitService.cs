using System;

using DoubleGis.Erm.BLCore.Aggregates.Themes;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Append
{
    public class AppendThemeOrganizationUnitService : IAppendGenericEntityService<OrganizationUnit, Theme>
    {
        private readonly IThemeRepository _themeRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public AppendThemeOrganizationUnitService(IThemeRepository themeRepository, IOperationScopeFactory scopeFactory)
        {
            _themeRepository = themeRepository;
            _scopeFactory = scopeFactory;
        }

        public void Append(AppendParams appendParams)
        {
            if (appendParams.ParentId == null || appendParams.AppendedId == null)
            {
                throw new ArgumentException(BLResources.UserIdOtTerritoryIdIsNotSpecified);
            }

            if (appendParams.ParentType != EntityName.Theme || appendParams.AppendedType != EntityName.OrganizationUnit)
            {
                throw new ArgumentException(BLResources.InvalidAppendThemeOrganizationUnitParamTypes);
            }

            var themeId = appendParams.ParentId.Value;
            var organizationUnitId = appendParams.AppendedId.Value;
            var theme = _themeRepository.FindTheme(themeId);
            if (theme.IsDefault)
            {
                throw new ArgumentException(BLResources.CannotAttachOrganizationUnitToDeafaultTheme);
            }

            var isLimitReached = _themeRepository.IsThemeLimitReachedInOrganizationUnit(theme, organizationUnitId);
            if (isLimitReached)
            {
                var message = string.Format(BLResources.CannotApppendMoreThemesToOrganizationUnit,
                                            _themeRepository.GetOrganizationUnitName(organizationUnitId));
                throw new ArgumentException(message);
            }

            var alreadyAppended = _themeRepository.IsThemeAppendedToOrganizationUnit(themeId, organizationUnitId);
            if (alreadyAppended)
            {
                return;
            }

            // FIXME {all, 05.08.2013}: реализация append, а декларируется операция createorupdate - исправить
            // DONE {i.maslennikov, 2013-08-07}: Исправил в рамках разбиения на атомарные операции.
            using (var scope = _scopeFactory.CreateSpecificFor<AppendIdentity, Theme, OrganizationUnit>())
            {
                _themeRepository.AppendThemeToOrganizationUnit(themeId, organizationUnitId);
                scope.Updated<Theme>(themeId)
                     .Updated<OrganizationUnit>(organizationUnitId);
                scope.Complete();
            }
            
        }
    }
}