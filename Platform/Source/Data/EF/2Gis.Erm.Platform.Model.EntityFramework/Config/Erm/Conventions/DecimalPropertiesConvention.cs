using System.Data.Entity.ModelConfiguration.Conventions;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm.Conventions
{
    public class DecimalPropertiesConvention : EntityConvention<ErmContainer>
    {
        protected override void Configure(Convention convention)
        {
            convention.Properties<decimal>().Configure(p => p.HasPrecision(19, 4));
            convention.Properties<decimal?>().Configure(p => p.HasPrecision(19, 4));
        }
    }
}