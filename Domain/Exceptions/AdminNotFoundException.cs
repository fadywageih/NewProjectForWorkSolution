namespace Domain.Exceptions
{
    public sealed class AdminNotFoundException : NotFoundException
    {
        public AdminNotFoundException(Guid id)
            : base($"Admin with ID {id} was not found.") { }

        public AdminNotFoundException(string email)
            : base($"Admin with email {email} was not found.") { }
    }
}
