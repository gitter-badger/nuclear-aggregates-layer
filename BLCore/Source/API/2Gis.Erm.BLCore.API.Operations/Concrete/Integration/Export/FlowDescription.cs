using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    public sealed class FlowDescription
    {
        public string FlowName { get; set; }
        public EntityName EntityName { get; set; }
        public string SchemaResourceName { get; set; }
        public EntityName IntegrationEntityName { get; set; }
        public string Context { get; set; }
    }
}
