//-----------------------------------------------------------------------
// <copyright file="MapInitializerTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SelfOrganizingMap.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the MapInitializer class.
    /// </summary>
    [TestClass]
    public class MapInitializerTests
    {
        /// <summary>
        /// Verifies that the Initialize method throws a meaningful message if the map argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initialize_ExceptionOnNullMap_Test()
        {
            MapInitializer mapInitializer = new MapInitializer();

            try
            {
                mapInitializer.Initialize(map: null, trainingData: new List<Vector>());
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to initialize a null self-organizing map.{0}Parameter name: map", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Verifies that the Initialize method throws a meaningful message if the training data argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initialize_ExceptionOnNullTrainingData_Test()
        {
            MapInitializer mapInitializer = new MapInitializer();

            try
            {
                mapInitializer.Initialize(map: new Map(0, 0, 0), trainingData: null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to initialize a self-organizing map with null training data.{0}Parameter name: trainingData", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Verifies that the Initialize method throws a meaningful message if the training data argument is non-null but empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialize_ExceptionOnEmptyTrainingData_Test()
        {
            MapInitializer mapInitializer = new MapInitializer();

            try
            {
                mapInitializer.Initialize(map: new Map(0, 0, 0), trainingData: new List<Vector>());
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("Unable to initialize a self-organizing map without training data.{0}Parameter name: trainingData", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Tests the Initialize method of a map.
        /// </summary>
        [TestMethod]
        public void Initialize_Test()
        {
            // Create the object to test.
            MapInitializer mapInitializer = new MapInitializer();

            // Setup some simple training data.
            IList<Vector> trainingData = new List<Vector>
            {
                new Vector { 1, 3 },
                new Vector { 2, 4 },
                new Vector { 5, 3 }
            };

            // Determine the min and max based on the training data.
            double min = trainingData.Min(vector => vector.Min());
            double max = trainingData.Max(vector => vector.Max());

            // Determine the depth of the map based on the training data.
            int depth = trainingData.First().Count;
            
            // Setup a map to initialize.
            Map map = new Map(width: 2, height: 2, depth: depth);

            // Execute the code to test.
            mapInitializer.Initialize(map, trainingData);
            
            // Verify that the map is filled with MapNodes with correct data.
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    MapNode mapNode = map[x, y];

                    Assert.IsNotNull(mapNode, "MapNode");
                    Assert.AreEqual(x, mapNode.X, "MapNode.X");
                    Assert.AreEqual(y, mapNode.Y, "MapNode.Y");
                    Assert.IsNotNull(mapNode.Weights, "MapNode.Weights");
                    Assert.AreEqual(depth, mapNode.Weights.Count, "MapNode.Weights.Count");

                    // Verify that the MapNodes weights are within the allowed bounds.
                    for (int z = 0; z < depth; z++)
                    {
                        double weight = mapNode.Weights[z];

                        Assert.IsTrue(weight >= min, "Min");
                        Assert.IsTrue(weight <= max, "Max");
                    }
                }
            }
        }
    }
}