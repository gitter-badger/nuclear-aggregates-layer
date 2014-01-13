namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public class CrudStatus
    {
        public string EntityCode { get; set; }
        public bool IsDeleteAllowed { get; set; }
        public string DeleteConformation { get; set; }
        public string DeleteDisallowedReason { get; set; }
    }
}