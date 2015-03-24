using System;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using Moq;

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
        private Mock<IFinder> _unsecureFinder;
        private Mock<ISecureFinder> _secureFinder;

        [SetUp]
        public void Setup()
        {
            _unsecureFinder = new Mock<IFinder>();
            _secureFinder = new Mock<ISecureFinder>();
            _conveter = new ReplicationCodeConverter(_unsecureFinder.Object, _secureFinder.Object);
        }

        [Test]
        public void ShouldThrowExceptionIfNoEntity()
        {
            var exception = Assert.Throws<ArgumentException>(() => _conveter.ConvertToEntityId(EntityName.Task, CrmId));
            Assert.That(exception.Message, Is.StringContaining("impossible to find the object").IgnoreCase);
        }

        [Test]
        public void ShouldThrowExceptionIfNoAccess()
        {
            SetupOne(_unsecureFinder, new Task());

            var exception = Assert.Throws<ArgumentException>(() => _conveter.ConvertToEntityId(EntityName.Task, CrmId));
            Assert.That(exception.Message, Is.StringContaining("user has no rights").IgnoreCase);
        }

        [Test]
        public void ShouldReturnTaskId()
        {
            var task = new Task { Id = ErmId };
            SetupOne(_unsecureFinder, task);
            SetupOne(_secureFinder, task);

            Assert.That(_conveter.ConvertToEntityId(EntityName.Task, CrmId), Is.EqualTo(ErmId));
        }

        [Test]
        public void ShouldThrowExceptionIfNoEntities()
        {
            var exception = Assert.Throws<ArgumentException>(() => _conveter.ConvertToEntityIds(new[] { new CrmEntityInfo { EntityName = EntityName.Task, Id = CrmId } }));
            Assert.That(exception.Message, Is.StringContaining("cannot be converted").IgnoreCase);
        }

        [Test]
        public void ShouldReturnTaskIds()
        {
            var task = new Task { Id = ErmId };
            SetupMany(_unsecureFinder, task);

            Assert.That(_conveter.ConvertToEntityIds(new[] { new CrmEntityInfo { EntityName = EntityName.Task, Id = CrmId } }), Is.EquivalentTo(new[] { ErmId }));
        }

        private static void SetupOne<TEntity>(Mock<IFinder> finder, TEntity entity)
            where TEntity : class, IEntity
        {
            finder.Setup(x => x.FindOne(It.IsAny<IFindSpecification<TEntity>>())).Returns(entity);
        }

        private static void SetupMany<TEntity>(Mock<IFinder> finder, params TEntity[] entities)
            where TEntity : class, IEntity
        {
            finder.Setup(x => x.FindMany(It.IsAny<IFindSpecification<TEntity>>())).Returns(entities);
        }

        private static void SetupOne<TEntity>(Mock<ISecureFinder> finder, TEntity entity)
            where TEntity : class, IEntity, IEntityKey
        {
            finder.Setup(x => x.FindOne(It.IsAny<IFindSpecification<TEntity>>())).Returns(entity);
        }
    }
}