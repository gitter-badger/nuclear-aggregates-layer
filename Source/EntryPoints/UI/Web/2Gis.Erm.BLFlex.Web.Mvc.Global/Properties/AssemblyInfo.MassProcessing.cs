using System.Web.Mvc;

using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Web.Mvc.Services;

[assembly: ContainedTypes(
    typeof(IController), 
    typeof(IUIService), 
    typeof(IEntityUIService))]