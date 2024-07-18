using announce_backend;
using announce_backend.DAL.AnnounceDbContext;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var startup = new Startup();
var app = startup.ConfigureServices();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AnnounceDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();