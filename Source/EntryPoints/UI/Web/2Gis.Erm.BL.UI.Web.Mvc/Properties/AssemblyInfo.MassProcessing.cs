using System.Web.Mvc;

using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

[assembly: ContainedTypes(
    typeof(IController), 
    typeof(IUIService),
    typeof(IEntityUIService),
    typeof(IViewModel))]