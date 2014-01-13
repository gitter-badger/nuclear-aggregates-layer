using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Xml;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using SaveOptions = System.Xml.Linq.SaveOptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public abstract class SerializeObjectsHandler<TEntity> : RequestHandler<SerializeObjectsRequest<TEntity>, SerializeObjectsResponse>
        where TEntity : class, IEntity, IEntityKey
    {
        private readonly IExportRepository<TEntity> _exportRepository;
        private readonly ICommonLog _logger;

        protected SerializeObjectsHandler(IExportRepository<TEntity> exportRepository, ICommonLog logger)
        {
            _logger = logger;
            _exportRepository = exportRepository;
        }

        protected abstract XElement SerializeDtoToXElement(IExportableEntityDto entityDto);

        protected abstract ISelectSpecification<TEntity, IExportableEntityDto> CreateDtoExpression();

        protected override SerializeObjectsResponse Handle(SerializeObjectsRequest<TEntity> request)
        {
            var schema = XmlValidator.CreateXmlSchemaSetForXsd(BLCore.Operations.Properties.Resources.ResourceManager.GetString(request.SchemaName));
            var builder = request.IsRecovery
                                  ? _exportRepository.GetBuilderForFailedObjects(request.FailedEntities)
                                  : _exportRepository.GetBuilderForOperations(request.Operations);
            var objectsDtos = _exportRepository.GetEntityDtos(builder, CreateDtoExpression());

            var processedObjectsDtos = ProcessDtosAfterMaterialization(objectsDtos).ToArray();

            var unsuccessfulExportObjects = new List<IExportableEntityDto>();

            var serializedObjects = processedObjectsDtos
                .Where(x => ValidDtoObject(x, unsuccessfulExportObjects))
                .Select(x => new
                    {
                        ExportableEntityDto = x,
                        SerializedEntityDto = new XDocument(SerializeDtoToXElement(x)),
                    })
                .Where(x => ValidXmlObject(x.ExportableEntityDto, x.SerializedEntityDto, schema, unsuccessfulExportObjects))
                .ToArray();

            if (unsuccessfulExportObjects.Any())
            {
                const string Message = "При подготовке к выгрузке записи типа {0} с идентификаторами {1} не прошли проверку";
                var invalidIds = string.Join(", ", unsuccessfulExportObjects.Select(dto => dto.Id));
                _logger.WarnFormatEx(Message, typeof(TEntity).Name, invalidIds);
            }

            _logger.InfoFormatEx("Подготовлено к экспорту {0} из {1} найденных записей.", serializedObjects.Length, processedObjectsDtos.Length);

            return new SerializeObjectsResponse
                {
                    SerializedObjects = serializedObjects.Select(x => x.SerializedEntityDto.ToString(SaveOptions.None)).ToArray(),
                    SuccessObjects = serializedObjects.Select(x => x.ExportableEntityDto).ToArray(), 
                    FailedObjects = unsuccessfulExportObjects
                };
        }

        /// <summary>
        /// Inmemory проверка является ли экземпляр сущности валидным или нет
        /// </summary>
        protected virtual string GetError(IExportableEntityDto entityDto)
        {
            return null;
        }

        /// <summary>
        /// Пост обработка всего массива экземпляров сущностей in memory.
        /// Необходима для выполнения каких-то custom обработок DTO,
        /// которые невозможно использовать в expression используемом при выборке из хранилища в силу ограичений Linq To Entities.
        /// Как пример - conditional operator (?:) для не элементарных типов и т.п.
        /// </summary>
        protected virtual IEnumerable<IExportableEntityDto> ProcessDtosAfterMaterialization(IEnumerable<IExportableEntityDto> entityDtos)
        {
            return entityDtos;
        }

        private bool ValidDtoObject(IExportableEntityDto exportableEntityDto, ICollection<IExportableEntityDto> unsuccessfulExportObjects)
        {
            var error = GetError(exportableEntityDto);
            if (!string.IsNullOrEmpty(error))
            {
                unsuccessfulExportObjects.Add(exportableEntityDto);
                _logger.ErrorFormatEx("Ошибка при экспорте в шину интеграции экземпляра сущности {0} [Id={1}]: {2}", typeof(TEntity).Name, exportableEntityDto.Id, error);
                return false;
            }

            return true;
        }

        private bool ValidXmlObject(IExportableEntityDto exportableEntityDto, XDocument document, XmlSchemaSet schema, ICollection<IExportableEntityDto> unsuccessfulExportObjects)
        {
            // validate order xml
            string error;
            var isValidXml = XmlValidator.Validate(document, schema, out error);
            if (!isValidXml)
            {
                unsuccessfulExportObjects.Add(exportableEntityDto);
                _logger.ErrorFormatEx("Ошибка при экспорте в шину интеграции экземпляра сущности {0} [Id={1}]: {2}", typeof(TEntity).Name, exportableEntityDto.Id, error);
            }

            return isValidXml;
        }
    }
}