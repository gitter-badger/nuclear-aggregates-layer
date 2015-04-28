using System;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using NuClear.Security.API.UserContext;
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
        private readonly IDealReadModel _dealReadModel;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        protected GetLegalPersonDtoServiceBase(
            IUserContext userContext, 
            IClientReadModel clientReadModel, 
            ILegalPersonReadModel legalPersonReadModel, 
            IDealReadModel dealReadModel)
            : base(userContext)
        {
            _clientReadModel = clientReadModel;
            _legalPersonReadModel = legalPersonReadModel;
            _dealReadModel = dealReadModel;
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
            if (TryGetClientId(parentEntityId, parentEntityName, extendedInfo, out clientId))
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

        private bool TryGetClientId(long? parentEntityId, EntityName parentEntityName, string extendedInfo, out long clientId)
        {
            if (parentEntityName == EntityName.Client && parentEntityId > 0)
            {
                clientId = parentEntityId.Value;
                return true;
            }

            if (parentEntityName == EntityName.Deal && parentEntityId > 0)
            {
                var deal = _dealReadModel.GetDeal(parentEntityId.Value);
                clientId = deal.ClientId;
                return true;
            }

            if (!string.IsNullOrEmpty(extendedInfo))
            {
                return long.TryParse(Regex.Match(extendedInfo, @"ClientId=(\d+)").Groups[1].Value, out clientId);
            }

            clientId = 0;
            return false;
        }
    }
}
