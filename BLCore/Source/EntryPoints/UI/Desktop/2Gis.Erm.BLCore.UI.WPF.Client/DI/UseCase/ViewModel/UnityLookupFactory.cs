using System;

using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityLookupFactory : ILookupFactory
    {
        public LookupViewModel Create(IUseCase useCase, IViewModelIdentity hostViewModelIdentity, LookupPropertyFeature settings)
        {
            var factory = useCase.ResolveFactoryContext();
            return factory.Resolve<LookupViewModel>(
                new ResolverOverride[]
                    {
                        new DependencyOverride(typeof(LookupPropertyFeature), settings),
                        new DependencyOverride(typeof(Guid), hostViewModelIdentity.Id) 
                    });
        }
    }
}