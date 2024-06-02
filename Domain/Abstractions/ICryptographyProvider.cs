namespace Domain.Abstractions;

public interface ICryptographyProvider<KeyType>
{
    public KeyType Keys { get; }
}
