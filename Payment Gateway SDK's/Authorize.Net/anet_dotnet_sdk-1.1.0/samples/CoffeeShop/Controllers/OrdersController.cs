using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using AuthorizeNet;
using System.Configuration;
using AuthorizeNet.Helpers;

namespace CoffeeShop.Web
{
	public class OrdersController : Controller
	{

        //pretend this is injected with IoC
        IGateway OpenGateway(){
            //we used the form builder so we can now just load it up
            //using the form reader
            var login = ConfigurationManager.AppSettings["ApiLogin"];
            var transactionKey = ConfigurationManager.AppSettings["TransactionKey"];

            //this is set to test mode - change as needed.
            var gate = new Gateway(login, transactionKey, true);
            return gate;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult New(string id)
		{

			
			//the "slug" is the item to be ordered. In our case it's "small", "medium", or "large"
			//if it's not one of these... the user must be confused :)
			if (id != "small" && id != "medium" && id != "large") {
                id = "small";
			}
			
			
			var order = new Order (id);
			
            //save to in-memory list to accomodate demo app
            //of course... save this to the DB in a real app.
            MvcApplication.SaveOrder(order);
            ViewData["order"] = order;
			return View ();
		}

        public ActionResult Details(string id) {
            var orderId = new Guid(id);
            ViewData["order"] = MvcApplication.FindOrder(orderId);
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Destroy(string id) {
            var orderId = new Guid(id);
            var order = MvcApplication.FindOrder(orderId);
            if (order == null)
                return Redirect("/");

            //this is a return, or a Void
            //just need the transaction ID
            var gate = OpenGateway();

            //void it
            var request = new VoidRequest(order.TransactionID);
            var response = gate.Send(request);

            if (response.Approved) {
                order.AuthCode = response.AuthorizationCode;
                order.OrderMessage = "Your order was refunded - we've put a fresh pot on";
                
                //reset it
                Session["order"] = order;

            } else {
                //error... oops. Reload the page
                order.OrderMessage = response.Message;
            }

            //record the order, send to the receipt page
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Create()
		{
            var orderId = new Guid(Request.Form["order_id"]);
            //pull from the store
            var order = MvcApplication.FindOrder(orderId);
            
            var gate = OpenGateway();

			//build the request from the Form post
            var apiRequest = CheckoutFormReaders.BuildAuthAndCaptureFromPost();
			
			//send to Auth.NET
			var response = gate.Send (apiRequest);
			
            //be sure the amount paid is the amount required
            if (response.Amount < order.Price) {
                order.OrderMessage = "The amount paid for is less than the amount of the order. Something's fishy...";
                MvcApplication.SaveOrder(order);
                return Redirect(Url.Action("error", "orders", new { id = orderId.ToString() }));
               
            }

			if (response.Approved) {
                order.AuthCode = response.AuthorizationCode;
                order.TransactionID = response.TransactionID;
                order.OrderMessage = string.Format("Thank you! Order approved: {0}", response.AuthorizationCode);
                MvcApplication.SaveOrder(order);
				//record the order, send to the receipt page
                return Redirect(Url.Action("details", "orders", new { id = orderId.ToString() }));
			
			} else {
				
				//error... oops. Reload the page
				order.OrderMessage = response.Message;
                MvcApplication.SaveOrder(order);
                return Redirect(Url.Action("error", "orders", new { id = orderId.ToString() }));
            }
			
			

		}

        public ActionResult Error(string id) {
            var orderId = new Guid(id);
            ViewData["order"] = MvcApplication.FindOrder(orderId);
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SimResponse(FormCollection post) {

            var response = new SIMResponse(post);

            //first order of business - validate that it was Auth.net that posted this using the 
            //MD5 Hash passed to use from Auth.net
            var isValid = response.Validate(ConfigurationManager.AppSettings["MerchantHash"],
                ConfigurationManager.AppSettings["ApiLogin"]);


            //if it's not valid - just send them to the home page. Don't throw - that's how
            //hackers figure out what's wrong :)
            if(!isValid)
                return Redirect("/");

            //pull the order ID from the order
            var orderId = new Guid(Request.Form["order_id"]);

            //pull the order
            var order = MvcApplication.FindOrder(orderId);

            //the URL to redirect to 
            var redirectAction = Url.Action("details", "orders", new { id = orderId.ToString() });
            var returnUrl = Url.SiteRoot()+redirectAction;


            if (response.Approved) {

                order.AuthCode = response.ToString();
                order.TransactionID = response.TransactionID;
                order.OrderMessage = string.Format("Thank you! Order approved: {0}", response.AuthorizationCode);

            } else {

                //pin the message to the order so we can show it to the user
                order.OrderMessage = response.Message;
                redirectAction = Url.Action("error", "orders", new { id = orderId.ToString() });
                returnUrl = Url.SiteRoot() + redirectAction;
            }

            //save the order somewhere
            MvcApplication.SaveOrder(order);

            //Direct Post method
            return Content(CheckoutFormBuilders.Redirecter(returnUrl));

            //or just return the page back to the AuthNet server if you don't want to bounce the return
            //MAKE SURE it has absolute URLs
            //return Redirect(redirectAction);

        }



	}
}

