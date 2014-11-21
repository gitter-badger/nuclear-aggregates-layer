using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    /// <summary>
    /// Модель для слияния юридических лиц
    /// </summary>
    public sealed class MergeLegalPersonsViewModel : ViewModel
    {
        #region Свойства
        [RequiredLocalized]
        public LookupField LegalPerson1 { get; set; }
        [RequiredLocalized]
        public LookupField LegalPerson2 { get; set; }

        /// <summary>
        /// Идентификатор второстепенной записи
        /// </summary>
        public long? AppendedLegalPersonId { get; set; }
        
        /// <summary>
        /// Идентификатор основной записи
        /// </summary>
        public long? MainLegalPersonId { get; set; }
        #endregion
    }

    public sealed class MergeLegalPersonsDataViewModel : ViewModel
    {
        public LegalPersonViewModel LegalPerson1 { get; set; }
        public LegalPersonViewModel LegalPerson2 { get; set; }
    }
}