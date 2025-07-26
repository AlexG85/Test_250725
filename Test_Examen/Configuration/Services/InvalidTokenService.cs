using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Test_Examen.Configuration.Database;

namespace Test_Examen.Configuration.Services
{
    public class InvalidTokenService : IHostedService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceProvider _serviceProvider;

        public InvalidTokenService(
            IMemoryCache memoryCache,
            IServiceProvider serviceProvider)
        {
            _memoryCache = memoryCache;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SQLDBContext>();

            (await dbContext.RefreshTokens
                .Where(x => x.Invalidated)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken))
                .ForEach(token => _memoryCache.Set(token.JwtId, token.Token));
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
