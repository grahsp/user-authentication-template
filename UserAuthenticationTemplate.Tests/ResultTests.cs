using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using UserAuthenticationTemplate.Extensions;
using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Tests
{
    [TestClass]
    public class ResultTests
    {
        [TestMethod]
        public void Success_ShouldCreateSuccessfulResult()
        {
            var result = Result.Success();

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsFailure);
            Assert.IsFalse(result.Errors.Count > 0);
        }

        [TestMethod]
        public void Failure_ShouldCreateFailedResult_WithErrorMessage()
        {
            var errorMessage = "An error occurred";
            var result = Result.Failure(errorMessage);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Errors.Contains(errorMessage));
        }

        [TestMethod]
        public void Success_WithData_ShouldCreateSuccessfulResultWithData()
        {
            var value = 42;
            var result = Result<int>.Success(value);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsFailure);
            Assert.AreEqual(value, result.Data);
        }

        [TestMethod]
        public void Failure_ShouldCreateFailedResult_WithErrorMessageAndNoData()
        {
            var errorMessage = "Something went wrong";
            var result = Result<string>.Failure(errorMessage);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.IsNull(result.Data);
            Assert.IsTrue(result.Errors.Contains(errorMessage));
        }

        [TestMethod]
        public void Failure_WithMultipleErrors_ShouldIncludeAllErrors()
        {
            var result = Result.Failure("Error 1", "Error 2", "Error 3");

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(3, result.Errors.Count);
            Assert.IsTrue(result.Errors.Contains("Error 1"));
            Assert.IsTrue(result.Errors.Contains("Error 2"));
            Assert.IsTrue(result.Errors.Contains("Error 3"));
        }

        [TestMethod]
        public void ImplicitConversion_ShouldCreateSuccessfulResultWithData()
        {
            var value = 42;
            Result<int> result = value;

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsFailure);
            Assert.AreEqual(value, result.Data);
        }

        [TestMethod]
        public void IdentityResultConversion_ShouldCreateEqualResult()
        {
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "An error occured" }, new IdentityError { Description = "A second error occured too!" });
            var result = identityResult.ToResult();

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Errors.Contains("An error occured"));
            Assert.IsTrue(result.Errors.Contains("A second error occured too!"));
        }
    }
}
