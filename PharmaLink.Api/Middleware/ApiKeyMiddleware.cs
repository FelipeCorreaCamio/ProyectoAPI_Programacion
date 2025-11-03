using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PharmaLink.Api.Utils;
using System.Text.Json;

namespace PharmaLink.Api.Middleware
{
	public class ApiKeyMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly string _apiKey;

		public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
		{
			_next = next;
			_apiKey = configuration["ApiKey"] ?? string.Empty;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

			// Solo protejo rutas específicas (no lo hago global para que otras rutas puedan quedar abiertas)
			if (path.StartsWith("/api/recetas") || path.StartsWith("/api/dispensaciones") || path.StartsWith("/api/reposiciones"))
			{
				if (!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
				{
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
					var err = new ErrorResponse { Code = "missing_api_key", Message = "API Key required" };
					context.Response.ContentType = "application/json";
					await context.Response.WriteAsync(JsonSerializer.Serialize(err));
					return;
				}

				if (string.IsNullOrEmpty(_apiKey) || !_apiKey.Equals(extractedApiKey))
				{
					context.Response.StatusCode = StatusCodes.Status403Forbidden;
					var err = new ErrorResponse { Code = "invalid_api_key", Message = "API Key inválida" };
					context.Response.ContentType = "application/json";
					await context.Response.WriteAsync(JsonSerializer.Serialize(err));
					return;
				}
			}

			await _next(context);
		}
	}
}

