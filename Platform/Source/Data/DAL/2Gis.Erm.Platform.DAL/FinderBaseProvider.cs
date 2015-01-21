using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.DAL
{
    public class FinderBaseProvider : IFinderBaseProvider
    {
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;

        public FinderBaseProvider(
            IFinder finder,
            ISecureFinder secureFinder)
        {
            _finder = finder;
            _secureFinder = secureFinder;
        }

        public IFinderBase GetFinderBase(IEntityType entityName)
        {
            return entityName.IsSecurableAccessRequired() ? (IFinderBase)_secureFinder : _finder;
        } 
    }
}