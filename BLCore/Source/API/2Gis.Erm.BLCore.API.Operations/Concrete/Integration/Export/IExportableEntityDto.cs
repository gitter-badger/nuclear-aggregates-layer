using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    /// <summary>
    /// Абстракция для DTO которая может быть экспортирована системой при интеграции с внешними системами, либо с какими-то автономными подсистемами ERM
    /// </summary>
    public interface IExportableEntityDto : IEntityKey, IOperationSpecificEntityDto<ExportIdentity>
    {
    }
}
