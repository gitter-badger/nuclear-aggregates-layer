using System.Web;

namespace DoubleGis.Erm.Platform.Web.Mvc.Security
{
    /// <summary>
    /// Создает значения по умолчанию для UserContext (т.е. identity и userprofile)
    /// Значения по умолчанию необходимы, до момента выставления реальных данных о пользователе в UserContext - до аутентификации
    /// </summary>
    public interface IDefaultUserContextConfigurator
    {
        void Configure(HttpRequest processingHttpRequest);
    }
}