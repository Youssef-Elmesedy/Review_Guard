namespace Review_Guard.Domain.Exceptions;

public class BusinessRuleViolationException : DomainException
{
    public string BusinessMessageKey { get; set; }
    public string RuleName { get; }

    public BusinessRuleViolationException(string ruleName, string message, string businessMessageKey)
        : base(message, businessMessageKey)
    {
        RuleName = ruleName;
        BusinessMessageKey = businessMessageKey;
    }
}