using System;

namespace DoubleGis.Erm.Platform.Migration.Core
{
    /// <summary>
    /// Номер миграции - время в локальном часовом поясе в формате yyyyMMddhhmm
    /// powershell snippet: [System.DateTime]::Now.ToString("yyyyMMddhhmm")
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class MigrationAttribute : Attribute
    {
        [Obsolete("Теперь нужно использовать конструктор с указанием авторства миграции")]
        public MigrationAttribute(long version, string description = "")
            : this(version, description, "unknown")
        {
        }

        public MigrationAttribute(long version, string description, string author)
        {
            Version = version;
            Description = description;
            Author = author;
        }

        public long Version { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
    }
}