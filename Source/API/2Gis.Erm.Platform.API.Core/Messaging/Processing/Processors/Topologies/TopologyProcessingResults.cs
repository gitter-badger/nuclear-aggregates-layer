namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors.Topologies
{
    public sealed class TopologyProcessingResults
    {
        public IMessage[] Passed { get; set; }
        public IMessage[] Succeeded { get; set; }
        public IMessage[] Failed { get; set; }
    }
}