using System.Collections.Immutable;
using System.Globalization;
using Microsoft.Extensions.Options;

namespace EnumTranslatorApp;

public class CultureQueryStringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ImmutableHashSet<string> _supportedCultures;
    
    public CultureQueryStringMiddleware(RequestDelegate next,  IOptions<RequestLocalizationOptions> localizationOptions)
    {
        _next = next;
        _supportedCultures = (localizationOptions.Value.SupportedCultures ?? []).Select(c => c.TwoLetterISOLanguageName.ToLowerInvariant()).ToImmutableHashSet();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cultureQuery = context.Request.Query["culture"];
        if (!string.IsNullOrEmpty(cultureQuery))
        {
            var culture = cultureQuery.ToString();
            if (_supportedCultures.Contains(culture.ToLowerInvariant()))
            {
                var requestCulture = new CultureInfo(culture);
                CultureInfo.CurrentCulture = requestCulture;
                CultureInfo.CurrentUICulture = requestCulture;
            }
        }

        await _next(context);
    }
}