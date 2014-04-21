using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.DTO;
using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;


namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public sealed class ChileBankObtainer : ISimplifiedModelEntityObtainer<Bank>, IChileAdapted
    {
        private readonly IFinder _finder;
        private readonly IDictionaryEntityPropertiesConverter<Bank> _dynamicEntityEntityPropertiesConverter;

        public ChileBankObtainer(
            IDictionaryEntityPropertiesConverter<Bank> dynamicEntityEntityPropertiesConverter,
            IFinder finder)
        {
            _dynamicEntityEntityPropertiesConverter = dynamicEntityEntityPropertiesConverter;
            _finder = finder;
        }

        public Bank ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (BankDomainEntityDto)domainEntityDto;
            var bank = dto.IsNew()
                           ? new Bank { IsActive = true, IsDeleted = false }
                           : SingleOrDefault(_finder, dto.Id, _dynamicEntityEntityPropertiesConverter.ConvertFromDynamicEntityInstance);
            
            CopyFields(dto, bank);
            
            return bank;
        }

        private void CopyFields(BankDomainEntityDto source, Bank target)
        {
            target.Name = source.Name;
        }

        // FIXME {all, 20.02.2014}: Ждём реализации этого метода в EavExtensions
        [Obsolete("Ждём реализации этого метода в EavExtensions")]
        private static TDictionaryEntity SingleOrDefault<TDictionaryEntity>(IFinder finder,
                                                                           long dictionaryEntityId,
                                                                           Func<DictionaryEntityInstance, ICollection<DictionaryEntityPropertyInstance>, TDictionaryEntity> propertiesConverter)
            where TDictionaryEntity : class, IEntity
        {
            var instanceDtos = finder.Find<DictionaryEntityInstance, DictionaryEntityInstanceDto>(
                DictionaryEntitySpecs.DictionaryEntity.Select.DictionaryEntityInstanceDto(),
                Specs.Find.ById<DictionaryEntityInstance>(dictionaryEntityId));

            var instanceDto = instanceDtos.SingleOrDefault();
            if (instanceDto == null)
            {
                return null;
            }

            if (instanceDto.DictionaryEntityInstance.EntityId.HasValue)
            {
                throw new InvalidDataException("Dictionary entity has incorrect storage representation. Main entity reference must be null");
            }

            return propertiesConverter(instanceDto.DictionaryEntityInstance, instanceDto.DictionaryEntityPropertyInstances);
        }
    }
}
