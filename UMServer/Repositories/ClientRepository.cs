namespace UMServer.Repositories;

public class ConnectedClient
{
    public required string Name { get; init; }
    public required string ConnectionId { get; init; }
}

public interface IClientRepository
{
    public Task<string> AddClient(string connectionId, string name);
    public Task RemoveClient(string connectionId);
    public Task<ConnectedClient?> GetClient(string connectionId);
}

public class ClientRepository(ILogger<ClientRepository> logger) : IClientRepository
{
    private readonly List<ConnectedClient> _clients = [];


    public async  Task<string> AddClient(string connectionId, string name)
    {
        logger.LogDebug("ADD {} {}", connectionId, name);

        if (_clients.Any(c => c.Name == name))
        {
            return $"User with name {name} already connected";
        }

        _clients.Add(new()
        {
            ConnectionId = connectionId,
            Name = name
        });

        return string.Empty;
    }

    public async Task RemoveClient(string connectionId)
    {
        _clients.RemoveAll(c => c.ConnectionId == connectionId);
    }

    public Task<ConnectedClient?> GetClient(string connectionId)
    {
        return Task.FromResult(
            _clients.SingleOrDefault(c => c.ConnectionId == connectionId)
        );
    }
}