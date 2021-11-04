using ServiceBusDriver.Core.Constants;
using Shouldly;
using Xunit;

namespace ServiceBusDriver.Core.Tests
{
    public class SbDriverExceptionFactoryTests
    {

        [Fact]
        public void CreateValidationFailedException_ShouldCreateException()
        {
            var ex = SbDriverExceptionFactory.CreateValidationFailedException("I am a validation error");

            ex.ErrorMessage.Code.ShouldBe(ErrorConstants.BadRequestErrorCode);
            ex.ErrorMessage.UserMessageText.ShouldBe("I am a validation error");
            ex.Message.ShouldBe("I am a validation error");
        }

        [Fact]
        public void CreateBadRequestException_ShouldCreateException()
        {
            var ex = SbDriverExceptionFactory.CreateBadRequestException("I am a validation error");

            ex.ErrorMessage.Code.ShouldBe(ErrorConstants.BadRequestErrorCode);
            ex.ErrorMessage.UserMessageText.ShouldBe("I am a validation error");
            ex.Message.ShouldBe("I am a validation error");

            ex = SbDriverExceptionFactory.CreateBadRequestException(null);

            ex.ErrorMessage.Code.ShouldBe(ErrorConstants.BadRequestErrorCode);
            ex.ErrorMessage.UserMessageText.ShouldBe(ErrorConstants.BadRequestErrorMessage);
        }

    }
}
