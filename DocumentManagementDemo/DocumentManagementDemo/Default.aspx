<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DocumentManagementDemo.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Document Management Demo</title>
    <meta name="viewport" content="user-scalable=0, width=device-width, initial-scale=1.0, maximum-scale=1.0" />
    <link rel="stylesheet" type="text/css" href="Css/styles.css?v=2" />
    <script type="text/javascript" src="Scripts/script.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxPanel runat="server" ID="HeaderPanel" ClientInstanceName="headerPanel" FixedPosition="WindowTop"
            FixedPositionOverlap="true" CssClass="app-header-toolbar">
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
                        <span class="title-text">Document Management Demo</span>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxPanel>

        <div class="app-content">
            <dx:ASPxPanel runat="server" ID="LeftPanel" ClientInstanceName="leftPanel" FixedPosition="WindowLeft"
                FixedPositionOverlap="true" Collapsible="true" CssClass="app-left-panel" Width="260px">
                <SettingsAdaptivity CollapseAtWindowInnerWidth="960" />
                <SettingsCollapsing Modal="true" ExpandEffect="PopupToRight" AnimationType="Slide" ExpandButton-Visible="false"/>
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

            <div class="app-content-wrapper">
                <dx:ASPxFileManager ID="fileManager" runat="server" Height="100%" Width="100%"
                    OnCustomCallback="fileManager_CustomCallback" CssClass="file-manager"
                    OnCustomThumbnail="fileManager_CustomThumbnail"
                    OnCustomJSProperties="fileManager_CustomJSProperties">

                    <ClientSideEvents
                        SelectedFileOpened="onFileManagerSelectedFileOpened"
                        Init="onFileManagerInit" EndCallback="onFileManagerEndCallback"
                        CurrentFolderChanged="onFileManagerCurrentFolderChanged" />

                    <SettingsAdaptivity Enabled="true" />

                    <Settings EnableMultiSelect="true" ThumbnailFolder="~\Thumb\"
                        AllowedFileExtensions=".rtf,.doc,.docx,.txt,.xlsx,.xls,.png,.gif,.jpg,.jpeg,.ico,.bmp,.avi,.mp3,.xml,.pdf" />
                    <SettingsUpload Enabled="true" ShowUploadPanel="false" ValidationSettings-MaxFileSize="4000000">
                        <AdvancedModeSettings EnableMultiSelect="true" EnableDragAndDrop="true" />
                    </SettingsUpload>
                    <SettingsEditing AllowCopy="True" AllowCreate="True" AllowDelete="True" AllowDownload="True" AllowMove="True" AllowRename="True" />

                    <SettingsFileList ShowFolders="true" ShowParentFolder="true" View="Details" AllowCustomizeFolderDisplayText="true">
                        <DetailsViewSettings ThumbnailHeight="16" ThumbnailWidth="16" ShowSelectAllCheckbox="true" AllowColumnDragDrop="false">
                            <SettingsCommandColumn AdaptivePriority="2" HeaderStyle-CssClass="fm-cmd-header" />
                            <Columns>
                                <dx:FileManagerDetailsColumn Caption="" FileInfoType="Thumbnail" AdaptivePriority="2">
                                </dx:FileManagerDetailsColumn>
                                <dx:FileManagerDetailsColumn Caption="Name" FileInfoType="FileName" AdaptivePriority="1">
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
                                <dx:FileManagerDetailsColumn Caption="Modified" FileInfoType="LastWriteTime" AdaptivePriority="2">
                                </dx:FileManagerDetailsColumn>
                                <dx:FileManagerDetailsColumn Caption="Size" FileInfoType="Size" AdaptivePriority="3">
                                    <HeaderStyle HorizontalAlign="Right" />
                                </dx:FileManagerDetailsColumn>
                                <dx:FileManagerDetailsCustomColumn AdaptivePriority="10" />
                            </Columns>

                        </DetailsViewSettings>
                    </SettingsFileList>
                    <SettingsFolders Visible="False" />
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

                    <SettingsBreadcrumbs Visible="true" ShowParentFolderButton="false" Position="Top" />

                    <SettingsFiltering FilteredFileListView="Standard" FilterBoxMode="Subfolders" />

                    <Styles>
                        <Breadcrumbs CssClass="fm-breadcrumbs" />
                        <BreadcrumbsItem CssClass="fm-breadcrumbs-item" />

                        <FileContainer CssClass="fm-item-list" />
                        <File FocusedStyle-CssClass="fm-item-focused" />
                        <Item CssClass="fm-item" HoverStyle-CssClass="fm-item-hovered"
                            SelectionActiveStyle-CssClass="fm-item-sel-act" SelectionInactiveStyle-CssClass="fm-item-sel-inact" />

                    </Styles>
                    <StylesDetailsView Header-CssClass="fm-header">
                        <CommandColumn CssClass="fm-check-col" HorizontalAlign="Left" />
                    </StylesDetailsView>

                    <Images>
                        <File Url="~/Images/document.svg" />
                        <FileAreaFolder Url="~/Images/folder.svg" />
                        <BreadcrumbsSeparator Url="~/Images/breadcrumbs-separator.svg" Width="11" Height="13" />
                        <DetailsCheckBoxChecked Url="~/Images/check.svg" Width="26" Height="26" />
                        <DetailsCheckBoxUnchecked Url="~/Images/check-unchecked.svg" Width="26" Height="26" />
                        <DetailsCheckBoxGrayed Url="~/Images/check.svg" Width="26" Height="26" />
                        <ParentFolder Url="~/Images/folder.svg"></ParentFolder>
                    </Images>

                </dx:ASPxFileManager>
            </div>
            <script>
                initHistoryState();
            </script>
        </div>
    </form>
</body>
</html>


