using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

public class FirebaseAuthMiddleware
{
    private readonly RequestDelegate _next;

    public FirebaseAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("📌 Headers recebidos:");
        foreach (var header in context.Request.Headers)
        {
            Console.WriteLine($"{header.Key}: {header.Value}");
        }

        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Authorization token is missing.");
            return;
        }
        token = token.Replace("Bearer ", "").Trim();

        Console.WriteLine($"🔍 Token recebido: {token}");

        try
        {
            // Verifica o token na firebase
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            context.Items["User"] = decodedToken;
        }
        catch (Exception)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid or expired token.");
            return;
        }

        await _next(context);
    }
}

