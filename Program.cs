using AutoMapper;
using AutoMapper.QueryableExtensions;
using EnumTranslator;
using Gridify;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer(); // Adds support for minimal APIs


builder.Services.AddSwaggerGen(c =>
{
    // Add a custom header for Accept-Language in Swagger UI
    c.AddSecurityDefinition("Accept-Language", new OpenApiSecurityScheme
    {
        Description = "Set the language for the request",
        In = ParameterLocation.Header,
        Name = "Accept-Language",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Accept-Language"
                }
            },
            new string[] { }
        }
    });
});


// Add Database
var connectionString = builder.Configuration.GetConnectionString("Database");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// Add automapper
builder.Services.AddAutoMapper(typeof(Program));


// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                     // Enables the Swagger JSON endpoint
    app.UseSwaggerUI();                   // Enables the Swagger UI
}

// Set up supported cultures
var supportedCultures = new[] { "en", "it", "fr", "es" }; // List of supported languages
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en") // Default culture
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseMiddleware<CultureQueryStringMiddleware>();
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

app.MapGet("/example", async (string culture, [AsParameters] GridifyQuery gridifyQuery, AppDbContext dbContext, IMapper mapper, CancellationToken cancellationToken) =>
    {
        var sqlQuery = dbContext.MyEntities
            .ProjectTo<ExampleDto>(mapper.ConfigurationProvider);
        
        var count = await sqlQuery.ApplyFiltering(gridifyQuery).CountAsync();
        
        var data= await 
            sqlQuery.ApplyFilteringOrderingPaging(gridifyQuery)
            .ToListAsync(cancellationToken);

        return new Paging<ExampleDto>(count, data);
    })
    .WithName("GetExampleEntities");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}