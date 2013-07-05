//-----------------------------------------------------------------------
// <copyright file="MapTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SelfOrganizingMap.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Tests for the Map class.
    /// </summary>
    [TestClass]
    public class MapTests
    {
        /// <summary>
        /// Verifies that the Width property comes from the argument passed into the constructor.
        /// </summary>
        [TestMethod]
        public void Width_SetViaConstructor_Test()
        {
            int width = 11;
            Assert.AreEqual(width, new Map(width, 0, 0).Width);
        }

        /// <summary>
        /// Verifies that the Height property comes from the argument passed into the constructor.
        /// </summary>
        [TestMethod]
        public void Height_SetViaConstructor_Test()
        {
            int height = 13;
            Assert.AreEqual(height, new Map(0, height, 0).Height);
        }

        /// <summary>
        /// Verifies that the Depth property comes from the argument passed into the constructor.
        /// </summary>
        [TestMethod]
        public void Depth_SetViaConstructor_Test()
        {
            int depth = 17;
            Assert.AreEqual(depth, new Map(0, 0, depth).Depth);
        }

        /// <summary>
        /// Verifies that a MapNode can be set via an indexer on the Map.
        /// </summary>
        [TestMethod]
        public void MapNodeCanBeSetViaIndexer_Test()
        {
            Map map = new Map(5, 5, 3);

            MapNode mapNode = new MapNode(1, 2, new Vector { 1, 3, 5 });
            map[mapNode.X, mapNode.Y] = mapNode;

            Assert.AreEqual(mapNode, map[mapNode.X, mapNode.Y]);
        }

        /// <summary>
        /// Verifies that the Train method throws a meaningful message if the training data argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Train_NullTrainingData_ThrowsMeaningfulException()
        {
            Map map = new Map(0, 0, 0);

            try
            {
                map.Train(trainingData: null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to train a self-organizing map with null training data.{0}Parameter name: trainingData", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Verifies that the Train method throws a meaningful message if the training data 
        /// argument contains a vector that has a length that doesn't equal the maps depth.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Train_TrainingDataWithVectorWithIncorrectCount_ThrowsMeaningfulException()
        {
            // Setup a map with a depth of 1.
            Map map = new Map(0, 0, 1);

            try
            {
                // Attempt to train the map with training data that contains 
                // a vector with a length that is different than the maps depth.
                map.Train(trainingData: new List<Vector> { new Vector { 1, 2 } });
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("The training data contains either a null vector, or a vector with an incorrect amount of data.{0}Parameter name: trainingData", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Verifies that the Train method throws a meaningful message 
        /// if the training data argument contains a null vector.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Train_TrainingDataWithNullVector_ThrowsMeaningfulException()
        {
            // Setup a map with a depth of 1.
            Map map = new Map(0, 0, 1);

            try
            {
                // Attempt to train the map with training data that contains a null Vector.
                map.Train(trainingData: new List<Vector> { null });
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("The training data contains either a null vector, or a vector with an incorrect amount of data.{0}Parameter name: trainingData", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Verifies that the Train method just forwards the call to the MapInitializer and MapTrainer objects.
        /// This also verifies that the call to MapInitializer.Initialize is executed prior to calling MapTrainer.Train.
        /// </summary>
        /// <remarks>
        /// This is an interaction test.
        /// </remarks>
        [TestMethod]
        public void Train_ForwardsToInitializerAndTrainer_Test()
        {
            // Setup mock objects that will be used to verify the interaction from the map.
            Mock<MapInitializer> mapInitializer = new Mock<MapInitializer>();
            Mock<MapTrainer> mapTrainer = new Mock<MapTrainer>();

            // Setup the object to test, passing in the mock objects for interaction verification.
            Map map = new Map(0, 0, 0, mapInitializer.Object, mapTrainer.Object);
            
            // Setup training data for the test - this data doesn't matter.
            IList<Vector> trainingData = new List<Vector>();

            // Setup some flags to determine the order that Initialize and Train are called.
            bool initializeCalled = false;
            bool trainCalledAfterInitialize = false;

            // Set the flag when Initialize is called.
            mapInitializer
                .Setup(m => m.Initialize(map, trainingData))
                .Callback(() => initializeCalled = true);

            // Set the flag when Train is called.
            mapTrainer
                .Setup(m => m.Train(map, trainingData))
                .Callback(() => trainCalledAfterInitialize = initializeCalled);

            // Execute the code to test.
            map.Train(trainingData);

            // Verify the mock objects were interacted with.
            mapInitializer.Verify(m => m.Initialize(map, trainingData), Times.Once());
            mapTrainer.Verify(m => m.Train(map, trainingData), Times.Once());

            // Verify that Train was called after Initialize.
            Assert.IsTrue(trainCalledAfterInitialize, "Train was not called after Initialize.");
        }

        /// <summary>
        /// Validates that the GetBestMatchingNode method throws a meaningful exception
        /// if given an input vector that is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetBestMatchingNode_NullInputVector_ThrowsMeaningfulException()
        {
            try
            {
                new Map(width: default(int), height: default(int), depth: default(int))
                    .GetBestMatchingNode(dataToMatch: null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to determine the best matching node when comparing to null data.{0}Parameter name: dataToMatch", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the GetBestMatchingNode method throws a meaningful exception
        /// if given an input vector that has a different dimension that the map's depth.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetBestMatchingNode_InputVectorWithDifferentDimension_ThrowsMeaningfulException()
        {
            try
            {
                new Map(width: default(int), height: default(int), depth: 1)
                    .GetBestMatchingNode(dataToMatch: new Vector { 2, 3 });
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("Unable to determine the best matching node when comparing to data with different dimensions.{0}Parameter name: dataToMatch", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the GetBestMatchingNode method will return the correct
        /// best matching node. To test this, we can set up a map where all the nodes
        /// have the same vector, and then one node has a drastically different vector.
        /// We can then pass in a node similar to this "odd" vector, and we expect
        /// to get the "odd" vector back in return.
        /// </summary>
        [TestMethod]
        public void GetBestMatchingNode_SimpleMap_ReturnsCorrectNode()
        {
            Map map = new Map(width: 4, height: 4, depth: 3);

            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    map[i, j] = new MapNode(x: i, y: j, weights: new Vector { 1, 2, 3 });
                }
            }

            MapNode expected = new MapNode(x: 1, y: 2, weights: new Vector { 5, 2, 7 }); 
            map[1, 2] = expected;

            MapNode actual = map.GetBestMatchingNode(new Vector { 4, 3, 6 });
            Assert.AreEqual(expected, actual);
        }
    }
}