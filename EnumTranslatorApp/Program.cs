using AutoMapper;
using AutoMapper.QueryableExtensions;
using EnumTranslator;
using EnumTranslatorApp;
using Gridify;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer(); // Adds support for minimal APIs
builder.Services.AddSwaggerGen();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Set up supported cultures
var supportedCultures = new[] { "en", "it", "fr", "es" }; // List of supported languages
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en") // Default culture
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Allow setting culture from query string (useful for debugging)
app.UseMiddleware<CultureQueryStringMiddleware>();

app.UseHttpsRedirection();

// Example endpoint with filtering based on translated enum value representation (using gridify)
app.MapGet(
        "/example",
        async (
            string culture,
            [AsParameters] GridifyQuery gridifyQuery,
            AppDbContext dbContext,
            IMapper mapper,
            CancellationToken cancellationToken
        ) =>
        {
            var sqlQuery = dbContext.MyEntities.ProjectTo<ExampleDto>(mapper.ConfigurationProvider);

            var count = await sqlQuery.ApplyFiltering(gridifyQuery).CountAsync();
            var data = await sqlQuery
                .ApplyFilteringOrderingPaging(gridifyQuery)
                .ToListAsync(cancellationToken);
            return new Paging<ExampleDto>(count, data);
        }
    )
    .WithName("GetExampleEntities");

// Migrate DB on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
