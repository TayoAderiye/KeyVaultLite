using KeyVaultLite.Api.Controllers.Base;
using KeyVaultLite.Application.DTOs.Requests;
using KeyVaultLite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static KeyVaultLite.Api.Helpers.RouteHelper;

namespace KeyVaultLite.Api.Controllers
{
    public class EncryptionKeyController(IEncryptionKeyService encryptionKeyService) : BaseController
    {
        [HttpPost]
        [Route(EncryptionKeyRoutes.Create)]
        public async Task<IActionResult> CreateEncryptionKey([FromBody] CreateEncryptionKeyRequest request, CancellationToken cancellationToken)
        {
            var response = await encryptionKeyService.CreateAsync(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [Route(EncryptionKeyRoutes.Base)]
        public async Task<IActionResult> GetKeys(CancellationToken cancellationToken, [FromQuery] bool includeInactive = false)
        {
            var response = await encryptionKeyService.GetAllAsync(includeInactive, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [Route(EncryptionKeyRoutes.ById)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var key = await encryptionKeyService.GetByIdAsync(id, cancellationToken);
            return Ok(key);
        }
    }
}
