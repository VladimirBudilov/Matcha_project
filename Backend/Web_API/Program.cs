using System.Text;
using BLL.Helpers;
using BLL.Sevices;
using DAL.Helpers;
using DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Web_API.Configurations;
using Web_API.Controllers.AutoMappers;
using Web_API.Helpers;
using Web_API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddSingleton  <ChatManager>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

// Add a JWT Bearer Authorization header to Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    options.AddPolicy("SignalRCorsPolicy",
        corsPolicyBuilder => corsPolicyBuilder
            .WithOrigins("https://localhost:8080")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddSingleton<DatabaseSettings>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ProfileRepository>();
builder.Services.AddScoped<InterestsRepository>();
builder.Services.AddScoped<PicturesRepository>();
builder.Services.AddScoped<LikesRepository>();
builder.Services.AddScoped<ProfileViewsRepository>();
builder.Services.AddScoped<UserInterestsRepository>();

builder.Services.AddScoped<ServiceValidator>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<PasswordManager>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ActionService>();
builder.Services.AddScoped<ClaimsService>();

builder.Services.AddScoped<QueryBuilder>();
builder.Services.AddScoped<FameRatingCalculator>();
builder.Services.AddScoped<EntityCreator>();
builder.Services.AddScoped<TableFetcher>();
builder.Services.AddScoped<ParameterInjector>();
builder.Services.AddScoped<DtoValidator>();

builder.Services.AddMvc(options => options.Filters.Add(new ExceptionHadlerFilter()));

//builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("SmtpConfig"));

builder.Services.AddAutoMapper(typeof(AutomapperProfile));

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.AddAuthentication(options =>
    {    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration[$"JwtConfig:Secret"] ?? throw new InvalidOperationException());    
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
    };
    jwt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
 
            // если запрос направлен хабу
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
            {
                // получаем токен из строки запроса
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.

app.UseCors("SignalRCorsPolicy");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHub<Chat>("/chat");

app.Run();