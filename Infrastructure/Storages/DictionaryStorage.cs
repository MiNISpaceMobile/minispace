using Domain.Abstractions;

namespace Infrastructure.Storages;

public class DictionaryStorage : IStorage
{
    public string RootUrl => "";

    public Dictionary<string, Dictionary<string, Stream>> Storage { get; }
        = new Dictionary<string, Dictionary<string, Stream>>();

    public bool TryCreateDirectory(string directory)
        => Storage.TryAdd(directory, new Dictionary<string, Stream>());

    public bool TryDeleteDirectory(string directory)
        => Storage.Remove(directory);

    public void UploadFile(Stream file, string directory, string filename)
    {
        TryCreateDirectory(directory);
        Storage[directory][filename] = file;
    }

    public void RenameFile(string directory, string oldFilename, string newFilename)
    {
        Storage[directory][newFilename] = Storage[directory][oldFilename];
        Storage[directory].Remove(oldFilename);
    }

    public void DeleteFile(string directory, string filename)
    {
        if (Storage.TryGetValue(directory, out var dir))
            dir.Remove(filename);
    }
}
