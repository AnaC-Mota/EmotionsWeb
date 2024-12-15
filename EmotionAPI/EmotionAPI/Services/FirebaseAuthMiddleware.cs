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
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Authorization token is missing.");
            return;
        }
        try
        {
            // Verify Firebase ID token
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            context.Items["User"] = decodedToken;  // Store decoded token for later use in the controller
        }
        catch (Exception)
        {
            // If token verification fails, return unauthorized response
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Invalid or expired token.");
            return;
        }

        await _next(context);  // Proceed to the next middleware/controller
    }
}

