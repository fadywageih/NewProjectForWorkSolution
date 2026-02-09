namespace Domain.Exceptions
{
    public sealed class UserNotFoundException(string email) : NotFoundException($"User with email {email} not found")
    {
    }
}
