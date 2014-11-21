namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications
{
    public interface IContextualNotification : INotification
    {
        string PropertyName { get; set; }
    }
}