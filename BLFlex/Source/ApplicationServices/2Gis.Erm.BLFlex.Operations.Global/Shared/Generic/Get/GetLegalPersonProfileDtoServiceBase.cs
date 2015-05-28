using System;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get
{
    // FIXME {d.ivanov, 17.06.2014}: Убрать этот базовый класс, см. комментарии к GetLegalPersonDtoServiceBase
    [Obsolete]
    public abstract class GetLegalPersonProfileDtoServiceBase<TDto> : GetDomainEntityDtoServiceBase<LegalPersonProfile>
        where TDto : IDomainEntityDto<LegalPersonProfile>
    {
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        protected GetLegalPersonProfileDtoServiceBase(IUserContext userContext, ILegalPersonReadModel legalPersonReadModel)
            : base(userContext)
        {
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override IDomainEntityDto<LegalPersonProfile> GetDto(long entityId)
        {
            var legalPersonProfile = _legalPersonReadModel.GetLegalPersonProfile(entityId);

            var dto = ProjectDto(legalPersonProfile);
            SetSpecificPropertyValues(dto);

            return dto;
        }

        protected override IDomainEntityDto<LegalPersonProfile> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            if (!parentEntityId.HasValue)
            {
                throw new ArgumentNullException("parentEntityId");
            }

            if (!parentEntityName.Equals(EntityType.Instance.LegalPerson()))
            {
                throw new ArgumentException("parentEntityName");
            }

            var legalPersonProfile = new LegalPersonProfile { LegalPersonId = parentEntityId.Value };

            var dto = ProjectDto(legalPersonProfile);

            SetSpecificPropertyValuesForNewDto(dto);

            return dto;
        }

        protected abstract MapSpecification<LegalPersonProfile, TDto> GetProjectSpecification();

        protected virtual void SetSpecificPropertyValues(TDto dto)
        {
            // do nothing
        }

        protected virtual void SetSpecificPropertyValuesForNewDto(TDto dto)
        {
            // do nothing
        }

        private TDto ProjectDto(LegalPersonProfile legalPersonProfile)
        {
            var dto = GetProjectSpecification().Map(legalPersonProfile);

            var legalPersonRef = dto.GetPropertyValue<TDto, EntityReference>("LegalPersonRef");
            if (legalPersonRef.Id.HasValue)
            {
                dto.SetPropertyValue("LegalPersonType", _legalPersonReadModel.GetLegalPersonType(legalPersonRef.Id.Value));
                legalPersonRef.Name = _legalPersonReadModel.GetLegalPersonName(legalPersonRef.Id.Value);
            }

            return dto;
        }
    }
}
