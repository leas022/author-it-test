using AuthorIt.Data;
using AuthorIt.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<LibraryStore>();
builder.Services.AddScoped<TopicsService>();
builder.Services.AddScoped<BooksService>();

builder.Services.AddControllers(options =>
{
    options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();
app.MapControllers();
app.Run();
