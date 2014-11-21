using DoubleGis.Erm.Platform.Model.Metadata.Enums;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports.DTO
{
    public sealed class DependencyDefinitionDto
    {
        public DependencyType DependencyType { get; set; }
        public string FieldName { get; set; }
        public string DependencyScript { get; set; }
    }
}
