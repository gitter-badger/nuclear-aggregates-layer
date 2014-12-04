using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class BusinessOperationServiceMap : EntityConfig<BusinessOperationService, ErmContainer>
    {
        public BusinessOperationServiceMap()
        {
            // Primary Key
            HasKey(t => new { t.Descriptor, t.Operation, t.Service });

            // Properties
            Property(t => t.Descriptor)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Operation)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Service)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("BusinessOperationServices", "Shared");
            Property(t => t.Descriptor).HasColumnName("Descriptor");
            Property(t => t.Operation).HasColumnName("Operation");
            Property(t => t.Service).HasColumnName("Service");
        }
    }
}