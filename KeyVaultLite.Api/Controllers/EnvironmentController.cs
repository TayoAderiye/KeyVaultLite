using KeyVaultLite.Api.Controllers.Base;
using KeyVaultLite.Application.DTOs.Requests;
using KeyVaultLite.Application.DTOs.Responses;
using KeyVaultLite.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static KeyVaultLite.Api.Helpers.RouteHelper;

namespace KeyVaultLite.Api.Controllers
{
    public class EnvironmentController(IEnvironmentService environmentService, ISecretService secretService) : BaseController
    {
        [HttpPost]
        [Route(EnvironmentRoutes.Create)]
        public async Task<IActionResult> CreateEnvironment([FromBody] CreateEnvironmentRequest request, CancellationToken cancellationToken)
        {
            var response = await environmentService.CreateEnvironmentAsync(request, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [Route(EnvironmentRoutes.Base)]
        public async Task<IActionResult> GetEnvironments(CancellationToken cancellationToken)
        {
            var response = await environmentService.ListEnvironmentsAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [Route(EnvironmentRoutes.ByName)]
        public async Task<IActionResult> GetEnvironmentByName(string name, CancellationToken cancellationToken)
        {
            var environment = await environmentService.GetEnvironmentByNameAsync(name, cancellationToken);
            if (environment == null)
            {
                return NotFound(new ErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "ENVIRONMENT_NOT_FOUND",
                        Message = $"Environment with name '{name}' not found"
                    }
                });
            }
            return Ok(new EnvironmentResponse
            {
                Id = environment.Id,
                Name = environment.Name,
                Description = environment.Description,
                CreatedAt = environment.CreatedAt,
                UpdatedAt = environment.UpdatedAt
            });
        }

        [HttpGet]
        [Route(EnvironmentRoutes.SecretsByEnvId)]
        public async Task<IActionResult> GetSecrets(Guid id, string? tag, string? search, CancellationToken cancellationToken)
        {
            var response = await secretService.ListSecretsAsync(id, tag, search, cancellationToken);
            return Ok(response);
        }
        [HttpGet]
        [Route(EnvironmentRoutes.SecretByEnvId)]
        public async Task<IActionResult> SecretByEnvId(Guid id, string name, CancellationToken cancellationToken)
        {
            var response = await secretService.GetSecretAsync(id, name, cancellationToken);
            if (response == null)
            {
                return NotFound(new ErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "SECRET_NOT_FOUND",
                        Message = $"Secret with name '{name}' not found"
                    }
                });
            }
            return Ok(response);
        }
    }
}
