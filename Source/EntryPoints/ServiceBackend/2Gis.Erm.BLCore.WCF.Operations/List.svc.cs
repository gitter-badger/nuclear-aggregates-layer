using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.API.Operations.Remote.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    [UseCase(Duration = UseCaseDuration.Long)]
    public class ListApplicationService : IListApplicationService, IListApplicationRestService
    {
        private readonly ICommonLog _logger;
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly IUseCaseTuner _useCaseTuner;

        public ListApplicationService(ICommonLog logger, IOperationServicesManager operationServicesManager, IUseCaseTuner useCaseTuner, IUserContext userContext)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;
            _useCaseTuner = useCaseTuner;

            BLResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
            MetadataResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
            EnumResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
        }

        public ListResult Execute(EntityName entityName,
                                  string whereExp,
                                  int start,
                                  string filterInput,
                                  string extendedInfo,
                                  string nameLocaleResourceId,
                                  int limit,
                                  string dir,
                                  string sort,
                                  string parentId,
                                  string parentType)
        {
            long parentIdInt;
            long.TryParse(parentId, out parentIdInt);
            try
            {
                return ExecuteInternal(entityName, whereExp, start, filterInput, extendedInfo, nameLocaleResourceId, limit, dir, sort, parentIdInt, parentType);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Error has occured in {0}. Entity type: {1}", GetType().Name, entityName);
                throw new FaultException<ListOperationErrorDescription>(new ListOperationErrorDescription(entityName, ex.Message));
            }
        }

        public ListResult Execute(string entityNameArg,
                                  string whereExp,
                                  int start,
                                  string filterInput,
                                  string extendedInfo,
                                  string nameLocaleResourceId,
                                  int limit,
                                  string dir,
                                  string sort,
                                  string parentId,
                                  string parentType)
        {
            long parentIdInt;
            long.TryParse(parentId, out parentIdInt);
            var entityName = EntityName.None;
            try
            {
                if (!Enum.TryParse(entityNameArg, out entityName))
                {
                    throw new ArgumentException("Entity Name cannot be parsed");
                }

                return ExecuteInternal(entityName, whereExp, start, filterInput, extendedInfo, nameLocaleResourceId, limit, dir, sort, parentIdInt, parentType);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Error has occured in {0}. Entity type: {1}", GetType().Name, entityName);
                throw new WebFaultException<ListOperationErrorDescription>(new ListOperationErrorDescription(entityName, ex.Message), HttpStatusCode.BadRequest);
            }
        }

        private ListResult ExecuteInternal(EntityName entityName,
                                           string whereExp,
                                           int start,
                                           string filterInput,
                                           string extendedInfo,
                                           string nameLocaleResourceId,
                                           int limit,
                                           string dir,
                                           string sort,
                                           long pId,
                                           string parentType)
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

            var searchListModel = new SearchListModel
                {
                    WhereExp = whereExp,
                    Start = start,
                    FilterInput = filterInput,
                    ExtendedInfo = extendedInfo,
                    NameLocaleResourceId = nameLocaleResourceId,
                    Limit = limit,
                    Dir = dir,
                    Sort = sort,
                    PId = pId,
                    PType = parentType
                };

            var listService = _operationServicesManager.GetListEntityService(entityName);
            var result = listService.List(searchListModel);
            return result;
        }
    }
}
