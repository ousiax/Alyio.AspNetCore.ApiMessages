using System;
using Xunit;

namespace Alyio.AspNetCore.ApiMessages.Tests
{
    public class ApiMessageTests
    {
        [Fact]
        public void Message_Test()
        {
#pragma warning disable CS8600,CS8625
            Assert.Throws<ArgumentException>(() => new ApiMessage { Message = null });
#pragma warning restore CS8600, CS8625
            Assert.Throws<ArgumentException>(() => new ApiMessage { Message = string.Empty });
            Assert.Throws<ArgumentException>(() => new ApiMessage { Message = "" });

#pragma warning disable CS8600,CS8625
            Assert.Throws<ArgumentException>(() => new BadRequestMessage((string)null));
#pragma warning restore CS8600,CS8625
            Assert.Throws<ArgumentException>(() => new BadRequestMessage(string.Empty));
            Assert.Throws<ArgumentException>(() => new BadRequestMessage(""));
        }
    }
}
