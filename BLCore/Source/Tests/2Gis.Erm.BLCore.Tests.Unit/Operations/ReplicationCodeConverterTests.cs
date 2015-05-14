using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using Moq;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

using NUnit.Framework;

namespace DoubleGis.Erm.BLCore.Tests.Unit.Operations
{
    [TestFixture]
    [SetCulture("en-US")]
    public class ReplicationCodeConverterTests
    {
        private static readonly Guid CrmId = Guid.Empty;
        private const long ErmId = 1;

        private IReplicationCodeConverter _conveter;
        private Mock<IQuery> _unsecureQuery;
        private Mock<ISecureQuery> _secureQuery;

        [SetUp]
        public void Setup()
        {
            _unsecureQuery = new Mock<IQuery>();
            _secureQuery = new Mock<ISecureQuery>();
            _conveter = new ReplicationCodeConverter(_unsecureQuery.Object, _secureQuery.Object);
        }

        [Test]
        public void ShouldThrowExceptionIfNoEntity()
        {
            var exception = Assert.Throws<ArgumentException>(() => _conveter.ConvertToEntityId(EntityType.Instance.Task(), CrmId));
            Assert.That(exception.Message, Is.StringContaining("impossible to find the object").IgnoreCase);
        }

        [Test]
        public void ShouldThrowExceptionIfNoAccess()
        {
            SetupOne(_unsecureQuery, new Task());

            var exception = Assert.Throws<ArgumentException>(() => _conveter.ConvertToEntityId(EntityType.Instance.Task(), CrmId));
            Assert.That(exception.Message, Is.StringContaining("user has no rights").IgnoreCase);
        }

        [Test]
        public void ShouldReturnTaskId()
        {
            var task = new Task { Id = ErmId };
            SetupOne(_unsecureQuery, task);
            SetupOne(_secureQuery, task);

            Assert.That(_conveter.ConvertToEntityId(EntityType.Instance.Task(), CrmId), Is.EqualTo(ErmId));
        }

        [Test]
        public void ShouldThrowExceptionIfNoEntities()
        {
            var exception = Assert.Throws<ArgumentException>(() => _conveter.ConvertToEntityIds(new[] { new CrmEntityInfo { EntityName = EntityType.Instance.Task(), Id = CrmId } }));
            Assert.That(exception.Message, Is.StringContaining("cannot be converted").IgnoreCase);
        }

        [Test]
        public void ShouldReturnTaskIds()
        {
            var task = new Task { Id = ErmId };
            SetupMany(_unsecureQuery, task);

            Assert.That(_conveter.ConvertToEntityIds(new[] { new CrmEntityInfo { EntityName = EntityType.Instance.Task(), Id = CrmId } }), Is.EquivalentTo(new[] { ErmId }));
        }

        private static void SetupOne<TEntity>(Mock<IQuery> finder, TEntity entity)
            where TEntity : class, IEntity
        {
            finder.Setup(x => x.For<TEntity>()).Returns(new[] { entity }.AsQueryable());
        }

        private static void SetupMany<TEntity>(Mock<IQuery> finder, params TEntity[] entities)
            where TEntity : class, IEntity
        {
            finder.Setup(x => x.For<TEntity>()).Returns(entities.AsQueryable());
        }

        private static void SetupOne<TEntity>(Mock<ISecureQuery> finder, TEntity entity)
            where TEntity : class, IEntity, IEntityKey
        {
            finder.Setup(x => x.For<TEntity>()).Returns(new[] { entity }.AsQueryable());
        }
    }
}