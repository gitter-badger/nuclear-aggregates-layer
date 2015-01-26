using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers
{
    public interface IViewModelMapperFactory
    {
        IViewModelMapper GetMapper(IEntityType entityName);
    }
}
