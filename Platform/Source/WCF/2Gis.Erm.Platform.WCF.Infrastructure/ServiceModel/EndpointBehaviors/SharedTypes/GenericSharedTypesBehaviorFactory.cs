using System;
using System.Collections.Generic;
using System.ServiceModel.Description;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes
{
    public class GenericSharedTypesBehaviorFactory : ISharedTypesBehaviorFactory
    {
        public IEndpointBehavior Create()
        {
            var typeNameConverter = new SoapTypeNameConveter();
            return new SharedTypesWsdlExportEndpointBehavior(new HashSet<Type>(),
                                                             typeNameConverter,
                                                             new Dictionary<string, string>(),
                                                             new SharedTypeResolver(typeNameConverter));
        }
    }
}