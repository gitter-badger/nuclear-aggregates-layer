using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AdvertisementElementTemplateMap : EntityConfig<AdvertisementElementTemplate, ErmContainer>
    {
        public AdvertisementElementTemplateMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.FileExtensionRestriction)
                .HasMaxLength(128);

            Property(t => t.ImageDimensionRestriction)
                .HasMaxLength(128);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("AdvertisementElementTemplates", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.RestrictionType).HasColumnName("RestrictionType");
            Property(t => t.TextLengthRestriction).HasColumnName("TextLengthRestriction");
            Property(t => t.MaxSymbolsInWord).HasColumnName("MaxSymbolsInWord");
            Property(t => t.TextLineBreaksCountRestriction).HasColumnName("TextLineBreaksCountRestriction");
            Property(t => t.FormattedText).HasColumnName("FormattedText");
            Property(t => t.FileSizeRestriction).HasColumnName("FileSizeRestriction");
            Property(t => t.FileExtensionRestriction).HasColumnName("FileExtensionRestriction");
            Property(t => t.FileNameLengthRestriction).HasColumnName("FileNameLengthRestriction");
            Property(t => t.ImageDimensionRestriction).HasColumnName("ImageDimensionRestriction");
            Property(t => t.IsRequired).HasColumnName("IsRequired");
            Property(t => t.NeedsValidation).HasColumnName("NeedsValidation");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.IsAdvertisementLink).HasColumnName("IsAdvertisementLink");
        }
    }
}