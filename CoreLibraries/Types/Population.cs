// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Population.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   A collection that holds an array of critters with helper functions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NUnit.Framework;

    #endregion

    /// <summary>
    /// A collection that holds an array of critters with helper functions.
    /// </summary>
    public class Population : List<Critter>, ICloneable
    {
        #region Constants and Fields

        /// <summary>
        ///   The epsilon.
        /// </summary>
        private const double Epsilon = double.Epsilon;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Population"/> class. 
        ///   Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements are copied to the new list.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="collection"/> is null.
        /// </exception>
        public Population(IEnumerable<Critter> collection)
            : base(collection)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Population" /> class. 
        ///   Initializes a new instance of the <see cref = "T:System.Collections.Generic.List`1" /> class that is empty and has the default initial capacity.
        /// </summary>
        public Population()
        {
        }

        #endregion

        #region Public Methods and Operators

        //// TODO: Remove dodgy hypervolume code?

        /*
         *      BASED ON CODE WITH THE FOLLOWING HEADER. (I can't find the source
         *      page)
         * 
        /*---------------------------------------------------------------------------*/
        /* Approximation of the Least Contributor                                    */
        /*---------------------------------------------------------------------------*/
        /* This program calculates for a given set of points in R^d a point, which   */
        /* has a hypervolume contribution very close to the minimal contribution of  */
        /* any point. In [BF09] we prove that for given delta,epsilon>0, the         */
        /* returned solution has a contribution at most (1+epsilon) times the        */
        /* minimal contribution with probability at least (1-delta).  More details   */
        /* can be found in [BF09].                                                   */
        /* This implementation may use the exact hypervolume algorithms HSO          */
        /* by Zitzler (available at                                                  */
        /* ftp://ftp.tik.ee.ethz.ch/pub/people/zitzler/hypervol.c) and BR by         */
        /* Beume and Rudolph (available upon request from the two) to speed up on    */
        /* small subcases.                                                           */
        /* If you do not use HSO and/or BR, you should disable the flags             */
        /* USE_EXACT_HSO and/or USE_EXACT_BR.                                        */
        /* Also note that the algorithm assumes that no point is dominated by        */
        /* another, meaning that you should run a test for domination before         */
        /* invocing the algorithm.                                                   */
        /* [BF09] K. Bringmann, T. Friedrich.  Approximating the least hypervolume   */
        /*        contributor: NP-hard in general, but fast in practice.             */
        /*        Proc. of the 5th International Conference on Evolutionary          */
        /*        Multi-Criterion Optimization (EMO 2009), Nantes, France,           */
        /*        Vol. 5467 of Lecture Notes in Computer Science, pages 6-20,        */
        /*        Springer-Verlag, 2009.                                             */
        /*---------------------------------------------------------------------------*/
        /* Karl Bringmann                                                            */
        /* Saarland University, Germany                                              */
        /* and                                                                       */
        /* Tobias Friedrich                                                          */
        /* Max-Planck-Institut für Informatik, Saarbrücken, Germany                  */
        /* (c)2010                                                                   */
        /*---------------------------------------------------------------------------*/

        /// <summary>
        /// Returns the hypervolume of the specified objectives.
        /// </summary>
        /// <param name="objectives">
        /// An array of objectives to use to calculate the hypervolume.
        /// </param>
        /// <param name="referencePoint">
        /// The reference point to use to calculate the hypervolume.
        /// </param>
        /// <returns>
        /// The hypervolume value.
        /// </returns>
        public double CalculateHyperVolume(FitnessParameter[] objectives, double[] referencePoint)
        {
            int dims = objectives.Length;

            var firstFrontRaw = this.Where(c => c.Rank == 1);

            // var firstFront = firstFrontRaw.Distinct().ToList();
            var firstFront = new List<Critter>();

            foreach (var c in firstFrontRaw)
            {
                bool same = false;
                foreach (var fitnessParameter in objectives)
                {
                    if (
                        firstFront.Any(
                            c2 => Math.Abs(c2.Fitness[fitnessParameter] - c.Fitness[fitnessParameter]) < Epsilon))
                    {
                        same = true;
                    }
                }

                if (!same)
                {
                    firstFront.Add(c);
                }
            }

            foreach (var critter in firstFront)
            {
                foreach (var critter1 in firstFront)
                {
                    Assert.That(!this.Dominates(objectives, critter, critter1));
                    Assert.That(!this.Dominates(objectives, critter1, critter));
                }
            }

            using (var streamWriter = new StreamWriter("Frontdump.json", false))
            {
                streamWriter.WriteLine("--------------------------------------------------------------------------");
                for (int i = 0; i < firstFront.Count; i++)
                {
                    for (int j = 0; j < firstFront.Count; j++)
                    {
                        streamWriter.Write("|{0,2}", this.Dominates(objectives, firstFront[i], firstFront[j]) ? 1 : 0);
                    }

                    streamWriter.WriteLine(
                        "\n--------------------------------------------------------------------------");
                }

                streamWriter.WriteLine("\n\n");

                var values = Enum.GetValues(typeof(FitnessParameter));
                for (int i = 0; i < values.Length; i++)
                {
                    Console.Write("{0},", values.GetValue(i));
                }

                foreach (var critter in firstFront)
                {
                    for (int i = 0; i < critter.Fitness.Length; i++)
                    {
                        streamWriter.Write("{0},", critter.Fitness[i]);
                    }

                    streamWriter.WriteLine();
                }
            }

            var points = new double[firstFront.Count][];

            var bounds = new double[objectives.Length];

            for (int i = 0; i < firstFront.Count; i++)
            {
                points[i] = new double[objectives.Length];
                int j = 0;
                foreach (var fitnessParameter in objectives)
                {
                    double fp = 1 - firstFront[i].Fitness[fitnessParameter];

                    // Set bounds to greatest value found.
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
        /// Returns a new <see cref="Population"/>  which contains members that have all been cloned from this object.
        /// </summary>
        /// <remarks>
        /// Be careful with this method as the memory usage can increase significantly as each object within the population is
        /// is also cloned rather than just this object.</remarks>
        /// <returns>
        /// The cloned <see cref="Population"/> object.
        /// </returns>
        public object Clone()
        {
            var clone = new Population();
            clone.AddRange(this.Select(critter => (Critter)critter.Clone()));
            return clone;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns if one critter dominates another according to the specified objectives.
        /// </summary>
        /// <param name="objectives">
        /// The objectives to use to determine domination.
        /// </param>
        /// <param name="c1">
        /// The first critter under comparison.
        /// </param>
        /// <param name="c2">
        /// The second critter under comparison.
        /// </param>
        /// <returns>
        /// True if <paramref name="c1"/> dominates <paramref name="c2"/>, otherwise false.
        /// </returns>
        private bool Dominates(FitnessParameter[] objectives, Critter c1, Critter c2)
        {
            var dominated = false;
            var flags = new[] { false, false };
            for (int i = 0; i < objectives.Length; i++)
            {
                if (c1.Fitness[(int)objectives[i]] < c2.Fitness[(int)objectives[i]])
                {
                    flags[0] = true;
                }
                else if (c1.Fitness[(int)objectives[i]] > c2.Fitness[(int)objectives[i]])
                {
                    flags[1] = true;
                }
            }

            if (flags[0] && !flags[1])
            {
                dominated = true;
            }

            return dominated;
        }

        /// <summary>
        /// Recursively calculates hypervolume.
        /// </summary>
        /// <param name="points">
        /// The set of points for hypervolume calculation.
        /// </param>
        /// <param name="n">
        /// The n parameter.
        /// </param>
        /// <param name="dims">
        /// The number of dimensions.
        /// </param>
        /// <param name="sign">
        /// The sign value.
        /// </param>
        /// <param name="took">
        /// The took value.
        /// </param>
        /// <param name="bounds">
        /// The bounds value.
        /// </param>
        /// <returns>
        /// The hypervolume value calculated recursively.
        /// </returns>
        private double RecurseCalcHyperVolume(double[][] points, int n, int dims, int sign, int took, double[] bounds)
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

        #endregion
    }
}