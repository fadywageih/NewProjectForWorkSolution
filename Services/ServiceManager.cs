namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAdminService _adminService;

        public ServiceManager(
            IAuthenticationService authenticationService  ,IAdminService adminService)
        {
            _authenticationService = authenticationService;
            _adminService = adminService;

        }
        public IAuthenticationService AuthenticationService => _authenticationService;
        public IAdminService AdminService => _adminService;


    }
}
