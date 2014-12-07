using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ChargesHistoryMap : EntityConfig<ChargesHistory, ErmContainer>
    {
        public ChargesHistoryMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Message)
                .IsRequired();

            // Table & Column Mappings
            ToTable("ChargesHistory", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ProjectId).HasColumnName("ProjectId");
            Property(t => t.PeriodStartDate).HasColumnName("PeriodStartDate");
            Property(t => t.PeriodEndDate).HasColumnName("PeriodEndDate");
            Property(t => t.Message).HasColumnName("Message");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.SessionId).HasColumnName("SessionId");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
        }
    }
}