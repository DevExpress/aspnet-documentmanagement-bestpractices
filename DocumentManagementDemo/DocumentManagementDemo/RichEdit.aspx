<%@ Page Language="C#" MasterPageFile="~/Document.Master" AutoEventWireup="true"  CodeBehind="RichEdit.aspx.cs" Inherits="DocumentManagementDemo.RichEdit" %>

<%@ Register assembly="DevExpress.Web.ASPxRichEdit.v18.1" namespace="DevExpress.Web.ASPxRichEdit" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <div class="viewer-wrapper" id="viewerWrapper" runat="server">
        <div runat="server" id="viewer" class="viewer">
        </div>
    </div>

    <dx:ASPxRichEdit ID="richEdit" runat="server" Width="100%" Height="100%" ClientInstanceName="richEdit" 
        OnSaving="richEdit_Saving">
        <Settings>
            <Behavior FullScreen="Hidden" CreateNew="Hidden" Open="Hidden" SaveAs="Hidden"></Behavior>
            <RangePermissions Visibility="Auto"></RangePermissions>
        </Settings>
    </dx:ASPxRichEdit>
</asp:Content>
