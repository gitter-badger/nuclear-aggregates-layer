using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Complete;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Revert;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations
{
    public interface IOperationServicesManager
    {
        IListEntityService GetListEntityService(EntityName entityName);
        IDeleteEntityService GetDeleteEntityService(EntityName entityName);
        IAssignEntityService GetAssignEntityService(EntityName entityName);
        IDeactivateEntityService GetDeactivateEntityService(EntityName entityName);
        IActivateEntityService GetActivateEntityService(EntityName entityName);
        IQualifyEntityService GetQualifyEntityService(EntityName entityName);
        IDisqualifyEntityService GetDisqualifyEntityService(EntityName entityName);
        ICheckEntityForDebtsService GetCheckEntityForDebtsService(EntityName entityName);
        IChangeEntityTerritoryService GetChangeEntityTerritoryService(EntityName entityName);
        IChangeEntityClientService GetChangeEntityClientService(EntityName entityName);
        IAppendEntityService GetAppendEntityService(EntityName parentType, EntityName appendedType);
        IActionsHistoryService GetActionHistoryService(EntityName entityName);
        IGetDomainEntityDtoService GetDomainEntityDtoService(EntityName entityName);
        IModifyDomainEntityService GetModifyDomainEntityService(EntityName entityName);
        IDownloadFileService GetDownloadFileService(EntityName entityName);
        IUploadFileService GetUploadFileService(EntityName entityName);
        IIntegrationProcessorOperationService GetOperationsExportService(EntityName entityName, EntityName integrationEntityName);
        ICancelService GetCancelService(EntityName entityName);
        ICompleteService GetCompleteService(EntityName entityName);
        IRevertService GetRevertService(EntityName entityName);
    }
}
