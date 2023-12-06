using Microsoft.ApplicationInsights.DataContracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run(async (context) =>
{
    int CHUNK_SIZE = 8192;

    context.Response.StatusCode = StatusCodes.Status200OK;
    context.Response.Headers["Content-Type"] = context.Request.Headers["Content-Type"];
    var rawRequestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var requestTelemetry = context.Features.Get<RequestTelemetry>();
    var chunks = Enumerable.Range(0, (rawRequestBody.Length / CHUNK_SIZE) + 1)
        .Select(i => rawRequestBody.Substring(i * CHUNK_SIZE, Math.Min(CHUNK_SIZE, rawRequestBody.Length - i * CHUNK_SIZE)))
        .ToList();
    for (int i = 0; i < chunks.Count; i++)
    {
        requestTelemetry?.Properties.Add($"RequestBody_{i + 1}", chunks[i]);
    }
    requestTelemetry?.Properties.Add("RequestContentTypeHeader", context.Request.Headers["Content-Type"]);
    requestTelemetry?.Properties.Add("RequestContentLengthHeader", context.Request.Headers["Content-Length"]);
    await context.Response.WriteAsync(rawRequestBody);
});

app.Run();
