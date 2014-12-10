using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IAppointmentViewModel : IEntityViewModelAbstract<Appointment>
    {
        string Title { get; set; }
    }
}
