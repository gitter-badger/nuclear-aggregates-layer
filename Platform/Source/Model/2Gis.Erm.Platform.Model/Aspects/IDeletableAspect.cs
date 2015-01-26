namespace DoubleGis.Erm.Platform.Model.Aspects
{
    public interface IDeletableAspect : IAspect
    {
        bool IsDeleted { get; }
    }
}