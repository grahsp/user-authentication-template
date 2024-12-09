using UserAuthenticationTemplate.Models;

namespace UserAuthenticationTemplate.Tests
{
    [TestClass]
    public class ResultTests
    {
        #region Constructor
        // Result
        [TestMethod]
        public void Result_Constructor_Success_ShouldInitializeCorrectly()
        {
            var result = new Result(true);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public void Result_Constructor_FailureWithErrors_ShouldInitializeCorrectly()
        {
            var result = new Result(false, "ERROR");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
        }

        [TestMethod]
        public void Result_Constructor_FailreWithManyErrors_ShouldInitializeCorrectly()
        {
            var result = new Result(false, "Error1", "Error2", "Error3");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Errors.Length == 3);
        }

        // Result<T>
        [TestMethod]
        public void ResultT_Constructor_SuccessWithValidData_ShouldInitializeCorrectly()
        {
            var result = new Result<int>(true, 42);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public void Result_Constructor_SuccessWithNullData_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Result<string>(true, null));
        }

        [TestMethod]
        public void ResultT_Constructor_FailureWithErrors_ShouldInitializeCorrectly()
        {
            var result = new Result<string>(false, null, "ERROR");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
        }
        #endregion

        #region Properties
        // Result
        [TestMethod]
        public void IsFailure_OppositeOfIsSuccess()
        {
            var result = Result.Success();
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.IsSuccess, !result.IsFailure);

            result = Result.Failure("Error");
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(!result.IsSuccess, result.IsFailure);
        }

        [TestMethod]
        public void HasErrors_WhenResultHasError_ShouldBeTrue()
        {
            var result = Result.Success();
            Assert.IsFalse(result.HasErrors);

            result = Result.Failure("Error");
            Assert.IsTrue(result.HasErrors);
        }

        // Result<T>
        [TestMethod]
        public void Data_WhenResultIsSuccessful_ShouldReturnData()
        {
            var result = new Result<int>(true, 42);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.Data, 42);
        }

        [TestMethod]
        public void Data_WhenResultIsFailed_ShouldThrowArgumentNullException()
        {
            var result = new Result<int>(false, 42);

            Assert.ThrowsException<ArgumentNullException>(() => result.Data);
        }
        #endregion

        #region Factory Methods
        // Result
        [TestMethod]
        public void Result_Success_ShouldCreateSuccessfulResult()
        {
            var result = Result.Success();

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
        }

        [TestMethod]
        public void Result_Failure_ShouldCreateFailureResult()
        {
            var result = Result.Failure("Error");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
        }

        // Result<T>
        [TestMethod]
        public void ResultT_Success_ShouldCreateSuccessfulResult_WithData()
        {
            var value = 42;
            var result = Result<int>.Success(value);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.HasErrors);
            Assert.AreEqual(result.Data, value);
        }

        [TestMethod]
        public void ResultT_Failure_ShouldCreateFailureResult_WithoutErrors()
        {
            var result = Result<int>.Failure("Error");

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.HasErrors);
        }
        #endregion

        #region Methods
        // Result
        [TestMethod]
        public void Result_Merge_ShouldReturnTrue_WhenAllValuesAreTrue()
        {
            var success = Result.Merge(true, true, true);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Result_Merge_ShouldReturnFalse_WhenAnyValueIsFalse()
        {
            var success = Result.Merge(true, true, false);

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void Result_Merge_ShouldReturnTrue_WhenSuccessfulResult()
        {
            var result = Result.Success();
            var success = Result.Merge(true, true, result);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Result_Merge_ShouldReturnFalse_WhenFailedResult()
        {
            var result = Result.Failure();
            var success = Result.Merge(true, true, result);

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void Result_Merge_ShouldReturnTrue_WhenSuccessfulResultT()
        {
            var result = Result<int>.Success(42);
            var success = Result.Merge(true, true, result);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Result_Merge_ShouldReturnFalse_WhenFailedResultT()
        {
            var result = Result<int>.Failure();
            var success = Result.Merge(true, true, result);

            Assert.IsFalse(success);
        }

        // Result<T>
        [TestMethod]
        public void ResultT_FromData_ShouldCreateSuccessfulResult_WhenDataNotNull()
        {
            var value = 42;
            var result = Result<int>.FromData(value);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.Data, value);
        }

        [TestMethod]
        public void ResultT_FromData_ShouldCreateFailedResult_WhenDataNull()
        {
            var result = Result<string>.FromData(null);

            Assert.IsTrue(result.IsFailure);
            Assert.ThrowsException<ArgumentNullException>(() => result.Data);
        }
        #endregion

        #region Implicit Conversion
        // Result
        [TestMethod]
        public void Result_ImplicitConversion_ReturnTrue_WhenSuccessfulResult()
        {
            bool success = Result.Success();

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void Result_ImplicitConversion_ReturnFalse_WhenFailedResult()
        {
            bool success = Result.Failure();

            Assert.IsFalse(success);
        }

        // Result<T>
        [TestMethod]
        public void ResultT_ImplicitConversion_SuccessfulResult_WhenDataIsNotNull()
        {
            var value = 42;
            Result<int> result = value;

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.Data, value);
        }

        [TestMethod]
        public void ResultT_ImplicitConversion_ThrowArgumentNullException_WhenDataIsNull()
        {
            Result<string> result;
            Assert.ThrowsException<ArgumentNullException>(() => result = (string)null!);
        }
        #endregion

        #region ToString
        // Result
        [TestMethod]
        public void Result_ToString_ReturnSuccessString_WhenSuccessfulResult()
        {
            var result = Result.Success();

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.ToString(), "Success");
        }

        [TestMethod]
        public void Result_ToString_ReturnFailureString_WhenFailedReslt()
        {
            var result = Result.Failure("Error1", "Error2", "Error3");

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(result.ToString(), "Failure: Error1, Error2, Error3");
        }

        // Result<T>
        [TestMethod]
        public void ResultT_ToString_ReturnSuccessString_WhenSuccessfulResult()
        {
            var result = Result<int>.Success(42);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(result.ToString(), "Success: 42");
        }

        [TestMethod]
        public void ResultT_ToString_ReturnFailureString_WhenFailedResult()
        {
            var result = Result<int>.Failure("Error1", "Error2", "Error3");

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(result.ToString(), "Failure: Error1, Error2, Error3");
        }
        #endregion
    }
}
