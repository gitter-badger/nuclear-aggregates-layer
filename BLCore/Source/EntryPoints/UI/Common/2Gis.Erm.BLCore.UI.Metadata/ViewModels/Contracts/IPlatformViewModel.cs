namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IPlatformViewModel : IEntityViewModelAbstract<Platform.Model.Entities.Erm.Platform>
    {
        string Name { get; set; }
    }
}