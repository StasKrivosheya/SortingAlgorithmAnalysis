using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Navigation;
using SortingAlgorithmAnalysis.Helpers;
using SortingAlgorithmAnalysis.Models;

namespace SortingAlgorithmAnalysis.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        private int _nMax;   // value that will be predicted (twice greater than ini)
        private readonly int _k = 20; // amount of different array sizes
        private readonly int _l = 10; // count of measurment for each array size

        public HomePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        private PlotModel _estimationChartModel = new();
        public PlotModel EstimationChartModel
        {
            get => _estimationChartModel;
            set => SetProperty(ref _estimationChartModel, value);
        }

        private ObservableCollection<MeasurementViewModel> _measurements = new();
        public ObservableCollection<MeasurementViewModel> Measurements
        {
            get => _measurements;
            set => SetProperty(ref _measurements, value);
        }

        private double _actualTime;
        public double ActualTime
        {
            get => _actualTime;
            set => SetProperty(ref _actualTime, value);
        }

        private double _predictedTime;
        public double PredictedTime
        {
            get => _predictedTime;
            set => SetProperty(ref _predictedTime, value);
        }

        #endregion

        #region -- Overrides --

        public override async void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            await Task.Run(PerformTesting);

            _nMax = Measurements.LastOrDefault().ElementsCount * 2;

            EstimationChartModel = GetCalculatedPlotModel();

            var coefficients = MathHelpers.GetCoefficients(Measurements.ToList());

            AddPredictionCurve(coefficients);

            AddLastPoint(coefficients);
        }

        #endregion

        #region -- Private helpers --

        private void PerformTesting()
        {
            var arraySize = 10000;
            var sizeIncrementStep = 10000;

            for (int i = 0; i < _k; i++)
            {
                Measurements.Add(new MeasurementViewModel
                {
                    Index = i + 1,
                    ElementsCount = arraySize,
                    AverageTime = 0d,
                });

                var times = new List<double>();

                for (int j = 0; j < _l; j++)
                {
                    var array = MathHelpers.GetRandomNumbers(arraySize);

                    var sw = new Stopwatch();

                    sw.Start();
                    MathHelpers.GapShellSort(array);
                    //MathHelpers.HibbardShellSort(array);
                    sw.Stop();

                    times.Add(sw.ElapsedMilliseconds);
                }

                Measurements[i].TimeElapsed = new(times);
                arraySize += sizeIncrementStep;
            }
        }

        private PlotModel GetCalculatedPlotModel()
        {
            var plotModel = new PlotModel
            {
                Title = "Dependence of Sorting time on the Number of items",
                LegendPosition = LegendPosition.TopLeft,
            };

            var xAxis = new LinearAxis
            {
                Title = "Elements count",
                Position = AxisPosition.Bottom,
                LabelFormatter = (param) => Math.Round(param, 3).ToString(),
                AxisDistance = 5,
                Maximum = _nMax,
            };
            plotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Title = "Average time, milliseconds",
                Position = AxisPosition.Left,
                AxisDistance = 5,
                Maximum = 190,
            };
            plotModel.Axes.Add(yAxis);

            var scatterSeries = new ScatterSeries()
            {
                Title = "Imperical values",
                MarkerType = MarkerType.Circle,
            };

            foreach (var item in Measurements)
            {
                scatterSeries.Points.Add(new ScatterPoint(item.ElementsCount, item.AverageTime, 4));
            }

            plotModel.Series.Add(scatterSeries);

            return plotModel;
        }

        private void AddPredictionCurve(Tuple<double, double, double> coefficients)
        {
            var linearSeries = new LineSeries
            {
                Title = "Calculated T(n) function",
            };

            var lineXMax = (int)(_nMax + (0.1 * _nMax));

            var xs = MathHelpers.GetEvenlyDistributedValues(0, lineXMax, 1000);

            foreach (var x in xs)
            {
                linearSeries.Points.Add(new DataPoint(x, MathHelpers.TFromNFunc(x, coefficients)));
            }

            EstimationChartModel.Series.Add(linearSeries);

            EstimationChartModel.InvalidatePlot(true);
        }

        private void AddLastPoint(Tuple<double, double, double> coefficients)
        {
            var scatterSeries = new ScatterSeries()
            {
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.Parse("#0000FF"),
            };

            Measurements.Add(new MeasurementViewModel
            {
                Index = Measurements.Count + 1,
                ElementsCount = _nMax,
                AverageTime = 0d,
            });

            var times = new List<double>();

            for (int i = 0; i < _l; i++)
            {
                var array = MathHelpers.GetRandomNumbers(_nMax);

                var sw = new Stopwatch();

                sw.Start();
                MathHelpers.GapShellSort(array);
                //MathHelpers.HibbardShellSort(array);
                sw.Stop();

                times.Add(sw.ElapsedMilliseconds);
            }

            Measurements.LastOrDefault().TimeElapsed = new(times);

            scatterSeries.Points.Add(new ScatterPoint(_nMax, Measurements.LastOrDefault().AverageTime , 5));

            EstimationChartModel.Series.Add(scatterSeries);

            ActualTime = Measurements.LastOrDefault().AverageTime;
            PredictedTime = MathHelpers.TFromNFunc(_nMax, coefficients);

            EstimationChartModel.InvalidatePlot(true);
        }

        #endregion
    }
}
