// -----------------------------------------------------------------------
// <copyright file="Population.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A collection that holds an array of critters with helper functions.
    /// </summary>
    public class Population : List<Critter>, ICloneable
    {

        private double RecurseCalcHyperVolume(double[][] points, int n, int dims, int sign, int took,double[] bounds)
        {
            int k;
            if (n < 0)
            {
                double res = 1.0;
                foreach (var bound in bounds)
                {
                    res *= bound;
                }
                if (took > 0)
                {
                    return sign * res;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                double res = this.RecurseCalcHyperVolume(points, n - 1, dims, sign, took, bounds);
                double[] nextPoint = new double[dims];
                for (k = 0; k < dims; k++)
                {
                    nextPoint[k] = Math.Min(bounds[k], points[n][k]);
                }
                res += this.RecurseCalcHyperVolume(points, n - 1, dims, -sign, took + 1, nextPoint);
                return res;
            }


        }
        
        /// <summary>
        /// Returns the hypervolume of the specified objectives.
        /// </summary>
        /// <param name="objectives">An array of objectives to use to calculate the hypervolume.</param>
        /// <param name="referencePoint">The reference point of the hypervolume.</param>
        /// <returns>The hypervolume.</returns>
        public double CalculateHyperVolume(FitnessParameter[] objectives,double[] referencePoint)
        {
            int dims = objectives.Length;
            double volume = 0.0;

            var firstFrontRaw = this.Where(c => c.Rank == 1);
            var firstFront = firstFrontRaw.Distinct().ToList();

            var points = new double[firstFront.Count][];

            var bounds = new double[objectives.Length];

            

            for (int i = 0; i < firstFront.Count; i++)
            {
                points[i] = new double[objectives.Length];
                int j = 0;
                foreach (var fitnessParameter in objectives)
                {
                    double fp = 1- firstFront[i].Fitness[fitnessParameter];

                    //Set bounds to greatest value found.
                    points[i][j] = fp;
                    if (fp > bounds[j])
                    {
                        bounds[j] = fp;
                    }
                    j++;
                }
            }

            return this.RecurseCalcHyperVolume(points, points.Length - 1, objectives.Length - 1, -1, 0, bounds);

        }

        /// <summary>
        /// Returns a new set of critters that have all been cloned from this list.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var clone = new Population();
            clone.AddRange(this.Select(critter => (Critter)critter.Clone()));
            return clone;
        }
    }
}
