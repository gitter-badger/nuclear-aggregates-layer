using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class PositionChildrenMap : EntityConfig<PositionChildren, ErmContainer>
    {
        public PositionChildrenMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("PositionChildren", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.MasterPositionId).HasColumnName("MasterPositionId");
            Property(t => t.ChildPositionId).HasColumnName("ChildPositionId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.ChildPosition)
                .WithMany(t => t.MasterPositions)
                .HasForeignKey(d => d.ChildPositionId);
            HasRequired(t => t.MasterPosition)
                .WithMany(t => t.ChildPositions)
                .HasForeignKey(d => d.MasterPositionId);
        }
    }
}