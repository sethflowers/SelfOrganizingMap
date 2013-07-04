//-----------------------------------------------------------------------
// <copyright file="Vector.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SelfOrganizingMap
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    
    /// <summary>
    /// Represents the weights/data at a node in a self organizing map.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is accurately named.")]
    public class Vector : Collection<double>
    {
        /// <summary>
        /// A quick and dirty distance calculation, skipping the root function,
        /// since we don't really care about the absolute value.
        /// We only care about the value relative to other results.
        /// A smaller number means a closer match.
        /// </summary>
        /// <param name="other">The vector to calculate the dirty distance to.</param>
        /// <returns>
        /// Returns the sum of the squares of the differences between this vectors items, and the other vectors items.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Unable to calculate the distance to a null Vector.</exception>
        /// <exception cref="System.ArgumentException">Unable to calculate the distance between vectors with different counts.</exception>
        /// <see href="http://en.wikipedia.org/wiki/Euclidean_distance"/>
        /// <see href="http://en.wikipedia.org/wiki/Quadrance#Quadrance"/>
        public virtual double DistanceToSquared(Vector other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other", "Unable to calculate the distance to a null Vector.");
            }
            else if (this.Count != other.Count)
            {
                throw new ArgumentException("Unable to calculate the distance between vectors with different counts.");
            }

            double total = 0;
            double difference;

            for (int i = 0; i < this.Count; i++)
            {
                difference = this[i] - other[i];
                total += difference * difference;
            }

            return total;
        }
    }
}