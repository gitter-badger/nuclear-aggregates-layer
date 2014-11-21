using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Core.Operations.Processing.Final.MsCRM;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.Platform.Tests.Unit.Core.Operations.Processing.Final
{
    [Subject(typeof(ReplicateToCRMMessageAggregatedProcessingResultHandler.Slicer<>))]
    class ReplicateToCRMMessageAggregatedProcessingResultHandlerSlicerSpecs
    {
        class SlicerContext
        {
            protected const int DefaultSequenceFactor = 2;
            
            protected static readonly List<Tuple<Guid, long>> LessThanOrEqualToMinamalSequence = new List<Tuple<Guid, long>>();
            protected static readonly List<Tuple<Guid, long>> BiggerThanMaximumSequence = new List<Tuple<Guid, long>>();

            protected static readonly ReplicateToCRMMessageAggregatedProcessingResultHandler.SlicerSettings Settings = 
                ReplicateToCRMMessageAggregatedProcessingResultHandler.SlicerSettings.Default;

            Establish context = () =>
            {
                LessThanOrEqualToMinamalSequence.Clear();
                BiggerThanMaximumSequence.Clear();

                int elementsCount = Math.Max(Settings.MinRangeLength / DefaultSequenceFactor, 1);
                for (int i = 0; i < elementsCount; i++)
                {
                    LessThanOrEqualToMinamalSequence.Add(new Tuple<Guid, long>(Guid.NewGuid(), i));
                }

                elementsCount = Settings.MaxRangeLength * DefaultSequenceFactor;
                for (int i = 0; i < elementsCount; i++)
                {
                    BiggerThanMaximumSequence.Add(new Tuple<Guid, long>(Guid.NewGuid(), i));
                }
            };
        }

        [Subject(typeof(ReplicateToCRMMessageAggregatedProcessingResultHandler.Slicer<>))]
        class When_Sequence_Is_Short : SlicerContext
        {
            static readonly List<IReadOnlyCollection<Tuple<Guid, long>>> SlicedSequence = new List<IReadOnlyCollection<Tuple<Guid, long>>>();

            Because of_sequence_lesser_or_equal_than_minimal = () =>
            {
                SlicedSequence.Clear();
                var slicer = new ReplicateToCRMMessageAggregatedProcessingResultHandler.Slicer<Tuple<Guid, long>>(Settings, LessThanOrEqualToMinamalSequence);

                IReadOnlyCollection<Tuple<Guid, long>> slice;
                while (slicer.TryGetRange(out slice))
                {
                    SlicedSequence.Add(slice);
                    slicer.Shift();
                }
            };

            It should_be_single_slice = () => SlicedSequence.Should().HaveCount(1);
            It should_be_slice_with_length_same_as_original = () => SlicedSequence.Should().Contain(tuples => tuples.Count == LessThanOrEqualToMinamalSequence.Count);
        }

        [Subject(typeof(ReplicateToCRMMessageAggregatedProcessingResultHandler.Slicer<>))]
        class When_Sequence_Is_Long : SlicerContext
        {
            static readonly List<IReadOnlyCollection<Tuple<Guid, long>>> SlicedSequence = new List<IReadOnlyCollection<Tuple<Guid, long>>>();

            Because of_sequence_longer_than_minimal = () =>
            {
                SlicedSequence.Clear();
                var slicer = new ReplicateToCRMMessageAggregatedProcessingResultHandler.Slicer<Tuple<Guid, long>>(Settings, BiggerThanMaximumSequence);

                IReadOnlyCollection<Tuple<Guid, long>> slice;
                while (slicer.TryGetRange(out slice))
                {
                    SlicedSequence.Add(slice);
                    slicer.Shift();
                }
            };

            It should_be_two_slices = () => SlicedSequence.Should().HaveCount(DefaultSequenceFactor);
            It should_be_each_slice_non_longer_than_max_count = () => SlicedSequence.Should().Contain(tuples => tuples.Count <= Settings.MaxRangeLength);
        }

        [Subject(typeof(ReplicateToCRMMessageAggregatedProcessingResultHandler.Slicer<>))]
        class When_Sequence_Requires_Several_Attempts : SlicerContext
        {
            static readonly List<IReadOnlyCollection<Tuple<Guid, long>>> SlicedSequence = new List<IReadOnlyCollection<Tuple<Guid, long>>>();
            static readonly List<IReadOnlyCollection<Tuple<Guid, long>>> FailedSlices = new List<IReadOnlyCollection<Tuple<Guid, long>>>();

            Because of_sequence_sliced_smaller_and_one_range_failed = () =>
            {
                var slicer = new ReplicateToCRMMessageAggregatedProcessingResultHandler.Slicer<Tuple<Guid, long>>(Settings, BiggerThanMaximumSequence);

                IReadOnlyCollection<Tuple<Guid, long>> slice;

                int iterationCount = 0;

                while (slicer.TryGetRange(out slice))
                {
                    if (iterationCount == 1)
                    {
                        if (slicer.TrySliceSmaller())
                        {
                            continue;
                        }
                        
                        FailedSlices.Add(slice);
                        slicer.Shift();
                        ++iterationCount;
                        continue;
                    }

                    SlicedSequence.Add(slice);
                    slicer.Shift();
                    ++iterationCount;
                }
            };

            It should_be_one_failed_range = () => FailedSlices.Should().HaveCount(1);
            It should_be_two_slices = () => SlicedSequence.Should().HaveCount(2);
            It should_be_failed_elements_count_greater_than_min_slice_length =
                () => SlicedSequence.Aggregate(BiggerThanMaximumSequence.Count, (i, tuples) => i - tuples.Count).Should().BeGreaterOrEqualTo(Settings.MinRangeLength);
        }
    }
}
