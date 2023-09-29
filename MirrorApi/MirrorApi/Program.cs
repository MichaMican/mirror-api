using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run(async (context) =>
{
    context.Response.StatusCode = StatusCodes.Status200OK;
    context.Response.Headers["Content-Type"] = context.Request.Headers["Content-Type"];
    var rawRequestBody = await (new StreamReader(context.Request.Body).ReadToEndAsync());
    await context.Response.WriteAsync(rawRequestBody);
});

app.Run();
