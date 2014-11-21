using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Model.Entities
{
    public sealed class OpenEnitiesSetEnumerator : IEnumerable<EntitySet>
    {
        private readonly EntityName[] _allEntities = EntityName.All
                                                               .GetDecomposed()
                                                               .Where(e => !e.IsVirtual())
                                                               .ToArray();
        private readonly EntityName[] _currentState;
        private readonly int[] _placeholdersIndexes;
        private readonly int[] _placeholdersState;

        public OpenEnitiesSetEnumerator(EntitySet sourceEntitySet)
        {
            if (!sourceEntitySet.Entities.Any() 
                || !sourceEntitySet.IsOpenSet() 
                || sourceEntitySet.Entities.Contains(EntitySet.EmptySetIndicator))
            {
                throw new ArgumentException("Specified entities set must  be not empty open set");
            }

            _currentState = new EntityName[sourceEntitySet.Entities.Length];

            var placeholdersIndexes = new List<int>();

            for (int index = 0; index < sourceEntitySet.Entities.Length; index++)
            {
                var entityName = sourceEntitySet.Entities[index];
                if (entityName != EntitySet.OpenEntitiesSetIndicator)
                {
                    _currentState[index] = entityName;
                }
                else
                {
                    placeholdersIndexes.Add(index);
                }
            }

            _placeholdersIndexes = placeholdersIndexes.ToArray();
            _placeholdersState = new int[_placeholdersIndexes.Length];
        }

        public IEnumerator<EntitySet> GetEnumerator()
        {
            InitializeState();
            yield return _currentState.ToArray().ToEntitySet();

            while (TryEvaluateNextState())
            {
                yield return _currentState.ToArray().ToEntitySet();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void InitializeState()
        {
            var firstValue = _allEntities.First();
            foreach (var placeholdersIndex in _placeholdersIndexes)
            {
                _currentState[placeholdersIndex] = firstValue;
            }
        }

        private bool TryEvaluateNextState()
        {
            bool shift = true;
            for (int i = 0; shift && i < _placeholdersState.Length; i++)
            {
                var current = _placeholdersState[i] + 1;
                if (current == _allEntities.Length)
                {
                    if (i == _placeholdersState.Length - 1)
                    {
                        return false;
                    }

                    current = 0;
                }
                else
                {
                    shift = false;
                }

                _currentState[_placeholdersIndexes[i]] = _allEntities[current];
                _placeholdersState[i] = current;
            }

            return !shift;
        }
    }
}