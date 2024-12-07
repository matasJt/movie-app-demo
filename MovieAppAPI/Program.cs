using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using MovieAppAPI.Data;
using MovieAppAPI.Endpoints;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieAppAPI.Auth;
using MovieAppAPI.Auth.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<JwtTokenGenerator>();
builder.Services.AddScoped<AuthSeeder>();
builder.Services.AddDbContext<MoviesDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddCors(policy=> policy.AddDefaultPolicy(options=> options.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddIdentity<User, IdentityRole>()
	.AddEntityFrameworkStores<MoviesDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ClockSkew = TimeSpan.Zero,
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
		ValidAudience = builder.Configuration["Jwt:ValidAudience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
	};

});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors();
app.AddAuth();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
	await next();

	if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Request.Path.Value.Contains("/auth"))
	{
		if (context.Request.Path.StartsWithSegments("/api/tags") && !context.Request.Path.Value.Contains("/movies"))
		{
			await context.Response.HttpContext.Response.WriteAsync("Tag was not found");

		}
		else if (context.Request.Path.StartsWithSegments("/api/movies") && !context.Request.Path.Value.Contains("/reviews"))
		{
			await context.Response.HttpContext.Response.WriteAsync("Movie was not found");
		}
		else if (context.Request.Path.Value.Contains("/reviews"))
		{
			await context.Response.HttpContext.Response.WriteAsync("Review was not found for movie");
		}
		else
		{
			await context.Response.WriteAsync("URL was not found");
		}
	}
});

var tagsGroup = app.MapGroup("/api").AddFluentValidationAutoValidation();
var moviesGroup = app.MapGroup("/api").AddFluentValidationAutoValidation();
var reviewsGroup = app.MapGroup("/api/").AddFluentValidationAutoValidation();

TagEndpoints.MapTagEndpoints(tagsGroup);
MovieEndpoints.MapMovieEndpoints(moviesGroup);
ReviewEndpoints.MapReviewEndpoints(reviewsGroup);


var scope = app.Services.CreateScope();
var seedData = scope.ServiceProvider.GetRequiredService<AuthSeeder>();
await seedData.SeedAsync();

app.Run();