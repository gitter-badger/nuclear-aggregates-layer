using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify
{
    public sealed class ChileBranchOfficeValidator : IPartableEntityValidator<BranchOffice>
    {
        private readonly ICheckInnService _checkRutService;

        public ChileBranchOfficeValidator(ICheckInnService checkRutService)
        {
            _checkRutService = checkRutService;
        }

        public void Check(BranchOffice entity)
        {
            var rut = entity.Inn;

            string rutError;
            if (_checkRutService.TryGetErrorMessage(rut, out rutError))
            {
                throw new NotificationException(rutError);
            }
        }
    }
}