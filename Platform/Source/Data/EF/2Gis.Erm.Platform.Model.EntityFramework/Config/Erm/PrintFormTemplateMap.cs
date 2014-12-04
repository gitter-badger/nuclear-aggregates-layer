using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class PrintFormTemplateMap : EntityConfig<PrintFormTemplate, ErmContainer>
    {
        public PrintFormTemplateMap()
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
            ToTable("PrintFormTemplates", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.BranchOfficeOrganizationUnitId).HasColumnName("BranchOfficeOrganizationUnitId");
            Property(t => t.FileId).HasColumnName("FileId");
            Property(t => t.TemplateCode).HasColumnName("TemplateCode");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.BranchOfficeOrganizationUnit)
                .WithMany(t => t.PrintFormTemplates)
                .HasForeignKey(d => d.BranchOfficeOrganizationUnitId);
            HasRequired(t => t.File)
                .WithMany(t => t.PrintFormTemplates)
                .HasForeignKey(d => d.FileId);
        }
    }
}