using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.Execute
{
    public sealed class ExecuteAssignMessageHandler : 
        OperationMessageHandlerBase<AssignIdentity, AssignCommonParameter, AssignEntityParameter, AssignResult>
    {
        public ExecuteAssignMessageHandler(IOperationServicesManager operationServicesManager, ITracer tracer)
            : base(operationServicesManager, tracer)
        {
        }

        protected override AssignResult[] ExecuteOperation(AssignCommonParameter commonParameter, AssignEntityParameter[] parameters)
        {
            return parameters.Length > 1 
                ? GroupAssign(commonParameter, parameters) 
                : SingleAssign(commonParameter, parameters);
        }

        private AssignResult[] SingleAssign(AssignCommonParameter commonParameter, IEnumerable<AssignEntityParameter> parameters)
        {
            var parameter = parameters.Single();
            var service = OperationServicesManager.GetAssignEntityService(commonParameter.EntityName);
            var result = service.Assign(parameter.EntityId, commonParameter.OwnerCode, commonParameter.BypassValidation, commonParameter.IsPartialAssign);
            return new[] { result };
        }

        private AssignResult[] GroupAssign(AssignCommonParameter commonParameter, IEnumerable<AssignEntityParameter> parameters)
        {
            var service = OperationServicesManager.GetAssignEntityService(commonParameter.EntityName) as IGroupAssignEntityService;
            if (service == null)
            {
                throw new InvalidOperationException("Can't get service for operation group mode. Operation: " + OperationIdentity);
            }

            return service.Assign(commonParameter, parameters).ToArray();
        }
    }
}