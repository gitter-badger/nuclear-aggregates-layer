using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Modify
{
    public class ModifyLegalPersonService : IModifyBusinessModelEntityService<LegalPerson>, IKazakhstanAdapted
    {
        private readonly IPublicService _publicService;
        private readonly ILegalPersonReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<LegalPerson> _obtainer;
        private readonly ICreateAggregateRepository<LegalPerson> _createRepository;
        private readonly IUpdateAggregateRepository<LegalPerson> _updateRepository;
        private readonly ICheckInnService _checkBinService;

        public ModifyLegalPersonService(IPublicService publicService,
                                        ILegalPersonReadModel readModel,
                                        IBusinessModelEntityObtainer<LegalPerson> obtainer,
                                        ICreateAggregateRepository<LegalPerson> createRepository,
                                        IUpdateAggregateRepository<LegalPerson> updateRepository,
                                        ICheckInnService checkBinService)
        {
            _publicService = publicService;
            _readModel = readModel;
            _obtainer = obtainer;
            _createRepository = createRepository;
            _updateRepository = updateRepository;
            _checkBinService = checkBinService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var entity = _obtainer.ObtainBusinessModelEntity(domainEntityDto);

            if (!entity.IsNew())
            {
                var hasProfiles = _readModel.HasAnyLegalPersonProfiles(entity.Id);
                if (!hasProfiles)
                {
                    throw new NotificationException(BLResources.MustMakeLegalPersonProfile);
                }
            }

            string message;
            if (_checkBinService.TryGetErrorMessage(entity.Inn, out message))
            {
                throw new NotificationException(string.Format(message, ResolveInnName((LegalPersonType)entity.LegalPersonTypeEnum)));
            }

            if ((entity.LegalPersonTypeEnum == LegalPersonType.LegalPerson ||
                 entity.LegalPersonTypeEnum == LegalPersonType.Businessman) &&
                string.IsNullOrEmpty(entity.LegalAddress))
            {
                throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.LegalAddress));
            }

            if (entity.LegalPersonTypeEnum == LegalPersonType.NaturalPerson)
            {
                ValidateIdentityCardNumber(entity.Within<KazakhstanLegalPersonPart>().GetPropertyValue(x => x.IdentityCardNumber));
            }

            if (entity.IsNew())
            {
                _publicService.Handle(new ValidatePaymentRequisitesIsUniqueRequest { Entity = entity });
            }

            try
            {
                if (entity.IsNew())
                {
                    _createRepository.Create(entity);
                }
                else
                {
                    _updateRepository.Update(entity);
                }
            }
            catch (Exception ex)
            {
                throw new NotificationException(BLResources.ErrorWhileSavingLegalPerson, ex);
            }

            return entity.Id;
        }

        private static void ValidateIdentityCardNumber(string identityCardNumber)
        {
            if (string.IsNullOrEmpty(identityCardNumber))
            {
                throw new NotificationException(string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.IdentityCardNumber));
            }

            if (identityCardNumber.Length != 9)
            {
                throw new NotificationException(string.Format(BLResources.StringLengthLocalizedAttribute_ValidationErrorEqualsLimitations, MetadataResources.IdentityCardNumber, 9));
            }
        }

        private static string ResolveInnName(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return MetadataResources.Bin;
                case LegalPersonType.Businessman:
                    return MetadataResources.BinIin;
                case LegalPersonType.NaturalPerson:
                    return MetadataResources.Iin;
                default:
                    throw new ArgumentOutOfRangeException("legalPersonType");
            }
        }
    }
}