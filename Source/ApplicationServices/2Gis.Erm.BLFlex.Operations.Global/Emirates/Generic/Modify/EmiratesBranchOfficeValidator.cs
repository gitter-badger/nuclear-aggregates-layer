using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify
{
    public sealed class EmiratesBranchOfficeValidator : IPartableEntityValidator<BranchOffice>
    {
        private readonly ICheckInnService _checkCommercialLicenseService;
        private readonly IBargainTypeReadModel _bargainTypeReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public EmiratesBranchOfficeValidator(IBargainTypeReadModel bargainTypeReadModel,
                                             ICheckInnService checkCommercialLicenseService,
                                             IBranchOfficeReadModel branchOfficeReadModel)
        {
            _bargainTypeReadModel = bargainTypeReadModel;
            _checkCommercialLicenseService = checkCommercialLicenseService;
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        // TODO {y.baranihin, 17.06.2014}: Правильно ли то, что ключи в ресурсниках начинаются с префикса Emirates? 
        //                                 Вопрос локализации решается же несколькими ресурсниками, а не отдельными группами записей в этих ресурсниках. Так?
        public void Check(BranchOffice entity)
        {
            var commercialLicense = entity.Inn;

            if (_bargainTypeReadModel.GetVatRate(entity.BargainTypeId.Value) != decimal.Zero)
            {
                throw new NotificationException(Resources.Server.Properties.BLResources.EmiratesYouHaveToSpecifyBargainTypeWithoutVat);
            }

            string commercialLicenseError;
            if (_checkCommercialLicenseService.TryGetErrorMessage(commercialLicense, out commercialLicenseError))
            {
                throw new NotificationException(commercialLicenseError);
            }

            if (_branchOfficeReadModel.AreThereAnyActiveInnDuplicates(entity.Id, commercialLicense))
            {
                throw new NotificationException(
                    Resources.Server.Properties.BLResources.EmiratesBranchOfficeWithSpecifiedCommercialLicenseAlreadyExists);
            }
        }
    }
}