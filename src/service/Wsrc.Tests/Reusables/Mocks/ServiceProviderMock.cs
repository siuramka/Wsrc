using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Wsrc.Tests.Reusables.Mocks;

public class ServiceProviderMock
{
    public (IServiceProvider, IServiceScopeFactory) SetupMock()
    {
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        var serviceScope = Substitute.For<IServiceScope>();
        var serviceProvider = Substitute.For<IServiceProvider>();

        serviceScopeFactory.CreateScope().Returns(serviceScope);
        serviceScope.ServiceProvider.Returns(serviceProvider);

        return (serviceProvider, serviceScopeFactory);
    }
}
