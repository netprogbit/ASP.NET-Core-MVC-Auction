using AuctionMvc.Helpers;
using AuctionMvc.Models;
using AuctionMvc.Settings;
using DataLayer.Entities;
using DataLayer.UnitOfWork;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuctionMvc.Services
{
    /// <summary>
    /// Authenticate service
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IOptions<AppSettings> appSettings, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// User registration
        /// </summary>
        public async Task<bool> RegisterAsync(RegisterViewModel userVM)
        {
            // Check if the user exists with this email

            User candidate = await _unitOfWork.Users.FindAsync(u => u.Email == userVM.Email);

            if (candidate != null)
                return false;

            User user = new User { FirstName = userVM.FirstName, LastName = userVM.LastName, Email = userVM.Email, Password = GetPasswordHash(userVM.Password, _appSettings.Secret), Role = "user" };

            // DB Transaction
            using (var dbContextTransaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    await _unitOfWork.Users.CreateAsync(user);
                    await _unitOfWork.SaveAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();  // Rollbacking DB                     
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }

            return true;
        }

        /// <summary>
        /// User authentication
        /// </summary>
        public async Task<AccessToken> Authenticate(string email, string password)
        {
            // Check if the user exists with this email and password

            User candidate = await _unitOfWork.Users.FindAsync(u => u.Email == email && u.Password == GetPasswordHash(password, _appSettings.Secret));

            if (candidate == null)
                return null;

            // Generate JWT token            

            var tokeOptions = new JwtSecurityToken(
                claims: new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, candidate.Role)
                },
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: new SigningCredentials(JwtHelper.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            string token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return new AccessToken { UserId = candidate.Id, Token = token };
        }

        /// <summary>
        /// Password encription
        /// </summary>
        private string GetPasswordHash(string password, string key)
        {
            string computedHash;

            using (var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }

            return computedHash;
        }

    }
}
