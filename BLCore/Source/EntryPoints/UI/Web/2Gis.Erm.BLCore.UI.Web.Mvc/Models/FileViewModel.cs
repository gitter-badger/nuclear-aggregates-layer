
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    // todo: refactor to use FileUploadModel as a member instead of inheriting from this class
    public abstract class FileViewModel<T> : EntityViewModelBase<T> where T : IEntityKey
    {
        [PresentationLayerProperty]
        public string FileName { get; set; }

        [PresentationLayerProperty]
        public string FileContentType { get; set; }

        [PresentationLayerProperty]
        public long FileContentLength { get; set; }

        [PresentationLayerProperty]
        public string FileContent { get; set; }
    }
}