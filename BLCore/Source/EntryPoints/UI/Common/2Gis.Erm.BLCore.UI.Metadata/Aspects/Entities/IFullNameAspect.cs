namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IFullNameAspect : IAspect
    {
        // COMMENT {all, 24.01.2015}: DisplayName?
        string FullName { get; }
    }
}