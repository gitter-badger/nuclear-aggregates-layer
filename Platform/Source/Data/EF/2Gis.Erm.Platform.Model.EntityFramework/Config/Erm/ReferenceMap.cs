using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ReferenceMap : EntityConfig<Reference, ErmContainer>
    {
        public ReferenceMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.CodeName)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Reference", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.CodeName).HasColumnName("CodeName");
        }
    }
}