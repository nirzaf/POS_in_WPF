<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
      <%if (TempData["errorMessage"] != null) { %>
        <div class = "error">
        <h2><%=Html.Encode(TempData["errorMessage"]) %></h2>
        </div>
      <%} %>
      <%using (Html.BeginForm("new", "orders")) { %>
      <%=Html.AntiForgeryToken() %>
      <fieldset class="centered">
        <div>
          <label class="orange">
            <img src="/content/images/mug_small.png">
            S
          </label>
          <input type="radio" name="id" value="small" checked="true">
          <br>
          $1.99
        </div>
        <div>
          <label class="orange">
            <img src="/content/images/mug_medium.png">
            M
          </label>
          <input type="radio" name="id" value="medium">
          <br>
          $2.99
        </div>
        <div>
          <label class="orange">
            <img src="/content/images/mug_large.png">
            L
          </label>
          <input type="radio" name="id" value="large">
          <br>
          $3.99
        </div>
      </fieldset>
      <input type="submit" class="submit" value="Continue">
      <%} %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Hot Coffee
</asp:Content>
