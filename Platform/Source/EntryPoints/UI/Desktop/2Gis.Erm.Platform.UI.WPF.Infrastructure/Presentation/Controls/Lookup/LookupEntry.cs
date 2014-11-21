using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup
{
    public sealed class LookupEntry
    {
        public static LookupEntry FromReference(EntityReference reference)
        {
            if (reference == null)
            {
                return new LookupEntry(0, null);
            }

            return new LookupEntry(reference.Id ?? 0, reference.Name);
        }

        public static EntityReference ToReference(LookupEntry lookupEntry)
        {
            return new EntityReference(lookupEntry != null ? lookupEntry.Key : (long?)null);
        }

        public LookupEntry(long key, string value)
        {
            Key = key;
            Value = value;
        }

        public long Key { get; set; }
        public string Value { get; set; }
    }
}