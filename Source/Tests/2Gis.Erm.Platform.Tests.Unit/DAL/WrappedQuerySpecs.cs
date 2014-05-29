using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EAV;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public static class WrappedQuerySpecs
    {
        [Tags("DAL")]
        [Subject(typeof(WrappedQuery))]
        public class When_calling_expression
        {
            static IQueryable<IEntity> Queryable;
            Establish context = () => Queryable = new IEntity[0].AsQueryable().ValidateQueryCorrectness();
            Because of = () => Queryable = Queryable.Where(x => true);
            It shoud_be_still_unparted = () => Queryable.Should().BeOfType<WrappedQuery<IEntity>>();
        }

        [Tags("DAL")]
        [Subject(typeof(WrappedQuery))]
        public class When_requesting_partable
        {
            static IQueryable<Entity> Queryable;
            Establish context = () => Queryable = new Entity[0].AsQueryable().ValidateQueryCorrectness();

            It shoud_not_allow_return_partable =
                () => Queryable.Invoking(q => q.Select(x => x.Partable.Entity).Select(x => new { x.Partable }).FirstOrDefault()).ShouldThrow<ArgumentException>();
            It shoud_not_allow_return_partable_as_select_many =
                () => Queryable.Invoking(q => q.SelectMany(x => x.Partables).FirstOrDefault()).ShouldThrow<ArgumentException>();
            It shoud_not_allow_return_partable_in_complex_type =
                () => Queryable.Invoking(q => q.Where(x => true).Select(x => x.Partable.Entity).Select(x => new { x, x.Partable }).FirstOrDefault()).ShouldThrow<ArgumentException>();
        }

        [Tags("DAL")]
        [Subject(typeof(WrappedQuery))]
        public class When_requesting_parts
        {
            static IQueryable<Entity> Queryable;
            Establish context = () => Queryable = new Entity[0].AsQueryable().ValidateQueryCorrectness();

            It shoud_not_allow_return_parts =
                () => Queryable.Invoking(q => q.Select(x => x.Partable.Parts).FirstOrDefault()).ShouldThrow<ArgumentException>();
            It shoud_not_allow_return_parts_as_select_many =
                () => Queryable.Invoking(q => q.SelectMany(x => x.Partable.Parts).FirstOrDefault()).ShouldThrow<ArgumentException>();
            It shoud_not_allow_return_parts_in_complex_type =
                () => Queryable.Invoking(q => q.Select(x => new { x, x.Partable.Parts }).FirstOrDefault()).ShouldThrow<ArgumentException>();
            It shoud_not_allow_inderect_access_to_parts =
                () => Queryable.Invoking(q => q.Select(x => x.Partable.Parts.Count()).Count(x => x > 0)).ShouldThrow<ArgumentException>();
        }

        [Tags("DAL")]
        [Subject(typeof(WrappedQuery))]
        public class When_only_using_partable
        {
            static IQueryable<Entity> Queryable;
            Establish context = () => Queryable = new Entity[0].AsQueryable().ValidateQueryCorrectness();

            It shoud_allow_return_field_of_partable =
                () => Queryable.Invoking(q => q.Select(x => x.Partable.Id).FirstOrDefault()).ShouldNotThrow();
            It shoud_allow_use_partable_in_intermediate_query =
                () => Queryable.Invoking(q => q.Select(x => x.Partable).Select(p => p.Id).FirstOrDefault()).ShouldNotThrow();
            It shoud_allow_return_not_partable =
                () => Queryable.Invoking(q => q.Select(x => x).FirstOrDefault()).ShouldNotThrow();
            It should_not_allow_select_from_parts =
                () => Queryable.Invoking(q => q.Select(e => e.Partable.Parts.Count()).Select(x => new { x }).FirstOrDefault()).ShouldThrow<ArgumentException>();

            It should_not_allow_select_partables =
                () => Queryable.Invoking(q => q.Select(entity => new { DtoField = entity.Partables.Select(p => p) }).FirstOrDefault()).ShouldThrow<ArgumentException>();
            
            // Защита давала ложные срабатывания: https://jira.2gis.ru/browse/ERM-4077
            It should_allow_select_from_partable =
                () => Queryable.Invoking(q => q.Select(x => x).Select(entity => new 
                {
                    DtoField = entity.Partables.Take(5).Select(p => new { ABC = p.Id }).Select(p => p.ABC).First(x => x == 1),
                    Dtos = entity.Partables.First().Id + entity.Partables.Count(),
                    Id = entity.Partable.Id,
                    Value = entity.Partables.First().Entity.Value1,
                    Value111 = new 
                        {
                            Entity = entity.Partable.Entity.Value2
                        }
                }).FirstOrDefault()).ShouldNotThrow();
        }

        [Tags("DAL")]
        [Subject(typeof(WrappedQuery))]
        public class When_using_any_statement
        {
            static IQueryable<Entity> Queryable;
            Establish context = () => Queryable = new Entity[0].AsQueryable().ValidateQueryCorrectness();

            It shoud_allow_use_partables_in_conjunction_with_statement_any_1 =
                () => Queryable.Invoking(q => q.Select(x => x.Partable).Any(x => x.Id == 0)).ShouldNotThrow();
            It shoud_allow_use_partables_in_conjunction_with_statement_any_2 =
                () => Queryable.Invoking(q => q.Select(x => x.Partable).Any()).ShouldNotThrow();
            It shoud_not_allow_use_parts_in_statement_any_1 =
                () => Queryable.Invoking(q => q.Select(x => x.Partable).Any(x => x.Parts.Count() == 1)).ShouldThrow<ArgumentException>();
            It shoud_not_allow_use_parts_in_statement_any_2 =
                () => Queryable.Invoking(q => q.Select(x => x.Partable.Parts).Any(x => x.Count() == 1)).ShouldThrow<ArgumentException>();
            It shoud_not_allow_use_parts_in_statement_any_3 =
                () => Queryable.Invoking(q => q.Select(x => x.Partable.Parts.Count()).Any(x => x == 1)).ShouldThrow<ArgumentException>();
        }

        class Entity : IEntity
        {
            public long Id { get; set; }
            public string Value1 { get; set; }
            public string Value2 { get; set; }
            public string Value3 { get; set; }
            public string Value4 { get; set; }
            public PartableEntity Partable { get; set; }
            public IEnumerable<PartableEntity> Partables { get; set; }
        }

        class PartableEntity : IEntity, IPartable
        {
            public long Id { get; set; }
            public Entity Entity { get; set; }
            public IEnumerable<IEntityPart> Parts { get; set; }
        }
    }
}
