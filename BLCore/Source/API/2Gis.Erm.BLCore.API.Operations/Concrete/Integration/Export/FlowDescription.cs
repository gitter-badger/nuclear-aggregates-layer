using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    public sealed class FlowDescription
    {
        public string FlowName { get; set; }
        public IEntityType EntityName { get; set; }
        public string SchemaResourceName { get; set; }
        public IEntityType IntegrationEntityName { get; set; }
        public string Context { get; set; }
    }
}
