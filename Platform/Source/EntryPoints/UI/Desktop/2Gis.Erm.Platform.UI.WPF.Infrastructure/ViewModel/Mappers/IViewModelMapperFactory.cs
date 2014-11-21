using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers
{
    public interface IViewModelMapperFactory
    {
        IViewModelMapper GetMapper(EntityName entityName);
    }
}
