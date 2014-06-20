using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get
{
    // TODO {v.lapeev, 21.05.2014}: Какую проблему решаем выделением этого уровня иерархии наследования? Перевешивают ли профиты те проблемы, которые мы получаем в результате?
    //                              Проблемы:
    //                               - смещение фокуса сервиса бизнес-операции от сущности к DTO
    //                               - отсутствие строгой типизации для DTO ведет к использованию строк-названий свойств (HasProfiles, ClientRef)
    //                               - виртуальные методы, предназначенные для одного наследника (SetSpecificPropertyValues)
    //                               - возможность в наследниках GetLegalPersonDtoServiceBase переопределить методы GetDto и CreateDto -> счастливой отладки
    //                               - тиражирование этого подхода (+ учет локальных особенностей по месту) ведет к тем же проблемам в других частях системы -> ухудшение дизайна на ровном месте
    //                              Предлагаю найти другой способ получения желаемых профитов
    // COMMENT {d.ivanov, 21.05.2014}: Решается проблема дублирования логики получения юрлиц. Выделение общего для всех адаптаций аспекта позволяет при его изменении не перетестировать все адаптации.
    //                              Использование строк-названий навязано ограничением "DomainEntityDto должен реализовывать только интерфейс IDomainEntityDto". 
    //                              Строковые названия исторически используются в базовой реализации GetDomainEntityDtoServiceBase. 
    //                              От них можно было бы избавиться, если выделить интерфейс или разрешить иерархию для Dto, но...
    // FIXME {d.ivanov, 17.06.2014}: Убрать этот базовый класс
    [Obsolete]
    public abstract class GetLegalPersonDtoServiceBase<TDto> : GetDomainEntityDtoServiceBase<LegalPerson>
        where TDto : IDomainEntityDto<LegalPerson>
    {
        private readonly IClientReadModel _clientReadModel;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        protected GetLegalPersonDtoServiceBase(IUserContext userContext, IClientReadModel clientReadModel, ILegalPersonReadModel legalPersonReadModel)
            : base(userContext)
        {
            _clientReadModel = clientReadModel;
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override IDomainEntityDto<LegalPerson> GetDto(long entityId)
        {
            var legalPerson = _legalPersonReadModel.GetLegalPerson(entityId);

            var dto = ProjectToDto(legalPerson);

            dto.SetPropertyValue("HasProfiles", _legalPersonReadModel.HasAnyLegalPersonProfiles(entityId));
            SetSpecificPropertyValues(dto);

            return dto;
        }

        protected override IDomainEntityDto<LegalPerson> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var legalPerson = new LegalPerson();

            long clientId;
            if (GetDtoServiceHelper.TryGetClientId(parentEntityId, parentEntityName, extendedInfo, out clientId))
            {
                legalPerson.ClientId = clientId;
            }

            return ProjectToDto(legalPerson);
        }

        protected abstract IProjectSpecification<LegalPerson, TDto> GetProjectSpecification();

        protected virtual void SetSpecificPropertyValues(TDto dto)
        {
            // do nothing
        }

        private TDto ProjectToDto(LegalPerson legalPerson)
        {
            var dto = GetProjectSpecification().Project(legalPerson);

            var clientRef = dto.GetPropertyValue<TDto, EntityReference>("ClientRef");
            if (clientRef.Id.HasValue)
            {
                clientRef.Name = _clientReadModel.GetClientName(clientRef.Id.Value);
            }

            return dto;
        }
    }
}
