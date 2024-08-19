using NLog.Web;
using SwiftMT799Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new SQLiteHelper("Data Source=MT799.db"));
builder.Services.AddSingleton<MT799Parser>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

var app = builder.Build();

var dbHelper = app.Services.GetRequiredService<SQLiteHelper>();
dbHelper.InitializeDatabase();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();