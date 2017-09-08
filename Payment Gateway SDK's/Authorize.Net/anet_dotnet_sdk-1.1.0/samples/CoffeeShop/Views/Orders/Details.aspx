<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="MainContentContent" ContentPlaceHolderID="MainContent" runat="server">
<section class = "centered">	
	

    <% var order = (CoffeeShop.Web.Order)ViewData["order"];%>
	<p>
	    <h1><%=Html.Encode(order.OrderMessage)%> </h1>    
    </p>

	<img src = "<%=Url.SiteRoot()%>/content/images/mug_<%=order.Slug%>.png" />
	<p>Yummy! Drink up...</p>
    <p>
        <form action = "<%=Url.AbsoluteActionLink("return","order", new {id = order.OrderID}) %>" method = "post">
        
            <%=Html.AntiForgeryToken() %>
            <input type="submit" value="Don't Like It?">
        
        </form>
    </p>

</section>
</asp:Content>
