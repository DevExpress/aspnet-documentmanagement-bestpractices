# Step 4: Reading and viewing office documents using Rich Text Editor and Spreadsheet controls

This article describes how to implement functionality for reading and viewing text documents and worksheets using DevExpress ASP.NET controls. The described functionality is used in the web application intended for document management.

- [General Information](#general-information)
- [View mode for text documents](#view-mode-for-text-documents)
- [View mode for worksheets](#view-mode-for-worksheets)
- [Edit mode for office documents](#editing-office-documents)


## General Information

The Document Management Demo project allows end-users to view and edit office documents (.rtf, .doc, .docx, .txt and .xlsx, .xls files) using the DevExpress ASP.NET controls. If users select other files, they are downloaded to a local machine.

Once a user selects a document, it is opened in a read-only mode. End-users can back to the file explorer or edit the document using navigation buttons:

![ReadingView](/img/ReadingView.png)

If end-users click the pen button, the editing functionality is enabled.

![EditingView](/img/EditingView.png)

### Opening office documents

When an end-user selects a file in the file explorer, it refers to the [OpenDocumentHandler.aspx](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/DocumentManagementDemo/DocumentManagementDemo/OpenDocumentHandler.aspx) page. This page contains code that defines the file's extension and performs the corresponding action: opens office documents or downloads other files:

```cs
public partial class OpenDocumentHandler : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            long id = long.Parse(Request.QueryString["id"]);
            var docType = Utils.GetDocumentTypeById(id);
            if (docType == Utils.RtfDocType)
                Response.Redirect("RichEdit.aspx?id=" + id.ToString());
            else if (docType == Utils.SheetDocType)
                Response.Redirect("Spreadsheet.aspx?id=" + id.ToString());
            else
                Utils.DownloadDocumentById(this, id);
        }
    }
}
```

Office documents are handled on separate pages:

- The .rtf, .doc, .docx, .txt files are handlded in the [RichEdit.aspx](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/DocumentManagementDemo/DocumentManagementDemo/RichEdit.aspx) page.

- The .xlsx, .xls files files are hanlded in the [Spreadsheet.aspx](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/DocumentManagementDemo/DocumentManagementDemo/Spreadsheet.aspx) page.

The *RichEdit.aspx* and *Spreadsheet.aspx* pages are placed in the same master page - [Document.Master](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/DocumentManagementDemo/DocumentManagementDemo/Document.Master).

### Page layout

The [Document.Master](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/DocumentManagementDemo/DocumentManagementDemo/Document.Master) page contains a header with text and two navigation buttons. Refer to the [Building responsive layout for the file explorer using DevExpress controls and CSS styling](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/Layout.md) article for information about this toolbar.

The navigation buttons allow end-users to switch between view and edit modes and or back to the file explorer. The following code adds or removes the `&edit=true` parameter to the query string when navigational buttons are clicked:

```cs
public partial class Document : System.Web.UI.MasterPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var currentDocumentId = Request.QueryString["id"];
            int id = -1;
            if (int.TryParse(currentDocumentId, out id))
            {
                DocumentTitle.InnerText = Utils.GetDocumentNameById(id);

                var queryParams = HttpUtility.ParseQueryString(Request.Url.Query);
                if (string.IsNullOrEmpty(queryParams["edit"]))
                {
                    queryParams.Add("edit", "true");
                    DocumentEditLink.NavigateUrl = Request.Url.AbsolutePath + "?" + queryParams.ToString();
                    DocumentEditLink.Visible = true;
                }
                else
                {
                    queryParams.Remove("edit");
                    DocumentBackLink.NavigateUrl = Request.Url.AbsolutePath + "?" + queryParams.ToString();
                    DocumentEditLink.Visible = false;
                }
            }
        }
    }
}
```
The `edit` request parameter is used in the *RichEdit.aspx* and *Spreadsheet.aspx* pages to detect whether open a document in view or edit mode. 

When a user opens a document in the file explorer, it is opened in view mode.

## View mode for text documents

Text documents (.rtf, .doc, .docx, .txt files) are processed in the [RichEdit.aspx](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/DocumentManagementDemo/DocumentManagementDemo/RichEdit.aspx) page.

The DevExpress [ASPxRichEdit](https://docs.devexpress.com/AspNet/17721/asp.net-webforms-controls/rich-text-editor) control allows end-users to view, edit, and save text documents. However, the control does not provide functionality to adjust the document's content to browser's width that is required for reading documents, especially on mobile devices. For this reason, text documents are converted to the HTML format and displayed in the `<div>` tag for viewing documents. This solution also accelerates the page's render in comparison with the page with the Rich Edit control.      

Text documents are converted to the HTML document using the [HTML Editor](https://docs.devexpress.com/AspNet/7917/asp.net-webforms-controls/html-editor/main-features) control. It is created in code only when users open a document in view mode. 

```cs
protected void Page_Load(object sender, EventArgs e)
{
    // Opening Document
    ...
    // View Mode emulation
    if (string.IsNullOrEmpty(Request.QueryString["edit"]))
    { // is view mode?
        using (MemoryStream ms = new MemoryStream())
        {
            ms.Write(data, 0, data.Length);
            ms.Position = 0;

            string documentPreviewFilesFolder = Utils.GetDocumentPreviewFilesFolderById(itemID);
            string documentPreviewFileName = documentPreviewFilesFolder + @"\preview";
            string html;
            if (Directory.Exists(documentPreviewFilesFolder) && File.Exists(documentPreviewFileName))
                html = File.ReadAllText(documentPreviewFileName);
            else
            {
                ASPxHtmlEditor htmlEditor = new ASPxHtmlEditor();
                htmlEditor.Import(GetHtmlEditorImportFormat(documentFormat), ms, true, documentPreviewFilesFolder);
                html = htmlEditor.Html;
                if (!Directory.Exists(documentPreviewFilesFolder))
                    Directory.CreateDirectory(documentPreviewFilesFolder);
                File.WriteAllText(documentPreviewFileName, html);
            }
            viewer.InnerHtml = html;
        }

        viewerWrapper.Visible = true;
        richEdit.Visible = false;
    }
    else
    {
        ...
    }
}
```
The HTML Editor's [Import](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxHtmlEditor.ASPxHtmlEditor.Import(DevExpress.Web.ASPxHtmlEditor.HtmlEditorImportFormat-System.IO.Stream-System.Boolean-System.String)) method imports a document's content to the HTML format. Then, the document's content in HTML format is placed to the *viewer* `<div>` tag to display the document. The HTML document is also written to the temporary folder to cache the document's content.

### Caching HTML documents

Saving documents in HTML format to the temporary folder optimizes the web application's performance. If the open document is not changed, its HTML representation can be loaded from this folder instead of initialization the HTML Editor control and converting the document's content to the HTML format.

If a document is changed by end-user and saved, the cached HTML document should be deleted. To accomplish this task, the *ClearDocumentPreviewFilesById* method is called in the [DataProvider.cs](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/DocumentManagementDemo/DocumentManagementDemo/Code/DataProvider.cs#L112-L128) file:

```cs
public void SetDocumentContent(long id, byte[] data) {
    DocumentItem fileItem = GetDocumentById(id);
    fileItem.Content.Data = data;
    fileItem.ContentSize = data.Length;
    fileItem.LastWriteTime = DateTime.UtcNow;
    // Clear preview Cache for RichText files 
    Utils.ClearDocumentPreviewFilesById(fileItem.ID);

    SaveChanges();
}
```

When the application is started, all HTML documents stored in the temporary storage are deleted:

```cs
public class Global_asax : System.Web.HttpApplication {
    void Application_Start(object sender, EventArgs e)
    {
        Utils.ClearDocumentPreviews();
    }
    ...
}
```
### Displaying HTML documents

HTML documents are displayed in the *viewer* `<div>` element stored inside the *viewerWrapper* `<div>` element:

```html
<div class="viewer-wrapper" id="viewerWrapper" runat="server">
    <div runat="server" id="viewer" class="viewer">
    </div>
</div>
```
The following CSS classes are applied to the these elements:

```css
/* == ViewMode Viewer == */
.viewer-wrapper {
    box-sizing: border-box;
    height: 100%;
    overflow-y: auto;
}

@media (min-width: 601px) {
    .viewer-wrapper {
        padding: 15px 0;
    }
}

.viewer {
    box-sizing: border-box;
    min-height: 100%;
    max-width: 816px;
    margin: 0px auto;
    padding: 24px;
    word-wrap: break-word;
}

@media (min-width: 816px) {
    .viewer {
        padding: 48px;
        border: 1px solid #C0C0C0;
        -webkit-box-shadow: rgba(0, 0, 0, 0.1) 0 1px 3px;
        -moz-box-shadow: rgba(0, 0, 0, 0.1) 0 1px 3px;
        box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    }
}
```
The media queries specify responsive paddings for different browser window's width value. Mobile screens require small paddings to provide more space for the application's content.

![RichPaddings](/img/richPaddings.gif)

## View mode for worksheets

Worksheets (.xlsx, .xls files) are processed in the [Spreadsheet.aspx](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/DocumentManagementDemo/DocumentManagementDemo/Spreadsheet.aspx) page. 

The ASPxSpreadsheet provides [Reading View Mode](https://docs.devexpress.com/AspNet/120172/asp.net-webforms-controls/spreadsheet/concepts/data-presentation/reading-view-mode) that disables editing functionality and replaces the Ribbon UI with a compact toolbar. When a document is opened in view mode, the Spreadsheet control's [Mode](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxSpreadsheet.SpreadsheetViewSettings.Mode) property is set to **Reading**.

In this project, the reading mode's toolbar and [formula bar](https://docs.devexpress.com/AspNet/117334/asp.net-webforms-controls/spreadsheet/visual-elements/formula-bar) are hidden to provide more space for vieving the document's content, expecially on mobile devices.


```cs
protected void Page_Load(object sender, EventArgs e)
{
    // Opening Document
    var documentID = Request.QueryString["id"];
    int itemID = -1;
    int.TryParse(documentID, out itemID);
    var item = Utils.CurrentDataProvider.GetDocumentById(itemID);

    var data = item.Content.Data;

    spreadSheet.Open(documentID + "&" + Guid.NewGuid().ToString(), () => data);

    // View Mode
    if (string.IsNullOrEmpty(Request.QueryString["edit"]))
    { // is view mode?
        spreadSheet.SettingsView.Mode = SpreadsheetViewMode.Reading;
        spreadSheet.ShowFormulaBar = false;
    }
}
```

When the page is loaded, the control parses the query string to determine in which mode the document should be opened.

## Editing office documents

In this solution, office documents are edited in the DevExpress Office-inspired controls: 
- [ASPxRichEdit](https://docs.devexpress.com/AspNet/17721/asp.net-webforms-controls/rich-text-editor) - for rich text documents
- [ASPxSpreadsheet](https://docs.devexpress.com/AspNet/16157/asp.net-webforms-controls/spreadsheet) for worksheets.
These controls provide similar API and features used in this solution.

### Opening documents for editing

Office documents are opened for editing using the **Open** methods of corresponding controls: [ASPxRichEdit.Open()](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxRichEdit.ASPxRichEdit.Open.overloads) and [ASPxSpreadsheet.Open()](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxSpreadsheet.ASPxSpreadsheet.Open.overloads). The document's format is detected automatically.

```cs
richEdit.Open(documentID + "&" + Guid.NewGuid().ToString(), () => data);
spreadSheet.Open(documentID + "&" + Guid.NewGuid().ToString(), () => data);
```

The `Guid.NewGuid().ToString()` code guarantees the *documentID* parameter's unique value when multiple users open the same document. Refer to the [DevExpress Office Controls sharing document rules](https://docs.devexpress.com/AspNet/117688/common-concepts/office-document-management/document-loading/sharing-documents) topic for more information about sharing documents between users.

### Saving documents

When a document is saved, the *Utils.CurrentDataProvider.SetDocumentContent* method is called to update the *ContentSize* filed described in [this topic](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/Database.md#1-create-the-database).

```cs
protected void richEdit_Saving(object source, DocumentSavingEventArgs e)
{
    ...
    var item = Utils.CurrentDataProvider.GetDocumentById(id);
    if (item != null)
        Utils.CurrentDataProvider.SetDocumentContent(id, richEdit.SaveCopy(Utils.GetRichEditDocumentFormatByExtension(item.Name)));
    e.Handled = true;
}

protected void spreadSheet_Saving(object source, DevExpress.Web.Office.DocumentSavingEventArgs e)
{
    ...
    var item = Utils.CurrentDataProvider.GetDocumentById(id);
    if (item != null)
        Utils.CurrentDataProvider.SetDocumentContent(id, spreadSheet.SaveCopy(Utils.GetSpreadsheetDocumentFormatByExtension(item.Name)));
    e.Handled = true;
}
```
### Ribbon and behavior settings

In this project, the ribbon UI is customized to hide unusable commands for both controls. The following code modifies the content of the [Rich Edit's](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxRichEdit.ASPxRichEdit.RibbonTabs) and  [Spreadsheet's](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxSpreadsheet.ASPxSpreadsheet.RibbonTabs) **RibbonTabs** collection respectively:

```cs
public static void HideFileTab(ASPxSpreadsheet spreadsheet)
{
    spreadsheet.CreateDefaultRibbonTabs(true);
    spreadsheet.RibbonTabs.RemoveAt(0);
    SRFileCommonGroup gr = new SRFileCommonGroup();
    gr.Items.Add(new SRFileSaveCommand());
    gr.Items.Add(new SRFilePrintCommand());
    spreadsheet.RibbonTabs[0].Groups.Insert(0, gr);
    spreadsheet.ActiveTabIndex = 0;
}

public static void HideFileTab(ASPxRichEdit richEdit)
{
    richEdit.CreateDefaultRibbonTabs(true);
    richEdit.RibbonTabs.RemoveAt(0);
    RERFileCommonGroup gr = new RERFileCommonGroup();
    gr.Items.Add(new RERSaveCommand());
    gr.Items.Add(new RERPrintCommand());
    richEdit.RibbonTabs[0].Groups.Insert(0, gr);
    richEdit.ActiveTabIndex = 0;
}
```

Note, that the ribbon UI is displayed only when a document is opened in edit mode for both controls.

Some commands can be executed on the client using the client controls (the [ASPxClientRichEdit](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxRichEdit.Scripts.ASPxClientRichEdit) and [ASPxClientSpreadsheet](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxSpreadsheet.Scripts.ASPxClientSpreadsheet) objects). Use the **Behavior** properties to disable the ability to execute client commands: [ASPxRichEdit.Settings.Behavior](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxRichEdit.ASPxRichEditSettings.Behavior) and [ASPxSpreadsheet.Settings.Behavior](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxSpreadsheet.ASPxSpreadsheetSettings.Behavior) respectively:

```html
<dx:ASPxRichEdit ID="richEdit" runat="server">
    <Settings>
        <Behavior FullScreen="Hidden" CreateNew="Hidden" Open="Hidden" SaveAs="Hidden"></Behavior>
    </Settings>
</dx:ASPxRichEdit>

<dx:ASPxSpreadsheet ID="spreadSheet" runat="server">
    <Settings>
        <Behavior CreateNew="Hidden" Open="Hidden" SaveAs="Hidden" SwitchViewModes="Hidden"></Behavior>
    </Settings>
</dx:ASPxSpreadsheet>
```
