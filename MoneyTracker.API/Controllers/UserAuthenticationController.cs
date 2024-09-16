using Microsoft.AspNetCore.Mvc;

namespace MoneyTracker.API.Controllers
{
    [ApiController]
    [Route("/api/userAuth/")]
    public class UserAuthenticationController : ControllerBase
    {

        private readonly ILogger<UserAuthenticationController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAuthenticationController(ILogger<UserAuthenticationController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
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
    }
}
