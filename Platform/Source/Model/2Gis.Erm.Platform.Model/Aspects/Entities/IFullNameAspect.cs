namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IFullNameAspect : IAspect
    {
        // COMMENT {all, 24.01.2015}: DisplayName?
        string FullName { get; }
    }
}