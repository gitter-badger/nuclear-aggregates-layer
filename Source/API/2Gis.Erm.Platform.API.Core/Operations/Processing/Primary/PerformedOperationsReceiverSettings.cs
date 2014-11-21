namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary
{
    public sealed class PerformedOperationsReceiverSettings : IPerformedOperationsReceiverSettings
    {
        private const int DefaultTimeSafetyOffsetHours = 24;

        public PerformedOperationsReceiverSettings()
        {
            TimeSafetyOffsetHours = DefaultTimeSafetyOffsetHours;
        }

        public int BatchSize { get; set; }
        public int TimeSafetyOffsetHours { get; set; }
    }
}