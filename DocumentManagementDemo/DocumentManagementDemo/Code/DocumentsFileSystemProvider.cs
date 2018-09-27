using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.Web;

namespace DocumentManagementDemo
{
    public class DocumentsFileSystemProvider : FileSystemProviderBase
    {
        DataProvider DataProvider { get; set; }

        public DocumentsFileSystemProvider(DataProvider dataProvider) : base(string.Empty)
        {
            DataProvider = dataProvider;
        }

        public override string RootFolderDisplayName { get { return DataProvider.RootItem.Name; } }

        public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder)
        {
            long folderId = GetItemId(folder);
            return DataProvider.GetDocumentsInFolder(folderId).
                ToList().
                Select(i => CreateFileManagerFile(i, folder));
        }
        public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder)
        {
            long? parentId = TryGetItemId(parentFolder);
            return DataProvider.GetFolders(parentId, parentFolder.RelativeName).
                Select(item => new FileManagerFolder(this, parentFolder, item.Name, item.ID.ToString(),
                    new FileManagerFolderProperties() { LastWriteTime = item.LastWriteTime }));
        }

        // Filtering
        public override void GetFilteredItems(FileManagerGetFilteredItemsArgs args)
        {
            args.Items = GetItemsByFilter(args.FileListCustomFilter, args.FilterBoxText);
        }
        IEnumerable<FileManagerItem> GetItemsByFilter(string filterName, string filterBoxText)
        {
            IQueryable<DocumentItem> query = null;
            switch (filterName)
            {
                case "Recent":
                    query = DataProvider.GetFilteredRecentDocuments(filterBoxText);
                    break;
                case "Docs":
                    query = DataProvider.GetFilteredTextDocuments(filterBoxText);
                    break;
                case "Sheets":
                    query = DataProvider.GetFilteredSheetDocuments(filterBoxText);
                    break;
                case "Images":
                    query = DataProvider.GetFilteredImageDocuments(filterBoxText);
                    break;
                case "Pdfs":
                    query = DataProvider.GetFilteredPdfDocuments(filterBoxText);
                    break;
                default:
                    query = DataProvider.GetFilteredDocuments(filterBoxText);
                    break;
            }
            return query.ToList().Select(i => CreateFileManagerFile(i));
        }

        public override bool Exists(FileManagerFile file)
        {
            long? id = TryGetItemId(file);
            return DataProvider.IsFileExists(id, file.RelativeName);
        }
        public override bool Exists(FileManagerFolder folder)
        {
            long? id = TryGetItemId(folder);
            return DataProvider.IsFolderExists(id, folder.RelativeName);
        }

        public override Stream ReadFile(FileManagerFile file)
        {
            long id = GetItemId(file);
            return DataProvider.GetDocumentContent(id);
        }

        public override void CreateFolder(FileManagerFolder parent, string name)
        {
            long parentId = GetItemId(parent);
            DataProvider.CreateNewFolder(parentId, name);
        }

        public override void UploadFile(FileManagerFolder folder, string fileName, Stream fileContent)
        {
            long folderId = GetItemId(folder);
            DataProvider.UploadFile(folderId, fileName, fileContent);
        }

        public override void DeleteFile(FileManagerFile file)
        {
            long fileId = GetItemId(file);
            DataProvider.DeleteFile(fileId);
        }
        public override void DeleteFolder(FileManagerFolder folder)
        {
            long folderId = GetItemId(folder);
            DataProvider.DeleteFolder(folderId);
        }

        public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            long fileId = GetItemId(file);
            long newParentId = GetItemId(newParentFolder);
            DataProvider.MoveFile(fileId, newParentId);
        }
        public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            long folderId = GetItemId(folder);
            long newParentId = GetItemId(newParentFolder);
            DataProvider.MoveFolder(folderId, newParentId);
        }

        public override void RenameFile(FileManagerFile file, string name)
        {
            long fileId = GetItemId(file);
            DataProvider.RenameFile(fileId, name);
        }
        public override void RenameFolder(FileManagerFolder folder, string name)
        {
            long folderId = GetItemId(folder);
            DataProvider.RenameFolder(folderId, name);
        }

        public override void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            long fileId = GetItemId(file);
            long newParentFolderId = GetItemId(newParentFolder);
            DataProvider.CopyFile(fileId, newParentFolderId);
        }
        public override void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            long folderId = GetItemId(folder);
            long newParentFolderId = GetItemId(newParentFolder);
            DataProvider.CopyFolder(folderId, newParentFolderId);
        }

        long? TryGetItemId(FileManagerItem fileManagerItem)
        {
            if (fileManagerItem.Id == null)
                return null;
            return GetItemId(fileManagerItem);
        }
        long GetItemId(FileManagerItem fileManagerItem)
        {
            if (string.IsNullOrEmpty(fileManagerItem.RelativeName))
                return DataProvider.RootItem.ID;
            return long.Parse(fileManagerItem.Id);
        }
        FileManagerFile CreateFileManagerFile(DocumentItem dataItem)
        {
            return CreateFileManagerFile(dataItem, CreateParentFolder(dataItem));
        }
        FileManagerFile CreateFileManagerFile(DocumentItem dataItem, FileManagerFolder folder)
        {
            return new FileManagerFile(this, folder, dataItem.Name, dataItem.ID.ToString(),
                new FileManagerFileProperties() { LastWriteTime = dataItem.LastWriteTime, Length = dataItem.ContentSize });
        }
        FileManagerFolder CreateParentFolder(DocumentItem item)
        {
            if (item.ParentItem == null)
                return new FileManagerFolder(this, string.Empty);
            string parentRelativeName = DataProvider.GetFolderRelativeName(item.ParentItem);
            return new FileManagerFolder(this, parentRelativeName, item.ParentItem.ID.ToString());
        }
    }
}