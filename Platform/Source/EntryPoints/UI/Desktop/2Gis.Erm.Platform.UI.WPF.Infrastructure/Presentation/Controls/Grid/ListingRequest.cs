namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public sealed class ListingRequest
    {
        public bool RequireNew { get; set; }
        public int? TargetPageNumber { get; set; }
        public string FilterText { get; set; }
        public DataViewViewModel ViewSettings { get; set; }
    }
}
