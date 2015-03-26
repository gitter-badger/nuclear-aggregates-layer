using System.ComponentModel;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public class LookupSortInfo
    {
        public string Field { get; set; }
        public ListSortDirection Direction { get; set; }

        public string GetDirectionString()
        {
            return Direction == ListSortDirection.Ascending ? "ASC" : "DESC";
        }
    }
}