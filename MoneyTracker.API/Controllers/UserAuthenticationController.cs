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
            var token = ControllerHelper.GetToken(_httpContextAccessor);
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }

            throw new InvalidDataException("Not authorised. Add token.");
        }

        [HttpPost]
        [Route("authenticate")]
        public Task<string> GemerateAuthToken([FromBody] LoginWithUsernameAndPassword user)
        {
            return _service.GenerateToken(user);
        }

        [HttpPost]
        [Route("decodeToken")]
        public Task<AuthenticatedUser> DecodeAuthToken()
        {
            var token = ControllerHelper.GetToken(_httpContextAccessor);
            if (!string.IsNullOrEmpty(token))
            {
                return _service.DecodeToken(token);
            }

            throw new InvalidDataException("Not authorised. Add token.");
        }
    }
}
