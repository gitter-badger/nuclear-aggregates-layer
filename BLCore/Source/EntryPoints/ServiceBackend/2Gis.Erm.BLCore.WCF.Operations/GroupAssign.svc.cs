using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    public sealed class GroupAssignApplicationService : IGroupAssignApplicationService
    {
        private readonly ICommonLog _logger;
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly INotifiyProgressSettings _notifiyProgressSettings;

        public GroupAssignApplicationService(IOperationServicesManager operationServicesManager, INotifiyProgressSettings notifiyProgressSettings, IUserContext userContext, IResourceGroupManager resourceGroupManager, ICommonLog logger)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;
            _notifiyProgressSettings = notifiyProgressSettings;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public AssignResult[] Assign(AssignCommonParameter operationParameter, AssignEntityParameter[] operationItemParameters)
        {
            return ExecuteInternal(operationParameter.OperationToken, operationParameter, operationItemParameters);
        }

        private AssignResult[] ExecuteInternal(Guid operationToken, AssignCommonParameter operationParameter, AssignEntityParameter[] operationItemParameters)
        {
            if (operationParameter == null || operationItemParameters == null || !operationItemParameters.Any())
            {
                return new AssignResult[0];
            }

            var notificationBatchSize = operationItemParameters.Length < _notifiyProgressSettings.ProgressCallbackBatchSize 
                ? operationItemParameters.Length 
                : _notifiyProgressSettings.ProgressCallbackBatchSize;

            var resultsCache = new List<AssignResult>();
            var notificationsBatch = new List<IOperationResult>(_notifiyProgressSettings.ProgressCallbackBatchSize);

            var operationService = _operationServicesManager.GetAssignEntityService(operationParameter.EntityName);

            foreach (var operationItem in operationItemParameters)
            {
                AssignResult operationResult;
                try
                {
                    operationResult = operationService.Assign(
                        operationItem.EntityId,
                        operationParameter.OwnerCode,
                        operationParameter.BypassValidation,
                        operationParameter.IsPartialAssign) 
                    ?? new AssignResult
                            {
                                Succeeded = true,
                                EntityId = operationItem.EntityId,
                                OwnerCode = operationParameter.OwnerCode,
                                CanProceed = true,
                                Message = string.Empty
                            };
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                    operationResult = new AssignResult
                        {
                            Succeeded = false,
                            EntityId = operationItem.EntityId,
                            OwnerCode = operationParameter.OwnerCode, 
                            CanProceed = false, 
                            Message = ex.Message
                        };
                }

                resultsCache.Add(operationResult);
                notificationsBatch.Add(operationResult);

                if (notificationBatchSize == notificationsBatch.Count)
                {
                    var notificationsSnapshot = notificationsBatch.ToArray();
                    notificationsBatch.Clear();
                    
                    try
                    {
                        ProgressCallback.NotifyAboutProgress(operationToken, notificationsSnapshot);
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorFormat(ex, "Error has occured in {0}. Callback failed", GetType().Name);
                    }
                }
            }

            return resultsCache.ToArray();
        }

        private IOperationProgressCallback ProgressCallback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IOperationProgressCallback>();
            }
        }
    }
}
