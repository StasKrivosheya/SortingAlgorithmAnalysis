using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Prism.Mvvm;

namespace SortingAlgorithmAnalysis.Models
{
    public class MeasurementViewModel : BindableBase
    {
        #region -- Public properties --

        private int _index;
        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        private int _elementsCount;
        public int ElementsCount
        {
            get => _elementsCount;
            set => SetProperty(ref _elementsCount, value);
        }

        private ObservableCollection<double> _timeElapsed;
        public ObservableCollection<double> TimeElapsed
        {
            get => _timeElapsed;
            set => SetProperty(ref _timeElapsed, value);
        }

        private double _averageTime;
        public double AverageTime
        {
            get => _averageTime;
            set => SetProperty(ref _averageTime, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(TimeElapsed))
            {
                RecalculateAverage();
            }
        }

        #endregion

        #region -- Public helpers --

        public void RecalculateAverage()
        {
            AverageTime = TimeElapsed.Average();
        }

        #endregion
    }
}
