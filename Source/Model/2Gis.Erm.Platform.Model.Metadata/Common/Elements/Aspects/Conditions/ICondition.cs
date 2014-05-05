namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Conditions
{
    public interface ICondition
    {
        bool EvaluateCondition(object document);
    }
}
