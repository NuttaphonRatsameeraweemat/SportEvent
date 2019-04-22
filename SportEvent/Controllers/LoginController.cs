using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SportEvent.Bll.Interfaces;
using SportEvent.Bll.Model;

namespace SportEvent.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        #region Fields

        /// <summary>
        /// The Login manager provides Login functionality.
        /// </summary>
        private ILogin _login;

        #endregion

        #region Constructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="LoginController" /> class.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="login"></param>
        public LoginController(IConfiguration config, ILogin login)
        {
            _login = login;
        }

        #endregion

        #region Methods

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody]LoginModel auth)
        {
            IActionResult response = Unauthorized();

            if (_login.Authenticate(auth))
            {
                var tokenString = _login.BuildToken(auth.Username);
                response = Ok(tokenString);
            }

            return response;
        }

        #endregion

    }
}