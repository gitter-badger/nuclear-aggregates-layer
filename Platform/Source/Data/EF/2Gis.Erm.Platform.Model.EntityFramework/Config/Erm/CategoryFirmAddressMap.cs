using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class CategoryFirmAddressMap : EntityConfig<CategoryFirmAddress, ErmContainer>
    {
        public CategoryFirmAddressMap()
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
            ToTable("CategoryFirmAddresses", "BusinessDirectory");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.CategoryId).HasColumnName("CategoryId");
            Property(t => t.FirmAddressId).HasColumnName("FirmAddressId");
            Property(t => t.SortingPosition).HasColumnName("SortingPosition");
            Property(t => t.IsPrimary).HasColumnName("IsPrimary");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Category)
                .WithMany(t => t.CategoryFirmAddresses)
                .HasForeignKey(d => d.CategoryId);
            HasRequired(t => t.FirmAddress)
                .WithMany(t => t.CategoryFirmAddresses)
                .HasForeignKey(d => d.FirmAddressId);
        }
    }
}