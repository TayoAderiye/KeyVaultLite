using KeyVaultLite.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace KeyVaultLite.Api.Controllers.Base
{
    [Route(RouteHelper.Version1)]
    [ApiController]
    public class BaseController : Controller
    {
    }
}
