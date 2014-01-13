using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories
{
    public sealed class CategoryImportContext
    {
        private readonly IDictionary<int, IList<long>> _affectedCategories;
        private readonly IDictionary<long, long> _dgppToErmCategoryCache;
        private readonly IDictionary<int, long> _dgppToErmOrganizationUnitCache;

        public CategoryImportContext()
        {
            _affectedCategories = new Dictionary<int, IList<long>>();
            _dgppToErmCategoryCache = new Dictionary<long, long>();
            _dgppToErmOrganizationUnitCache = new Dictionary<int, long>();
        }

        public void PutAffectedCategory(int level, long ermId)
        {
            var list = default(IList<long>);
            if (!_affectedCategories.TryGetValue(level, out list) || list == null)
                _affectedCategories.Add(level, list = new List<long>());
            list.Add(ermId);
        }

        public IEnumerable<long> GetAffectedCategories(int level)
        {
            var list = default(IList<long>);
            _affectedCategories.TryGetValue(level, out list);
            return list != null ? list.Distinct() : new List<long>();
        }

        public void CacheDgppToErmCategory(long dgppCategoryId, int ermCategoryId)
        {
            _dgppToErmCategoryCache[dgppCategoryId] = ermCategoryId;
        }

        public bool TryResolveDgppCategory(long dgppCategoryId, out long ermCategoryId)
        {
            return _dgppToErmCategoryCache.TryGetValue(dgppCategoryId, out ermCategoryId);
        }

        public void CacheOrganizationUnitDgppToErmId(int dgppId, long ermId)
        {
            _dgppToErmOrganizationUnitCache[dgppId] = ermId;
        }

        public bool TryResolveOrganizationUnitDgppId(int dgppId, out long ermId)
        {
            return _dgppToErmOrganizationUnitCache.TryGetValue(dgppId, out ermId);
        }
    }
}