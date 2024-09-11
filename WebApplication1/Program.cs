using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var clients = new ConcurrentDictionary<string, Channel<string>>();

app.MapGet("/sse/{id}", async (HttpContext context, string id) =>
{
    var channel = Channel.CreateUnbounded<string>();
    clients[id] = channel;

    context.Response.Headers.Append("Content-Type", "text/event-stream");
    context.Response.Headers.Append("Cache-Control", "no-cache");
    context.Response.Headers.Append("Connection", "keep-alive");

    var clientChannelReader = channel.Reader;
    while (!context.RequestAborted.IsCancellationRequested)
    {
        if (await clientChannelReader.WaitToReadAsync(context.RequestAborted))
        {
            while (clientChannelReader.TryRead(out var message))
            {
                var jsonResponse = JsonSerializer.Serialize(new { message });

                await context.Response.WriteAsync($"data: {jsonResponse}\n\n");
                await context.Response.Body.FlushAsync();
            }
        }
    }

    clients.TryRemove(id, out _);
});

app.MapPost("/send", async (string id, string message) =>
{
    if (clients.TryGetValue(id, out var channel))
    {
        await channel.Writer.WriteAsync(message);
        return Results.Ok();
    }
    else
    {
        return Results.NotFound($"Client {id} not found.");
    }
});

app.Run();
