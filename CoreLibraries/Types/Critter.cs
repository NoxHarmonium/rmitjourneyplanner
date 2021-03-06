﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Critter.cs" company="RMIT University">
//   Copyright RMIT University 2012.
// </copyright>
// <summary>
//   Represents a member of a population in an evolutionary algorithm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RmitJourneyPlanner.CoreLibraries.Types
{
    #region Using Directives

    using System;
    using System.Linq;

    using NUnit.Framework;

    #endregion

    /// <summary>
    /// Represents a member of a population (a journey) in an evolutionary algorithm.
    /// </summary>
    public class Critter : ICloneable, IEquatable<Critter>
    {
        #region Constants and Fields

        /// <summary>
        ///   The departure time of the journey.
        /// </summary>
        private DateTime departureTime;

        /// <summary>
        ///   The route.
        /// </summary>
        private Route route;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Critter"/> class.
        /// </summary>
        /// <param name="route">
        /// The route. 
        /// </param>
        /// <param name="fitness">
        /// The fitness.
        /// </param>
        public Critter(Route route, Fitness fitness)
        {
            this.route = route;
            this.Fitness = fitness;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the age of this critter.
        /// </summary>
        /// <value>
        ///   The age of this critter.
        /// </value>
        public int Age { get; set; }

        /// <summary>
        ///   Gets or sets the distance value of this critter.
        /// </summary>
        /// <value>
        ///   The distance value of this critter.
        /// </value>
        public double Distance { get; set; }

        /// <summary>
        ///   Gets or sets the fitness object for this critter.
        /// </summary>
        /// <value>
        ///   The fitness object for this critter.
        /// </value>
        public Fitness Fitness { get; set; }

        /// <summary>
        ///   Gets or sets the N value of this critter.
        /// </summary>
        /// <value>
        ///   The N value of this critter.
        /// </value>
        public int N { get; set; }

        /// <summary>
        ///   Gets or sets the rank of this critter.
        /// </summary>
        /// <value>
        ///   The rank of this critter.
        /// </value>
        public int Rank { get; set; }

        /// <summary>
        ///   Gets or sets the route associated with this critter.
        /// </summary>
        public Route Route
        {
            get
            {
                return this.route;
            }

            set
            {
                this.route = value;
            }
        }

        /// <summary>
        ///   Gets or sets the unified fitness score of this critter.
        /// </summary>
        /// <value>
        ///   The unified fitness score of this critter.
        /// </value>
        public double UnifiedFitnessScore { get; set; }

        /// <summary>
        ///   Gets or sets the departure time of the the journey.
        /// </summary>
        public DateTime DepartureTime
        {
            get
            {
                return this.departureTime;
            }

            set
            {
                this.departureTime = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        //// TODO: Are the Critter overloaded operators for equality even used or even correct to use?

        /// <summary>
        ///   The equality operator for a <see cref="Critter"/> object.
        /// </summary>
        /// <param name = "left">
        ///   The object on the left side of the statement.
        /// </param>
        /// <param name = "right">
        ///  The object on the right side of the statement.
        /// </param>
        /// <returns>
        /// If the two objects are equal by value.
        /// </returns>
        public static bool operator ==(Critter left, Critter right)
        {
            return Critter.Equals(left, right);
        }

        /// <summary>
        ///   The inequality operator for a <see cref="Critter"/> object.
        /// </summary>
        /// <param name = "left">
        ///   The object on the left side of the statement.
        /// </param>
        /// <param name = "right">
        ///  The object on the right side of the statement.
        /// </param>
        /// <returns>
        /// If the two objects are equal by value.
        /// </returns>
        public static bool operator !=(Critter left, Critter right)
        {
            return !Critter.Equals(left, right);
        }

        /// <summary>
        /// Return a copy of this critter.
        /// </summary>
        /// <returns>
        /// A cloned critter. 
        /// </returns>
        public object Clone()
        {
            Assert.That(this.departureTime != default(DateTime));
            var cloned = new Critter((Route)this.Route.Clone(), (Fitness)this.Fitness.Clone())
                {
                   departureTime = this.departureTime, Age = this.Age, N = this.N, Rank = this.Rank 
                };
            Assert.That(cloned.departureTime != default(DateTime));
            return cloned;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(Critter other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.route.Intersect(this.route).Count() == this.route.Count
                   && other.departureTime.Equals(this.departureTime);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(Critter))
            {
                return false;
            }

            return this.Equals((Critter)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.route != null ? this.route.GetHashCode() : 0) * 397) ^ this.departureTime.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the string representation of this object.
        /// </summary>
        /// <returns>
        /// A string which provides human readable information on this object.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "Rank: {0}, JT: {1}, DepartureTime: {2}, Route: {3}", 
                this.Rank, 
                this.Fitness.TotalJourneyTime, 
                this.departureTime, 
                this.route);
        }

        #endregion
    }
}