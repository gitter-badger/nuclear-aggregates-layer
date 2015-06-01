using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Platforms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Platforms.DTO;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Platforms
{
    public sealed class PlatformService : IPlatformService
    {
        private readonly IFinder _finder;
        private readonly IRepository<Platform.Model.Entities.Erm.Platform> _platformGenericRepository;

        public PlatformService(IRepository<Platform.Model.Entities.Erm.Platform> platformGenericRepository, IFinder finder)
        {
            _platformGenericRepository = platformGenericRepository;
            _finder = finder;
        }

        public int Delete(Platform.Model.Entities.Erm.Platform platform)
        {
            _platformGenericRepository.Delete(platform);
            return _platformGenericRepository.Save();
        }

        public PlatwormWithPositionsDto GetPlatformWithPositions(long entityId)
        {
            return _finder.Find(Specs.Find.ById<Platform.Model.Entities.Erm.Platform>(entityId))
                          .Map(q => q.Select(platform => new PlatwormWithPositionsDto
                              {
                                  Platform = platform,
                                  Positions = platform.Positions.Where(position => position.IsActive && !position.IsDeleted)
                              }))
                          .One();
        }

        public Platform.Model.Entities.Erm.Platform GetPlatform(long entityId)
        {
            return _finder.Find(Specs.Find.ById<Platform.Model.Entities.Erm.Platform>(entityId)).One();
        }

        public void CreateOrUpdate(Platform.Model.Entities.Erm.Platform platform)
        {
            var isAlreadyExist = _finder.Find(new FindSpecification<Platform.Model.Entities.Erm.Platform>(x => x.Id != platform.Id && (x.Name == platform.Name || x.DgppId == platform.DgppId))).Any();
            if (isAlreadyExist)
            {
                throw new NotificationException(BLResources.PlatfromAlreadyExist);
            }

            if (platform.IsNew())
            {
                _platformGenericRepository.Add(platform);
            }
            else
            {
                _platformGenericRepository.Update(platform);
            }

            _platformGenericRepository.Save();
        }

        public bool IsPlatformLinked(long platformId)
        {
            return _finder.Find(new FindSpecification<Platform.Model.Entities.Erm.Platform>(p => p.Id == platformId && (p.Orders.Any() || p.Orders.Any()))).Any();
        }

        public int Delete(int entityId)
        {
            var platform = _finder.FindObsolete(Specs.Find.ById<Platform.Model.Entities.Erm.Platform>(entityId)).Single();
            return Delete(platform);
        }
    }
}