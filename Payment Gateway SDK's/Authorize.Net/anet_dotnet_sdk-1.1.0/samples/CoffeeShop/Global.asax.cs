using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CoffeeShop.Web;

namespace CoffeeShop {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Coffee", "coffee", new { controller = "Home", action = "Coffee" });
            routes.MapRoute("SIM", "orders/sim", new { controller = "Orders", action = "SimResponse" });
            routes.MapRoute("Return", "order/return/{id}", new { controller = "Orders", action = "Destroy", id = "" });
            routes.MapRoute("Receipt", "order/receipt/{id}", new { controller = "Orders", action = "Details" });
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = "" });
        }

        protected void Application_Start() {
            RegisterRoutes(RouteTable.Routes);
            Orders = new List<Order>();
        }
        public static List<Order> Orders;
        public static void SaveOrder(Order order) {
            var found = FindOrder(order.OrderID);
            if (found != null) {
                Orders.Remove(found);
            }
            Orders.Add(order);

        }
        public static Order FindOrder(Guid orderID) {
            return Orders.FirstOrDefault(x => x.OrderID == orderID);
        }

    }
}