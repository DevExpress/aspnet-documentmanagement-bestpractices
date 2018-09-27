<%@ Page Language="C#" MasterPageFile="~/Document.Master" AutoEventWireup="true" CodeBehind="Spreadsheet.aspx.cs" Inherits="DocumentManagementDemo.Spreadsheet" %>

<%@ Register Assembly="DevExpress.Web.ASPxSpreadsheet.v18.1" Namespace="DevExpress.Web.ASPxSpreadsheet" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <dx:ASPxSpreadsheet Theme="Mulberry" ID="spreadSheet" runat="server" Width="100%" Height="100%" ClientInstanceName="spreadSheet"
        OnSaving="spreadSheet_Saving" FullscreenMode="false">
        <Settings>
            <Behavior CreateNew="Hidden" Open="Hidden" SaveAs="Hidden" SwitchViewModes="Hidden"></Behavior>
        </Settings>
    </dx:ASPxSpreadsheet>
</asp:Content>
