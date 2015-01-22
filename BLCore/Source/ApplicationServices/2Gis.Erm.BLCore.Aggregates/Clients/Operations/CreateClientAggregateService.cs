using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.Operations
{
    public sealed class CreateClientAggregateService : ICreateClientAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Client> _clientGenericSecureRepository;

        public CreateClientAggregateService(IIdentityProvider identityProvider,
                                            IFirmReadModel firmReadModel,
                                            IOperationScopeFactory operationScopeFactory,
                                            ISecureRepository<Client> clientGenericSecureRepository)
        {
            _identityProvider = identityProvider;
            _firmReadModel = firmReadModel;
            _operationScopeFactory = operationScopeFactory;
            _clientGenericSecureRepository = clientGenericSecureRepository;
        }

        public int Create(Client client, out FirmAddress mainFirmAddress)
        {
            mainFirmAddress = null;

            if (client.MainFirmId != null)
            {
                var firmAddresses = _firmReadModel.GetFirmAddressesByFirm(client.MainFirmId.Value);

                foreach (var firmAddress in firmAddresses)
                {
                    var firmContacts = _firmReadModel.GetContacts(firmAddress.Id);

                    var contacts = firmContacts as FirmContact[] ?? firmContacts.ToArray();
                    if (contacts.Any())
                    {
                        if (string.IsNullOrEmpty(client.MainAddress))
                        {
                            client.MainAddress = string.Concat(firmAddress.Address,
                                                               firmAddress.ReferencePoint != null
                                                                   ? string.Format(" — {0}", firmAddress.ReferencePoint)
                                                                   : string.Empty);
                            mainFirmAddress = firmAddress;
                        }

                        FillClientPropertiesFromFirmContacts(contacts, client);
                        break;
                    }
                }
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Client>())
            {
                _identityProvider.SetFor(client);
                _clientGenericSecureRepository.Add(client);
                var result = _clientGenericSecureRepository.Save();

                operationScope.Added<Client>(client.Id)
                              .Complete();

                return result;
            }
        }

        private static void FillClientPropertiesFromFirmContacts(IEnumerable<FirmContact> firmContacts, Client client)
        {
            foreach (var firmContact in firmContacts)
            {
                switch (firmContact.ContactType)
                {
                    case FirmAddressContactType.Phone:
                    {
                        if (string.IsNullOrEmpty(client.MainPhoneNumber))
                        {
                            // MainPhoneNumber
                            client.MainPhoneNumber = firmContact.Contact;
                        }
                        else if (string.IsNullOrEmpty(client.AdditionalPhoneNumber1))
                        {
                            // AdditionalPhoneNumber1
                            client.AdditionalPhoneNumber1 = firmContact.Contact;
                        }
                        else if (string.IsNullOrEmpty(client.AdditionalPhoneNumber2))
                        {
                            // AdditionalPhoneNumber2
                            client.AdditionalPhoneNumber2 = firmContact.Contact;
                        }
                    }

                        break;
                    case FirmAddressContactType.Fax:
                        if (string.IsNullOrEmpty(client.Fax))
                        {
                            // Fax
                            client.Fax = firmContact.Contact;
                        }

                        break;
                    case FirmAddressContactType.Email:
                        if (string.IsNullOrEmpty(client.Email))
                        {
                            // Email
                            client.Email = firmContact.Contact;
                        }

                        break;
                    case FirmAddressContactType.Website:
                        if (string.IsNullOrEmpty(client.Website))
                        {
                            // Website
                            client.Website = firmContact.Contact;
                        }

                        break;
                }
            }
        }
    }
}
