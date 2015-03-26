using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

namespace DoubleGis.Erm.Platform.API.Security
{
    /// <summary>
    /// Позволяет имперсонироваться под указанным user account, 
    /// т.е. фактически подменяет для системы знания о текущем аутентифицированом пользователе на указанного пользователя
    /// ALARMA!!! Использовать нужно только в исключительных ситуациях, т.к. подмена пользователя влияет на все, а, самое главное, на последующие проверки безопасности.
    /// </summary>
    public interface IUserImpersonationService
    {
        IUserInfo ImpersonateAsUser(string userAccount);
    }
}
