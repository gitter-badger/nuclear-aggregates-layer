using NuClear.Model.Common.Entities.Aspects;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels
// ReSharper restore CheckNamespace
{
    public partial interface IEntityViewModelBase : IViewModelAbstract
    {
        EntityViewConfig ViewConfig { get; set; }

        bool IsAuditable { get; }
        bool IsDeletable { get; }
        bool IsCurated { get; }
        bool IsDeactivatable { get; }
        bool IsSecurityRoot { get; }

        void SetEntityStateToken();
        void GetEntityStateToken();

        void LoadDomainEntityDto(IDomainEntityDto domainEntityDto);
        IDomainEntityDto TransformToDomainEntityDto();
    }
}