using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Metadata
{
    public interface IEntityViewNameProvider
    {
        string GetView<TViewModel, TEntity>()
            where TViewModel : class, IViewModel
            where TEntity : IEntityKey;
    }
}
