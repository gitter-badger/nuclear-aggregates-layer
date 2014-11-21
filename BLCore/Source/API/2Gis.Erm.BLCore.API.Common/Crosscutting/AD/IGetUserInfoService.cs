using System;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD
{
    /// <summary>
    /// Получает информацию о пользователе
    /// </summary>
    public interface IGetUserInfoService
    {
        /// <summary>
        /// Получает информацию о пользователе по его учетной записи.
        /// </summary>
        /// <param name="domainUserName">Учетная запись пользователя.</param>
        /// <returns></returns>
        ADUserProfile GetInfo(String domainUserName);

        /// <summary>
        /// Получает информацию о пользователе по его учетной записи.
        /// </summary>
        /// <param name="domainUserName">Учетная запись пользователя.</param>
        /// <param name="profile">Полученные данные</param>
        /// <returns>Возвращает индикатор, того удалось ли получить информацию о пользователе</returns>
        Boolean TryGetInfo(String domainUserName, out ADUserProfile profile);
    }
}
