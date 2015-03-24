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

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations
{
    public interface IOperationServicesManager
    {
        IListEntityService GetListEntityService(IEntityType entityName);
        IDeleteEntityService GetDeleteEntityService(IEntityType entityName);
        IAssignEntityService GetAssignEntityService(IEntityType entityName);
        IDeactivateEntityService GetDeactivateEntityService(IEntityType entityName);
        IActivateEntityService GetActivateEntityService(IEntityType entityName);
        IQualifyEntityService GetQualifyEntityService(IEntityType entityName);
        IDisqualifyEntityService GetDisqualifyEntityService(IEntityType entityName);
        ICheckEntityForDebtsService GetCheckEntityForDebtsService(IEntityType entityName);
        IChangeEntityTerritoryService GetChangeEntityTerritoryService(IEntityType entityName);
        IChangeEntityClientService GetChangeEntityClientService(IEntityType entityName);
        IAppendEntityService GetAppendEntityService(IEntityType parentType, IEntityType appendedType);
        IActionsHistoryService GetActionHistoryService(IEntityType entityName);
        IGetDomainEntityDtoService GetDomainEntityDtoService(IEntityType entityName);
        IModifyDomainEntityService GetModifyDomainEntityService(IEntityType entityName);
        IDownloadFileService GetDownloadFileService(IEntityType entityName);
        IUploadFileService GetUploadFileService(IEntityType entityName);
        IIntegrationProcessorOperationService GetOperationsExportService(IEntityType entityName, IEntityType integrationEntityName);
        ICancelOperationService GetCancelService(IEntityType entityName);
        ICompleteOperationService GetCompleteService(IEntityType entityName);
        IReopenOperationService GetReopenService(IEntityType entityName);
    }
}
