//-----------------------------------------------------------------------
// <copyright file="Map.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SelfOrganizingMap
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Represents a self organizing map.
    /// </summary>
    /// <remarks>
    /// Ported from the excellent ai-junkie website. 
    /// </remarks>
    /// <see href="http://www.ai-junkie.com/ann/som/som1.html" />
    /// <see href="http://en.wikipedia.org/wiki/Self-organizing_map" />
    public class Map
    {
        #region Fields

        /// <summary>
        /// The grid of MapNodes contained in this Map.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member", Justification = "The point of this rule is to reduce wasted space. This rule can be suppressed when the multi-dimensional array does not waste space.")]
        private readonly MapNode[,] grid;

        /// <summary>
        /// Provides a mechanism to initialize a self-organizing map, prior to training it.
        /// </summary>
        private readonly MapInitializer mapInitializer;

        /// <summary>
        /// Provides a way to train a self-organizing Map.
        /// </summary>
        private readonly MapTrainer mapTrainer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Map" /> class.
        /// </summary>
        /// <param name="width">The width of this Map.</param>
        /// <param name="height">The height of this Map.</param>
        /// <param name="depth">The depth of this Map, which represents how much data each MapNode contains.</param>
        public Map(int width, int height, int depth)
            : this(width, height, depth, new MapInitializer(), new MapTrainer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Map" /> class.
        /// </summary>
        /// <param name="width">The width of this Map.</param>
        /// <param name="height">The height of this Map.</param>
        /// <param name="depth">The depth of this Map, which represents how much data each MapNode contains.</param>
        /// <param name="mapInitializer">Provides a mechanism to initialize a self-organizing map, prior to training it.</param>
        /// <param name="mapTrainer">Provides a way to train a self-organizing Map.</param>
        public Map(int width, int height, int depth, MapInitializer mapInitializer, MapTrainer mapTrainer)
        {
            this.Depth = depth;
            this.grid = new MapNode[width, height];

            this.mapInitializer = mapInitializer;
            this.mapTrainer = mapTrainer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the depth of this Map, which represents how much data each MapNode contains.
        /// </summary>
        /// <value>
        /// The depth of this Map.
        /// </value>
        public int Depth { get; private set; }

        /// <summary>
        /// Gets the width of this Map.
        /// </summary>
        /// <value>
        /// The width of this Map.
        /// </value>
        public int Width
        {
            get { return this.grid.GetLength(0); }
        }

        /// <summary>
        /// Gets the height of this Map.
        /// </summary>
        /// <value>
        /// The height of this Map.
        /// </value>
        public int Height
        {
            get { return this.grid.GetLength(1); }
        }

        /// <summary>
        /// Gets or sets the <see cref="MapNode" /> at the specified location in the Map.
        /// </summary>
        /// <value>
        /// The <see cref="MapNode" />.
        /// </value>
        /// <param name="x">The x position of the MapNode in the Map.</param>
        /// <param name="y">The y position of the MapNode in the Map.</param>
        /// <returns>Returns the <see cref="MapNode" /> at the specified location in the Map.</returns>
        [SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional", Justification = "A multi-dimensional array should have a multi-dimensional indexer.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x", Justification = "This is accurately named.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y", Justification = "This is accurately named.")]
        public MapNode this[int x, int y]
        {
            get { return this.grid[x, y]; }
            set { this.grid[x, y] = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Trains the self-organizing map using the specified training data.
        /// </summary>
        /// <param name="trainingData">The training data.</param>
        public virtual void Train(IList<Vector> trainingData)
        {
            if (trainingData == null)
            {
                throw new ArgumentNullException("trainingData", "Unable to train a self-organizing map with null training data.");
            }
            else if (trainingData.Any(d => d == null || d.Count != this.Depth))
            {
                throw new ArgumentException("The training data contains either a null vector, or a vector with an incorrect amount of data.", "trainingData");
            }

            this.mapInitializer.Initialize(this, trainingData);
            this.mapTrainer.Train(this, trainingData);
        }

        /// <summary>
        /// Iterate through the whole map, and retrieve the node whose data is the best match to the given data.
        /// </summary>
        /// <param name="dataToMatch">The data to match to the nodes in the map.</param>
        /// <returns>
        /// Returns the node in the map whose data is the best match to the given data.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Unable to determine the best matching node when comparing to null data.</exception>
        public MapNode GetBestMatchingNode(Vector dataToMatch)
        {
            if (dataToMatch == null)
            {
                throw new ArgumentNullException("dataToMatch", "Unable to determine the best matching node when comparing to null data.");
            }
            else if (dataToMatch.Count != this.Depth)
            {
                throw new ArgumentException("Unable to determine the best matching node when comparing to data with different dimensions.", "dataToMatch");
            }

            // start off with the first mapnode, assuming it is the best matching node
            MapNode bestMatchingNode = this[0, 0];

            // initialize the best distance to the highest possible, which is the worst distance, so anything closer is better.
            double bestDistance = double.MaxValue;
            double currentDistance;
            MapNode currentNode;

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    currentNode = this[x, y];
                    currentDistance = dataToMatch.DistanceToSquared(currentNode.Weights);

                    if (currentDistance < bestDistance)
                    {
                        bestMatchingNode = currentNode;
                        bestDistance = currentDistance;
                    }
                }
            }

            return bestMatchingNode;
        }

        #endregion
    }
}