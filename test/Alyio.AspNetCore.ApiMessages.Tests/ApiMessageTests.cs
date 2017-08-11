using System;
using Xunit;

namespace Alyio.AspNetCore.ApiMessages.Tests
{
    public class ApiMessageTests
    {
        [Fact]
        public void Message_Test()
        {
            Assert.Throws<ArgumentException>(() => new ApiMessage { Message = null });
            Assert.Throws<ArgumentException>(() => new ApiMessage { Message = string.Empty });
            Assert.Throws<ArgumentException>(() => new ApiMessage { Message = "" });

            Assert.Throws<ArgumentException>(() => new BadRequestMessage((string)null));
            Assert.Throws<ArgumentException>(() => new BadRequestMessage(string.Empty));
            Assert.Throws<ArgumentException>(() => new BadRequestMessage(""));
        }
    }
}
