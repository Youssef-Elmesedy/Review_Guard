using Review_Guard.Domain.Common;

namespace Review_Guard.Domain.Events;

/// <summary>
/// Evaluation of the user registration process, including the generation of a verification code for email confirmation. This event is crucial for ensuring that the user's email address is valid and that they have completed the necessary steps to activate their account. The event captures essential information such as the user's unique identifier, email, full name, and the generated verification code, which will be used in subsequent processes to verify the user's identity and complete the registration process.
/// </summary>
/// <param name="UserId">The User Id</param>
/// <param name="Email">The user's email address</param>
/// <param name="FullName">The user's full name</param>
/// <param name="Code">The verification code generated for email confirmation</param>
public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FullName,
    string Code
) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
