using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.Platform.Tests.Unit.Common.Utils
{
    public class CompareObjectsHelperSpecs
    {
        public class Partable : IPartable
        {
            public IEnumerable<IEntityPart> Parts { get; set; }
        }

        public class Part : IEntityPart
        {
            public long Id { get; set; }
            public long CreatedBy { get; set; }
            public DateTime CreatedOn { get; set; }
            public long? ModifiedBy { get; set; }
            public DateTime? ModifiedOn { get; set; }
            public byte[] Timestamp { get; set; }
            public bool IsActive { get; set; }
            public bool IsDeleted { get; set; }
            public long EntityId { get; set; }

            public string PartSpecificField { get; set; }
        }

        [Subject(typeof(CompareObjectsHelper))]
        class When_Cloning_Partable_With_Part
        {
            static Partable partable;
            static Partable clone;

            Establish context = () =>
                                    {
                                        var parts = new[] { 1 }.Select(id => new Part { Id = id, PartSpecificField = "foobar" });
                                        partable = new Partable { Parts = parts };
                                    };
            
            Because of_cloning = () =>
                                     {
                                         clone = CompareObjectsHelper.CreateObjectDeepClone(partable);
                                     };

            It should_clone_part = () => clone.Parts.Count().Should().Be(1);
            It should_clone_part_correctly = () => clone.Parts.Cast<Part>().Single().PartSpecificField.Should().Be(partable.Parts.Cast<Part>().Single().PartSpecificField);
        }
    }
}
