//-----------------------------------------------------------------------
// <copyright file="MapTrainer.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SelfOrganizingMap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a way to train a self-organizing Map.
    /// </summary>
    public class MapTrainer
    {
        #region Methods

        /// <summary>
        /// Trains the specified self-organizing Map using the given training data.
        /// </summary>
        /// <param name="map">The map to train.</param>
        /// <param name="trainingData">The data to train the given Map with..</param>
        /// <exception cref="System.ArgumentNullException">A null self-organizing map cannot be trained.</exception>
        /// <exception cref="System.ArgumentNullException">Non-null training data is required to train a self-organizing map.</exception>
        public virtual void Train(Map map, IList<Vector> trainingData)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map", "A null self-organizing map cannot be trained.");
            }
            else if (trainingData == null)
            {
                throw new ArgumentNullException("trainingData", "Non-null training data is required to train a self-organizing map.");
            }

            const double StartLearningRate = 0.02;
            const int NumberOfTrainingIterations = 100;

            double latticeRadius = Math.Max(map.Width, map.Height) / 2;
            double timeConstant = NumberOfTrainingIterations / Math.Log(latticeRadius);
            
            int iteration = 0;
            double learningRate = StartLearningRate;

            while (iteration < NumberOfTrainingIterations)
            {
                double neighborhoodRadius = latticeRadius * Math.Exp(-iteration / timeConstant);
                double neighborhoodDiameter = neighborhoodRadius * 2;
                double neighborhoodRadiusSquared = neighborhoodRadius * neighborhoodRadius;

                foreach (Vector input in trainingData)
                {
                    AdjustBestMatchingNodesNeighbors(
                        map, 
                        input,
                        neighborhoodDiameter,
                        neighborhoodRadius, 
                        neighborhoodRadiusSquared,
                        learningRate);
                }

                iteration++;
                learningRate = StartLearningRate *
                    Math.Exp(-(double)iteration / NumberOfTrainingIterations);
            }
        }

        /// <summary>
        /// Finds the node that best matches the given training data,
        /// and adjusts the nodes in it's vicinity,
        /// based on the given radius, and learning rate.
        /// </summary>
        /// <param name="map">The map being trained.</param>
        /// <param name="trainingDataToMatch">The data to match when determining the best matching node.</param>
        /// <param name="neighborhoodDiameter">The diameter of the neighborhood of the best matching node used when adjusting nodes.</param>
        /// <param name="neighborhoodRadius">The radius that we will allow when adjusting nodes.
        /// Any node that falls within this radius from the best matching node
        /// will have its weights adjusted based on the given learning rate.</param>
        /// <param name="neighborhoodRadiusSquared">The square of the neighborhood radius.</param>
        /// <param name="learningRate">The learning rate.</param>
        /// <exception cref="System.ArgumentException">Training a self-organizing map requires the training data to have the same depth as the Map.</exception>
        private static void AdjustBestMatchingNodesNeighbors(
            Map map, 
            Vector trainingDataToMatch,
            double neighborhoodDiameter,
            double neighborhoodRadius, 
            double neighborhoodRadiusSquared,
            double learningRate)
        {
            if (trainingDataToMatch.Count != map.Depth)
            {
                throw new ArgumentException("Training a self-organizing map requires the training data to have the same depth as the Map.");
            }

            // Find the MapNode whose data best matches the training data we are currently trying to match.
            MapNode bestMatchingNode = map.GetBestMatchingNode(trainingDataToMatch);

            // Calculate the bounds of the neighborhood of MapNodes in the vicinity of the best matching node to adjust.
            int startX = (int)Math.Max(0, bestMatchingNode.X - neighborhoodRadius - 1);
            int startY = (int)Math.Max(0, bestMatchingNode.Y - neighborhoodRadius - 1);
            int endX = (int)Math.Min(map.Width, startX + neighborhoodDiameter + 1);
            int endY = (int)Math.Min(map.Height, startY + neighborhoodDiameter + 1);

            // Loop through all the nodes in the neighborhood of the best matching node.
            // Note that these two loops don't represent a circle in the Map, but instead represent a square. 
            // This can be considered an initial step to filter out
            // all nodes that are definitely not in the neighborhood radius.
            // Just think about how a circle with diameter "X" overlaps a square with width "X".
            // The corners of the square are exposed. 
            // This means we need another calculation within the inner loop 
            // to only retrieve nodes actually in the neighborhood (circle).
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    // Get the node to adjust.
                    MapNode nodeToAdjust = map[x, y];
                    double distanceSquared = bestMatchingNode.DistanceToSquared(nodeToAdjust);

                    // Perform a more fine-grained filter to only get the nodes within the neighborhood (circle).
                    if (distanceSquared <= neighborhoodRadiusSquared)
                    {
                        double distanceFalloff = Math.Exp(-distanceSquared / (2 * neighborhoodRadiusSquared));
                        nodeToAdjust.AdjustWeights(trainingDataToMatch, learningRate, distanceFalloff);
                    }
                }
            }
        }

        #endregion
    }
}