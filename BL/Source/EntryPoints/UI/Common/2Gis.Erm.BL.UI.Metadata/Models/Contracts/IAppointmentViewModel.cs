using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BL.UI.Metadata.Models.Contracts
{
    public interface IAppointmentViewModel : IEntityViewModelAbstract<Appointment>
    {
        string Header { get; set; }
    }
}
