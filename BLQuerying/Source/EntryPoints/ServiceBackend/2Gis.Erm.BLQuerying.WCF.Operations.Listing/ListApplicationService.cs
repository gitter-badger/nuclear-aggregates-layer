using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.Remote.List;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using NuClear.Model.Common.Entities;
using NuClear.ResourceUtilities;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLQuerying.WCF.Operations.Listing
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    [UseCase(Duration = UseCaseDuration.Long)]
    public class ListApplicationService : IListApplicationService, IListApplicationRestService
    {
        private readonly IUIConfigurationService _configurationService;
        private readonly IUserContext _userContext;
        private readonly ITracer _tracer;
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly IUseCaseTuner _useCaseTuner;

        public ListApplicationService(ITracer tracer,
                                      IOperationServicesManager operationServicesManager,
                                      IUseCaseTuner useCaseTuner,
                                      IUIConfigurationService configurationService,
                                      IUserContext userContext,
                                      IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _operationServicesManager = operationServicesManager;
            _useCaseTuner = useCaseTuner;
            _configurationService = configurationService;
            _userContext = userContext;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public ListResult Execute(IEntityType entityName,
                                  int start,
                                  string filterInput,
                                  string extendedInfo,
                                  string nameLocaleResourceId,
                                  int limit,
                                  string sort,
                                  long? parentId,
                                  IEntityType parentType)
        {
            try
            {
                return ExecuteInternal(entityName, start, filterInput, extendedInfo, nameLocaleResourceId, limit, sort, parentId, parentType);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occurred in {0}. Entity type: {1}", GetType().Name, entityName);
                throw new FaultException<ListOperationErrorDescription>(new ListOperationErrorDescription(entityName, ex.Message));
            }
        }

        public ListResult Execute(string entityNameArg,
                                  int start,
                                  string filterInput,
                                  string extendedInfo,
                                  string nameLocaleResourceId,
                                  int limit,
                                  string sort,
                                  string parentIdArg,
                                  string parentTypeArg)
        {
            IEntityType entityName = EntityType.Instance.None();

            try
            {
                if (!EntityType.Instance.TryParse(entityNameArg, out entityName))
                {
                    throw new ArgumentException("Entity name cannot be parsed");
                }

                IEntityType parentType = EntityType.Instance.None();
                if (!string.IsNullOrEmpty(parentTypeArg) && !EntityType.Instance.TryParse(parentTypeArg, out parentType))
                {
                    throw new ArgumentException("Parent entity type cannot be parsed");
                }

                long? parentId;
                if (string.IsNullOrEmpty(parentIdArg) || string.Equals(parentIdArg, "null", StringComparison.OrdinalIgnoreCase))
                {
                    parentId = null;
                }
                else
                {
                    long parentIdParsed;
                    if (!long.TryParse(parentIdArg, out parentIdParsed))
                    {
                        throw new ArgumentException("Parent Id cannot be parsed");
                    }

                    parentId = parentIdParsed;
                }              

                return ExecuteInternal(entityName, start, filterInput, extendedInfo, nameLocaleResourceId, limit, sort, parentId, parentType);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}. Entity type: {1}", GetType().Name, entityName);
                throw new WebFaultException<ListOperationErrorDescription>(new ListOperationErrorDescription(entityName, ex.Message), HttpStatusCode.BadRequest);
            }
        }

        private ListResult ExecuteInternal(IEntityType entityName,
                                           int start,
                                           string filterInput,
                                           string extendedInfo,
                                           string nameLocaleResourceId,
                                           int limit,
                                           string sort,
                                           long? parentId,
                                           IEntityType parentType)
        {
            // TODO {all, 18.09.2013}: обеспечить управление настройками usecase (duration и т.п.) на основе метаданных, неявно для operation services
            // Пока информации о длительности usecase, иногда, применяется в точке входа WCF (Web и т.д.)
            // Это и не очень хорошо, т.к. точка входа WCF - может объединяет несколько операций, в рамках одного service contract точки входа,
            // кроме того, один и то же operation service может запускаться из нескольких точек входа => придется клонировать настройки длительности
            // Цель - обеспечить управление настройками usecase (duration и т.п.) на основе метаданных, неявно для operation services.
            // Т.е. когда появятся метаданные по usecase - будет понятно, что одна и та же операция в рамках таких-то usecase ожидается длительной, далее, используя эту информацию,
            // некая фабричная прослойка- запускатор usecase (которой пока нет), должна при вызове операции (важно, то что при вызове, т.к. при создании применять может быть рано, 
            // т.к. не обязательно время жизни operationservice всегда будет perusecase) применять к ней знания из метаданных
            _useCaseTuner.AlterDuration<ListApplicationService>();

            var dataListStructure = GetDataListStructure(entityName, nameLocaleResourceId);
            if (string.IsNullOrEmpty(dataListStructure.MainAttribute))
            {
                throw new ArgumentException(BLResources.MainAttributeForEntityIsNotSpecified);
            }

            var searchListModel = new SearchListModel
            {
                Start = start,
                FilterInput = filterInput,
                ExtendedInfo = extendedInfo,
                NameLocaleResourceId = dataListStructure.NameLocaleResourceId,
                Limit = limit,
                Sort = !string.IsNullOrEmpty(sort) ? sort : string.Format("{0} {1}",dataListStructure.DefaultSortField, dataListStructure.DefaultSortDirection == 1 ? "DESC" : "ASC"),
                ParentEntityId = parentId,
                ParentEntityName = parentType
            };

            var listService = _operationServicesManager.GetListEntityService(entityName);
            var remoteCollection = listService.List(searchListModel);

            return new EntityDtoListResult
            {
                Data = remoteCollection,
                RowCount = remoteCollection.TotalCount,
                MainAttribute = dataListStructure.MainAttribute,
            };
        }

        private DataListStructure GetDataListStructure(IEntityType entityName, string nameLocaleResourceId)
        {
            var userCultureInfo = _userContext.Profile.UserLocaleInfo.UserCultureInfo;
            var gridSettings = _configurationService.GetGridSettings(entityName, userCultureInfo);

            if (string.IsNullOrEmpty(nameLocaleResourceId))
            {
                // lookup case
                return gridSettings.DataViews.First();
            }

            return gridSettings.DataViews.Single(x => string.Equals(x.NameLocaleResourceId, nameLocaleResourceId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
