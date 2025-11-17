using KeyVaultLite.Api.Extensions;
using KeyVaultLite.Application.Extension;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog logging
builder.AddLoggerProvider();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddServices();

var app = builder.Build();


// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseRouting();
app.UseSiteCors();

await app.UseServicesAsync();

await app.RunAsync();

