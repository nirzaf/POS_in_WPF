<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="MainContentContent" ContentPlaceHolderID="MainContent" runat="server">
	<% var order = (CoffeeShop.Web.Order)Session["order"];%>
	<section class = "centered">	
		<h1>Very sorry - your order was returned. We'll put a fresh pot on for you! Receipt: <%=Html.Encode(order.AuthCode)%> </h1>
	</section>
	<section>
		<img src = "/content/images/mug_<%=order.Slug%>.png" />
        <p>
            <a href = "/coffee" class = "button positive">Buy Another</a>
        </p>
	</section>
</asp:Content>
