﻿using System;

using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.Assembling.Zones;
using NuClear.Settings.API;
using NuClear.Settings.Unity;

namespace DoubleGis.Erm.BLCore.DI.Config
{
    public static partial class CommonBootstrapper
    {
        /// <summary>
        /// необхоимость в двух проходах возникла из-за особенностей работы 
        /// DoubleGis.Common.DI.Config.RegistrationUtils.RegisterTypeWithDependencies - он для определения создавать ResolvedParameter с указанным scope или БЕЗ конкретного scope
        /// делает проверку если тип параметра уже зарегистрирован в контейнере БЕЗ использования named mappings - то resolveparameter также будет работать без scope
        /// иначе для ResolvedParameter будет указан scope
        /// Т.о. при первом проходе создаются все mapping, но для некоторых из них значение ResolvedParameter будет ошибочно использовать scope
        /// второй проход перезатирает уже имеющиеся mapping -т.о. resolvedparameter будет правильно (не)связан с scope
        /// </summary>
        public static IUnityContainer ConfigureUnityTwoPhase(
            this IUnityContainer unityContainer,
            CompositionRoot root,
            ISettingsContainer settingsContainer,
            IMassProcessor[] massProcessors,
            Func<IUnityContainer, IUnityContainer> configurator)
        {
            var businessModelSettings = settingsContainer.AsSettings<IBusinessModelSettings>();
            // первый проход
            unityContainer.ConfigureSettingsAspects(settingsContainer);
            configurator(unityContainer);
            root.PerformTypesMassProcessing(massProcessors, true, businessModelSettings.BusinessModelIndicator);

            // второй проход
            unityContainer.ConfigureSettingsAspects(settingsContainer);
            configurator(unityContainer);
            root.PerformTypesMassProcessing(massProcessors, false, businessModelSettings.BusinessModelIndicator);

            return unityContainer;
        }
    }
}
