# ASP.NET Best Practices: Responsive Document Management Application

This repository stores a project that demonstrates best practices of using the DevExpress ASP.NET [File Manager](https://docs.devexpress.com/AspNet/14829/asp.net-webforms-controls/file-management), [Panel](https://docs.devexpress.com/AspNet/14778/asp.net-webforms-controls/site-navigation-and-layout/panel-overview),  [Spreadsheet](https://docs.devexpress.com/AspNet/16157/asp.net-webforms-controls/spreadsheet), and [Rich Text Editor](https://docs.devexpress.com/AspNet/17721/asp.net-webforms-controls/rich-text-editor). These controls are used to implement a responsive web application for managing office documents and media files (*.docx, *.xlsx, *.jpg, etc.).

You can download this repository and run it on your machine, click the QR code below to open the [online example](https://codecentral.devexpress.com/141286477/), or scan the code to execute the demo application on your mobile device. You will need to have DevExpress ASP.NET controls installed in order to load and compile the solution. You can download the installer from the [DevExpress website](https://devexpress.com).

[![QRCode](http://chart.apis.google.com/chart?choe=UTF-8&chld=H&cht=qr&chs=250x250&chl=https://demos.devexpress.com/BestPractices/DocumentManagementDemo/)](https://demos.devexpress.com/BestPractices/DocumentManagementDemo/)

This README file describes the functionality of the web application stored in this repository. You also can refer to [topics](#conceptual-topics) listed at the end of this README. These topics describe the application's features and how they were implemented with comments to the source code. 

- [Application description](#application-description)
- [How to launch the project on a local machine](#how-to-launch-the-project-on-a-local-machine)
- [Conceptual topics](#conceptual-topics)

## Application description

The application's main screen includes three main layout elements: 

- Header with text and two navigation buttons
- Left-side collapsible menu
- File explorer 

![MainElements](/img/MainElements.png)

The file explorer represented by the [ASPxFileManager](https://docs.devexpress.com/AspNet/9030/asp.net-webforms-controls/file-management/file-manager/aspxfilemanager-overview) displays the application's file system. End-users can manage files and folders: create, delete, copy, rename, or replace them.

The left-side menu allows end-users to display files only with specific extensions or files that were modified recently. End-users can collapse/expand the menu by clicking the hamburger button. If a browser window's width is less than 960px, the menu is collapsed when the application is started; otherwise, it is expanded. 

![AdaptivityGif](/img/adaptivityDemo.gif)

End-users can view and edit office documents (xlsx, xls, csv, docx, doc, rtf, and txt files) by clicking the corresponding item in the file explorer. The documents are opened in view mode:

![ReadingView](/img/ReadingView.png) 

This mode is adapted for viewing documents, especially on mobile devices:

- The page's render is faster than in the edit mode;
- Readability for different devices because of the adaptive layout;
- Easy navigation through documents on mobile devices using the browser's built-in search on the page.

End-users can to return to the file explorer or edit the document using navigation buttons. When users click the pen button, the document is opened in edit mode office documents in the [ASPxRichEdit](https://docs.devexpress.com/AspNet/17723/asp.net-webforms-controls/rich-text-editor/product-information/main-features) or [ASPxSpreadsheet](https://docs.devexpress.com/AspNet/16159/asp.net-webforms-controls/spreadsheet/product-information/main-features) controls according to the document's format.

![EditingView](/img/EditingView.png)

End-users can edit and save documents in this mode. The back button returns users to the read-only view.

## How to launch the project on a local machine

Follow the instructions below to launch the project on your machine:

1. Run the **CreateDatabase.aspx** page in your browser.
2. Click the **Create and populate Database** button. 
3. Wait until the button's text is changed to **Database created**.
4. Run the **Default.aspx** to execute the application.

These steps are required to create a database that stores files contained in the [AppData/Files](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/tree/web-forms/DocumentManagementDemo/DocumentManagementDemo/App_Data/Files) folder. The database stores files displayed in the file explorer. Refer to the [Populating the File Manager with files using a database as a file storage](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/Database.md) topic to see this feature's description.

## Conceptual topics
  
The following topics describe how this application was implemented.

- **Step 1**: [Populating the File Manager with files using a database as a file storage](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/Database.md)
- **Step 2**: [Implementing custom filtering and applying security settings for the File Manager](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/FileManager.md)
- **Step 3**: [Building a responsive layout for the file explorer using DevExpress controls and CSS styles](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/Layout.md)
- **Step 4**: [Reading and viewing office documents using Rich Text Editor and Spreadsheet controls](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/OfficeDocs.md)

## First Step

**Step 1**: [Populating the File Manager with files using a database as a file storage](https://github.com/DevExpress/aspnet-documentmanagement-bestpractices/blob/web-forms/Database.md)