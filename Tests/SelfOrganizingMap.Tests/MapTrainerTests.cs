//-----------------------------------------------------------------------
// <copyright file="MapTrainerTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SelfOrganizingMap.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the MapTrainer class.
    /// </summary>
    [TestClass]
    public class MapTrainerTests
    {
        /// <summary>
        /// Verifies that the Train method throws a meaningful exception if the map argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Train_NullMapArgument_CorrectExceptionThrown()
        {
            try
            {
                new MapTrainer().Train(map: null, trainingData: new List<Vector>());
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("A null self-organizing map cannot be trained.{0}Parameter name: map", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Verifies that the Train method throws a meaningful exception if the training data argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Train_NullTrainingDataArgument_CorrectExceptionThrown()
        {
            try
            {
                new MapTrainer().Train(map: new Map(0, 0, 0), trainingData: null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Non-null training data is required to train a self-organizing map.{0}Parameter name: trainingData", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }
    }
}