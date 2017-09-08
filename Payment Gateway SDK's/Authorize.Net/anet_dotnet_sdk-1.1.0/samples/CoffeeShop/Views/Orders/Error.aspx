<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% var order = (CoffeeShop.Web.Order)ViewData["order"];%>
    <h2>Oops! There was an error with your order.</h2>
    <p>Sorry for the inconvenience - but your order couldn't be processed</p>
    <p>
        <i><%=Html.Encode(order.OrderMessage) %></i>
    </p>
    <p>
        <a href = "<%=Url.SiteRoot() %>/coffee">Order Another?</a>
    </p>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Purchase Error
</asp:Content>
