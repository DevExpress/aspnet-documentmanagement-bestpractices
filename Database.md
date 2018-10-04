# Step 1: Populating the File Manager with files using a database as a file storage

The DevExpress ASP.NET [ASPxFileManager control](https://docs.devexpress.com/AspNet/9030/asp.net-webforms-controls/file-management/file-manager/aspxfilemanager-overview) can be bound to [different file system providers](https://documentation.devexpress.com/AspNet/9928/ASP-NET-WebForms-Controls/File-Management/File-Manager/Concepts/File-System-Providers). 

As a best practice, we recommend using a database to store your files' path and hierarchy information. The target files can be stored in the database too if their size is moderate. This approach is not acceptable for large files because they uploaded from the database longer than from the file system. In this project, target files are small office documents and images, so they are stored in the database.

We recommend using databases because they can be backed up and shared easily which allows for integrating it with other data tables and CMS.

Perform the following steps to populate the File Manager control with files:

1. [Create the database](#1-create-the-database)
2. [Populate the database with files from the file system](#2-populate-the-database-with-files-from-the-file-system) 
2. [Connect the File Manager with the database](#3-connect-the-file-manager-with-the-database)


## 1. Create the database

The database is created using [Entity Framework](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ef/overview). You can add the framework to the project using the [Nuget Package Manager](https://www.nuget.org/packages/EntityFramework/). The [DbContext](https://msdn.microsoft.com/en-us/library/system.data.entity.dbcontext(v=vs.113).aspx) class and data models are stored in the [Code/Context.cs](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Code/Context.cs) file.

The [Code/Context.cs](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Code/Context.cs) file also contains models that describe data contained in the database. The *DocumentItem* class represents items stored in the file explorer.

```cs
public class DocumentItem {
    public long ID { get; set; }
    public string Name { get; set; }
    public bool IsFolder { get; set; }
    public long ContentSize { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastWriteTime { get; set; }
    public virtual DocumentBinaryContentItem Content { get; set; }
    public virtual DocumentItem ParentItem { get; set; }

    public bool IsRoot { get { return ParentItem == null; } }
}
```

The file explorer's items can be a file or folder that stores other files and folders. Each item has a parent item - a folder that stores the current file or folder. 

The *ParentItem* field refers to the current item's parent folder (for folders and files both). The root folder does not have the parent folder; its *ParentItem* field returns **null**. The *IsFolder* bool field indicates whether the current item is a folder or a file.

The file's content is stored in a separate table. *Content* field is the link that is represented by the *DocumentBinaryContentItems* model.

```csharp
public class DocumentBinaryContentItem {
    public long ID { get; set; }
    public byte[] Data { get; set; }
}
```
This table is required for two main operations: obtaining a file's info (name, last modified date, etc.) and its content. The second operation can take a certain time and required only when a user opens some document. In turn, the file's info is required for all files displayed in the file explorer.

For the same purposes, the file's size is calculated once and stored in the *ContentSize* field. Note that the field's value should be recalculated if users changed the file.


## 2. Populate the database with files from the file system

The *DocumentsDbPopulationHelper* class stored in the [Code/DocumentPopulation.cs](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Code/DocumentsPopulation.cs   ) file contains code that creates a new database (if it was not created before) and populates it with content. The class provides the public [Populate](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Code/DocumentsPopulation.cs#L7-L21) method to execute private methods that recursively walk through each file and folder starting from the target folder and adds them to the database.

In this solution, the *Populate* method is called in the [CreateDatabase.aspx](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/CreateDatabase.aspx) page. This page contains a button that calls the *Populate* method. 


## 3. Connect the File Manager with the database

The ASPxFileManager control is located in the [Default.aspx](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Default.aspx) file and bound to the database using a [file system provider](https://docs.devexpress.com/AspNet/9905/asp.net-webforms-controls/file-management/file-manager/concepts/file-system-providers/file-system-providers-overview). The file system provider is an API to access the virtual file system in the File Manager control. The file system provider should be assigned to the control in the *Page_Init* event handler.

``` cs
protected void Page_Init(object sender, EventArgs e) {
    fileManager.CustomFileSystemProvider = new DocumentsFileSystemProvider(Utils.CurrentDataProvider);
}
```
There are several types of file system providers described in [this topic](https://docs.devexpress.com/AspNet/9905/asp.net-webforms-controls/file-management/file-manager/concepts/file-system-providers/file-system-providers-overview). In this project, the [custom file system provider](https://docs.devexpress.com/AspNet/9907/asp.net-webforms-controls/file-management/file-manager/concepts/file-system-providers/custom-file-system-provider) is implemented via the *DocumentsFileSystemProvider* class stored in the [Code/DocumentsFileSystemProvider](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Code/DocumentsFileSystemProvider.cs) file.

The custom file system provider overrides the [FileSystemProviderBase](https://docs.devexpress.com/AspNet/DevExpress.Web.FileSystemProviderBase) class's required virtual methods. These methods use common LINQ code that works for most database providers. You can add the code stored in the [Code/DocumentsFileSystemProvider](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Code/DocumentsFileSystemProvider.cs) file in your web application and use it without any changes for other data sources. 

## Next Step: 
**Step 2**: [Implementing custom filtering and applying security settings for the File Manager](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/FileManager.md)



