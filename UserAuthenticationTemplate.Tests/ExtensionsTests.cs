using Microsoft.AspNetCore.Identity;
using UserAuthenticationTemplate.Extensions;
using UserAuthenticationTemplate.Models;

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
