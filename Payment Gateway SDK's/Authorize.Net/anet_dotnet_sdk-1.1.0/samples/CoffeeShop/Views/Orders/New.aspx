<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    New Order
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<section class = "centered">	
	
    <% var order = (CoffeeShop.Web.Order)ViewData["order"];%>
		
     <h1>Order: <%=order.ProductName%> </h1>
    </section>
	<section class = "purchase">
		<img src = "/content/images/mug_<%=order.Slug%>.png" />
		<p><%=order.Price.ToString("C")%></p>
        
        <div style="padding-top:20px">
        <%using(Html.BeginForm("create","orders")) {%>           
            <%=Html.AntiForgeryToken() %>
            <%=Html.Hidden(AuthorizeNet.ApiFields.Amount, order.Price)%>
		    <%=Html.Hidden("order_id",order.OrderID) %>
            <%=Html.CheckoutFormInputs(true) %>
		    
            <div style = "clear:both" ></div>
            <br>


		    <input type = "submit" value = "Pay Up!" />
	    <%}%>
	    </div>
	    <div style="padding-top:20px">
	    
	    <h2>Or Checkout using SIM</h2>
	    
	    <%using (Html.BeginSIMForm("http://YOURSERVER.com/orders/sim",
                order.Price,
                ConfigurationManager.AppSettings["ApiLogin"],
	            ConfigurationManager.AppSettings["TransactionKey"],true)){%>
	     
		    <%=Html.CheckoutFormInputs(true)%>
		    
		    <%=Html.Hidden("order_id",order.OrderID) %>
            <div style = "clear:both" ></div>
		    <input type = "submit" value = "Pay Up with SIM!" />
	     
	     
	     <%} %>
	     </div>
    </section>


</asp:Content>


