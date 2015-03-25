using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.ErmSecurity
{
    public class FunctionalPrivilegeDepthMap : EntityConfig<FunctionalPrivilegeDepth, ErmSecurityContainer>
    {
        public FunctionalPrivilegeDepthMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.LocalResourceName)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            ToTable("FunctionalPrivilegeDepths", "Security");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.PrivilegeId).HasColumnName("PrivilegeId");
            Property(t => t.LocalResourceName).HasColumnName("LocalResourceName");
            Property(t => t.Priority).HasColumnName("Priority");
            Property(t => t.Mask).HasColumnName("Mask");

            // Relationships
            HasRequired(t => t.Privilege)
                .WithMany(t => t.FunctionalPrivilegeDepths)
                .HasForeignKey(d => d.PrivilegeId);
        }
    }
}