using Autocompletion.DataStructures;
using Autocompletion.WordImport;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseWebSockets();

app.UseDefaultFiles();
app.UseStaticFiles();

Trie trie = new Trie(new TrieNode());
ReadFile readFile = new ReadFile();

await readFile.StartReading(trie);

app.Map("/ws", async context => {
    if (context.WebSockets.IsWebSocketRequest) {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!result.CloseStatus.HasValue) {
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Empfangen: {receivedMessage}");

            List<string> ans = trie.AutoComplete(receivedMessage);

            var node = trie.GetNodeForPrefix(receivedMessage);

            if (node != null) {
                var json = System.Text.Json.JsonSerializer.Serialize(ans);
                var responseBytes = Encoding.UTF8.GetBytes(json);

                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), result.MessageType, true, CancellationToken.None);
            }
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        }
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    } else {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

app.Run();
