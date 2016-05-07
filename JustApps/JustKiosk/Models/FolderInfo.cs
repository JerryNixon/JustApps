using Windows.Storage;

namespace JustKiosk.Models
{
    public class FolderInfo
    {
        public FolderInfo(StorageFolder storageFolder)
        {
            StorageFolder = storageFolder;
        }
        public string title => this.StorageFolder.DisplayName;
        public StorageFolder StorageFolder { get; }
    }

    public static class Extensions
    {
        public static FolderInfo ToFolderInfo(this StorageFolder folder) => new FolderInfo(folder);
    }
}
