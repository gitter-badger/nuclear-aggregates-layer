using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ChargeMap : EntityConfig<Charge, ErmContainer>
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
            Property(t => t.PeriodStartDate).HasColumnName("PeriodStartDate");
            Property(t => t.PeriodEndDate).HasColumnName("PeriodEndDate");
            Property(t => t.SessionId).HasColumnName("SessionId");
            Property(t => t.Amount).HasColumnName("Amount");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
        }
    }
}