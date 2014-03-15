using System.Collections.Generic;
using System.Linq;

namespace TimeSeriesCollection
{
    public class SMACalculator
    {
        private readonly int _movingWindowSize;
        private readonly IEnumerable<double?> _series;

        public SMACalculator(IEnumerable<double?> series, int movingWindowSize)
        {
            _series = series;
            _movingWindowSize = movingWindowSize;
        }

        public IEnumerable<double?> Calculate()
        {
            var movingWindow = new MovingWindow(_movingWindowSize);
            return _series.Aggregate(new List<double?>(),
                (results, element) =>
                {
                    movingWindow.Add(element);
                    results.Add(element.HasValue ? (double?) movingWindow.Average : null);
                    return results;
                });
        }

        private class MovingWindow
        {
            private readonly Queue<double?> _queue = new Queue<double?>();
            private readonly int _size;
            private int _hasValue;
            private double _sum;

            public MovingWindow(int size)
            {
                _size = size;
            }

            public double Average
            {
                get { return _sum/_hasValue; }
            }

            public void Add(double? element)
            {
                _queue.Enqueue(element);
                if (element.HasValue)
                {
                    _hasValue++;
                    _sum += element.Value;
                }

                if (_queue.Count > _size)
                {
                    element = _queue.Dequeue();
                    if (element.HasValue)
                    {
                        _hasValue--;
                        _sum -= element.Value;
                    }
                }
            }
        }
    }
}