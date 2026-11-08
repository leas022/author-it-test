using AuthorIt.Data;
using AuthorIt.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<LibraryStore>();
builder.Services.AddScoped<TopicsService>();
builder.Services.AddScoped<BooksService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();
app.MapControllers();
app.Run();