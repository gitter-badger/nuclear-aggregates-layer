namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AdvertisementElementPreviewModel
    {
        // org_view_expanded
        public bool Highlight { get; set; }
        public string Microcomment { get; set; }
        public string ArticleRef { get; set; }
        public string AdComment { get; set; }
        public string ImgDiscountRef { get; set; }
        public string AdditionalTitleContent { get; set; }
        public string OrgName { get; set; }
        public string TxtDiscountRef { get; set; }
        public string MicrocommentWarning { get; set; }
        public object Banner { get; set; }
        public string BannerWarning { get; set; }
        public string AdCommentWarning { get; set; }
        public int? RubricsCount { get; set; }
        public object[] Rubrics { get; set; }
        public object Filials { get; set; }
        public object PluginsContent { get; set; }
        public object MoreFilials { get; set; }
        public string FeedbackRef { get; set; }

        // fil_view
        public string Title { get; set; }
        public string Address { get; set; }
        public object MapRef { get; set; }
        public string Office { get; set; }
        public object InfocardRef { get; set; }
        public string PluginText { get; set; }
        public string AddrComment { get; set; }
        public string WarningComment { get; set; }
        public ContactGroup[] ContactGroups { get; set; }
        public string WorkTime { get; set; }
        public Payment[] Payments { get; set; }

        // public PlatformEnum? PlatformEnum { get; set; }
    }

    public class Payment
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string TooltipImage { get; set; }
    }

    public class ContactGroup
    {
        public string Name { get; set; }
        public string[] Contacts { get; set; }
    }
}