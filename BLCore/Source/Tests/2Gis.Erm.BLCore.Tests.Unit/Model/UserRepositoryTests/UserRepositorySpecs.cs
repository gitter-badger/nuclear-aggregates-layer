using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NUnit.Framework;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.Model.UserRepositoryTests
{
    [Category("BL")]
    [Subject(typeof(UserRepository))]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It'test, dude.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It'test, dude.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It'test, dude.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It'test, dude.")]
    public class UserRepositoryGetCategoryGroupMembershipSpecs
    {
        abstract class UserRepositoryGetCategoryGroupMembershipContext
        {
            protected const long OrganizationUnitId = 111;

            protected static IEnumerable<CategoryOrganizationUnit> SourceData;
            protected static IEnumerable<CategoryGroupMembershipDto> Result;

            static Mock<IFinder> Finder;
            static UserRepository UserRepository;

            Establish context = () =>
                {
                    Finder = new Mock<IFinder>();

                    Finder.Setup(finder => finder.Find(Moq.It.IsAny<Expression<Func<CategoryOrganizationUnit, bool>>>()))
                       .Returns<Expression<Func<CategoryOrganizationUnit, bool>>>(expression => SourceData.Where(expression.Compile()).AsQueryable());

                    UserRepository = new UserRepository(null, null, null, null, null, null, null, null, null, null, Finder.Object, null, null, null, null, null, null, null, null, null, null, null, null, null);
                };

            Because of = () => Result = UserRepository.GetCategoryGroupMembership(OrganizationUnitId);

            protected static CategoryOrganizationUnit CreateActualCategoryOrganizationUnit(long id, long organizationUnitId)
            {
                return new CategoryOrganizationUnit
                {
                    Id = id,
                    OrganizationUnitId = organizationUnitId,
                    Category = new Category(),
                    IsActive = true,
                    IsDeleted = false
                };
            }
        }

        private class When_there_are_many_organization_units_GetCategoryGroupMembership_method : UserRepositoryGetCategoryGroupMembershipContext
        {
            const long Id_1 = 1;
            const long Id_2 = 2;
            const long OrganizationUnitId_2 = OrganizationUnitId + 1;

            static IEnumerable<CategoryGroupMembershipDto> ExpectedResult;

            Establish context = () =>
                {
                    SourceData = new[]
                        {
                            CreateActualCategoryOrganizationUnit(Id_1, OrganizationUnitId),
                            CreateActualCategoryOrganizationUnit(Id_2, OrganizationUnitId_2)
                        };

                    ExpectedResult = new[]
                        {
                            CreateCategoryGroupMembershipDto(Id_1)
                        };
                };

            It returns_only_current_organization_unit_categories = () => Result.ShouldBeEquivalentTo(ExpectedResult);

            private static CategoryGroupMembershipDto CreateCategoryGroupMembershipDto(long id)
            {
                return new CategoryGroupMembershipDto { Id = id };
            }
        }

        private class GetCategoryGroupMembership_method : UserRepositoryGetCategoryGroupMembershipContext
        {
            const long CategoryId = 1; 
            const int Level = 8;

            Establish context = () =>
                {
                    var item = CreateActualCategoryOrganizationUnit(0, OrganizationUnitId);

                    item.CategoryId = CategoryId;
                    item.Category.Id = CategoryId;
                    item.Category.Level = Level;

                    SourceData = new[] { item };
                };

            It returns_dto_with_right_category_id_value = () => Result.First().CategoryId.Should().Be(CategoryId);

            It returns_dto_with_right_category_level_value = () => Result.First().CategoryLevel.Should().Be(Level);
        }
    }
}
