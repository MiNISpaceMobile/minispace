using Domain.BaseTypes;
using System.Security.Cryptography;
using System.Text;

namespace Domain.DataModel;

public class User : BaseEntity
{
    public static byte[] CalculatePasswordHash(string password, string salt)
        => SHA512.HashData(Encoding.UTF8.GetBytes($"{password}{salt}"));

    public DateTime CreationDate { get; }

    public string Username { get; set; }
    public string Email { get; set; }

    // Profile Picture should be stored in external storage (Azure Blob?) under User Guid
    // But maybe it will be easier to store it here (i.e. in database) base64-encoded
    public string? ProfilePicture { get; set; }

    // You *don't* store user passwords. You just store their hashes
    // I suggest we add salt to these hashes as it is an easy technique
    // which additionally increases security in case of a breach
    // We can use User Guid or CreationDate as salt
    // I suggest we use the first one
    public byte[] SaltedPasswordHash { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    // Entity Framework likes empty constructors, but we shouldn't use them
    protected User() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public User(string username, string email, string password, DateTime? creationDate = null)
    {
        CreationDate = creationDate ?? DateTime.Now;

        Username = username;
        Email = email;

        SaltedPasswordHash = CalculatePasswordHash(password);
    }

    public byte[] CalculatePasswordHash(string password) => CalculatePasswordHash(password, Guid.ToString()); // CreationDate.ToString("s"));

    public bool UpdatePassword(string password)
    {
        var newHash = CalculatePasswordHash(password);
        if (SaltedPasswordHash.SequenceEqual(newHash))
            return false;
        SaltedPasswordHash = newHash;
        return true;
    }

    public bool CheckPassword(string password)
        => SaltedPasswordHash.SequenceEqual(CalculatePasswordHash(password));
}
