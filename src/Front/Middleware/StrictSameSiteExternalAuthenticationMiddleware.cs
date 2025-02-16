using Microsoft.AspNetCore.Authentication;

namespace Sample.Front.Middleware;

// Fixes for Safari
// https://brockallen.com/2019/01/11/same-site-cookies-asp-net-core-and-external-authentication-providers/
public class StrictSameSiteExternalAuthenticationMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx)
    {
        var schemes = ctx.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
        var handlers = ctx.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();

        foreach (var scheme in await schemes.GetRequestHandlerSchemesAsync())
        {
            if (await handlers.GetHandlerAsync(ctx, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
            {
                // start same-site cookie special handling
                string? location = null;
                if (ctx.Response.StatusCode == 302)
                {
                    location = ctx.Response.Headers.Location;
                }
                else if (ctx.Request.Method == "GET" && ctx.Request.Query["skip"].Count == 0)
                {
                    location = ctx.Request.Path + ctx.Request.QueryString + "&skip=1";
                }

                if (location != null)
                {
                    ctx.Response.StatusCode = 200;
                    var html = $@"
                        <html><head>
                            <meta http-equiv='refresh' content='0;url={location}' />
                        </head></html>";
                    await ctx.Response.WriteAsync(html);
                }
                // end same-site cookie special handling

                return;
            }
        }

        await next(ctx);
    }
}