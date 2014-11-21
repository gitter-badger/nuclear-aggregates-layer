using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public abstract class ValidationParams
    {
        private readonly Guid _token;
        private readonly ValidationType _type;

        protected ValidationParams(Guid validationToken, ValidationType type, IReadOnlyCollection<ValidationType> admissibleValidationTypes)
        {
            if (!admissibleValidationTypes.Contains(type))
            {
                throw new ArgumentException("Specified value " + type + " is not in the admissible values list: " + string.Join(";", admissibleValidationTypes));
            }

            _token = validationToken;
            _type = type;
        }

        /// <summary>
        /// ID сеанса проверки заказов, уникален в пределах одного вызова сервиса проверок (сеанса), пока чаще всего используется как correlationid для связывания сообщений в логе
        /// </summary>
        public Guid Token
        {
            get { return _token; }
        }
        
        /// <summary>
        /// Необходимо указать, запускается ли проверка в массовом режиме (перед сборкой), или в ручном (для одного Заказа)
        /// </summary>
        public ValidationType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Период размещения заказа(ов) затрагиваемый проверками
        /// </summary>
        // FIXME {all, 29.09.2014}: пока используется в единственном rule = AdvertisementsOnlyWhiteListOrderValidationRule, без конкретной привязки к массовой/единичной проверке - если там реализовать определение period для режима единичной проверки непосредственно в rule, то можно это свойство удалить из базового класса, оставив только в Mass*Params
        public TimePeriod Period { get; set; }
    }
}