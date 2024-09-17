using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Core;

namespace MoneyTracker.API.Controllers
{
#if (!DEBUG)
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [ApiController]
    [Route("/api/userAuth/")]
    public class UserAuthenticationController : ControllerBase
    {

        private readonly ILogger<UserAuthenticationController> _logger;
        private readonly IUserAuthenticationService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAuthenticationController(ILogger<UserAuthenticationController> logger,
            IUserAuthenticationService userAuthenticationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _service = userAuthenticationService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("getToken")]
        public string RepeatAuthTokenBack()
        {
            var authHeader = _httpContextAccessor.HttpContext.Request
                .Headers.Authorization.ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            throw new InvalidDataException("Not authorised. Add token.");
        }

        [HttpPost]
        [Route("authenticate")]
        public Task<string> GemerateAuthToken([FromBody] UnauthenticatedUser user)
        {
            return _service.GenerateToken(user);
        }

        [HttpPost]
        [Route("decodeToken")]
        public AuthenticatedUser DecodeAuthToken()
        {
            var authHeader = _httpContextAccessor.HttpContext.Request
                .Headers.Authorization.ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return _service.DecodeToken(authHeader.Substring("Bearer ".Length).Trim());
            }

            throw new InvalidDataException("Not authorised. Add token.");
        }
    }
}
