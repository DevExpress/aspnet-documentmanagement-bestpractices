# Step 2: Implement custom filtering and apply security settings for the File Manager

This article describes how to implement the following features for the [ASPxFileManager control](https://docs.devexpress.com/AspNet/9030/asp.net-webforms-controls/file-management/file-manager/aspxfilemanager-overview):

1. [Filtering by file extensions and last modification date](#1-filtering-by-file-extensions-and-last-modification-date)
    * [Displaying filtered files' location](#displaying-filtered-files-location)
    * [Using history API for saving the application's state](#using-history-api-for-saving-the-applications-state)
2. [Preventing uploading of files based on their extension and size](#2-preventing-uploading-of-files-based-on-their-extension-and-size)

## 1. Filtering by file extensions and last modification date

The ASPxFileManager control allows end-users to filter files by name in the right top corner of the control.  
 
However, there are many scenarios in which it is required to display documents filtered by custom criteria. This application allows users to display documents that were modified recently or display worksheets/text documents/images/pdfs only. These scenarios are implemented using the [ASPxFileManager](https://docs.devexpress.com/AspNet/9030/asp.net-webforms-controls/file-management/file-manager/aspxfilemanager-overview)'s [file system provider](https://docs.devexpress.com/AspNet/9905/asp.net-webforms-controls/file-management/file-manager/concepts/file-system-providers/file-system-providers-overview) that is stored in the [Code/DocumentsFileSystemProvider.cs](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Code/DocumentsFileSystemProvider.cs) file:

```cs
public override void GetFilteredItems(FileManagerGetFilteredItemsArgs args) {
    args.Items = GetItemsByFilter(args.FileListCustomFilter, args.FilterBoxText);
}

IEnumerable<FileManagerItem> GetItemsByFilter(string filterName, string filterBoxText) {
    IQueryable<DocumentItem> query = null;
    switch(filterName) {
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
```

This code provides interface for filtering and does not operate with a data source and data. The [GetFilteredItems](https://docs.devexpress.com/AspNet/DevExpress.Web.FileSystemProviderBase.GetFilteredItems(DevExpress.Web.FileManagerGetFilteredItemsArgs)) method's overload allows you to implement a function for receiving a collection of files that matches the filter criteria. The *GetItemsByFilter* method calls methods stored in the [Code/DataProvider.cs](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Code/DataProvider.cs) file that operate with a data source. These methods are marked by the *Filtering* `#region`.

The [Code/DataProvider.cs](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Code/DataProvider.cs) file also contains structures required for filtering by the file extension and the number of recently modified documents received from the data source.

```cs
    readonly string[] TextDocsExtensions = new[] { ".rtf", ".doc", ".docx", ".txt" };
    readonly string[] SheetDocsExtensions = new[] { ".xlsx", ".xls" };
    readonly string[] ImageDocsExtensions = new[] { ".png", ".gif", ".jpg", ".jpeg", ".ico", ".bmp" };
    readonly string[] PdfDocsExtensions = new[] { ".pdf" };
    readonly int RecentDocCount = 10;
```

In conjunction with the code that implements the filtering feature, end-users require a UI that allows them to select the filter's options. In this solution, the left-side collapsible menu provides the UI to display the filter's options. Refer to [Building responsive layout for the file explorer using DevExpress controls and CSS styling](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/Layout.md) topic to learn how to implement collapsible menu using DevExpress ASP.NET controls. 

### Displaying filtered files' location

When the custom filter is applied, you can display the file's location in the file system.     

![DetailView](/img/DetailViewFileLoc.png)

The file location is displayed using an [item template](https://docs.devexpress.com/AspNet/8956/asp.net-webforms-controls/file-management/file-manager/visual-elements/item). 

```html
<dx:ASPxFileManager ID="fileManager" runat="server">
    ...
    <SettingsFileList View="Details">
        <DetailsViewSettings>
            ...
            <Columns>
                ...
                <dx:FileManagerDetailsColumn Caption="Name" FileInfoType="FileName">
                    <ItemTemplate>
                        <div class="dxfm-fileName">
                            <a class="templateItem" runat="server" href='<%# GetItemUrl(Container.Item) %>'
                                target='<%# Container.Item is FileManagerFolder ? "_self" : "_blank" %>'>
                                <%# Container.Item.Name %>
                            </a>
                        </div>
                        <%--for Custom Filtering--%>
                        <span class="fm-location"><%# Container.Item.Location %></span>
                    </ItemTemplate>
                </dx:FileManagerDetailsColumn>
                ..
            </Columns>
        </DetailsViewSettings>
    </SettingsFileList>
</dx:ASPxFileManager>
```
The file's location is displayed within the `<span>` element with the *fm-location* CSS class. By default, this element is hidden. It is displayed when the *filtered* selector is added to the file manager:

```css
.file-manager .fm-location {
    display: none;
}

.file-manager.filtered .fm-location {
    font-size: 10px;
    opacity: 0.5;
    display: block;
}
```

When users select any value in the custom filter, the following JS code adds or removes the *filtered* selector to the file manager element:

```js
function updateFileManagerFiltered(hasFilter) {
    if (hasFilter)
        ASPx.AddClassNameToElement(fileManager.GetMainElement(), "filtered");
    else
        ASPx.RemoveClassNameFromElement(fileManager.GetMainElement(), "filtered");
};
```
As a result, the `<span>` element's visibility is changed depending on the custom filter's value.



### Using history API for saving the application's state

In this project, the [window.history](https://developer.mozilla.org/en-US/docs/Web/API/Window/history) object is used to save the file manager's current state before end-users open a folder or apply a custom filter. This mechanism allows end-users to return the previous state of the file manager (previous folder or filter) by clicking the browser's **Back** button instead of navigating from the root folder. Android users also can use the Android **Back** button.  

The *PushToHistoryState* method adds a record to the [window.history](https://developer.mozilla.org/en-US/docs/Web/API/Window/history) object using the [pushState](https://developer.mozilla.org/ru/docs/Web/API/History/pushState) method.  

```js
function PushToHistoryState(kind, filterName, filterText, path) {
    history.pushState({ kind: kind, filter: filterName, text: filterText, path: path }, 'Documents');
}
```
The *kind* parameter specifies an action that triggers saving to the [window.history](https://developer.mozilla.org/en-US/docs/Web/API/Window/history) object:

- if an end-user changed the current folder, the *kind* parameter is set to **path**
- if an end-user applied the custom filter, the *kind* parameter is set to **filter**

The *PushToHistoryState* method is called with a corresponding parameter in the following client event handlers:

```js
function onFileManagerCurrentFolderChanged(s, e) {
    if (!skipUpdateHistoryState)
        PushToHistoryState('path', '', '', s.GetCurrentFolderPath('/', true));
    skipUpdateHistoryState = false;
}

function onFiltersMenuItemClick(s, e) {
    var filterName = e.item.name;
    var filterText = e.item.GetText();

    if (leftPanel.IsExpandable() && leftPanel.IsExpanded())
        leftPanel.Collapse();

    setTimeout(function () { // Timeout - trick for Mobile - wait animation complete on Mobile
        applyFilter(filterName, filterText);
    }, 100);

    // History API
    var kind = filterName === 'All' ? 'path' : 'filter';
    PushToHistoryState(kind, filterName, filterText, '');
    skipUpdateHistoryState = false;
}
```
Note that when an end-user selects the **All** filter item, the file manager opens the root folder, so, in this case, the *kind* parameter is set to **path**.

Pressing the **Back** button triggers the [popstate](https://developer.mozilla.org/en-US/docs/Web/Events/popstate) event:

```js
function onPopHistoryState(evt) {
    skipUpdateHistoryState = true; // Prevent repeated push to state in onFileManagerCurrentFolderChanged
    var state = evt.state;
    if (state.kind === 'path') {
        fileManagerFiltersMenu.SetSelectedItem(fileManagerFiltersMenu.GetItemByName('All'));
        fileManager.SetCurrentFolderPath(evt.state.path);
    } else {
        applyFilter(state.filter, state.text);
        fileManagerFiltersMenu.SetSelectedItem(fileManagerFiltersMenu.GetItemByName(state.filter));
    }
}
```
The event's handler also prevents the closing of the web application for Android users when they press the Android **Back** button accidentally.

## 2. Preventing uploading of files based on their extension and size

The File Manager control provides an ability to upload files in the application's file system. This operation may cause the following issues:

- A DoS attack or memory overflow caused by large files.

- Errors caused by files with unsecured extensions.

To avoid these issues, you can limit the uploaded files' size and define the list of authorized extensions using the ASPxFileManager control's settings in the [Default.aspx](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/DocumentManagementDemo/DocumentManagementDemo/Default.aspx) file. 

```html
<dx:ASPxFileManager ID="fileManager" runat="server">

     <Settings EnableMultiSelect="true" ThumbnailFolder="~/Thumb/" AllowedFileExtensions=".rtf,.doc,.docx,.txt,.xlsx,.xls,.png,.gif,.jpg,.jpeg,.ico,.bmp,.avi,.mp3,.xml,.pdf" />
    
 
    <SettingsUpload Enabled="true" ShowUploadPanel="false" ValidationSettings-MaxFileSize="4000000">
        <AdvancedModeSettings EnableMultiSelect="true" EnableDragAndDrop="true" />
    </SettingsUpload>
    
    <SettingsEditing AllowCopy="True" AllowCreate="True" AllowDelete="True" AllowDownload="True" AllowMove="True" AllowRename="True" />

    ...

</dx:ASPxFileManager>
```

See the [ASP.NET Security Best Practices](https://github.com/DevExpress/aspnet-security-bestpractices) repository for information about best practices that you should follow when you develop your applications to avoid introducing any security breaches.

## Next Step
**Step 3**: [Building responsive layout for the file explorer using DevExpress controls and CSS styling](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/Layout.md)
