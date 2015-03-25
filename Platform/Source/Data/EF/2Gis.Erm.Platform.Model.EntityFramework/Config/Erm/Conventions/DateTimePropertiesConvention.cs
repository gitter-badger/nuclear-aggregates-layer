using System;
using System.Data.Entity.ModelConfiguration.Conventions;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm.Conventions
{
    public class DateTimePropertiesConvention : EntityConvention<ErmContainer>
    {
        protected override void Configure(Convention convention)
        {
            convention.Properties<DateTime>().Configure(p => p.HasColumnType("datetime2").HasPrecision(2));
            convention.Properties<DateTime?>().Configure(p => p.HasColumnType("datetime2").HasPrecision(2));
        }
    }
}