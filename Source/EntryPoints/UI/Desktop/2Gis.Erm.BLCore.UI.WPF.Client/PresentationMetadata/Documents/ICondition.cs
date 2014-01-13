namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public interface ICondition
    {
        bool EvaluateCondition(object document);
    }
}
