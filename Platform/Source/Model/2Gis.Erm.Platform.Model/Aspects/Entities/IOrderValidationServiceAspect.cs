using System;

namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IOrderValidationServiceAspect : IAspect
    {
        Uri OrderValidationServiceUrl { get; set; }
    }
}