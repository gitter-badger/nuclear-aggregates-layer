using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class ErmExtractor : IExtractor
    {
        private readonly IFinder _finder;

        public ErmExtractor(IFinder finder)
        {
            if (finder == null)
            {
                throw new ArgumentNullException("finder");
            }

            _finder = finder;
        }

        public void Extract(IDataSource dataSource, IDataConsumer consumer)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }

            var entityLinks = dataSource as EntityLinksDataSource;
            if (entityLinks == null)
            {
                throw new ArgumentException(string.Format("Тип параметра должен быть {0}.", typeof(EntityLinksDataSource)), "dataSource");
            }

            Extract(entityLinks, consumer);
        }

        private void Extract(EntityLinksDataSource entityLinks, IDataConsumer consumer)
        {
            var descriptors = new List<TypedEntitySet>();

            foreach (var item in entityLinks.Links)
            {
                EntityLink reference = item;
                Type entityType = reference.Name.AsEntityType();

                var query = _finder
                    .FindAll(entityType)
                    .Cast<IEntityKey>()
                    .Where(e => e.Id == reference.Id);

                descriptors.Add(new TypedEntitySet(entityType, query));
            }

            var extractedData = new ErmData(descriptors, entityLinks.State);

            consumer.DataExtracted(extractedData);
        }
    }
}