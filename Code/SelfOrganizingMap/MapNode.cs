//-----------------------------------------------------------------------
// <copyright file="MapNode.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SelfOrganizingMap
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a single node in a self-organizing map. 
    /// A node contains data, giving the map depth.
    /// </summary>
    public class MapNode
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MapNode" /> class.
        /// </summary>
        /// <param name="x">The x value of this node in the containing Map.</param>
        /// <param name="y">The y value of this node in the containing Map.</param>
        /// <param name="weights">The weights to initialize the node with.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x", Justification = "This is accurately named."), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y", Justification = "This is accurately named.")]
        public MapNode(int x, int y, Vector weights)
        {
            if (weights == null)
            {
                throw new ArgumentNullException("weights", "Unable to create a MapNode with null weights.");
            }

            this.X = x;
            this.Y = y;
            this.Weights = new Vector();

            // copy the weights
            foreach (double weight in weights)
            {
                this.Weights.Add(weight);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the weights.
        /// </summary>
        /// <value>
        /// The weights.
        /// </value>
        public Vector Weights { get; private set; }

        /// <summary>
        /// Gets the X value of this node in the containing Map.
        /// </summary>
        /// <value>
        /// The X value of this node in the containing Map.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X", Justification = "This is accurately named.")]
        public int X { get; private set; }

        /// <summary>
        /// Gets the Y value of this node in the containing Map.
        /// </summary>
        /// <value>
        /// The Y value of this node in the containing Map.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y", Justification = "This is accurately named.")]
        public int Y { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the literal distance between this MapNode and the given MapNode based on the standard Euclidean distance function,
        /// comparing the X and Y properties of this MapNode to the X and Y property of the given MapNode.
        /// A shortcut is taken, making this a quick and dirty distance calculation, by skipping the root function,
        /// since we don't really care about the absolute value.
        /// We only care about the value relative to other results.
        /// This is referred to as the Squared Euclidean Distance function.
        /// </summary>
        /// <param name="node">The node to calculate the distance squared to.</param>
        /// <returns>
        /// Returns the sum of the squares of the differences between this nodes X and Y to the other nodes X and Y properties.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">A non-null MapNode is required to calculate the distance.</exception>
        /// <see href="http://en.wikipedia.org/wiki/Euclidean_distance"/>
        /// <see href="http://en.wikipedia.org/wiki/Quadrance#Quadrance"/>
        public double DistanceToSquared(MapNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node", "A non-null MapNode is required to calculate the distance.");
            }

            int differenceInX = this.X - node.X;
            int differenceInY = this.Y - node.Y;

            return (differenceInX * differenceInX) + (differenceInY * differenceInY);
        }

        /// <summary>
        /// Adjusts the weights in this MapNode based on the weights in the given vector, by applying a standard learning function for self-organizing maps.
        /// </summary>
        /// <param name="input">The input data to use to adjust this MapNodes weights by.</param>
        /// <param name="learningRate">The learning rate to plug into the learning function.</param>
        /// <param name="distanceFalloff">The distance falloff to plug into the learning function.</param>
        /// <exception cref="System.ArgumentException">The weights in a MapNode cannot be adjusted when the input vector is either null or has an incorrect count.</exception>
        public void AdjustWeights(Vector input, double learningRate, double distanceFalloff)
        {
            if (input == null || input.Count != this.Weights.Count)
            {
                throw new ArgumentException("The weights in a MapNode cannot be adjusted when the input vector is either null or has an incorrect count.");
            }

            double learningRateTimesDistanceFalloff = learningRate * distanceFalloff;

            for (int index = 0; index < this.Weights.Count; index++)
            {
                double weightAtIndex = this.Weights[index];

                this.Weights[index] = weightAtIndex +
                    (learningRateTimesDistanceFalloff * (input[index] - weightAtIndex));
            }
        }

        #endregion
    }
}