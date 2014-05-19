using DoubleGis.Erm.BLCore.API.Aggregates.Themes;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Themes;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Themes;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Themes
{
    // FIXME {all, 26.11.2013}: название операции не соответсвует контракту - фактически это change default status, либо две отдельные операции setasdefault и cleardefaultstatus
    public sealed class SetAsDefaultThemeOperationService : ISetAsDefaultThemeOperationService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IThemeRepository _themeRepository;

        public SetAsDefaultThemeOperationService(
            IThemeRepository themeRepository, 
            IOperationScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _themeRepository = themeRepository;
        }

        public void SetAsDefault(long entityId, bool isDefault)
        {
            using (var operationScope = _scopeFactory.CreateNonCoupled<SetAsDefaultThemeIdentity>())
            {
                var theme = _themeRepository.FindTheme(entityId);
                if (theme != null && theme.IsDefault != isDefault)
                {
                    theme.IsDefault = isDefault;
                    _themeRepository.CreateOrUpdate(theme);

                    operationScope
                        .Updated<Theme>(theme.Id)
                        .Complete();
                }
            }
        }
    }
}
