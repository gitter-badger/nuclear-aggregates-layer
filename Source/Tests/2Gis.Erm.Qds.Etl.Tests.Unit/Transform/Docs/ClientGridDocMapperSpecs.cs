using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.Docs
{
    // FIXME {f.zaharov, 4.06.2014}: copypaste подчистить
    class ClientGridDocMapperSpecs
    {
        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_legal_person_with_new_parent_client_id : ClientGridDocMapperContext
        {
            Establish context = () =>
                {
                    const long legalPersonId = 42;
                    const long newParentId = 62;

                    ChangedLegalPersonEntity = new LegalPerson
                        {
                            Id = legalPersonId,
                            ClientId = newParentId,
                            IsActive = true,
                            IsDeleted = true,
                            Accounts = new[]
                            {
                                new Account
                                {
                                    Id = 111,
                                    LegalPersonId = newParentId,
                                    Balance = 333.33m,
                                    IsActive = true,
                                    IsDeleted = true
                                }
                            }
                        };

                    OldClientGridDoc = new ClientGridDoc
                        {
                            Id = "55",
                            LegalPersons = new[] { new LegalPersonDoc { Id = legalPersonId.ToString() } }
                        };

                    NewClientGridDoc = new ClientGridDoc
                        {
                            Id = newParentId.ToString(),
                        };
                };

            Because of = () => Target.UpdateDocByEntity(new[] { OldClientGridDoc, NewClientGridDoc }, ChangedLegalPersonEntity);

            It should_remove_account_from_old_legal_person = () => OldClientGridDoc.LegalPersons.Should().BeEmpty();
            It should_add_account_to_new_legal_person = () => NewClientGridDoc.LegalPersons.Single().ShouldBeEquivalentTo(ChangedLegalPersonEntity);

            static LegalPerson ChangedLegalPersonEntity;
            static ClientGridDoc OldClientGridDoc;
            static ClientGridDoc NewClientGridDoc;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_account_with_new_parent_legal_person_id : ClientGridDocMapperContext
        {
            Establish context = () =>
                {
                    const long accountId = 42;
                    const long newParentId = 62;

                    ChangedAccountEntity = new Account
                        {
                            Id = accountId,
                            LegalPersonId = newParentId,
                            Balance = 333.33m,
                            IsActive = true,
                            IsDeleted = true
                        };

                    OldLegalPerson = new LegalPersonDoc
                        {
                            Id = "55",
                            Accounts = new[] { new AccountDoc { Id = accountId.ToString() } }
                        };

                    NewlegalPersonDoc = new LegalPersonDoc
                        {
                            Id = newParentId.ToString(),
                        };

                    Document.LegalPersons = new[] { OldLegalPerson, NewlegalPersonDoc };
                };

            Because of = () => Target.UpdateDocByEntity(new[] { Document }, ChangedAccountEntity);

            It should_remove_account_from_old_legal_person = () => OldLegalPerson.Accounts.Should().BeEmpty();
            It should_add_account_to_new_legal_person = () => NewlegalPersonDoc.Accounts.Single().ShouldBeEquivalentTo(ChangedAccountEntity);

            static Account ChangedAccountEntity;
            static LegalPersonDoc OldLegalPerson;
            static LegalPersonDoc NewlegalPersonDoc;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_account : ClientGridDocMapperContext
        {
            Establish context = () =>
                {
                    const long accountId = 42;
                    const long legalPersonId = 52;

                    ChangedAccountEntity = new Account { Id = accountId, LegalPersonId = legalPersonId, Balance = 123.4m, IsActive = true, IsDeleted = true };
                    NestedAccountDoc = new AccountDoc { Id = accountId.ToString() };

                    Document.LegalPersons = new[]{ new LegalPersonDoc
                                {
                                    Id=legalPersonId.ToString(),
                                    Accounts = new []{ NestedAccountDoc }
                                }
                        };
                };

            Because of = () => Target.UpdateDocByEntity(new[] { Document }, ChangedAccountEntity);

            It should_updated_nested_document = () => NestedAccountDoc.ShouldBeEquivalentTo(ChangedAccountEntity);

            static Account ChangedAccountEntity;
            static AccountDoc NestedAccountDoc;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_unsupported_entity_type : ClientGridDocMapperContext
        {
            Because of = () => Result = Catch.Exception(() => Target.UpdateDocByEntity(new[] { new ClientGridDoc() }, Mock.Of<IEntityKey>()));

            It should_throw_not_supported_excpetion = () => Result.Should().BeOfType<NotSupportedException>();

            static Exception Result;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_user : ClientGridDocMapperContext
        {
            Establish context = () =>
            {
                User = new User
                    {
                        DisplayName = "display Name"
                    };
            };

            Because of = () => Target.UpdateDocByEntity(new[] { Document }, User);

            It should_map_user_fields = () =>
            {
                Document.ShouldBeEquivalentTo(User, options => options
                        .ExcludingMissingProperties()
                        .Excluding(x => x.Id));

                Document.OwnerName.Should().Be(User.DisplayName);
            };

            private static User User;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_territory : ClientGridDocMapperContext
        {
            Establish context = () =>
            {
                Territory = new Territory
                    {
                        Name = "Some name",
                    };
            };

            Because of = () => Target.UpdateDocByEntity(new[] { Document }, Territory);

            It should_map_territory_fields = () => Document.TerritoryName.Should().Be(Territory.Name);

            private static Territory Territory;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_сlient_with_changed_territory_id : ClientGridDocMapperContext
        {
            Establish context = () =>
            {
                Client = new Client
                {
                    TerritoryId = 42,
                };

                _expectedTerritoryName = "Some territory name";

                var clientGridDocRelation = new Mock<IDocumentRelation<ClientGridDoc>>();
                clientGridDocRelation.Setup(x => x.OnDocumentUpdated(Moq.It.IsAny<ClientGridDoc>())).Callback<ClientGridDoc>(x => x.TerritoryName = _expectedTerritoryName);
                Relations.Add(clientGridDocRelation.Object);
            };

            Because of = () => Target.UpdateDocByEntity(new[] { Document }, Client);

            It should_update_territory_from_docs_catalog = () => Document.TerritoryName.Should().Be(_expectedTerritoryName);
            It should_update_territory_id = () => Document.TerritoryId.Should().Be(Client.TerritoryId.ToString());

            static Client Client;
            static string _expectedTerritoryName;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_сlient_with_changed_owner_code : ClientGridDocMapperContext
        {
            Establish context = () =>
            {
                Сlient = new Client
                {
                    OwnerCode = 42,
                };

                _expectedOwnerName = "Owner name";

                var clientGridDocRelation = new Mock<IDocumentRelation<ClientGridDoc>>();
                clientGridDocRelation.Setup(x => x.OnDocumentUpdated(Moq.It.IsAny<ClientGridDoc>())).Callback<ClientGridDoc>(x => x.OwnerName = _expectedOwnerName);
                Relations.Add(clientGridDocRelation.Object);
            };

            Because of = () => Target.UpdateDocByEntity(new[] { Document }, Сlient);

            It should_update_owner_name_from_docs_catalog = () => Document.OwnerName.Should().Be(_expectedOwnerName);
            It should_update_owner_code_from = () => Document.OwnerCode.Should().Be(Сlient.OwnerCode.ToString());

            static Client Сlient;
            static string _expectedOwnerName;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_client : ClientGridDocMapperContext
        {
            Establish context = () =>
                {
                    Client = new Client
                        {
                            Id = 42,
                            ReplicationCode = Guid.NewGuid(),
                            Name = "User name",
                            MainAddress = "address",
                            IsActive = true,
                            MainFirmId = 35,
                            IsDeleted = false,
                            CreatedOn = DateTime.Now,
                            InformationSource = 2
                        };
                };

            Because of = () => Target.UpdateDocByEntity(new[] { Document }, Client);

            It should_map_fields = () =>
                {
                    Document.ShouldBeEquivalentTo(Client,
                                              options => options
                                                             .ExcludingMissingProperties()
                                                             .Excluding(d => d.ReplicationCode)
                                                             .Excluding(d => d.LegalPersons));

                    Document.ReplicationCode.Should().Be(Client.ReplicationCode.ToString());
                };

            private static Client Client;
        }

        class ClientGridDocMapperContext
        {
            Establish context = () =>
                {
                    Document = new ClientGridDoc();
                    DocumentRelationsRegistry = new Mock<IDocumentRelationsRegistry>();

                    IEnumerable<IDocumentRelation<ClientGridDoc>> relations = Relations;
                    DocumentRelationsRegistry.Setup(x => x.TryGetDocumentRelations(out relations)).Returns(true);

                    Target = new ClientGridDocMapper(DocumentRelationsRegistry.Object);
                };

            protected static ClientGridDoc Document;
            protected static readonly List<IDocumentRelation<ClientGridDoc>> Relations = new List<IDocumentRelation<ClientGridDoc>>();
            protected static Mock<IDocumentRelationsRegistry> DocumentRelationsRegistry { get; private set; }
            protected static ClientGridDocMapper Target { get; private set; }
        }
    }
}