using System;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase
{
    public static class UseCaseDIUtils
    {
        public static IUnityContainer ResolveFactoryContext(this IUseCase useCase)
        {
            var container = useCase.FactoriesContext as IUnityContainer;
            if (container == null)
            {
                throw new InvalidOperationException("Unsupported use case factories context type. " + useCase.FactoriesContext);
            }

            return container;
        }
    }
}