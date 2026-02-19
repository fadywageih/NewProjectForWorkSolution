namespace Domain.Exceptions
{
    public sealed class AdminExistsException : Exception
    {
        public AdminExistsException(string email)
            : base($"Admin with email {email} already exists.") { }
    }
}
    