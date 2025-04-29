<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="QuanLyBaoCao.aspx.cs" Inherits="WebDatDoAnOnline.Admin.QuanLyBaoCao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .report-container {
            margin: 20px;
            padding: 20px;
            background-color: #f9f9f9;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }
        .report-title {
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 20px;
            color: #333;
        }
        .report-list {
            margin-bottom: 20px;
        }
        .report-list-item {
            padding: 10px;
            margin: 5px 0;
            background-color: #fff;
            border: 1px solid #ddd;
            border-radius: 5px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .export-button, .view-button {
            padding: 8px 16px;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            margin-left: 10px;
        }
        .export-button {
            background-color: #ff4500;
        }
        .export-button:hover {
            background-color: #ff6347;
        }
        .view-button {
            background-color: #007bff;
        }
        .view-button:hover {
            background-color: #0056b3;
        }
        .report-details {
            margin-top: 20px;
            padding: 15px;
            background-color: #fff;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-family: 'Roboto', sans-serif;
        }
        .report-details .logo {
            text-align: center;
            margin-bottom: 10px;
        }
        .report-details .logo img {
            max-width: 100px;
        }
        .report-details .pdf-title {
            font-size: 25px;
            font-weight: bold;
            color: #c8102e;
            text-align: center;
            margin-bottom: 10px;
            text-transform: uppercase;
        }
        .report-details .line-separator {
            border-top: 1px solid #c8102e;
            margin: 10px 0;
        }
        .report-details .info-table {
            width: 100%;
            margin-bottom: 10px;
        }
        .report-details .info-table td {
            padding: 5px;
            font-size: 11px;
        }
        .report-details .info-table .left {
            text-align: left;
        }
        .report-details .info-table .right {
            text-align: right;
        }
        .report-details .data-table {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
        }
        .report-details .data-table th {
            background-color: #c8102e;
            color: white;
            font-weight: bold;
            font-size: 11px;
            padding: 8px;
            text-align: center;
            border: 1px solid #c8102e;
        }
        .report-details .data-table td {
            background-color: #f5f5f5;
            font-size: 11px;
            padding: 5px;
            border: 1px solid #ddd;
            text-align: left;
        }
        .report-details .signature-table {
            width: 100%;
            margin: 20px 0;
        }
        .report-details .signature-table td {
            font-size: 11px;
            vertical-align: top;
        }
        .report-details .signature-table .signature-title {
            font-weight: bold;
        }
        .report-details .signature-table .left {
            text-align: left;
        }
        .report-details .signature-table .right {
            text-align: right;
        }
        .report-details .signature-table .space {
            height: 80px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="report-container">
        <div class="report-title">Quản Lý Báo Cáo</div>
        <div class="report-list">
            <asp:Repeater ID="rptReports" runat="server" OnItemCommand="rptReports_ItemCommand">
                <ItemTemplate>
                    <div class="report-list-item">
                        <span><%# Eval("Title") %></span>
                        <div>
                            <asp:Button ID="btnViewDetails" runat="server" Text="Xem chi tiết" CssClass="view-button" CommandName="ViewDetails" CommandArgument='<%# Eval("ReportID") %>' />
                            <asp:Button ID="btnExportPDF" runat="server" Text="Tải PDF" CssClass="export-button" CommandName="ExportPDF" CommandArgument='<%# Eval("ReportID") %>' />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <!-- Khu vực hiển thị chi tiết báo cáo -->
        <asp:Panel ID="pnlReportDetails" runat="server" CssClass="report-details" Visible="false">
            <asp:Literal ID="litReportDetails" runat="server"></asp:Literal>
        </asp:Panel>
    </div>
</asp:Content>