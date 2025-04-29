<%@ Page Title="" Language="C#" MasterPageFile="~/NguoiDung/NguoiDung.Master" AutoEventWireup="true" CodeBehind="GetCartCount.aspx.cs" Inherits="WebDatDoAnOnline.NguoiDung.GetCartCount" %>
<% 
    Response.Clear();
    Response.ContentType = "text/plain";
    Response.Write(Session["cartCount"] != null ? Session["cartCount"].ToString() : "0");
    Response.End();
%>
