using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Bargains
{
    // FIXME {y.baranihin, 09.07.2014}: Преобразовать в исключение, которое можно переиспользовать для различных функциональных привелегий
    // Аналогичный вопрос уже был с AdvertisementElementValidationAccessDeniedException в другой ветке
    // COMMENT {a.rechkalov, 10.07.2014}: то исключение создано в той ветке. Заиспользуем его, когда оно приедет.
    [Serializable]
    public class AccessToAgentBargainCreationIsDeniedException : BusinessLogicException
    {
        public AccessToAgentBargainCreationIsDeniedException()
        {
        }

        public AccessToAgentBargainCreationIsDeniedException(string message)
            : base(message)
        {
        }

        public AccessToAgentBargainCreationIsDeniedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AccessToAgentBargainCreationIsDeniedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}