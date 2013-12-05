using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace DoubleGis.Erm.Platform.Model.Entities
{
    public static class EntitySetExtension
    {
        public static EntitySet ToEntitySet(this EntityName entityName)
        {
            return new EntitySet(new[] { entityName });
        }

        public static EntitySet ToEntitySet(this EntityName[] entityNames)
        {
            return new EntitySet(entityNames);
        }

        public static EntitySet Merge(this IEnumerable<EntitySet> descriptors)
        {
            return new EntitySet(descriptors.SelectMany(d => d.Entities).Distinct().ToArray());
        }

        /// <summary>
        /// ���������� ��� �� �������� � ��������� ������ ����������, ��� ���� placeholder, 
        /// �.�. fake composite entityname, ������ ������� ����� ����������� ����� ��������  �� ���������� ��������� 
        /// </summary>
        public static bool IsOpenSet(this EntitySet entitySet)
        {
            return entitySet.Entities.Contains(EntitySet.OpenEntitiesSetIndicator);
        }

        /// <summary>
        /// ���������� ������������������ EntitySet, ������� ��������� ��� ��������� ���������, ����� ����������, 
        /// ������� ����� ���� ������������ ������ openset placeholders � �������� EntitySet.
        /// ���������� ��������� ��� ���������, ������� ����� �� ������� open entity set (���������� open -> closed generics)
        /// </summary>
        public static IEnumerable<EntitySet> ToConcreteSets(this EntitySet openEntitySet)
        {
            return new OpenEnitiesSetEnumerator(openEntitySet);
        }
    }
}