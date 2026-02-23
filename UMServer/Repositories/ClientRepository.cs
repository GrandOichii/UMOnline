namespace UMServer.Repositories;

public class ConnectedClient
{
    public required string Name { get; init; }
    public required string Id { get; init; }
}

public interface IClientRepository
{
    public Task<string> Add(string connectionId, string name);
    public Task Remove(ConnectedClient client);
    public Task<ConnectedClient?> Get(string connectionId);
}

public class ClientRepository(ILogger<ClientRepository> logger) : IClientRepository
{
    private readonly List<ConnectedClient> _clients = [];


    public async  Task<string> Add(string connectionId, string name)
    {
        if (_clients.Any(c => c.Name == name))
        {
            return $"User with name {name} already connected";
        }

        _clients.Add(new()
        {
            Id = connectionId,
            Name = name
        });

        return string.Empty;
    }

    public async Task Remove(ConnectedClient client)
    {
        _clients.RemoveAll(c => c.Id == client.Id);
    }

    public Task<ConnectedClient?> Get(string connectionId)
    {
        return Task.FromResult(
            _clients.SingleOrDefault(c => c.Id == connectionId)
        );
    }
}