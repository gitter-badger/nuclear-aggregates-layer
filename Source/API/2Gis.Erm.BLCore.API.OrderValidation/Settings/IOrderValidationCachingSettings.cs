﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Settings
{
    public interface IOrderValidationCachingSettings : ISettings
    {
        IEnumerable<string> RulesExplicitlyDisabledCaching { get; }
        
        /// <summary>
        /// Флаг показывает включен ли "старый" режим кэширования результатов проверок заказов - который использует сортировку по Id для результатов
        /// </summary>
        [Obsolete("После исключения проблем с нагрузкой на tempdb варианта кэша проверок использующего версии заказов - выпилить")]
        bool UseLegacyCachingMode { get; }
    }
}
