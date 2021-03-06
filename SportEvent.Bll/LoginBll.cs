﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SportEvent.Bll.Interfaces;
using SportEvent.Bll.Model;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SportEvent.Bll
{
    public class LoginBll : ILogin
    {
        #region [Fields]

        /// <summary>
        /// The config value in appsetting.json
        /// </summary>
        private readonly IConfiguration _config;

        #endregion

        #region [Constructors]

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginBll" /> class.
        /// </summary>
        /// <param name="config">The config value.</param>
        /// <param name="dsUnitOfWork">The utilities unit of work.</param>
        public LoginBll(IConfiguration config)
        {
            _config = config;
        }

        #endregion

        #region [Methods]

        /// <summary>
        /// Validate username and password is valid.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public bool Authenticate(LoginModel login)
        {
            bool result = false;
            if (login.Username == _config["Authen:Username"] && login.Password == _config["Authen:Password"])
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Create and setting payload on token.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string BuildToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, username));

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds,
              claims: identity.Claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
