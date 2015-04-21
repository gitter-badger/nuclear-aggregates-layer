namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface ISelectableToWhiteListAspect : IAspect
    {
        bool IsSelectedToWhiteList { get; set; }
    }
}
