using ServicesAbstraction;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly IAuthenticationService _authenticationService;
        public ServiceManager(
            IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;

        }
        public IAuthenticationService AuthenticationService => _authenticationService;

    }
}
