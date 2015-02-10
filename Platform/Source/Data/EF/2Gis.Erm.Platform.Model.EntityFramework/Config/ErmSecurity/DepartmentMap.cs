using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.ErmSecurity
{
    public class DepartmentMap : EntityConfig<Department, ErmSecurityContainer>
    {
        public DepartmentMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Departments", "Security");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ParentId).HasColumnName("ParentId");
            Property(t => t.LeftBorder).HasColumnName("LeftBorder");
            Property(t => t.RightBorder).HasColumnName("RightBorder");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.Parent)
                .WithMany(t => t.Children)
                .HasForeignKey(d => d.ParentId);

            // CUD mappings
            MapToStoredProcedures(map => map.Insert(i => i.HasName("DepartmentInsert", "Security")
                                                          .Parameter(x => x.Id, "i_DepartmentID")
                                                          .Parameter(x => x.Name, "i_Name")
                                                          .Parameter(x => x.ParentId, "i_ParentID")
                                                          .Parameter(x => x.CreatedBy, "CreatedBy")
                                                          .Parameter(x => x.CreatedOn, "CreatedOn")

                                                             // ignored
                                                          .Parameter(x => x.LeftBorder, "LeftBorder")
                                                          .Parameter(x => x.RightBorder, "RightBorder")
                                                          .Parameter(x => x.IsActive, "IsActive")
                                                          .Parameter(x => x.IsDeleted, "IsDeleted")
                                                          .Parameter(x => x.ModifiedBy, "ModifiedBy")
                                                          .Parameter(x => x.ModifiedOn, "ModifiedOn"))
                                            .Update(u => u.HasName("DepartmentUpdate", "Security")
                                                          .Parameter(x => x.Id, "i_DepartmentID")
                                                          .Parameter(x => x.Name, "@i_Name")
                                                          .Parameter(x => x.ParentId, "@i_ParentId")
                                                          .Parameter(x => x.IsActive, "@i_IsActive")
                                                          .Parameter(x => x.Timestamp, "@i_timestamp")
                                                          .Parameter(x => x.ModifiedBy, "@ModifiedBy")
                                                          .Parameter(x => x.ModifiedOn, "@ModifiedOn")

                                                             // ignored
                                                          .Parameter(x => x.LeftBorder, "@LeftBorder")
                                                          .Parameter(x => x.RightBorder, "@RightBorder")
                                                          .Parameter(x => x.IsDeleted, "@IsDeleted")
                                                          .Parameter(x => x.CreatedBy, "@CreatedBy")
                                                          .Parameter(x => x.CreatedOn, "@CreatedOn")
                                                          .RowsAffectedParameter("RowsAffected")));
        }
    }
}