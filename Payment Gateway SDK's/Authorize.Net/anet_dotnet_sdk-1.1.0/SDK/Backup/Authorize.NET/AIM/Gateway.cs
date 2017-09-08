using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Xml;

namespace AuthorizeNet {

    public enum ServiceMode {
        Test,
        Live
    }

    public class Gateway : AuthorizeNet.IGateway {

		
		public const string TEST_URL = "https://test.authorize.net/gateway/transact.dll";
        public const string LIVE_URL = "https://secure.authorize.net/gateway/transact.dll";

		
		public string ApiLogin { get; set;}
		public string TransactionKey { get; set;}
		public bool TestMode { get; set;}
		public Gateway (string apiLogin, string transactionKey, bool testMode)
		{
			ApiLogin = apiLogin;
			TransactionKey = transactionKey;
			TestMode = testMode;

            
			
		}
		
		public Gateway(string apiLogin, string transactionKey):this(apiLogin,transactionKey,true){}

        public IGatewayResponse Send(IGatewayRequest request) {
            return Send(request, "");
        }

		public IGatewayResponse Send (IGatewayRequest request, string description)
		{

            var serviceUrl = TEST_URL;
            if (!TestMode)
                serviceUrl = LIVE_URL;

            request.Queue(ApiFields.ApiLogin, ApiLogin);
            request.Queue(ApiFields.TransactionKey, TransactionKey);
            request.Queue(ApiFields.Description, description);
            //validate the inputs
            Validate(request);
            var result = "";
            var postData = request.ToPostString();

            //override the local cert policy
            ServicePointManager.CertificatePolicy = new PolicyOverride();

            var webRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            webRequest.Method = "POST";
            webRequest.ContentLength = postData.Length;
            webRequest.ContentType = "application/x-www-form-urlencoded";

            // post data is sent as a stream
            StreamWriter myWriter = null;
            myWriter = new StreamWriter(webRequest.GetRequestStream());
            myWriter.Write(postData);
            myWriter.Close();

            // returned values are returned as a stream, then read into a string
            var response = (HttpWebResponse)webRequest.GetResponse();
            using (StreamReader responseStream = new StreamReader(response.GetResponseStream())) {
                result = responseStream.ReadToEnd();
                responseStream.Close();
            }

            // the response string is broken into an array
            // The split character specified here must match the delimiting character specified above
            var response_array = result.Split('|');
            return DecideResponse(response_array);
		}


        /// <summary>
        /// Validates the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        public void Validate(IGatewayRequest req) {
            //make sure we have all the fields we need
            
            //starting with the login/key pair
            AssertValidation(req, ApiFields.ApiLogin, ApiFields.TransactionKey);
            
            //each call has its own requirements... check each
            switch (req.ApiAction) {
                case RequestAction.AuthorizeAndCapture:
                case RequestAction.Authorize:
                    AssertValidation(req, ApiFields.CreditCardNumber, ApiFields.CreditCardExpiration, ApiFields.Amount);
                    break;
                case RequestAction.Capture:
                    AssertValidation(req, ApiFields.TransactionID,ApiFields.AuthorizationCode);
                    break;
                case RequestAction.Credit:
                    AssertValidation(req, ApiFields.TransactionID, ApiFields.Amount, ApiFields.CreditCardNumber);
                 break;
                case RequestAction.Void:
                    AssertValidation(req, ApiFields.TransactionID);
                    break;
            }

        }


        /// <summary>
        /// Asserts the validation.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="keys">The keys.</param>
        public void AssertValidation(IGatewayRequest req, params string[] keys) {
            var sb = new StringBuilder();
            foreach (var item in keys) {
                if (!req.Post.Keys.Contains(item)) {
                    //add the item to the output... 
                    sb.AppendFormat("{0}, ", item);
                } else {
                    //make sure it's not null
                    if (string.IsNullOrEmpty(req.Post[item])) {
                       sb.AppendFormat("No value for '{0}', which is required. ", item);
                    }
                }

            }
            var result = sb.ToString();
            if (result.Length > 0) {
                result.Trim().TrimEnd(',');
                throw new InvalidDataException("Can't submit to Gateway - missing these input fields: " + result);
            }


        }

        /// <summary>
        /// Decides the response.
        /// </summary>
        /// <param name="rawResponse">The raw response.</param>
        /// <returns></returns>
		public IGatewayResponse DecideResponse (string[] rawResponse)
		{
			
			if (rawResponse.Length == 1)
				throw new InvalidDataException ("There was an error returned from AuthorizeNet: " + rawResponse[0] + "; this usually means your data sent along was incorrect. Please recheck that all dates and amounts are formatted correctly");
			
			return new GatewayResponse (rawResponse);
		}
		
		
		class PolicyOverride : ICertificatePolicy
		{

			bool ICertificatePolicy.CheckValidationResult (ServicePoint srvPoint, System.Security.Cryptography.X509Certificates.X509Certificate cert, WebRequest request, int certificateProblem)
			{
				return true;
			}
		}



	}
}
