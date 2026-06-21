namespace Review_Guard.Domain.Exceptions;

public class DomainException : Exception
{
    public string MessageKey { get; }

    public DomainException(
        string messageKey)
        : base(messageKey)
    {
        MessageKey = messageKey;
    }
}