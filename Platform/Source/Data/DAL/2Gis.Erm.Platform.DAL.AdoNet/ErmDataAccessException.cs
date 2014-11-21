using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.AdoNet
{
    public class ErmDataAccessException : Exception
    {
        public ErmDataAccessException(string connectionString, string procedureName, IEnumerable<Tuple<string, object>> procedureParameters, Exception innerException)
            : base(CreateMessage(connectionString, procedureName, procedureParameters), innerException)
        {
        }

        private static string CreateMessage(string connectionString, string procedureName, IEnumerable<Tuple<string, object>> procedureParameters)
        {
            return string.Format("Ошибка выполнения запроса к базе данных. " +
                                 "Строка подключения: '{0}'. " +
                                 "Вызвана хранимая процедура '{1}'. " +
                                 "Для вызова использованы параметры '{2}'",
                                 connectionString,
                                 procedureName,
                                 string.Join(", ", procedureParameters.Select(tuple => string.Format("{0}={1}", tuple.Item1, ShortenParameter(tuple.Item2)))));
        }

        private static string ShortenParameter(object item2)
        {
            var x = item2.ToString();
            return x.Length > 100
                       ? x.Substring(0, 100) + "..."
                       : x;
        }
    }
}
