namespace DoubleGis.Erm.Platform.Migration.Sql
{
    public class SchemaQualifiedObjectName
    {
        public SchemaQualifiedObjectName(string schema, string name)
        {
            Name = name;
            Schema = schema;
        }

        public SchemaQualifiedObjectName(string name) : this(null, name)
        {
        }

        /// <summary>
        /// Название объекта
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Название схемы БД
        /// </summary>
        public string Schema { get; private set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Schema) ? Name : string.Format("{0}.{1}", Schema, Name);
        }
    }
}
