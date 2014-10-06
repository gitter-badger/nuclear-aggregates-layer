using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class ChargeMap : EntityTypeConfiguration<Charge>
    {
        public ChargeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("Charges", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderPositionId).HasColumnName("OrderPositionId");
            Property(t => t.ProjectId).HasColumnName("ProjectId");
            Property(t => t.PositionId).HasColumnName("PositionId");
            Property(t => t.PeriodStartDate).HasColumnName("PeriodStartDate");
            Property(t => t.PeriodEndDate).HasColumnName("PeriodEndDate");
            Property(t => t.SessionId).HasColumnName("SessionId");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
        }
    }
}