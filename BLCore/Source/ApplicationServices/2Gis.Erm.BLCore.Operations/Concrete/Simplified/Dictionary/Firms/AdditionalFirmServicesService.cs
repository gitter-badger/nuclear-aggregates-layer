using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Firms;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Firms
{
    public sealed class AdditionalFirmServicesService : IAdditionalFirmServicesService
    {
        private readonly IRepository<AdditionalFirmService> _genericRepository;
        private readonly IFinder _finder;
        private readonly IIdentityProvider _identityProvider;

        public AdditionalFirmServicesService(IRepository<AdditionalFirmService> repository, IFinder finder, IIdentityProvider identityProvider)
        {
            _genericRepository = repository;
            _finder = finder;
            _identityProvider = identityProvider;
        }

        public void CreateOrUpdate(AdditionalFirmService entity)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var existing = _finder.Find(Specs.Find.ById<AdditionalFirmService>(entity.Id)).SingleOrDefault();

                if (existing == null)
                {
                    _identityProvider.SetFor(entity);
                    _genericRepository.Add(entity);
                }
                else
                {
                    existing.IsManaged = entity.IsManaged;
                    existing.ServiceCode = entity.ServiceCode;
                    existing.Description = entity.Description;

                    _genericRepository.Update(existing);
                }

                _genericRepository.Save();
                transaction.Complete();
            }
        }

        public void Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<AdditionalFirmService>(entityId)).Single();
            Delete(entity);
        }

        public void Delete(AdditionalFirmService firmService)
        {
            var usageCount = _finder.Find<FirmAddressService>(service => service.AdditionalFirmService.Id == firmService.Id).Count();

            if (usageCount > 0)
            {
                throw new NotificationException(string.Format(BLResources.CannotDeleteAdditionalFirmService, firmService.ServiceCode, usageCount));
            }

            _genericRepository.Delete(firmService);
            _genericRepository.Save();
        }
    }
}
