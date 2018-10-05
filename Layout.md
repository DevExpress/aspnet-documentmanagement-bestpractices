# Step 3: Building a responsive layout for the file explorer using DevExpress controls and CSS styles

This article describes how to implement a responsive layout for a web application using DevExpress ASP.NET controls. The described layout is used in the web application intended for document management.

The layout consists of the following areas:

- [Page Header](#page-header)
- [Left-side Collapsible Menu](#left-side-collapsible-menu)
- [File Manager](#adaptive-file-manager)

![MainElements](/img/layoutHTML.png)

The **Page Header** is implemented using the [ASPxPanel](https://docs.devexpress.com/AspNet/14778/asp.net-webforms-controls/site-navigation-and-layout/panel-overview) control whose ID id *HeaderPanel*.

The **Left-side Menu** and **Content Area** are stored inside the ```<div class="app-content">``` container.
- The **Left-side Menu** is implemented using the [ASPxPanel](https://docs.devexpress.com/AspNet/14778/asp.net-webforms-controls/site-navigation-and-layout/panel-overview) control whose ID is *LeftPanel*.
- The **Content Area** is contained inside the ```<div class="app-content-wrapper">``` tab. This is a container for a large adaptive control that displays most of the page's content. In this application, the **Content Area** is populated with the [ASPxFileManager](https://docs.devexpress.com/AspNet/9032/asp.net-webforms-controls/file-management/file-manager) control. 

```html

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="user-scalable=0, width=device-width, initial-scale=1.0, maximum-scale=1.0" />
    ...
</head>
<body>
    <form>
        <dx:ASPxPanel runat="server" ID="HeaderPanel" ClientInstanceName="headerPanel">
            ...
        </dx:ASPxPanel>

        <div class="app-content">
            <dx:ASPxPanel runat="server" ID="LeftPanel" ClientInstanceName="leftPanel">
            </dx:ASPxPanel>

            <div class="app-content-wrapper">
                <dx:ASPxFileManager ID="fileManager" runat="server">
                    ...
                </dx:ASPxFileManager>
            </div>
        </div>
    </form>
</body>
</html>
```
Note that it is required to specify the *meta* tag in the *head* section to specify the [viewport's settings](https://developer.mozilla.org/en-US/docs/Mozilla/Mobile/Viewport_meta_tag).


The following sections describe how to modify the DevExpress controls to make them adaptive and responsive.

## Page Header

The page header is implemented using the [ASPxPanel](https://docs.devexpress.com/AspNet/14778/asp.net-webforms-controls/site-navigation-and-layout/panel-overview) control. The page header includes the following UI elements:

- **Toolbar Left and Right Buttons**
- **Title** 

 ![Header](/img/Header.png)

The left button (hamburger button) expands/collapses the left-side menu. The right button is a dummy that can be replaced for any button.

The header panel is docked to the top of the page (regardless of page scrolling) because its [FixedPosition](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCollapsiblePanel.FixedPosition) property is set to **WindowTop**.

The header's content is stored in three ```<div>``` sections:

```html
<dx:ASPxPanel runat="server" ID="HeaderPanel" ClientInstanceName="headerPanel" FixedPosition="WindowTop" FixedPositionOverlap="true" CssClass="app-header-toolbar">
    <PanelCollection>
        <dx:PanelContent runat="server" SupportsDisabledAttribute="True">
            <div class="left">
                <dx:ASPxHyperLink runat="server" ImageUrl="~/Images/burger-menu.svg"
                    NavigateUrl="javascript:toggleLeftPanel()" Width="20px" Height="20px" CssClass="button">
                </dx:ASPxHyperLink>
            </div>
            <div class="right">
                <dx:ASPxHyperLink runat="server" ImageUrl="~/Images/bar-contacts.svg"
                    NavigateUrl="javascript:void(0)" Width="20px" Height="20px" CssClass="button">
                </dx:ASPxHyperLink>
            </div>
            <div class="title">
                <span class="title-text">Document Management</span>
            </div>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxPanel>
```

### Page Header's Styles

The header panel's height, paddings, and background color are defined in the following CSS classes:

```css
.app-header-toolbar {
    border: none;
    padding-top: 15px;
    padding-bottom: 14px;
    height: 42px;
}

.dxpnlControl_Office365.dxpnl-edge.app-header-toolbar {
    background-color: #4a4a4a;
}

.dxpnlControl_Office365.dxpnl-edge, .dxpnlControl_Office365.dxpnl-edge.dxpnl-bar {
    padding: 0px;
}
```

The *.dxpnlControl_Office365.dxpnl-edge* class is used to override the header panel's default background color and paddings. To access a target CSS class's name use the Page Inspector tools in a web browser.

If the title text does not fit the header, it is trimmed to adjust the header panel, and an ellipsis is inserted at the end of the trimmed text. This behavior is specified using the *text-overflow: ellipsis* CSS rule.

```css
.app-header-toolbar .title .title-text {
    overflow: hidden;
    display: block;
    text-overflow: ellipsis;
    white-space: nowrap;
    padding: 0 8px;
}
```


## Left-side Collapsible Menu

The collapsible menu that is expanded from the browser window's left edge is a standard template for mobile-friendly web applications. 

In this project, the [ASPxPanel](https://docs.devexpress.com/AspNet/14778/asp.net-webforms-controls/site-navigation-and-layout/panel-overview) control is used as a collapsible container for the [ASPxMenu](https://docs.devexpress.com/AspNet/3575/asp.net-webforms-controls/site-navigation-and-layout/menu/aspxmenu-overview) control.

```html

<dx:ASPxPanel runat="server" ID="LeftPanel" ClientInstanceName="leftPanel" FixedPosition="WindowLeft" FixedPositionOverlap="true" Collapsible="true" CssClass="app-left-panel" Width="260px">
    <SettingsAdaptivity CollapseAtWindowInnerWidth="960" />
    <SettingsCollapsing Modal="true" ExpandEffect="PopupToRight" AnimationType="Slide" ExpandButton-Visible="false" />
    <Styles>
        <ExpandedPanel CssClass="expanded" />
    </Styles>
    <PanelCollection>
        <dx:PanelContent>
            <dx:ASPxMenu ID="FileManagerFiltersMenu" runat="server" Orientation="Vertical" AllowSelectItem="true" Width="100%"
                CssClass="filters-menu" SeparatorCssClass="menu-separator" ClientInstanceName="fileManagerFiltersMenu">
                <ItemStyle CssClass="menu-item">
                    <HoverStyle CssClass="menu-item-hover" />
                    <SelectedStyle CssClass="menu-item-selected" />
                </ItemStyle>

                <Items>
                    <dx:MenuItem Text="All" Name="All" Selected="true" />
                    <dx:MenuItem Text="Recent" Name="Recent" />
                    <dx:MenuItem Text="RichTexts" Name="Docs" />
                    <dx:MenuItem Text="Worksheets" Name="Sheets" />
                    <dx:MenuItem Text="Images" Name="Images" />
                    <dx:MenuItem Text="Pdfs" Name="Pdfs" />
                </Items>
                <ClientSideEvents ItemClick="onFiltersMenuItemClick" />
            </dx:ASPxMenu>

        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxPanel>
```

The panel is automatically collapsed when the browser window's width is less than 960 px. This behavior is defined by setting the [Collapsible](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxCollapsiblePanel.Collapsible) property to **true** and the [CollapseAtWindowInnerWidth](https://docs.devexpress.com/AspNet/DevExpress.Web.PanelAdaptivitySettings.CollapseAtWindowInnerWidth) property to **960**. 

Click the hamburger button expands the panel. The following client code is executed when the button is pressed:

```js
function toggleLeftPanel() {
    if (leftPanel.IsExpandable())
        leftPanel.Toggle();
    else {
        leftPanel.SetVisible(!leftPanel.GetVisible());
        fileManager.AdjustControl();
    }
}
```

This code checks whether the panel can be expanded/collapsed using the [IsExpanded()](https://docs.devexpress.com/AspNet/DevExpress.Web.Scripts.ASPxClientPanel.IsExpanded) method. According to the [CollapseAtWindowInnerWidth](https://docs.devexpress.com/AspNet/DevExpress.Web.PanelAdaptivitySettings.CollapseAtWindowInnerWidth) property, the method returns **true** if the browser window's width is less than **960**, otherwise it returns **false**.

- If the method returns **true**, the panel is expanded or collapsed depending on its current state. The panel is displayed in modal mode because the [Modal](https://docs.devexpress.com/AspNet/DevExpress.Web.PanelCollapsingSettings.Modal) property set to **true**. In this mode, end-users can collapse the menu by clicking the darkened area. 
![ModalWindow](/img/ModalWindow.png) 
If the panel is in the modal mode, it should not occupy the whole browser window's width. Otherwise, there will be no place for the darkened area.

- If the method returns **false**, the panel is not collapsed/expanded automatically. However, it can be hidden/displayed when a user clicks the hamburger button using the control's [SetVisible](https://docs.devexpress.com/AspNet/DevExpress.Web.Scripts.ASPxClientControlBase.SetVisible(System.Boolean)) method. The [AdjustControl](https://docs.devexpress.com/AspNet/DevExpress.Web.Scripts.ASPxClientControl.AdjustControls) method is called to recalculate the control's size when the panel is hidden/displayed.

### Left-side Menu's Styles

The following CSS classes are attached to the **ASPxMenu** control and its container - **ASPxPanel** control:

```html
<dx:ASPxPanel runat="server" ID="LeftPanel" CssClass="app-left-panel">
    ...
    <Styles>
        <ExpandedPanel CssClass="expanded" />
    </Styles>
    <PanelCollection>
        <dx:PanelContent>
            <dx:ASPxMenu ID="FileManagerFiltersMenu" runat="server" CssClass="filters-menu" SeparatorCssClass="menu-separator">
                <ItemStyle CssClass="menu-item">
                    <HoverStyle CssClass="menu-item-hover" />
                    <SelectedStyle CssClass="menu-item-selected" />
                </ItemStyle>
                <Items>
                   ...
                </Items>
            </dx:ASPxMenu>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxPanel>

```
The CSS rules applied to these controls modify their appearance: 

- remove unnecessary borders, background color, fonts,
- removes default separators (using the *menu-separator* selector),
- changes paddings,
- changing hover and selected styles (using the *menu-item-hover* and *menu-item-selected* classes respectively).


The following CSS rule is used to apply shadow to the menu container:

```css
/* Shadow */
.app-left-panel {
    border-right: 1px solid #f0f0f0;
    box-shadow: 0px 2px 8px 0 rgba(0, 0, 0, 0.23);
}
```

## Adaptive File Manager

The File Manager control provides the [SettingsAdaptivity](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxFileManager.SettingsAdaptivity)  property that enables the mobile-friendly mode that resizes the control to fit the browser's window. This property is set to **true**. In this project, the mobile-friendly mode enables an adaptive toolbar and adaptive detail view mode for the File Manager control. You can refer to [this article](https://docs.devexpress.com/AspNet/119725/asp.net-webforms-controls/file-management/file-manager/concepts/adaptivity) for more information about the ASPxFileManager's adaptivity. 

```cs
<dx:ASPxFileManager ID="ASPxFileManager1" runat="server">
    <SettingsAdaptivity Enabled="true"/>
</dx:ASPxFileManager>
```

### Adaptive Toolbar

 The File Manager's adaptive toolbar hides its items to adjust the toolbar's size to the browser window's width. An end-users can access the hidden items in the popup window available by clicking the ellipsis button on the toolbar's right. 

![Adaptive Toolbar](/img/AdaptiveToolbar.png)

You can specify the order in which toolbar items are hidden using their **AdaptivePriority** property. The lesser value, the later the toolbar item is hidden. The default **AdaptivePriority** value is 0.

In this project, the **Refresh**, **Delete**, **Copy**, **Move**, **Rename**, and **Create** buttons' **AdaptivePriority** value is equaled to **10**. These buttons are hidden at first (from right to left) if the toolbar does not fit the browser's window. 

```html
<dx:ASPxFileManager ID="ASPxFileManager1" runat="server">
    
    <SettingsAdaptivity Enabled="true"/>

    <SettingsToolbar ShowPath="False">
        <Items>
            <dx:FileManagerToolbarDownloadButton ToolTip="Download" AdaptivePriority="1" />
            <dx:FileManagerToolbarUploadButton ToolTip="Upload" AdaptivePriority="1" />
            <dx:FileManagerToolbarCreateButton ToolTip="Create (F7)" AdaptivePriority="10" BeginGroup="false" />
            <dx:FileManagerToolbarRenameButton ToolTip="Rename (F2)" AdaptivePriority="10" />
            <dx:FileManagerToolbarMoveButton ToolTip="Move (F6)" AdaptivePriority="10" />
            <dx:FileManagerToolbarCopyButton ToolTip="Copy" AdaptivePriority="10" />
            <dx:FileManagerToolbarDeleteButton ToolTip="Delete (Del)" AdaptivePriority="10" />
            <dx:FileManagerToolbarRefreshButton ToolTip="Refresh" AdaptivePriority="10" BeginGroup="false" />
        </Items>
    </SettingsToolbar>

</dx:ASPxFileManager>
```

### Adaptive Detail View

The File Manager supports two [view modes](https://docs.devexpress.com/AspNet/14550/asp.net-webforms-controls/file-management/file-manager/concepts/view-modes): **Thumbnails** and **Details**. In this project, the File Manager is in **Details** view mode. In the **Details** view mode, a file list displays information about files (such as their files, date modified, etc.) in a grid view. 

When the adaptive mode is enabled for the File Manager control, the detail view grid hides its columns if they do not fit the browser window.

![Adaptive Detail View](/img/AdaptiveDetailView.png)

The detail view grid's columns provide **AdaptivePriority** property to specify the order in which columns are hidden.


```html
<dx:ASPxFileManager ID="ASPxFileManager1" runat="server">
    
    <SettingsAdaptivity Enabled="true"/>

    <SettingsFileList ShowFolders="true" ShowParentFolder="true" View="Details" AllowCustomizeFolderDisplayText="true">
        <DetailsViewSettings ThumbnailHeight="16" ThumbnailWidth="16" ShowSelectAllCheckbox="true" AllowColumnDragDrop="false">
            <SettingsCommandColumn AdaptivePriority="2" HeaderStyle-CssClass="fm-cmd-header" />
            <Columns>
                <dx:FileManagerDetailsColumn Caption="" FileInfoType="Thumbnail" AdaptivePriority="2">
                </dx:FileManagerDetailsColumn>
                <dx:FileManagerDetailsColumn Caption="Name" FileInfoType="FileName" AdaptivePriority="1">
                    <ItemTemplate>
                        ...
                    </ItemTemplate>
                </dx:FileManagerDetailsColumn>
                <dx:FileManagerDetailsColumn Caption="Modified" FileInfoType="LastWriteTime" AdaptivePriority="2">
                </dx:FileManagerDetailsColumn>
                <dx:FileManagerDetailsColumn Caption="Size" FileInfoType="Size" AdaptivePriority="3">
                    <HeaderStyle HorizontalAlign="Right" />
                </dx:FileManagerDetailsColumn>
                <dx:FileManagerDetailsCustomColumn AdaptivePriority="10" />
            </Columns>

        </DetailsViewSettings>
    </SettingsFileList>

</dx:ASPxFileManager>
```

The dummy column at the end of the *Columns* collection improves readability on large screens. This column occupies space in the detail view grid to prevent "spreading" of the file explorer's content over the entire screen's width.  

### File Manager: Relative Height

The ASPxFileManager should occupy the whole **ContentArea**. For this purpose, the ASPxFileManager's **Height** property is set to **100%** and the following CSS rules are applied to the **ContentArea** using the *app-content* selector:

```css
html, body, form, .app-content {
    height: 100%;
    margin: 0;
    padding: 0;
    overflow: hidden;
}
```

See the [Relative height (100%) in DevExpress ASP.NET controls](https://www.devexpress.com/Support/Center/Question/Details/KA18866/relative-height-100-in-devexpress-asp-net-controls) KB article to find more technical information about this trick. 


### File Manager: Item Template

The file manager's items are implemented using [item templates](https://docs.devexpress.com/AspNet/8956/asp.net-webforms-controls/file-management/file-manager/visual-elements/item). The item template customizes the *Name* column in the detail view grid.

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

The template displays a file's name inside the `<a>` tag that defines a hyperlink. When a user clicks the item, the *GetItemUrl* method detects whether a folder or file was clicked and execute the corresponding action:
```cs
protected string GetItemUrl(FileManagerItem item)
{
    if (item is FileManagerFolder)
        return "javascript:openCurrentFolder();";
    else
        return "OpenDocumentHandler.aspx?id=" + item.Id;
}
```

## CSS Rules for mobile devices 

This section describes CSS rules that are used to optimize the DevExpress controls' appearance on mobile devices:

### Preventing overscrolling on iOS devices

The overscrolling is an ability to scroll the page's content beyond the height of the window in mobile Safari. 

The following CSS rule allows you to prevent this behavior for DevExpress controls.

```css
html.dxMacOSMobilePlatform {
    position: fixed;
    width: 100%;
}
```

The *dxMacOSMobilePlatform* CSS class is automatically attached to the `<html>` tag when you use DevExpress controls on mobile devices with iOS. You can use this CSS class to apply platform-oriented rules for iOS devices. 

In this project, this rule prevents overscrolling in the File Manager control for iOS devices.

### Smooth scrolling for iOS devices:

When end-users scroll web pages on iOS devices, the scrolling is going until 

Web pages on iOS by default have a "momentum" style scrolling where a flick of the finger sends the web page scrolling, and it keeps going until eventually slowing down and stopping as if friction is slowing it down ([source](https://css-tricks.com/snippets/css/momentum-scrolling-on-ios-overflow-elements/)).

The CSS rule below allows you to enable this scrolling for a web page's elements:

```css
.viewer-wrapper {
    box-sizing: border-box;
    height: 100%;
    overflow-y: auto;
    -webkit-overflow-scrolling: touch; /* smooth scrolling for iOS */
}
```

## Next Step

**Step 4**: [Reading and viewing office text documents using Rich Text Editor and Spreadsheet controls](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/master/OfficeDocs.md)