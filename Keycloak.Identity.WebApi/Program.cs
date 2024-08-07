using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Keycloak", Version = "v1" });
});

// Configure Keycloak settings
var keycloakConfig = builder.Configuration.GetSection("Keycloak").Get<KeycloakConfig>();
builder.Services.AddTransient<IAuthService, AuthService>(); // Register AuthService

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.Authority = $"{keycloakConfig.HostName}/realms/{keycloakConfig.Realm}";
	options.Audience = keycloakConfig.ClientId;
	options.RequireHttpsMetadata = false;
});

builder.Services.AddHttpClient(); // Add HttpClient service
builder.Services.AddSingleton(keycloakConfig); // Register KeycloakConfig as a singleton
builder.Services.AddTransient<IKeycloakService, KeycloakService>(); // Register KeycloakService
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRoleService, RoleService>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Keycloak v1"));
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

