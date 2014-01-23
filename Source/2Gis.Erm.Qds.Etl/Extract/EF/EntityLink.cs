using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    /// <summary>
    /// Ссылка на сущность в БД ERM.
    /// </summary>
    public class EntityLink
    {
        public EntityLink(EntityName name, long id)
        {
            Id = id;
            Name = name;
        }

        public EntityName Name { get; private set; }
        public long Id { get; private set; }
    }
}