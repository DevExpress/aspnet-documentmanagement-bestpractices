using System.IO;

namespace DocumentManagementDemo
{
    public static class DocumentsDbPopulationHelper
    {
        static public void Populate(string directoryPath)
        {
            using (var dataContext = new DocumentsDb())
            {
                if (!dataContext.Database.Exists())
                    dataContext.Database.Create();

                // Clear
                dataContext.DocumentBinaryContentItems.RemoveRange(dataContext.DocumentBinaryContentItems);
                dataContext.Documents.RemoveRange(dataContext.Documents);

                PopulateDbFromDirectoryRecursive(directoryPath, dataContext, null);
                dataContext.SaveChanges();
            }
        }

        static void PopulateDbFromDirectoryRecursive(string directoryPath, DocumentsDb dataContext,
            DocumentItem parentItem)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            DocumentItem directoryItem = CreateItemFromFileSystemInfo(directoryInfo, true, parentItem);
            dataContext.Documents.Add(directoryItem);
            foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles())
            {
                DocumentItem fileItem = CreateItemFromFileSystemInfo(fileInfo, false, directoryItem);
                fileItem.Content = CreateBinaryContent(dataContext, fileInfo.FullName);
                fileItem.ContentSize = fileItem.Content.Data.Length;
                dataContext.Documents.Add(fileItem);
            }
            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.EnumerateDirectories())
            {
                PopulateDbFromDirectoryRecursive(subDirectoryInfo.FullName, dataContext, directoryItem);
            }

        }
        static DocumentItem CreateItemFromFileSystemInfo(FileSystemInfo fsi, bool isFolder, DocumentItem parentItem)
        {
            return new DocumentItem()
            {
                ParentItem = parentItem,
                Name = fsi.Name,
                IsFolder = isFolder,
                CreationTime = fsi.CreationTimeUtc,
                LastWriteTime = fsi.LastWriteTimeUtc
            };
        }
        static DocumentBinaryContentItem CreateBinaryContent(DocumentsDb context, string filePath)
        {
            DocumentBinaryContentItem content = new DocumentBinaryContentItem();
            content.Data = File.ReadAllBytes(filePath);
            context.DocumentBinaryContentItems.Add(content);
            return content;
        }
    }
}

