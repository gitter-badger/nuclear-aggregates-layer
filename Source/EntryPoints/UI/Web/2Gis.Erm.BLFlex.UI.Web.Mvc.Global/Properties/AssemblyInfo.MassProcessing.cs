using System.Web.Mvc;

using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;

[assembly: ContainedTypes(
    typeof(IController), 
    typeof(IUIService), 
    typeof(IEnumAdaptationService),
    typeof(IEntityUIService))]