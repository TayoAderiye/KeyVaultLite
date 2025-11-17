using KeyVaultLite.Api.Controllers.Base;
using KeyVaultLite.Application.DTOs.Requests;
using KeyVaultLite.Application.DTOs.Responses;
using KeyVaultLite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static KeyVaultLite.Api.Helpers.RouteHelper;

namespace KeyVaultLite.Api.Controllers
{
    public class SecretsController(ISecretService secretService) : BaseController
    {
        [HttpPost]
        [Route(SecretsRoutes.Create)]
        public async Task<IActionResult> CreateSecret([FromBody] CreateSecretRequest request, CancellationToken cancellationToken)
        {
            var response = await secretService.CreateSecretAsync(request, cancellationToken);
            return Ok(response);
        }
        [HttpGet]
        [Route(SecretsRoutes.ById)]
        public async Task<IActionResult> GetSecrets(Guid envId, Guid secretId, CancellationToken cancellationToken)
        {
            //public const string ById = Base + "/{envId:guid}/{secretId:guid}";
            var response = await secretService.GetSecretAsync(envId, secretId, cancellationToken);
            if (response == null)
            {
                return NotFound(new ErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "SECRET_NOT_FOUND",
                        Message = $"Secret with id '{secretId}' not found"
                    }
                });
            }
            return Ok(response);
        }
        [HttpGet]
        [Route(SecretsRoutes.Reveal)]
        public async Task<IActionResult> SecretByEnvId(Guid envId, Guid secretId, CancellationToken cancellationToken)
        {
            var response = await secretService.RevealSecretAsync(envId, secretId, cancellationToken);
            if (response == null)
            {
                return NotFound(new ErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "SECRET_NOT_FOUND",
                        Message = $"Secret with id '{secretId}' not found"
                    }
                });
            }
            return Ok(response);
        }
    }
}
