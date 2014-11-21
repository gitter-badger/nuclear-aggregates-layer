using System.Globalization;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Settings
{
    public interface IAppSettings
    {
        /// <summary>
        /// Культура для использование приложением - та же, что и культура текущего пользователя на серверной стороне
        /// </summary>
        CultureInfo TargetCulture { get; }

        /// <summary>
        /// Erm Id пользователя для которого конфигурируем профиль
        /// </summary>
        long UserCode { get; }
    }
}