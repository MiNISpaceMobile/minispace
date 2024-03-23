using Domain.BaseTypes;
using System.Security.Cryptography;
using System.Text;

namespace Domain.DataModel;

public class User : BaseEntity
{
    public static byte[] CalculatePasswordHash(string password, DateTime salt)
        => SHA512.HashData(Encoding.UTF8.GetBytes($"{password}{salt.ToString("s")}"));

    public DateTime CreationDate { get; }

    public string Username { get; set; }
    public string Email { get; set; }

    // Profile Picture should be stored in external storage (Azure Blob?) under User Guid
    // But maybe it will be easier to store it here (i.e. in database) base64-encoded
    public string? ProfilePicture { get; set; }

    // You *don't* store user passwords. You just store their hashes
    // I suggest we add "salt" to these hashes as it is an easy technique
    // which additionally increases security in case of a breach
    // We can use account creation date as salt
    public byte[] SaltedPasswordHash { get; set; }

    // Entity Framework likes empty constructors, but we shouldn't use them
    protected User() { }

    public User(string username, string email, string password, DateTime? creationDate = null, string? profilePicture = null)
    {
        CreationDate = creationDate ?? DateTime.Now;

        Username = username;
        Email = email;

        ProfilePicture = profilePicture;

        SaltedPasswordHash = CalculatePasswordHash(password, CreationDate);
    }

    public bool CheckPassword(string password)
        => SaltedPasswordHash.SequenceEqual(CalculatePasswordHash(password, CreationDate));
}
