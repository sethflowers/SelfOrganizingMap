//-----------------------------------------------------------------------
// <copyright file="MapInitializer.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SelfOrganizingMap
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a mechanism to initialize a self-organizing map, prior to training it.
    /// </summary>
    public class MapInitializer
    {
        /// <summary>
        /// A random number generator which will be used to initialize a Map.
        /// </summary>
        private readonly Random randomGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapInitializer" /> class.
        /// </summary>
        public MapInitializer()
            : this(new Random())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapInitializer" /> class.
        /// </summary>
        /// <param name="randomGenerator">A random number generator which will be used to initialize a Map.</param>
        public MapInitializer(Random randomGenerator)
        {
            this.randomGenerator = randomGenerator;
        }

        /// <summary>
        /// Randomly initializes the specified map using the specified training data for determining the bounds of the random data.
        /// </summary>
        /// <param name="map">The map to initialize.</param>
        /// <param name="trainingData">The training data which will provide the bounds of the randomized map data.</param>
        public virtual void Initialize(Map map, IList<Vector> trainingData)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map", "Unable to initialize a null self-organizing map.");
            }
            else if (trainingData == null)
            {
                throw new ArgumentNullException("trainingData", "Unable to initialize a self-organizing map with null training data.");
            }
            else if (trainingData.Count == 0)
            {
                throw new ArgumentException("Unable to initialize a self-organizing map without training data.", "trainingData");
            }

            // We need to know the min and max of all the data 
            // that the Map is being trained with, in order to have properly randomized data.
            // For instance, if the data contains values from 10 to 100, 
            // we want our random initialization to have values in between these two limits.
            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (Vector vector in trainingData)
            {
                foreach (double dataPoint in vector)
                {
                    min = Math.Min(min, dataPoint);
                    max = Math.Max(max, dataPoint);
                }
            }
            
            // Determine the spread of the min and max, 
            // which will be used when we randomize the data.
            double spread = max - min;

            // Loop through all the points in the map, 
            // and create and initialize a MapNode in each point.
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    Vector weights = new Vector();
                    
                    for (int z = 0; z < map.Depth; z++)
                    {
                        weights.Add((this.randomGenerator.NextDouble() * spread) + min);
                    }

                    map[x, y] = new MapNode(x, y, weights);
                }
            }
        }
    }
}