//-----------------------------------------------------------------------
// <copyright file="MapNodeTests.cs" company="Seth Flowers">
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
    /// Tests for the MapNode class.
    /// </summary>
    [TestClass]
    public class MapNodeTests
    {
        /// <summary>
        /// Verifies that the constructor throws a meaningful message if the weights argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullWeights_ThrowsCorrectException()
        {
            try
            {
                new MapNode(x: default(int), y: default(int), weights: null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to create a MapNode with null weights.{0}Parameter name: weights", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Verifies that the X property comes from the argument passed into the constructor.
        /// </summary>
        [TestMethod]
        public void Constructor_XProperty_CopiedCorrectly()
        {
            int x = 11;
            Assert.AreEqual(x, new MapNode(x: x, y: default(int), weights: new Vector()).X);
        }

        /// <summary>
        /// Verifies that the Y property comes from the argument passed into the constructor.
        /// </summary>
        [TestMethod]
        public void Constructor_YProperty_CopiedCorrectly()
        {
            int y = 11;
            Assert.AreEqual(y, new MapNode(x: default(int), y: y, weights: new Vector()).Y);
        }

        /// <summary>
        /// Verifies that the Weights property is copied from the Vector argument passed into the constructor.
        /// </summary>
        [TestMethod]
        public void Constructor_WeightsProperty_CopiedCorrectly()
        {
            Vector weights = new Vector { 1.2, 2.3, 3.1, 0.9, 5.0 };
            MapNode node = new MapNode(x: default(int), y: default(int), weights: weights);

            CollectionAssert.AreEqual(weights, node.Weights);
        }

        /// <summary>
        /// Validates that the DistanceToSquared method throws the correct exception if the node argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DistanceToSquared_NullTargetNode_ThrowsCorrectException()
        {
            try
            {
                new MapNode(x: default(int), y: default(int), weights: new Vector())
                    .DistanceToSquared(node: null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("A non-null MapNode is required to calculate the distance.{0}Parameter name: node", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        ///  Validates that the DistanceToSquared method returns correct values based on the distance between the two vectors.
        ///  The distance is a euclidean distance calculation without the square root, which is unnecessary for our purposes.
        /// </summary>
        [TestMethod]
        public void DistanceToSquared_BasicScenarios_ReturnsCorrectValues()
        {
            Assert.AreEqual(100, new MapNode(0, 0, new Vector()).DistanceToSquared(new MapNode(8, 6, new Vector())));
            Assert.AreEqual(100, new MapNode(0, 0, new Vector()).DistanceToSquared(new MapNode(6, 8, new Vector())));
            Assert.AreEqual(100, new MapNode(6, 8, new Vector()).DistanceToSquared(new MapNode(0, 0, new Vector())));
            Assert.AreEqual(100, new MapNode(8, 6, new Vector()).DistanceToSquared(new MapNode(0, 0, new Vector())));

            Assert.AreEqual(25, new MapNode(1, 2, new Vector()).DistanceToSquared(new MapNode(4, 6, new Vector())));
            Assert.AreEqual(25, new MapNode(2, 1, new Vector()).DistanceToSquared(new MapNode(6, 4, new Vector())));
            Assert.AreEqual(25, new MapNode(4, 6, new Vector()).DistanceToSquared(new MapNode(1, 2, new Vector())));
            Assert.AreEqual(25, new MapNode(6, 4, new Vector()).DistanceToSquared(new MapNode(2, 1, new Vector())));
        }

        /// <summary>
        ///  Validates that the DistanceToSquared method returns 0 if the target node is at the same location.
        /// </summary>
        [TestMethod]
        public void DistanceToSquared_NodeAtSameLocation_ReturnsZero()
        {
            Assert.AreEqual(0, new MapNode(1, 3, new Vector()).DistanceToSquared(new MapNode(1, 3, new Vector())));
        }

        /// <summary>
        /// Validates that the AdjustWeights method throws the correct exception if the input vector argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AdjustWeights_NullInputVector_ThrowsCorrectException()
        {
            try
            {
                new MapNode(x: default(int), y: default(int), weights: new Vector())
                    .AdjustWeights(input: null, learningRate: default(double), distanceFalloff: default(double));
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    "The weights in a MapNode cannot be adjusted when the input vector is either null or has an incorrect count.",
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the AdjustWeights method throws the correct exception 
        /// if the input vector argument has a different number of weights than the source node.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AdjustWeights_InputVectorWithDifferentAmountOfWeights_ThrowsCorrectException()
        {
            try
            {
                new MapNode(x: default(int), y: default(int), weights: new Vector { 1 })
                    .AdjustWeights(input: new Vector { 2, 3 }, learningRate: default(double), distanceFalloff: default(double));
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    "The weights in a MapNode cannot be adjusted when the input vector is either null or has an incorrect count.",
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the AdjustWeights method adjusts the weights properly if given valid input.
        /// </summary>
        [TestMethod]
        public void AdjustWeights_ValidInputs_WeightsAreAdjustedProperly()
        {
            MapNode node = new MapNode(
                x: default(int),
                y: default(int),
                weights: new Vector { 1, 4, 3, 2 });

            Vector input = new Vector { 2, 5, 5, 1 };

            node.AdjustWeights(input, learningRate: 0.07, distanceFalloff: 0.2);

            // The weights should be adjusted based on the following calculation:
            // Weights[x] = Weights[x] + ( learningRate * distanceFalloff * ( input[x] - Weights[x] ) )
            // So, for example if Weights[0] is 1, and input[0] is 1, and learningRate is 0.07 and distanceFalloff = 0.2
            // Weights[0] = 1 + (0.07 * 0.2 * (2 - 1))
            // Weights[0] = 1 + (0.014 * (1))
            // Weights[0] = 1 + (0.014)
            // Weights[0] = 1.014
            Assert.AreEqual(1.014, node.Weights[0]);
            Assert.AreEqual(4.014, node.Weights[1]);
            Assert.AreEqual(3.028, node.Weights[2]);
            Assert.AreEqual(1.986, node.Weights[3]);
        }
    }
}