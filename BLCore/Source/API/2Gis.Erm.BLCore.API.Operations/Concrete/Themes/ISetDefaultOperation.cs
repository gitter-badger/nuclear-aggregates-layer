using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Themes;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Themes
{
    // FIXME {all, 26.11.2013}: название операции не соответсвует контракту - фактически это change default status, либо две отдельные операции setasdefault и cleardefaultstatus
    public interface ISetAsDefaultThemeOperationService : IOperation<SetAsDefaultThemeIdentity>
    {
        void SetAsDefault(long entityId, bool isDefault);
    }
}
