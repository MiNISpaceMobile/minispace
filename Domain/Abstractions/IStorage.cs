namespace Domain.Abstractions;

public interface IStorage
{
    public static string UserDirectory(Guid guid) => $"user-{guid}";
    public static string EventDirectory(Guid guid) => $"event-{guid}";
    public static string PostDirectory(Guid guid) => $"post-{guid}";

    public string RootUrl { get; }

    public bool TryCreateDirectory(string directory);
    public bool TryDeleteDirectory(string directory);

    public string UploadFile(Stream file, string directory, string filename);
    public string RenameFile(string directory, string oldFilename, string newFilename);
    public void DeleteFile(string directory, string filename);
}
