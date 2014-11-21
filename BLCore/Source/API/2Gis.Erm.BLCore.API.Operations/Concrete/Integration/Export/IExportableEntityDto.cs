using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    /// <summary>
    /// Абстракция для DTO которая может быть экспортирована системой при интеграции с внешними системами, либо с какими-то автономными подсистемами ERM
    /// </summary>
    public interface IExportableEntityDto : IEntityKey, IOperationSpecificEntityDto<ExportIdentity>
    {
    }
}
