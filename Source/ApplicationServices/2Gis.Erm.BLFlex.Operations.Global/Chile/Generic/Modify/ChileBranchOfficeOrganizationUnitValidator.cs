using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify
{
    public sealed class ChileBranchOfficeOrganizationUnitValidator : IPartableEntityValidator<BranchOfficeOrganizationUnit>
    {
        private readonly ICheckInnService _checkRutService;

        public ChileBranchOfficeOrganizationUnitValidator(ICheckInnService checkRutService)
        {
            _checkRutService = checkRutService;
        }

        public void Check(BranchOfficeOrganizationUnit entity)
        {
            var rut = entity.ChilePart().RepresentativeRut;

            string rutError;
            if (_checkRutService.TryGetErrorMessage(rut, out rutError))
            {
                throw new NotificationException(rutError);
            }
        }
    }
}
