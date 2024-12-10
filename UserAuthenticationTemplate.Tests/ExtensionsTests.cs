using UserAuthenticationTemplate.Extensions;

namespace UserAuthenticationTemplate.Tests
{
    [TestClass]
    public class ExtensionsTests
    {
        #region ResultExtensions
        [TestMethod]
        public void CreateResult_ReturnsSuccess_WhenDataIsNotNull()
        {
            var data = 42;

            var result = data.CreateResult();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(data, result.Data);
        }

        [TestMethod]
        public void CreateResult_ReturnsFailure_WhenDataIsNull()
        {
            int? data = null;

            var result = data.CreateResult();

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Data must not be null", result.Errors.Single());
        }

        [TestMethod]
        public void CreateResult_UsesCustomFailureMessage_WhenDataIsNull()
        {
            string? data = null;
            string errorMessage = "An error occured";

            var result = data.CreateResult(errorMessage);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(errorMessage, result.Errors.Single());
        }

        [TestMethod]
        public void CreateResult_ReturnsSuccess_WithDifferentTypes()
        {
            var intData = 42;
            var stringData = "Hello, World!";

            var intResult = intData.CreateResult();
            var stringResult = stringData.CreateResult();

            Assert.IsTrue(intResult.IsSuccess);
            Assert.AreEqual(intData, intResult.Data);

            Assert.IsTrue(stringResult.IsSuccess);
            Assert.AreEqual(stringData, stringResult.Data);
        }
        #endregion
    }
}
