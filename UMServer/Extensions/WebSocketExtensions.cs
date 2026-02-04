using System.Net.WebSockets;
using System.Text;

namespace UMServer.Extensions;

/// <summary>
/// General WebSocket exception class
/// </summary>
[System.Serializable]
public class WebSocketClosedException : System.Exception
{
    public WebSocketClosedException() { }
    public WebSocketClosedException(string message) : base(message) { }
}

/// <summary>
/// Extension class for WebSocket objects
/// </summary>
public static class WebSocketExtensions {
    /// <summary>
    /// Reads the information from the WebSocket
    /// </summary>
    /// <param name="socket">WebSocket object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Read data as a UTF8 string</returns>
    /// <exception cref="WebSocketClosedException"></exception>
    public static async Task<string> Read(this WebSocket socket, CancellationToken cancellationToken) {
        WebSocketReceiveResult result;
		var buffer = new ArraySegment<byte>(new byte[1024]);
		var message = new StringBuilder();
		do
		{
			result = await socket.ReceiveAsync(buffer, CancellationToken.None);
			string messagePart = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);
			message.Append(messagePart);
		}
		while (!result.EndOfMessage);
		return message.ToString().Replace("\0", string.Empty);
    }

    /// <summary>
    /// Reads the information from the WebSocket, uses empty cancellation token
    /// </summary>
    /// <param name="socket">WebSocket object</param>
    /// <returns>Read data as a UTF8 string</returns>
    public static async Task<string> Read(this WebSocket socket) {
        return await Read(socket, CancellationToken.None);
    }

    /// <summary>
    /// Writes the string data to the socket, uses empty cancellation token
    /// </summary>
    /// <param name="socket">WebSocket object</param>
    /// <param name="message">Message</param>
    /// <returns></returns>
    public static async Task Write(this WebSocket socket, string message) {
        var serverMsg = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    /// <summary>
    /// Writes the string data to the socket, uses empty cancellation token
    /// </summary>
    /// <param name="socket">WebSocket object</param>
    /// <param name="message">Message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public static async Task Write(this WebSocket socket, string message, CancellationToken cancellationToken) {
        var serverMsg = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), WebSocketMessageType.Text, true, cancellationToken);
    }
}