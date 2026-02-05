using LABHTTP.Data;
using LABHTTP.Model.DTO;
using LABHTTP.Repository;
using LABHTTP.Services;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LABHTTP.TEST.Services
{
    public class UserServiceTests
    {
        private readonly UserService _service;

        public UserServiceTests()
        {
            var repoMock = new Mock<IUserRepository>();
            _service = new UserService(repoMock.Object);
        }

        [Fact]
        public void GenerateJwtToken_ReturnsToken()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "test@example.com",
                Role = "User"
            };

            // Act
            var token = _service.GenerateJwtToken(user);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void GenerateJwtToken_ContainsCorrectClaims()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "test@example.com",
                Role = "Admin"
            };

            // Act
            var tokenString = _service.GenerateJwtToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenString);

            // Assert
            Assert.Equal(user.UserId.ToString(),
                jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            Assert.Equal(user.Email,
                jwt.Claims.First(c => c.Type == ClaimTypes.Email).Value);

            Assert.Equal(user.Role,
                jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        [Fact]
        public void GenerateJwtToken_HasCorrectIssuerAndAudience()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "issuer@test.com",
                Role = "User"
            };

            // Act
            var tokenString = _service.GenerateJwtToken(user);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);

            // Assert
            Assert.Equal("yourIssuer", jwt.Issuer);
            Assert.Contains("yourAudience", jwt.Audiences);
        }

        [Fact]
        public void GenerateJwtToken_HasExpiration()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "exp@test.com",
                Role = "User"
            };

            // Act
            var tokenString = _service.GenerateJwtToken(user);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);

            // Assert
            Assert.True(jwt.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateJwtToken_HasValidSignature()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = "sign@test.com",
                Role = "User"
            };

            var tokenString = _service.GenerateJwtToken(user);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "yourIssuer",

                ValidateAudience = true,
                ValidAudience = "yourAudience",

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("TimaRadexxSuperSecretKeyTestingBeforeDeploy")
                    )
            };

            var handler = new JwtSecurityTokenHandler();

            // Act
            handler.ValidateToken(tokenString, validationParameters, out _);

            // Assert
            // No exception = valid token
            Assert.True(true);
        }
        [Fact]
        public void PasswordHasher_GeneratesDifferentHash()
        {
            var hasher = new PasswordHasher();

            var hash1 = hasher.Generate("Password123!");
            var hash2 = hasher.Generate("Password123!");

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void PasswordHasher_Verify_ValidPassword_ReturnsTrue()
        {
            var hasher = new PasswordHasher();
            var hash = hasher.Generate("Password123!");

            Assert.True(hasher.Verify("Password123!", hash));
        }
    }
}
