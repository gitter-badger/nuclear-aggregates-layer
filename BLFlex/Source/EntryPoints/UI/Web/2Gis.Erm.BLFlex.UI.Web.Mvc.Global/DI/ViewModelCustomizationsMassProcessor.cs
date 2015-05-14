using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.DI
{
    [Obsolete("Must be merged with UIServicesMassProcessor")]
    public sealed class ViewModelCustomizationsMassProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;
        private readonly HashSet<Type> _customizationImplTypes = new HashSet<Type>();

        public ViewModelCustomizationsMassProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { typeof(IViewModelCustomization) };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                _customizationImplTypes.Add(type);
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                return;
            }

            foreach (var implType in _customizationImplTypes)
            {
                _container.RegisterTypeWithDependencies(implType, CustomLifetime.PerRequest, Mapping.Erm);
            }
        }

        private static bool ShouldBeProcessed(Type type)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                return false;
            }

            return true;
        }
    }
}