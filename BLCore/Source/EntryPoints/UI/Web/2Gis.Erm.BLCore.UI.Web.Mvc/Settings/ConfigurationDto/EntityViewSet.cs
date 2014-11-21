using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
{
    public sealed class EntityViewSet
    {
        public bool HasCard { get; set; }
        public string EntityName { get; set; }
        public IEnumerable<DataViewJson> DataViews { get; set; }
    }
}