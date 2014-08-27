using System;
using System.Collections.Generic;
using System.ServiceModel.Description;

using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;

namespace DoubleGis.Erm.API.WCF.Operations.Special
{
    public sealed class SpecialOperationsSharedTypesBehaviorFactory : ISharedTypesBehaviorFactory
    {
        public IEndpointBehavior Create()
        {
            var typeNameConveter = new SoapTypeNameConveter();
            return new SharedTypesWsdlExportEndpointBehavior(new HashSet<Type>(SharedTypesProvider.SharedTypes),
                                                             typeNameConveter,
                                                             SharedTypesProvider.NamespacesByAssemblies,
                                                             new SharedTypeResolver(typeNameConveter));
        }
    }
}