using Microsoft.AspNetCore.Identity;
using UserAuthenticationTemplate.Extensions;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Tests
{
    [TestClass]
    public class ExtensionsTests
    {

        #region ResultExtensions
        // Result
        [TestMethod]
        public void ToResult_ReturnSuccess_WhenTrue()
        {
            var isSuccess = true;
            var errorMessage = "This message should not be stored.";

            var result = isSuccess.ToResult(errorMessage);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public void ToResult_ReturnFailed_WhenFalse()
        {
            var isSuccess = false;
            var errorMessage = "This message will be stored in Errors.";

            var result = isSuccess.ToResult(errorMessage);

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
            Assert.AreEqual(result.Errors.Single(), errorMessage);
        }

        // Result<T>
        [TestMethod]
        public void ToResultT_ReturnSuccess_WhenDataIsNotNull()
        {
            var data = 42;

            var result = data.ToResult("Error");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(data, result.Data);
        }

        [TestMethod]
        public void ToResultT_ReturnFailure_WhenDataIsNull()
        {
            int? data = null;

            var result = data.ToResult("Error");

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Error", result.Errors.Single());
        }

        [TestMethod]
        public void ToResultT_UseFailureMessage_WhenDataIsNull()
        {
            string? data = null;
            string errorMessage = "An error occured";

            var result = data.ToResult(errorMessage);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(errorMessage, result.Errors.Single());
        }

        [TestMethod]
        public void ToResultT_ReturnSuccess_WithDifferentTypes()
        {
            var intData = 42;
            var stringData = "Hello, World!";

            var intResult = intData.ToResult("Error");
            var stringResult = stringData.ToResult("Error");

            Assert.IsTrue(intResult.IsSuccess);
            Assert.AreEqual(intData, intResult.Data);

            Assert.IsTrue(stringResult.IsSuccess);
            Assert.AreEqual(stringData, stringResult.Data);
        }
        #endregion

        #region IdentityResultsExtensions
        // Result
        [TestMethod]
        public void ToResult_ReturnsSuccess_WhenIdentityResultIsSuccessful()
        {
            var identityResult = IdentityResult.Success;

            var result = identityResult.ToResult();

            Assert.IsInstanceOfType<Result>(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public void ToResult_ReturnsFailure_WhenIdentityResultFails()
        {
            var errors = new[] { new IdentityError { Description = "Error 1" }, new IdentityError { Description = "Error 2" } };
            var identityResult = IdentityResult.Failed(errors);

            var result = identityResult.ToResult();

            Assert.IsTrue(result.IsFailure);
            CollectionAssert.AreEquivalent(new[] { "Error 1", "Error 2" }, result.Errors);
        }

        [TestMethod]
        public void ToResult_HandlesEmptyErrors_WhenIdentityResultFails()
        {
            var identityResult = IdentityResult.Failed();

            var result = identityResult.ToResult();

            Assert.IsTrue(result.IsFailure);
            Assert.IsFalse(result.HasErrors);
        }

        // Result<T>
        [TestMethod]
        public void ToResultT_ReturnsSuccess_WhenIdentityResultIsSuccessful()
        {
            var identityResult = IdentityResult.Success;
            var data = 42;

            var result = identityResult.ToResult(data);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(data, result.Data);
        }

        [TestMethod]
        public void ToResultT_ReturnsFailure_WhenIdentityResultFails()
        {
            var errors = new[] { new IdentityError { Description = "Error 1" }, new IdentityError { Description = "Error 2" } };
            var identityResult = IdentityResult.Failed(errors);
            var data = 42;

            var result = identityResult.ToResult(data);

            Assert.IsTrue(result.IsFailure);
            Assert.ThrowsException<ArgumentNullException>(() => result.Data);
            CollectionAssert.AreEquivalent(new[] { "Error 1", "Error 2" }, result.Errors);
        }
        #endregion
    }
}
