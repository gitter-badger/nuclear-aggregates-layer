namespace DoubleGis.Erm.Platform.Model.Aspects
{
    public interface INewableAspect : IAspect
    {
        bool IsNew { get; }
    }
}