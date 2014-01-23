using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.Docs
{
    class UserDocMapperSpecs
    {
        [Subject(typeof(UserDocMapper))]
        class When_update_user_doc_by_not_supported_entity : UserDocUpdaterContext
        {
            Because of = () => Result = Catch.Exception(() => Target.UpdateDocByEntity(new[] { new UserDoc() }, Mock.Of<IEntityKey>()));

            It should_throw_not_supported_exception = () => Result.Should().BeOfType<NotSupportedException>();

            static Exception Result;
        }

        [Subject(typeof(UserDocMapper))]
        class When_update_user_doc_by_user_entity : UserDocUpdaterContext
        {
            Establish context = () =>
                {
                    _userEntity = new User
                        {
                            DisplayName = "Display Name"
                        };

                    _userDoc = new UserDoc();
                };

            Because of = () => Target.UpdateDocByEntity(new[] { _userDoc }, _userEntity);

            It should_map_user_entity_fields = () => _userDoc.Name.Should().Be(_userEntity.DisplayName);

            static UserDoc _userDoc;
            static User _userEntity;
        }

        class UserDocUpdaterContext
        {
            Establish context = () =>
                {
                    Target = new UserDocMapper();
                };

            protected static UserDocMapper Target { get; private set; }
        }
    }
}