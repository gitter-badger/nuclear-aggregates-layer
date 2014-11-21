namespace DoubleGis.Erm.BLCore.API.Operations.Metadata
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations\Metadata
    public interface IMessageWithType
    {
        string MessageText { get; }
        MessageType Type { get; }
    }
}
