
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using ReimbursementTrackingApplication.Models.DTOs;
using ReimbursementTrackingApplication.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReimbursementUnitProjectTest.Services
{
    internal class TokenServiceTest
    {
        private string _secretKey;
        private TokenService _tokenService;
        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void Setup()
        {
           
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(config => config["JWT:SecretKey"]).Returns("hellothisisthedummysecreatekeyforreimbursement");

           
            _tokenService = new TokenService(_mockConfiguration.Object);
        }

        [Test]
        public async Task GenerateToken_ShouldReturnValidToken_WithCorrectClaims()
        {
            // Arrange
            var userTokenDto = new UserTokenDTO
            {
                Username = "testuser",
                Department = "IT"
            };

            // Act
            var token = await _tokenService.GenerateToken(userTokenDto);

            // Assert
            Assert.IsNotNull(token, "Token should not be null");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("hellothisisthedummysecreatekeyforreimbursement"); // The key you mocked

            // Validate the token
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // For testing purposes, skipping issuer validation
                ValidateAudience = false, // For testing purposes, skipping audience validation
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            // Cast the validated token to JwtSecurityToken to access its claims
            var jwtToken = (JwtSecurityToken)validatedToken;

            // Assert the claims
            Assert.AreEqual("testuser", jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.GivenName).Value);
            Assert.AreEqual("IT", jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }
    }
}
