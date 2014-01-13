using System.Web;

using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services
{
    /// <summary>
    /// Специфичная для веб-приложения фабрика ссылок на карточки сущностей.
    /// Специфичность обусловлена использованием HttpContext.Current для получения адреса веб-приложения.
    /// </summary>
    public class WebAppLinkToEntityCardFactory : ILinkToEntityCardFactory
    {
        const string PathTemplate = "{0}/CreateOrUpdate/{1}/{2}";

        public string CreateLink(EntityName entity, long entityId)
        {
            var url = HttpContext.Current.Request.Url;
            var webApplicationRoot = string.Format("{0}://{1}", url.Scheme, url.Authority);
            return string.Format(PathTemplate, webApplicationRoot, entity, entityId);
        }

        public string CreateLinkTag(EntityName entity, long entityId, string linkText)
        {
            var link = CreateLink(entity, entityId);
            return string.Format("<a href='{0}'>{1}</a>", link, linkText);
        }
    }
}