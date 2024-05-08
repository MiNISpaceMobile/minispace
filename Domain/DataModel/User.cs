using Domain.BaseTypes;
using System.Security.Cryptography;
using System.Text;

namespace Domain.DataModel;

public abstract class User : BaseEntity
{
    public static byte[] CalculatePasswordHash(string password, string salt)
        => SHA512.HashData(Encoding.UTF8.GetBytes($"{password}{salt}"));

    // USOS is the source of ExternalId in our case
    public string? ExternalId { get; private set; }
    public DateTime CreationDate { get; private set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    // Profile Picture should be stored in external storage (Azure Blob?) under User Guid
    // But maybe it will be easier to store it here (i.e. in database) base64-encoded
    // public string? ProfilePicture { get; set; }

    // You *don't* store user passwords. You just store their hashes
    // I suggest we add salt to these hashes as it is an easy technique
    // which additionally increases security in case of a breach
    // We can use CreationDate as salt, because it never changes
    // public byte[] SaltedPasswordHash { get; set; }

#pragma warning disable CS8618 // Unassigned non-nullables
    // Entity Framework likes empty constructors, but we shouldn't use them
    protected User() { }
#pragma warning restore CS8618 // Unassigned non-nullables

    public User(string firstName, string lastName, string email, string? externalId, DateTime? creationDate)
    {
        ExternalId = externalId;
        CreationDate = creationDate ?? DateTime.Now;

        FirstName = firstName;
        LastName = lastName;
        Email = email;

        // SaltedPasswordHash = CalculatePasswordHash(password);
    }

    public byte[] CalculatePasswordHash(string password) => CalculatePasswordHash(password, CreationDate.ToString("s"));

    //public bool UpdatePassword(string password)
    //{
    //    var newHash = CalculatePasswordHash(password);

    //    if (SaltedPasswordHash.SequenceEqual(newHash))
    //        return false;

    //    SaltedPasswordHash = newHash;

    //    return true;
    //}

    //public bool CheckPassword(string password)
    //    => SaltedPasswordHash.SequenceEqual(CalculatePasswordHash(password));
}
