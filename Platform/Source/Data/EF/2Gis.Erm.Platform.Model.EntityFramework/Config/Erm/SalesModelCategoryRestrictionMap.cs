using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class SalesModelCategoryRestrictionMap : EntityConfig<SalesModelCategoryRestriction, ErmContainer>
    {
        public SalesModelCategoryRestrictionMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("SalesModelCategoryRestrictions", "BusinessDirectory");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.CategoryId).HasColumnName("CategoryId");
            Property(t => t.ProjectId).HasColumnName("ProjectId");
            Property(t => t.SalesModel).HasColumnName("SalesModel");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");

            // Relationships
            HasRequired(t => t.Category)
                .WithMany(t => t.SalesModelRestrictions)
                .HasForeignKey(d => d.CategoryId);
            HasRequired(t => t.Project)
                .WithMany(t => t.SalesModelRestrictions)
                .HasForeignKey(d => d.ProjectId);
        }
    }
}