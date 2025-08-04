using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Test_Examen.Controllers.Authentication;
using Test_Examen.Configuration.Interfaces;
using Test_Examen.Configuration.Models;
using Test_Examen.Configuration.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Testing.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new AuthController(_userServiceMock.Object);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenCredentialIsNull()
        {
            var result = await _controller.Login(null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(((ResponseDTO<string>)badRequest.Value).Success);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenAuthenticationSucceeds()
        {
            var request = new AuthenticationRequest { UserName = "user", Password = "pass" };
            var response = new AuthenticateResponse(new AppUser(), "token", "refresh");
            _userServiceMock.Setup(s => s.AuthenticateAsync(request)).ReturnsAsync(response);

            var result = await _controller.Login(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenExceptionThrown()
        {
            var request = new AuthenticationRequest { UserName = "user", Password = "pass" };
            _userServiceMock.Setup(s => s.AuthenticateAsync(request)).ThrowsAsync(new System.Exception("error"));

            var result = await _controller.Login(request);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(((ResponseDTO<string>)badRequest.Value).Success);
        }

        [Fact]
        public async Task SignIn_ReturnsOk_WhenUserCreated()
        {
            var signInRequest = new SignInRequest { FirstName = "A", LastName = "B", EMail = "a@b.com", Password = "pass" };
            _userServiceMock.Setup(s => s.AddUserAsync(signInRequest)).ReturnsAsync(true);

            var result = await _controller.SignIn(signInRequest);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User created correctly", ((ResponseDTO<string>)okResult.Value).Data);
        }

        [Fact]
        public async Task SignIn_ReturnsOk_WhenExceptionThrown()
        {
            var signInRequest = new SignInRequest { FirstName = "A", LastName = "B", EMail = "a@b.com", Password = "pass" };
            _userServiceMock.Setup(s => s.AddUserAsync(signInRequest)).ThrowsAsync(new System.Exception("fail"));

            var result = await _controller.SignIn(signInRequest);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.False(((ResponseDTO<string>)okResult.Value).Success);
        }

        [Fact]
        public async Task Refresh_ReturnsBadRequest_WhenDataIsNull()
        {
            var result = await _controller.Refresh(null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.False(((ResponseDTO<string>)badRequest.Value).Success);
        }

        [Fact]
        public async Task Refresh_ReturnsOk_WhenRefreshSucceeds()
        {
            var refreshRequest = new RefreshTokenRequest("token", "refresh");
            var response = new AuthenticateResponse(new AppUser(), "token", "refresh");
            _userServiceMock.Setup(s => s.RefreshAuthenticationAsync(refreshRequest.Token, refreshRequest.Refresh)).ReturnsAsync(response);

            var result = await _controller.Refresh(refreshRequest);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task Refresh_ReturnsBadRequest_WhenExceptionThrown()
        {
            var refreshRequest = new RefreshTokenRequest("token", "refresh");
            _userServiceMock.Setup(s => s.RefreshAuthenticationAsync(refreshRequest.Token, refreshRequest.Refresh)).ThrowsAsync(new System.Exception("fail"));

            var result = await _controller.Refresh(refreshRequest);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("fail", badRequest.Value);
        }

        [Fact]
        public void GetName_ReturnsOk_WithAuthenticatedUserName()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = _controller.GetName();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("User Authenticated: TestUser", ((ResponseDTO<string>)okResult.Value).Data);
        }
    }
}
