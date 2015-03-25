using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class DictionaryEntityPropertyInstanceMap : EntityConfig<DictionaryEntityPropertyInstance, ErmContainer>
    {
        public DictionaryEntityPropertyInstanceMap()
        {
            // Primary Key
            HasKey(t => new { t.EntityInstanceId, t.PropertyId });

            // Properties
            Property(t => t.EntityInstanceId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.PropertyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.NumericValue)
                .HasPrecision(23, 4);

            // Table & Column Mappings
            ToTable("DictionaryEntityPropertyInstances", "DynamicStorage");
            Property(t => t.EntityInstanceId).HasColumnName("EntityInstanceId");
            Property(t => t.PropertyId).HasColumnName("PropertyId");
            Property(t => t.TextValue).HasColumnName("TextValue");
            Property(t => t.NumericValue).HasColumnName("NumericValue");
            Property(t => t.DateTimeValue).HasColumnName("DateTimeValue");

            // Relationships
            HasRequired(t => t.DictionaryEntityInstance)
                .WithMany(t => t.DictionaryEntityPropertyInstances)
                .HasForeignKey(d => d.EntityInstanceId);
        }
    }
}