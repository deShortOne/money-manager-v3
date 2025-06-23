using Testcontainers.MongoDb;

namespace MoneyTracker.Queries.Tests.Fixture;
public class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _mongo = new MongoDbBuilder()
#if RUN_LOCAL
       .WithDockerEndpoint("tcp://localhost:2375")
#endif
       .WithImage("mongo:8")
       .WithCleanUp(true)
       .Build();

    public string ConnectionString => _mongo.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _mongo.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _mongo.DisposeAsync();
    }
}
