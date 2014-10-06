using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class BusinessEntityPropertyInstanceMap : EntityTypeConfiguration<BusinessEntityPropertyInstance>
    {
        public BusinessEntityPropertyInstanceMap()
        {
            // Primary Key
            HasKey(t => new { t.EntityInstanceId, t.PropertyId });

            // Properties
            Property(t => t.EntityInstanceId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.PropertyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("BusinessEntityPropertyInstances", "DynamicStorage");
            Property(t => t.EntityInstanceId).HasColumnName("EntityInstanceId");
            Property(t => t.PropertyId).HasColumnName("PropertyId");
            Property(t => t.TextValue).HasColumnName("TextValue");
            Property(t => t.NumericValue).HasColumnName("NumericValue");
            Property(t => t.DateTimeValue).HasColumnName("DateTimeValue");

            // Relationships
            HasRequired(t => t.BusinessEntityInstance)
                .WithMany(t => t.BusinessEntityPropertyInstances)
                .HasForeignKey(d => d.EntityInstanceId);
        }
    }
}