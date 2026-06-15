using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Rules;

public static class UserBusinessRules
{
    public static void EmailMustBeUnique(bool emailExists)
    {
        if (emailExists)
            throw new DomainException(
                "A user with this email address already exists.",
                DomainMessagies.EmailAlreadyExists);
    }

    public static void AccountMustBeActive(User user)
    {
        if (user.Status != AccountStatus.Active)
            throw new DomainException(
    DomainMessagies.AccountNotActive, DomainMessagies.AccountNotActive);
    }

    public static void EmailMustBeVerified(User user)
    {
        if (!user.IsEmailVerified)
            throw new DomainException(
                "Please verify your email address before performing this action.",
                DomainMessagies.EmailNotVerified);
    }

    public static void TrustScoreMustMeetThreshold(User user, decimal minimumScore)
    {
        if (user.TrustScoreValue < minimumScore)
            throw new DomainException(
                $"Your trust score ({user.TrustScoreValue:F1}) is too low for this action.",
                DomainMessagies.TrustScoreTooLow);
    }

    public static void UserMustBeUniqueFullName(User user, bool fullNameExists)
    {
        if (fullNameExists)
            throw new DomainException(
                "A user with this full name already exists.",
                DomainMessagies.UserUniqueFullName);
    }

    public static void UserMustBeUniquePhone(User user, bool phoneExists)
    {
        if (phoneExists)
            throw new DomainException(
                "A user with this phone number already exists.",
                DomainMessagies.UserUniquePhone);
    }
}