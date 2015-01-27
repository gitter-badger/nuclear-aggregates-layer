using System;

using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IOrderDatesAspect : IAspect
    {
        DateTime SignupDate { get; set; }
        DateTime BeginDistributionDate { get; set; }
        DateTime EndDistributionDateFact { get; set; }
    }
}
