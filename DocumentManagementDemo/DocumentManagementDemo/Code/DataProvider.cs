using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace DocumentManagementDemo
{
    public class DataProvider : IDisposable
    {
        readonly string[] TextDocsExtensions = new[] { ".rtf", ".doc", ".docx", ".txt" };
        readonly string[] SheetDocsExtensions = new[] { ".xlsx", ".xls" };
        readonly string[] ImageDocsExtensions = new[] { ".png", ".gif", ".jpg", ".jpeg", ".ico", ".bmp" };
        readonly string[] PdfDocsExtensions = new[] { ".pdf" };
        readonly int RecentDocCount = 10;

        DocumentsDb DataContext { get; set; }

        IQueryable<DocumentItem> Items { get { return DataContext.Documents; } }
        Dictionary<long, DocumentItem> FolderCache { get; set; }
        public DocumentItem RootItem { get; private set; }

        public DataProvider()
        {
            DataContext = new DocumentsDb();
            RefreshFolderCache();
        }
        public void Dispose()
        {
            DataContext.Dispose();
        }

        public void RefreshFolderCache()
        {
            FolderCache = Items.Where(item => item.IsFolder).ToDictionary(item => item.ID);
            RootItem = FolderCache.Values.First(item => item.IsRoot);
        }

        public IQueryable<DocumentItem> GetDocumentsInFolder(long folderId)
        {
            return Items.Where(i => i.ParentItem != null && i.ParentItem.ID == folderId && !i.IsFolder);
        }

        public IEnumerable<DocumentItem> GetFolders(long? parentId, string parentRelativeName)
        {
            DocumentItem parentFolder = FindFolder(parentId, parentRelativeName);
            return GetFolders(parentFolder);
        }
        IEnumerable<DocumentItem> GetFolders(DocumentItem parentFolder)
        {
            return FolderCache.Values.Where(i => i.ParentItem == parentFolder && i.IsFolder);
        }

        public bool IsFileExists(long? id, string relativeName)
        {
            return FindFile(id, relativeName) != null;
        }
        public bool IsFolderExists(long? id, string relativeName)
        {
            return FindFolder(id, relativeName) != null;
        }

        #region Filtering
        public IQueryable<DocumentItem> GetFilteredRecentDocuments(string filterText)
        {
            return GetFilteredDocuments(filterText).
                OrderByDescending(i => i.LastWriteTime).
                Take(RecentDocCount);
        }
        public IQueryable<DocumentItem> GetFilteredTextDocuments(string filterText)
        {
            return GetFilteredDocumentsByExtension(TextDocsExtensions, filterText);
        }
        public IQueryable<DocumentItem> GetFilteredSheetDocuments(string filterText)
        {
            return GetFilteredDocumentsByExtension(SheetDocsExtensions, filterText);
        }
        public IQueryable<DocumentItem> GetFilteredImageDocuments(string filterText)
        {
            return GetFilteredDocumentsByExtension(ImageDocsExtensions, filterText);
        }
        public IQueryable<DocumentItem> GetFilteredPdfDocuments(string filterText)
        {
            return GetFilteredDocumentsByExtension(PdfDocsExtensions, filterText);
        }
        IQueryable<DocumentItem> GetFilteredDocumentsByExtension(string[] extensions, string filterText)
        {
            return GetFilteredDocuments(filterText).
                Where(i => extensions.Contains(i.Name.Substring(i.Name.Length - DbFunctions.Reverse(i.Name).IndexOf(".") - 1).ToLower()));
        }
        public IQueryable<DocumentItem> GetFilteredDocuments(string filterText)
        {
            filterText = filterText.ToLower();
            var query = Items.Where(i => !i.IsFolder);
            if (!string.IsNullOrEmpty(filterText))
                query = query.Where(i => i.Name.ToLower().Contains(filterText));
            return query;
        }
        #endregion

        #region Public API
        public DocumentItem GetDocumentById(long id)
        {
            return FindFileById(id);
        }
        public Stream GetDocumentContent(long id)
        {
            DocumentItem fileItem = GetDocumentById(id);
            return new MemoryStream(fileItem.Content.Data.ToArray());
        }
        public void SetDocumentContent(long id, byte[] data)
        {
            DocumentItem fileItem = GetDocumentById(id);

            // Put code here for saving previous Document content
            // SaveDocumentContentToHistory(id, fileItem.LastWriteTime, fileItem.Content.Data);
            // ...

            fileItem.Content.Data = data;
            fileItem.ContentSize = data.Length;
            fileItem.LastWriteTime = DateTime.UtcNow;
            
            // Clear preview Cache for RichText files 
            Utils.ClearDocumentPreviewFilesById(fileItem.ID);

            SaveChanges();
        }
        #endregion

        public void CreateNewFolder(long parentId, string name)
        {
            DocumentItem parentItem = FindFolderById(parentId);
            DocumentItem newItem = new DocumentItem
            {
                IsFolder = true,
                CreationTime = DateTime.UtcNow,
                LastWriteTime = DateTime.UtcNow,
                Name = name,
                ParentItem = parentItem
            };
            DataContext.Documents.Add(newItem);
            SaveChanges();
            RefreshFolderCache();
        }

        public void UploadFile(long folderId, string fileName, Stream fileContent)
        {
            DocumentItem folder = FindFolderById(folderId);
            var file = new DocumentItem()
            {
                Name = fileName,
                Content = CreateBinaryContent(fileContent),
                ContentSize = fileContent.Length,
                ParentItem = folder,
                CreationTime = DateTime.UtcNow,
                LastWriteTime = DateTime.UtcNow,
                IsFolder = false
            };
            DataContext.Documents.Add(file);
            SaveChanges();
        }
        DocumentBinaryContentItem CreateBinaryContent(byte[] data)
        {
            var result = new DocumentBinaryContentItem();
            result.Data = data;
            DataContext.DocumentBinaryContentItems.Add(result);
            return result;
        }
        DocumentBinaryContentItem CreateBinaryContent(Stream dataStream)
        {
            byte[] data = ReadAllBytes(dataStream);
            return CreateBinaryContent(data);
        }

        public void DeleteFile(long fileId)
        {
            DocumentItem file = FindFileById(fileId);
            DeleteItemInternal(file);
            SaveChanges();
        }
        public void DeleteFolder(long folderId)
        {
            DocumentItem folder = FindFolderById(folderId);
            DeleteItemInternal(folder);
            SaveChanges();
            RefreshFolderCache();
        }
        void DeleteItemInternal(DocumentItem item)
        {
            if (item.IsFolder)
            {
                var childItems = Items.Where(i => i.ParentItem.ID == item.ID).ToList();
                foreach (var childItem in childItems)
                    DeleteItemInternal(childItem);
            }
            else
                DataContext.DocumentBinaryContentItems.Remove(item.Content);
            DataContext.Documents.Remove(item);
        }

        public void RenameFile(long fileId, string newName)
        {
            DocumentItem file = FindFileById(fileId);
            EnsureItemExists(file, false);
            file.Name = newName;
            SaveChanges();
        }
        public void RenameFolder(long folderId, string newName)
        {
            DocumentItem folder = FindFolderById(folderId);
            EnsureItemExists(folder, true);
            folder.Name = newName;
            SaveChanges();
            RefreshFolderCache();
        }

        public void MoveFile(long fileId, long newParentFolderId)
        {
            DocumentItem file = FindFileById(fileId);
            DocumentItem folder = FindFolderById(newParentFolderId);
            EnsureItemExists(folder, true);
            if (file != null)
            {
                file.ParentItem = folder;
                SaveChanges();
            }
        }
        public void MoveFolder(long folderId, long newParentFolderId)
        {
            DocumentItem folder = FindFolderById(folderId);
            DocumentItem newParentFolder = FindFolderById(newParentFolderId);
            EnsureItemExists(newParentFolder, true);
            if (folder != null)
            {
                folder.ParentItem = newParentFolder;
                SaveChanges();
            }
            RefreshFolderCache();
        }

        public void CopyFile(long fileId, long newParentFolderId)
        {
            DocumentItem file = FindFileById(fileId);
            DocumentItem newParentFolder = FindFolderById(newParentFolderId);
            EnsureItemExists(newParentFolder, true);
            if (file != null)
            {
                CopyItemInternal(file, newParentFolder);
                SaveChanges();
            }
        }
        public void CopyFolder(long folderId, long newParentFolderId)
        {
            DocumentItem folder = FindFileById(folderId);
            DocumentItem newParentFolder = FindFolderById(newParentFolderId);
            EnsureItemExists(newParentFolder, true);
            if (folder != null)
            {
                CopyFolderInternal(folder, newParentFolder);
                SaveChanges();
            }
            RefreshFolderCache();
        }
        void CopyFolderInternal(DocumentItem folder, DocumentItem newParentFolder)
        {
            DocumentItem copiedFolder = CopyItemInternal(folder, newParentFolder);

            List<DocumentItem> files = GetDocumentsInFolder(folder.ID).ToList();
            foreach (DocumentItem file in files)
                CopyItemInternal(file, copiedFolder);

            List<DocumentItem> subFolders = GetFolders(folder).ToList();
            foreach (DocumentItem subFolder in subFolders)
                CopyFolderInternal(subFolder, copiedFolder);
        }
        DocumentItem CopyItemInternal(DocumentItem item, DocumentItem newParentItem)
        {
            var newFile = new DocumentItem
            {
                Name = item.Name,
                Content = item.IsFolder ? null : CreateBinaryContent(item.Content.Data),
                ContentSize = item.IsFolder ? 0 : item.Content.Data.Length,
                ParentItem = newParentItem,
                CreationTime = DateTime.UtcNow,
                LastWriteTime = DateTime.UtcNow,
                IsFolder = item.IsFolder
            };
            DataContext.Documents.Add(newFile);
            return newFile;
        }

        public string GetFolderRelativeName(DocumentItem folderItem)
        {
            if (folderItem.IsRoot)
                return string.Empty;
            if (folderItem.ParentItem.IsRoot)
                return folderItem.Name;
            string name = GetFolderRelativeName(folderItem.ParentItem);
            return Path.Combine(name, folderItem.Name);
        }

        void SaveChanges()
        {
            DataContext.SaveChanges();
        }

        DocumentItem FindFile(long? id, string relativeName)
        {
            if (id == null)
                return FindFileByRelativeName(relativeName);
            return FindFileById(id.Value);
        }
        DocumentItem FindFileByRelativeName(string relativeName)
        {
            string relativePath = Path.GetDirectoryName(relativeName);
            string name = Path.GetFileName(relativeName);
            DocumentItem parentItem = FindFolderByRelativeName(relativePath);
            return Items.FirstOrDefault(i => i.ParentItem != null && i.ParentItem.ID == parentItem.ID && !i.IsFolder && i.Name == name);
        }
        DocumentItem FindFileById(long id)
        {
            return Items.FirstOrDefault(i => i.ID == id);
        }
        DocumentItem FindFolder(long? id, string relativeName)
        {
            return id != null ? FindFolderById(id.Value) : FindFolderByRelativeName(relativeName);
        }
        DocumentItem FindFolderById(long id)
        {
            return FolderCache[id];
        }
        DocumentItem FindFolderByRelativeName(string relativeName)
        {
            return FolderCache.Values.
                Where(item => item.IsFolder && GetFolderRelativeName(item) == relativeName).
                FirstOrDefault();
        }

        void EnsureItemExists(DocumentItem item, bool isFolder)
        {
            if (item == null)
            {
                string message = (isFolder ? "Folder" : "File") + " not found";
                throw new Exception(message);
            }
        }

        byte[] ReadAllBytes(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            int readCount;
            using (MemoryStream ms = new MemoryStream())
            {
                while ((readCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, readCount);
                }
                return ms.ToArray();
            }
        }
    }
}