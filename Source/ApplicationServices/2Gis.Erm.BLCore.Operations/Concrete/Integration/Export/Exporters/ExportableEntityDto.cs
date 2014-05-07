using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Exporters
{
    public sealed class ExportableEntityDto : IExportableEntityDto
    {
        public long Id { get; set; }
    }
}