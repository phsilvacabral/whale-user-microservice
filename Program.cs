using users_service.Repositories;
using users_service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Users Service API", Version = "v1" });
});

// Database
builder.Services.AddScoped<IDatabaseService, DatabaseService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Root endpoint - API information
app.MapGet("/", () => new
{
    message = "👤 Whale Users Service API",
    version = "1.0.0",
    description = "Microserviço de gerenciamento de usuários",
    endpoints = new
    {
        health = "/health",
        documentation = "/swagger",
        auth = "/api/Auth",
        users = "/api/Users"
    },
    timestamp = DateTime.UtcNow,
    status = "OK"
});

// Health check
app.MapGet("/health", () => new { 
    status = "OK", 
    timestamp = DateTime.UtcNow,
    service = "users-service-dotnet"
});

app.Run();
