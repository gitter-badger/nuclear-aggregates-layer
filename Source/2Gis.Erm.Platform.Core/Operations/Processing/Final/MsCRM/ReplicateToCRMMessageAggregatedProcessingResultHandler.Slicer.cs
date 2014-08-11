using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Final.MsCRM
{
    public sealed partial class ReplicateToCRMMessageAggregatedProcessingResultHandler
    {
        public sealed class Slicer<T>
        {
            private readonly SlicerSettings _settings;
            private readonly List<T> _elements;
            
            private int _currentOffset;
            private int _currentRangeLength;

            public Slicer(SlicerSettings settings, List<T> elements)
            {
                _settings = settings;
                _elements = elements;
                _currentRangeLength = EvaluateNextRangeLength();
            }

            public bool TryGetRange(out IReadOnlyCollection<T> resultRange)
            {
                resultRange = null;
                if (_currentRangeLength == 0)
                {
                    return false;
                }

                resultRange = _elements.GetRange(_currentOffset, _currentRangeLength);
                return true;
            }

            public bool TrySliceSmaller()
            {
                int targetRangeLength = _currentRangeLength / _settings.Ratio;
                if (targetRangeLength < _settings.MinRangeLength)
                {
                    return false;
                }

                _currentRangeLength = targetRangeLength;
                return true;
            }

            public void Shift()
            {
                _currentOffset += _currentRangeLength;
                _currentRangeLength = EvaluateNextRangeLength();
            }

            private int EvaluateNextRangeLength()
            {
                int remainderElementsCount = _elements.Count - _currentOffset;
                return _settings.MaxRangeLength < remainderElementsCount
                        ? _settings.MaxRangeLength
                        : remainderElementsCount;
            }
        }
        
        public sealed class SlicerSettings
        {
            private const int DefaultRatio = 2;
            private const int DefaultMinRangeLength = 10;
            private const int DefaultMaxRangeLength = 1000;

            public static SlicerSettings Default 
            {
                get 
                { 
                    return new SlicerSettings
                    {
                        Ratio = DefaultRatio, 
                        MinRangeLength = DefaultMinRangeLength, 
                        MaxRangeLength = DefaultMaxRangeLength
                    };
                }
            }

            public int Ratio { get; set; }
            public int MinRangeLength { get; set; }
            public int MaxRangeLength { get; set; }
        }
    }
}