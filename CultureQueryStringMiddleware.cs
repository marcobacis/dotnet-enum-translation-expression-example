using System.Globalization;

public class CultureQueryStringMiddleware
{
    private readonly RequestDelegate _next;

    public CultureQueryStringMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cultureQuery = context.Request.Query["culture"];
        if (!string.IsNullOrEmpty(cultureQuery))
        {
            var culture = cultureQuery.ToString();
            if (new[] { "en", "fr", "es" }.Contains(culture))
            {
                var requestCulture = new CultureInfo(culture);
                CultureInfo.CurrentCulture = requestCulture;
                CultureInfo.CurrentUICulture = requestCulture;
            }
        }
        
        await _next(context);
    }
}