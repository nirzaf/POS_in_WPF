using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Net;

namespace AuthorizeNet {

    public enum RequestAction {
        Authorize,
        Capture,
        AuthorizeAndCapture,
        Credit,
        Void
    }


    /// <summary>
    /// An abstract base class, from which all Request classes must inherit
    /// </summary>
    public abstract class GatewayRequest : IGatewayRequest {
        public Dictionary<string,string> Post { get; set; }
        RequestAction _apiAction;
        public RequestAction ApiAction {
            get {
                return _apiAction;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayRequest"/> class.
        /// </summary>
        public GatewayRequest ()
		{
			Post = new Dictionary<string, string> ();
            LoadCoreValues();
            _apiAction = RequestAction.AuthorizeAndCapture;
        }

        internal void SetApiAction(RequestAction action) {
            _apiAction = action;
            var apiValue = "AUTH_CAPTURE";

            switch (action) {
                case RequestAction.Authorize:
                    apiValue = "AUTH_ONLY";
                    break;
                case RequestAction.Capture:
                    apiValue = "PRIOR_AUTH_CAPTURE";
                    break;
                case RequestAction.Credit:
                    apiValue = "CREDIT";
                    break;
                case RequestAction.Void:
                    apiValue = "VOID";
                    break;
            }
            Queue(ApiFields.TransactionType, apiValue);
        }



        /// <summary>
        /// Outputs the queue as a delimited, URL-safe string for sending to Authorize.net as a form POST
        /// </summary>
        public string ToPostString() {
            var sb = new StringBuilder();
            foreach (var key in Post.Keys) {
                sb.AppendFormat("{0}={1}&", key, HttpUtility.UrlEncode(Post[key]));
                Console.WriteLine("{0} = {1}", key, Post[key]);
            }
            var result = sb.ToString();
            //have to remove the last ampersand
            return result.TrimEnd('&');

        }

        /// <summary>
        /// Queues the specified key into the request.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
		public void Queue (string key, string value)
		{

            //allow for pushing of new bits...
			if (Post.ContainsKey (key))
				Post.Remove (key);

            Post.Add (key, value);
		}

        /// <summary>
        /// Loads the core values tp the API request, including auth and basic settings.
        /// </summary>
        void LoadCoreValues() {
            this.Queue(ApiFields.DelimitData, "TRUE");
            this.Queue(ApiFields.DelimitCharacter, "|");
            this.Queue(ApiFields.RelayResponse, "FALSE");
            this.Queue(ApiFields.Method, "CC");
        }
        /// <summary>
        /// Adds a Customer record to the current request
        /// </summary>
        public IGatewayRequest AddCustomer(string ID, string first, string last, string address, string state, string zip) {
            Queue(ApiFields.FirstName, first);
            Queue(ApiFields.LastName, last);
            Queue(ApiFields.Address, address);
            Queue(ApiFields.State, state);
            Queue(ApiFields.Zip, zip);
            Queue(ApiFields.CustomerID, ID);

            return this;
        }

        /// <summary>
        /// Adds a Shipping Record to the current request
        /// </summary>
        public IGatewayRequest AddShipping(string ID, string first, string last, string address, string state, string zip) {
            Queue(ApiFields.ShipFirstName, first);
            Queue(ApiFields.ShipLastName, last);
            Queue(ApiFields.ShipAddress, address);
            Queue(ApiFields.ShipState, state);
            Queue(ApiFields.ShipZip, zip);
            return this;

        }

        /// <summary>
        /// This is where you can add custom values to the request, which will be returned to you
        /// in the response
        /// </summary>
        public IGatewayRequest AddMerchantValue(string key, string value) {
            Queue(key, value);
            return this;
        }

        /// <summary>
        /// Adds an InvoiceNumber to the request
        /// </summary>
        public IGatewayRequest AddInvoice(string invoiceNumber) {
            Queue(ApiFields.InvoiceNumber, invoiceNumber);
            return this;
        }
    }
}
