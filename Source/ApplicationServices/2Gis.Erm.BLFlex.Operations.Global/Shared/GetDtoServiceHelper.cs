using System.Text.RegularExpressions;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    // FIXME {v.lapeev, 21.05.2014}: В чем отвественность этого утилитного класса? Может ли эта функциональность использоваться где-то кроме IGetDomainEntityDtoService-сервиса?
    //                               Предлагаю вернуть этот код в сервис, ибо cohesion 
    internal static class GetDtoServiceHelper
    {
        public static bool TryGetClientId(long? parentEntityId, EntityName parentEntityName, string extendedInfo, out long clientId)
        {
            if (parentEntityName == EntityName.Client && parentEntityId > 0)
            {
                clientId = parentEntityId.Value;
                return true;
            }

            if (!string.IsNullOrEmpty(extendedInfo))
            {
                return long.TryParse(Regex.Match(extendedInfo, @"ClientId=(\d+)").Groups[1].Value, out clientId);
            }

            clientId = 0;
            return false;
        }
    }
}
