using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.BargainTypes.ReadModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.BranchOfficesAggregate.ReadModel;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify
{
    public sealed class UkraineBranchOfficeValidator : IPartableEntityValidator<BranchOffice>
    {
        private readonly ICheckInnService _checkIpnService;
        private readonly IBargainTypeReadModel _bargainTypeReadModel;
        private readonly IUkraineBranchOfficeReadModel _ukraineBranchOfficeReadModel;

        private const int BusinessmanEgrpouLength = 10;
        private const int LegalPersonEgrpouLength = 8;

        public UkraineBranchOfficeValidator(ICheckInnService checkIpnService,
                                            IBargainTypeReadModel bargainTypeReadModel,
                                            IUkraineBranchOfficeReadModel ukraineBranchOfficeReadModel)
        {
            _checkIpnService = checkIpnService;
            _bargainTypeReadModel = bargainTypeReadModel;
            _ukraineBranchOfficeReadModel = ukraineBranchOfficeReadModel;
        }

        public void Check(BranchOffice entity)
        {
            var ipn = entity.Within<UkraineBranchOfficePart>().GetPropertyValue(part => part.Ipn);

            if (_bargainTypeReadModel.GetVatRate(entity.BargainTypeId.Value) > 0 && string.IsNullOrWhiteSpace(ipn))
            {
                throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.Ipn));
            }

            string ipnError;
            if (!string.IsNullOrWhiteSpace(ipn) && _checkIpnService.TryGetErrorMessage(ipn, out ipnError))
            {
                throw new NotificationException(ipnError);
            }

            if (entity.Inn.Length != BusinessmanEgrpouLength && entity.Inn.Length != LegalPersonEgrpouLength)
            {
                throw new NotificationException(Resources.Server.Properties.BLResources.UkraineEnteredEgrpouIsNotCorrect);
            }

            if (_ukraineBranchOfficeReadModel.AreThereAnyActiveIpnDuplicates(entity.Id, ipn))
            {
                throw new NotificationException(Resources.Server.Properties.BLResources.UkraineBranchOfficeWithSpecifiedIpnAlreadyExists);
            }

            var egrpou = entity.Inn;
            if (_ukraineBranchOfficeReadModel.AreThereAnyActiveEgrpouDuplicates(entity.Id, egrpou))
            {
                throw new NotificationException(Resources.Server.Properties.BLResources.UkraineBranchOfficeWithSpecifiedEgrpouAlreadyExists);
            }
        }
    }
}