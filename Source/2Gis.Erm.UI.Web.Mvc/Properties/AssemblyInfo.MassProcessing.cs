using System.Web.Mvc;

using DoubleGis.Erm.Model.Entities.MassProcessing;
using DoubleGis.Erm.Model.SimplifiedModel;
using DoubleGis.Erm.UI.Web.Mvc.Services;
using DoubleGis.Erm.UI.Web.Mvc.Services.Enums;

[assembly: ContainedTypes(
    typeof(ISimplifiedModelConsumer),
    typeof(IController),
    typeof(IEnumAdaptationService),
    typeof(IUIService),
    typeof(IEntityUIService))]