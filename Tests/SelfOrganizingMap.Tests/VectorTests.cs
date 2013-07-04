//-----------------------------------------------------------------------
// <copyright file="VectorTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace SelfOrganizingMap.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the Vector class.
    /// </summary>
    [TestClass]
    public class VectorTests
    {
        /// <summary>
        /// Verifies that the DistanceToSquared method throws a meaningful message if the "other" vector argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DistanceToSquared_NullWeights_ThrowsCorrectException()
        {
            try
            {
                new Vector().DistanceToSquared(other: null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to calculate the distance to a null Vector.{0}Parameter name: other", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Verifies that the DistanceToSquared method throws a meaningful message 
        /// if the "other" vector argument has a different count than expected.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DistanceToSquared_WeightsWithDifferentCount_ThrowsCorrectException()
        {
            try
            {
                new Vector { 1, 2 }.DistanceToSquared(new Vector { 1 });
            }
            catch (ArgumentException argumentNullException)
            {
                Assert.AreEqual(
                    "Unable to calculate the distance between vectors with different counts.",
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the DistanceToSquared method will return the correct output when given valid inputs.
        /// We expect the output to be the squared 
        /// </summary>
        [TestMethod]
        public void DistanceToSquared_ValidInputs_CorrectOutput()
        {
            Assert.AreEqual(100, new Vector { 0, 0 }.DistanceToSquared(new Vector { 6, 8 }));
            Assert.AreEqual(100, new Vector { 0, 0 }.DistanceToSquared(new Vector { 8, 6 }));
            Assert.AreEqual(100, new Vector { 6, 8 }.DistanceToSquared(new Vector { 0, 0 }));
            Assert.AreEqual(100, new Vector { 8, 6 }.DistanceToSquared(new Vector { 0, 0 }));

            Assert.AreEqual(25, new Vector { 1, 2 }.DistanceToSquared(new Vector { 4, 6 }));
            Assert.AreEqual(25, new Vector { 2, 1 }.DistanceToSquared(new Vector { 6, 4 }));
            Assert.AreEqual(25, new Vector { 4, 6 }.DistanceToSquared(new Vector { 1, 2 }));
            Assert.AreEqual(25, new Vector { 6, 4 }.DistanceToSquared(new Vector { 2, 1 }));

            Vector vector = new Vector { 1.2, 5.4, 3.0 };
            Vector other = new Vector { 2.3, 0.1, 1.1 };
            
            double expected =
                Math.Pow(vector[0] - other[0], 2) +
                Math.Pow(vector[1] - other[1], 2) +
                Math.Pow(vector[2] - other[2], 2);

            Assert.AreEqual(expected, vector.DistanceToSquared(other));
        }
    }
}