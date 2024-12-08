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
        public void IdentityResultConversion_ShouldCreateSuccessfulResult()
        {
            var identityResult = IdentityResult.Success;
            var result = identityResult.ToResult();

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.Errors.Count > 0);
        }

        [TestMethod]
        public void IdentityResultConversion_ShouldCreateFailedResult()
        {
            var error1 = "An error occured";
            var error2 = "A second error occured too!";
            var identityResult = IdentityResult.Failed(new IdentityError { Description = error1 }, new IdentityError { Description = error2 });
            var result = identityResult.ToResult();

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Errors.Contains(error1));
            Assert.IsTrue(result.Errors.Contains(error2));
        }

        [TestMethod]
        public void IdentityResultConversion_WithData_ShouldCreateSuccessfulResult()
        {
            var value = 42;
            var identityResult = IdentityResult.Success;
            var result = identityResult.ToResult(value);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.Data, value);
            Assert.IsFalse(result.Errors.Count > 0);
        }

        [TestMethod]
        public void IdentityResultConversion_WithData_ShouldCreateFailedResult()
        {
            var value = 42;
            var error1 = "An error occured";
            var identityResult = IdentityResult.Failed(new IdentityError { Description = error1 });
            var result = identityResult.ToResult(value);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(result.Data, value);
            Assert.IsTrue(result.Errors.Contains(error1));
        }
    }
}
