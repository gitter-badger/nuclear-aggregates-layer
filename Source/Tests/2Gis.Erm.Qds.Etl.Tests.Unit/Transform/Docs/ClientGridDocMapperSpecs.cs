using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.Docs
{
    class ClientGridDocMapperSpecs
    {
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
                _doc = new ClientGridDoc();

                _user = new User
                    {
                        DisplayName = "display Name"
                    };
            };

            Because of = () => Target.UpdateDocByEntity(new[] { _doc }, _user);

            It should_map_user_fields = () =>
            {
                _doc.ShouldBeEquivalentTo(_user, options => options
                        .ExcludingMissingProperties());

                _doc.OwnerName.Should().Be(_user.DisplayName);
            };

            private static ClientGridDoc _doc;
            private static User _user;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_territory : ClientGridDocMapperContext
        {
            Establish context = () =>
            {
                _doc = new ClientGridDoc();

                _territory = new Territory
                    {
                        Name = "Some name",
                    };
            };

            Because of = () => Target.UpdateDocByEntity(new[] { _doc }, _territory);

            It should_map_territory_fields = () => _doc.TerritoryName.Should().Be(_territory.Name);

            private static ClientGridDoc _doc;
            private static Territory _territory;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_сlient_with_changed_territory_id : ClientGridDocMapperContext
        {
            Establish context = () =>
            {
                _doc = new ClientGridDoc();

                _client = new Client
                {
                    TerritoryId = 42,
                };

                _expectedTerritoryName = "Some territory name";

                DocsCatalog.Setup(c => c.FindDocsByIndirectRelationPart<TerritoryDoc>(_client))
                          .Returns(new[] { new TerritoryDoc { Name = _expectedTerritoryName } });

                //DocsCatalog.Setup(c => c.FindDocsByRelatedPart<TerritoryDoc>(_client))
                //         .Returns(new[] { new TerritoryDoc { Name = _expectedTerritoryName } });
            };

            Because of = () => Target.UpdateDocByEntity(new[] { _doc }, _client);

            It should_update_territory_from_docs_catalog = () => _doc.TerritoryName.Should().Be(_expectedTerritoryName);
            It should_update_territory_id = () => _doc.TerritoryId.Should().Be(_client.TerritoryId);

            static ClientGridDoc _doc;
            static Client _client;
            static string _expectedTerritoryName;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_сlient_with_changed_owner_code : ClientGridDocMapperContext
        {
            Establish context = () =>
            {
                _doc = new ClientGridDoc();

                _client = new Client
                {
                    OwnerCode = 42,
                };

                _expectedOwnerName = "Owner name";

                DocsCatalog.Setup(c => c.FindDocsByIndirectRelationPart<UserDoc>(_client)).Returns(new[] { new UserDoc { Name = _expectedOwnerName }, });
                //DocsCatalog.Setup(c => c.FindDocsByRelatedPart<UserDoc>(_client)).Returns(new[] { new UserDoc { Name = _expectedOwnerName }, });
            };

            Because of = () => Target.UpdateDocByEntity(new[] { _doc }, _client);

            It should_update_owner_name_from_docs_catalog = () => _doc.OwnerName.Should().Be(_expectedOwnerName);
            It should_update_owner_code_from = () => _doc.OwnerCode.Should().Be(_client.OwnerCode);

            static ClientGridDoc _doc;
            static Client _client;
            static string _expectedOwnerName;
        }

        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_client : ClientGridDocMapperContext
        {
            Establish context = () =>
                {
                    _doc = new ClientGridDoc();

                    _client = new Client
                        {
                            Id = 42,
                            ReplicationCode = Guid.NewGuid(),
                            Name = "User name",
                            MainAddress = "address",
                            IsActive = true,
                            IsDeleted = false,
                            CreatedOn = DateTime.Now,
                            InformationSource = 2
                        };

                    _localizedInformationSource = "Локализованный инф.сорс";

                    EnumLocalizer.Setup(el => el.GetLocalizedString((InformationSource)_client.InformationSource)).Returns(_localizedInformationSource);
                };

            Because of = () => Target.UpdateDocByEntity(new[] { _doc }, _client);

            It should_map_fields = () =>
                {
                    _doc.ShouldBeEquivalentTo(_client, options => options
                        .ExcludingMissingProperties()
                        .Excluding(d => d.ReplicationCode)
                        .Excluding(d => d.InformationSource));

                    _doc.Id.Should().Be(_client.Id);
                    _doc.ReplicationCode.Should().Be(_client.ReplicationCode.ToString());
                    _doc.InformationSource.Should().Be(_localizedInformationSource);
                };

            private static ClientGridDoc _doc;
            private static Client _client;
            private static string _localizedInformationSource;
        }

        class ClientGridDocMapperContext
        {
            Establish context = () =>
                {
                    EnumLocalizer = new Mock<IEnumLocalizer>();
                    DocsCatalog = new Mock<IRelationalDocsFinder>();

                    Target = new ClientGridDocMapper(EnumLocalizer.Object, DocsCatalog.Object);
                };

            protected static Mock<IRelationalDocsFinder> DocsCatalog { get; private set; }
            protected static Mock<IEnumLocalizer> EnumLocalizer { get; private set; }
            protected static ClientGridDocMapper Target { get; private set; }
        }
    }
}