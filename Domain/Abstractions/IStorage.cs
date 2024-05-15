namespace Domain.Abstractions;

public interface IStorage
{
    public string FileStorageUrl { get; }

    public bool TryCreateDirectory(string directory);
    public bool TryDeleteDirectory(string directory);

    public void UploadFile(Stream file, string directory, string filename);
    public void RenameFile(string directory, string oldFilename, string newFilename);
    public void DeleteFile(string directory, string filename);
}
