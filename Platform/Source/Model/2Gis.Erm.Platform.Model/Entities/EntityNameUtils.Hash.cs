namespace DoubleGis.Erm.Platform.Model.Entities
{
    public static partial class EntityNameUtils
    {
        public static int EvaluateHashSimplified(this EntityName[] entities)
        {
            return EvaluateEntitiesHashSimplified(entities);
        }

        public static int EvaluateEntitiesHashSimplified(params EntityName[] entities)
        {
            const int Multipler = 0x1000193;
            int hash = 0x7b26bcc5;
            hash = (hash ^ entities.Length) * Multipler;
            hash = (hash ^ (int)entities[0]) * Multipler;
            hash = (hash ^ (int)entities[entities.Length - 1]) * Multipler;
            hash += hash << 13;
            hash ^= hash >> 7;
            hash += hash << 3;
            hash ^= hash >> 17;
            hash += hash << 5;

            return hash;
        }

        public static int EvaluateHash(this EntityName[] entities)
        {
            return EvaluateEntitiesHash(entities);
        }

        public static int EvaluateEntitiesHash(params EntityName[] entities)
        {
            // http://en.wikipedia.org/wiki/Jenkins_hash_function
            uint hash = 0;
            for (int index = 0; index < entities.Length; index++)
            {
                var entityName = entities[index];
                hash += (uint)entityName;
                hash += hash << 10;
                hash ^= hash >> 6;
            }

            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;

            return (int)hash;
        }
    }
}
