namespace PFC.Domain.ValueObjects;

public sealed class Email
{
    public string Address { get; private set; }

    private Email() { }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Email cannot be empty");

        if (!address.Contains("@"))
            throw new ArgumentException("Invalid email format");

        Address = address.Trim().ToLower();
    }

    public override string ToString() => Address;
}