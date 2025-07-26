using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test_Examen.Controllers;
using Test_Examen.Configuration.Interfaces;
using Test_Examen.Configuration.Models;
using Test_Examen.Configuration.Entities;

namespace Testing.Controllers
{
    public class ReportUserControllerTests
    {
        private readonly Mock<ILogger<ReportUserController>> _loggerMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly ReportUserController _controller;

        public ReportUserControllerTests()
        {
            _loggerMock = new Mock<ILogger<ReportUserController>>();
            _userServiceMock = new Mock<IUserService>();
            _controller = new ReportUserController(_loggerMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsOk_WithLoginData()
        {
            var userId = 1;
            var size = 2;
            var logins = new List<UserLoginDTO>
            {
                new UserLoginDTO { LoginProvider = "Test", ProviderKey = "Key1" },
                new UserLoginDTO { LoginProvider = "Test2", ProviderKey = "Key2" }
            };
            _userServiceMock.Setup(s => s.GetLoginsByUserIdAsync(userId, size)).ReturnsAsync(logins);

            var result = await _controller.Get(userId, size);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDTO<List<UserLoginDTO>>>(okResult.Value);
            Assert.Equal(logins, response.Data);
            Assert.Contains($"{size} login sessions", response.Message);
        }

        [Fact]
        public async Task Get_ReturnsOk_WithEmptyList()
        {
            var userId = 2;
            var size = 5;
            var logins = new List<UserLoginDTO>();
            _userServiceMock.Setup(s => s.GetLoginsByUserIdAsync(userId, size)).ReturnsAsync(logins);

            var result = await _controller.Get(userId, size);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDTO<List<UserLoginDTO>>>(okResult.Value);
            Assert.Empty(response.Data);
        }

        [Fact]
        public async Task Get_CallsServiceWithCorrectParameters()
        {
            var userId = 3;
            var size = 10;
            _userServiceMock.Setup(s => s.GetLoginsByUserIdAsync(userId, size)).ReturnsAsync(new List<UserLoginDTO>());

            await _controller.Get(userId, size);

            _userServiceMock.Verify(s => s.GetLoginsByUserIdAsync(userId, size), Times.Once);
        }
    }
}
