using EmotionAPI.Services;
using Firebase.Auth;
using FirebaseAdminAuthentication.DependencyInjection.Extensions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.WithOrigins("http://localhost:5173", "https://localhost:5173")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Path to the service account JSON key
string pathToCredential = "./AdminSDK.json";

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToCredential);

FirebaseApp.Create(new AppOptions()
{
    Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(pathToCredential)
});


builder.Services.AddSingleton<FirestoreDb>(sp =>
{
    // Initialize Firestore
    return FirestoreDb.Create("emocoes-4f9b5");
});



// Configure o esquema de autenticação e validadores de token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure o pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseMiddleware<FirebaseAuthMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
