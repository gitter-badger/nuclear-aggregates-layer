using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.ErmSecurity
{
    public class UserProfileMap : EntityConfig<UserProfile, ErmSecurityContainer>
    {
        public UserProfileMap()
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

            Property(t => t.Email)
                .HasMaxLength(50);

            Property(t => t.Phone)
                .HasMaxLength(50);

            Property(t => t.Mobile)
                .HasMaxLength(50);

            Property(t => t.Address)
                .HasMaxLength(100);

            Property(t => t.Company)
                .HasMaxLength(100);

            Property(t => t.Position)
                .HasMaxLength(100);

            Property(t => t.PlanetURL)
                .HasMaxLength(100);

            // Table & Column Mappings
            ToTable("UserProfiles", "Security");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.TelephonyUnitId).HasColumnName("TelephonyUnitId");
            Property(t => t.TimeZoneId).HasColumnName("TimeZoneId");
            Property(t => t.CultureInfoLCID).HasColumnName("CultureInfoLCID");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.Email).HasColumnName("Email");
            Property(t => t.Phone).HasColumnName("Phone");
            Property(t => t.Mobile).HasColumnName("Mobile");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.Company).HasColumnName("Company");
            Property(t => t.Position).HasColumnName("Position");
            Property(t => t.Birthday).HasColumnName("Birthday");
            Property(t => t.Gender).HasColumnName("Gender");
            Property(t => t.PlanetURL).HasColumnName("PlanetURL");
            

            // Relationships
            HasRequired(t => t.TimeZone)
                .WithMany(t => t.UserProfiles)
                .HasForeignKey(d => d.TimeZoneId);
            HasRequired(t => t.User)
                .WithMany(t => t.UserProfiles)
                .HasForeignKey(d => d.UserId);
            HasRequired(t => t.TelephonyUnit)
                .WithMany(t => t.UserProfiles)
                .HasForeignKey(t => t.TelephonyUnitId);
        }
    }
}