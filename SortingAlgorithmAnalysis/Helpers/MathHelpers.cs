using System;
using System.Collections.Generic;
using SortingAlgorithmAnalysis.Models;

namespace SortingAlgorithmAnalysis.Helpers
{
    public static class MathHelpers
    {
        public static List<int> GetEvenlyDistributedValues(int min, int max, int count)
        {
            int step = (max - min) / count;

            var result = new List<int>();

            for (var i = min; i < max; i += step)
            {
                result.Add(i);
            }

            return result;
        }

        public static double[] GetRandomNumbers(int count)
        {
            var result = new double[count];
            var rnd = new Random();

            for (int i = 0; i < count; i++)
            {
                result[i] = rnd.NextDouble();
            }

            return result;
        }

        public static double TFromNFunc(int n, Tuple<double, double, double> coefficients)
        {
            var result = coefficients.Item1 * Math.Pow(n, 1.5) + coefficients.Item2 * n + coefficients.Item3;

            return result > 0
                ? result
                : 0;
        }

        public static Tuple<double, double, double> GetCoefficients(IList<MeasurementViewModel> measurements)
        {
            var k = measurements.Count;

            double x11, x12, x13, x14, x21, x22, x23, x24, x31, x32, x33, x34;
            x11 = x12 = x13 = x14 = x21 = x22 = x23 = x24 = x31 = x32 = x33 = x34 = 0d;

            for (int i = 0; i < k; i++)
            {
                x11 += Math.Pow(measurements[i].ElementsCount, 3);
                x12 += Math.Pow(measurements[i].ElementsCount, 2.5);
                x13 += Math.Pow(measurements[i].ElementsCount, 1.5);

                x14 += measurements[i].AverageTime * Math.Pow(measurements[i].ElementsCount, 1.5);

                x21 += Math.Pow(measurements[i].ElementsCount, 2.5);
                x22 += Math.Pow(measurements[i].ElementsCount, 2);
                x23 += measurements[i].ElementsCount;

                x24 += measurements[i].AverageTime * measurements[i].ElementsCount;

                x31 += Math.Pow(measurements[i].ElementsCount, 1.5);
                x32 += measurements[i].ElementsCount;
                x33 += 1;

                x34 += measurements[i].AverageTime;
            }

            double[,] matrix =
            {
                { x11, x12, x13, x14 },
                { x21, x22, x23, x24 },
                { x31, x32, x33, x34 },
            };

            var isSolved = SolveByGaussianMethod(matrix);

            if (isSolved)
            {
                return new Tuple<double, double, double>(matrix[0, 3], matrix[1, 3], matrix[2, 3]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Uses Shell Sorting Algorithm
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static int GapShellSort(double[] arr)
        {
            int n = arr.Length;

            // Start with a big gap,
            // then reduce the gap
            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                // Do a gapped insertion sort for this gap size.
                // The first gap elements a[0..gap-1] are already
                // in gapped order keep adding one more element
                // until the entire array is gap sorted
                for (int i = gap; i < n; i += 1)
                {
                    // add a[i] to the elements that have
                    // been gap sorted save a[i] in temp and
                    // make a hole at position i
                    var temp = arr[i];

                    // shift earlier gap-sorted elements up until
                    // the correct location for a[i] is found
                    int j;
                    for (j = i; j >= gap && arr[j - gap] > temp; j -= gap)
                    {
                        arr[j] = arr[j - gap];
                    }

                    // put temp (the original a[i])
                    // in its correct location
                    arr[j] = temp;
                }
            }

            return 0;
        }

        public static int HibbardShellSort(double[] array)
        {
            var d = (int)Math.Pow(2, (int)(Math.Log(array.Length) / Math.Log(2))) - 1;

            while (d >= 1)
            {
                for (var i = d; i < array.Length; i++)
                {
                    var j = i;

                    while ((j >= d) && (array[j - d] > array[j]))
                    {
                        Swap(ref array[j], ref array[j - d]);
                        j -= d;
                    }
                }

                d /= 2;
            }

            return 0;
        }

        private static void Swap(ref double v1, ref double v2)
        {
            var tmp = v1;

            v1 = v2;
            v2 = tmp;
        }

        /// <summary>Computes the solution of a linear equation system.</summary>
        /// <param name="M">
        /// The system of linear equations as an augmented matrix[row, col] where (rows + 1 == cols).
        /// It will contain the solution in "row canonical form" if the function returns "true".
        /// </param>
        /// <returns>Returns whether the matrix has a unique solution or not.</returns>
        public static bool SolveByGaussianMethod(double[,] M)
        {
            // input checks
            int rowCount = M.GetUpperBound(0) + 1;
            if (M == null || M.Length != rowCount * (rowCount + 1))
                throw new ArgumentException("The algorithm must be provided with a (n x n+1) matrix.");
            if (rowCount < 1)
                throw new ArgumentException("The matrix must at least have one row.");

            // pivoting
            for (int col = 0; col + 1 < rowCount; col++) if (M[col, col] == 0)
                // check for zero coefficients
                {
                    // find non-zero coefficient
                    int swapRow = col + 1;
                    for (; swapRow < rowCount; swapRow++) if (M[swapRow, col] != 0) break;

                    if (M[swapRow, col] != 0) // found a non-zero coefficient?
                    {
                        // yes, then swap it with the above
                        double[] tmp = new double[rowCount + 1];
                        for (int i = 0; i < rowCount + 1; i++)
                        { tmp[i] = M[swapRow, i]; M[swapRow, i] = M[col, i]; M[col, i] = tmp[i]; }
                    }
                    else return false; // no, then the matrix has no unique solution
                }

            // elimination
            for (int sourceRow = 0; sourceRow + 1 < rowCount; sourceRow++)
            {
                for (int destRow = sourceRow + 1; destRow < rowCount; destRow++)
                {
                    double df = M[sourceRow, sourceRow];
                    double sf = M[destRow, sourceRow];
                    for (int i = 0; i < rowCount + 1; i++)
                        M[destRow, i] = M[destRow, i] * df - M[sourceRow, i] * sf;
                }
            }

            // back-insertion
            for (int row = rowCount - 1; row >= 0; row--)
            {
                double f = M[row, row];
                if (f == 0) return false;

                for (int i = 0; i < rowCount + 1; i++) M[row, i] /= f;
                for (int destRow = 0; destRow < row; destRow++)
                { M[destRow, rowCount] -= M[destRow, row] * M[row, rowCount]; M[destRow, row] = 0; }
            }

            return true;
        }
    }
}
