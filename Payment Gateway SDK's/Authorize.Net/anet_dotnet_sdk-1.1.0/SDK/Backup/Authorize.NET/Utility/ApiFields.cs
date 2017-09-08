using System;
using System.Collections.Generic;

namespace AuthorizeNet
{
	
	/// <summary>
	/// These are field names and explanations only
	/// </summary>
	public class ApiFields
	{
		
		/// <summary>
		/// The merchant's unique API Login ID
		/// </summary>
		public static readonly string ApiLogin = "x_login";
		
		/// <summary>
		/// The merchant's unique Transaction Key
		/// </summary>	
		public static readonly string TransactionKey = "x_tran_key";
		
		/// <summary>
		/// True, False
		/// </summary>
		public static readonly string AllowPartialAuth = "x_allow_partial_Auth";
		
		/// <summary>
		/// Whether to return the data in delimited fashion
		/// </summary>
		public static readonly string DelimitData = "x_delim_data";
		
		/// <summary>
		/// If the return from AuthorizeNet is delimited - this is the character to use. Default is pipe
		/// </summary>
		public static readonly string DelimitCharacter = "x_delim_char";
		
		
		/// <summary>
		/// The relay response - leave this set as TRUE
		/// </summary>
		public static readonly string RelayResponse = "x_relay_response";

        public ApiFields() {
            ApiKeys = new List<string>();

            ApiKeys.Add("x_login");

            ApiKeys.Add("x_tran_key");

            ApiKeys.Add("x_allow_partial_Auth");

            ApiKeys.Add("x_delim_data");

            ApiKeys.Add("x_delim_char");

            ApiKeys.Add("x_relay_response");

            ApiKeys.Add("x_version");

            ApiKeys.Add("x_type");

            ApiKeys.Add("x_method");

            ApiKeys.Add("x_recurring_billing");

            ApiKeys.Add("x_amount");

            ApiKeys.Add("x_card_num");

            ApiKeys.Add("x_exp_date");

            ApiKeys.Add("x_card_code");

            ApiKeys.Add("x_trans_id");

            ApiKeys.Add("x_split_tender");

            ApiKeys.Add("x_auth_code");

            ApiKeys.Add("x_test_request");

            ApiKeys.Add("x_duplicate_window");

            ApiKeys.Add("x_invoice_num");

            ApiKeys.Add("x_description");

            ApiKeys.Add("x_first_name");

            ApiKeys.Add("x_last_name");

            ApiKeys.Add("x_company");

            ApiKeys.Add("x_address");

            ApiKeys.Add("x_city");

            ApiKeys.Add("x_state");

            ApiKeys.Add("x_zip");

            ApiKeys.Add("x_country");

            ApiKeys.Add("x_phone");

            ApiKeys.Add("x_fax");

            ApiKeys.Add("x_email");

            ApiKeys.Add("x_cust_id");

            ApiKeys.Add("x_cust_ip");

            ApiKeys.Add("x_ship_to_first_name");

            ApiKeys.Add("x_ship_to_last_name");

            ApiKeys.Add("x_ship_to_company");

            ApiKeys.Add("x_ship_to_address");

            ApiKeys.Add("x_ship_to_city");

            ApiKeys.Add("x_ship_to_state");

            ApiKeys.Add("x_ship_to_zip");

            ApiKeys.Add("x_ship_to_country");

            ApiKeys.Add("x_tax");
            ApiKeys.Add("x_frieght");
            ApiKeys.Add("x_duty");
            ApiKeys.Add("x_tax_exempt");
            ApiKeys.Add("x_po_num");


        }

        public List<string> ApiKeys {
            get;
            set;
        }

		
		
		/// <summary>
		/// Required - The merchant's transaction version
		/// </summary>
		public static readonly string ApiVersion = "x_version";
		
		/// <summary>
		/// The type of transaction:
		/// AUTH_CAPTURE (default), AUTH_ONLY, CAPTURE_ONLY, CREDIT, PRIOR_AUTH_CAPTURE, VOID
		/// </summary>
		public static readonly string TransactionType = "x_type";
		
		/// <summary>
		/// CC or ECHECK
		/// </summary>
		public static readonly string Method = "x_method";
		
		/// <summary>
		/// The recurring billing status
		/// </summary>
		public static readonly string RecurringBilling = "x_recurring_billing";
		
		/// <summary>
		/// The amount of the transaction
		/// </summary>
		public static readonly string Amount = "x_amount";
		/// <summary>
		/// The credit card number - between 13 and 16 digits without spaces. When x_type=CREDIT, only the last four digits are required
		/// </summary>
		public static readonly string CreditCardNumber = "x_card_num";
		/// <summary>
		/// The expiration date - MMYY, MM/YY, MM-YY, MMYYYY, MM/YYYY, MM-YYYY
		/// </summary>
		public static readonly string CreditCardExpiration = "x_exp_date";
		/// <summary>
		/// The three- or four-digit number on the back of a credit card (on the front for American Express).
		/// </summary>
		public static readonly string CreditCardCode = "x_card_code";
		/// <summary>
		/// The payment gateway assigned transaction ID of an original transaction - Required only for CREDIT, PRIOR_ AUTH_ CAPTURE, and VOID transactions
		/// </summary>
		public static readonly string TransactionID = "x_trans_id";
		/// <summary>
		/// The payment gateway-assitned ID assigned when the original transaction includes  two or more partial payments. This is the identifier that is used to group transactions that are part of a split tender order.
		/// </summary>
		public static readonly string SplitTender = "x_split_tender";
		/// <summary>
		/// The authorization code of an original transaction not authorized on the payment gateway
		/// </summary>
		public static readonly string AuthorizationCode = "x_auth_code";
		/// <summary>
		/// The request to process test transactions
		/// </summary>
		public static readonly string IsTestRequest = "x_test_request";
		/// <summary>
		/// The window of time after the submission of a transaction that a duplicate transaction can not be submitted
		/// </summary>
		public static readonly string DuplicateWindowTime = "x_duplicate_window";
		
		/// <summary>
		/// The merchant assigned invoice number for the transaction
		/// </summary>
		public static readonly string InvoiceNumber = "x_invoice_num";
		
		/// <summary>
		/// The transaction description
		/// </summary>
		public static readonly string Description = "x_description";
		

		public static readonly string FirstName = "x_first_name";
		
		public static readonly string LastName = "x_last_name";
		
		public static readonly string Company = "x_company";
		
		public static readonly string Address = "x_address";
		
		public static readonly string City = "x_city";
		
		public static readonly string State = "x_state";
		
		public static readonly string Zip = "x_zip";
		
		public static readonly string Country = "x_country";
		
		public static readonly string Phone = "x_phone";
		
		public static readonly string Fax = "x_fax";
		
		public static readonly string Email = "x_email";
		
		/// <summary>
		/// The ID of the Customer as relates to your application
		/// </summary>
		public static readonly string CustomerID = "x_cust_id";
		
		public static readonly string CustomerIPAddress = "x_cust_ip";
		
		
		public static readonly string ShipFirstName = "x_ship_to_first_name";

		public static readonly string ShipLastName = "x_ship_to_last_name";

		public static readonly string ShipCompany = "x_ship_to_company";

		public static readonly string ShipAddress = "x_ship_to_address";

		public static readonly string ShipCity = "x_ship_to_city";

		public static readonly string ShipState = "x_ship_to_state";

		public static readonly string ShipZip = "x_ship_to_zip";

		public static readonly string ShipCountry = "x_ship_to_country";
		
		
		public static readonly string Tax = "x_tax";
		public static readonly string Freight = "x_frieght";
		public static readonly string Duty = "x_duty";
		public static readonly string TaxExempt = "x_tax_exempt";
		public static readonly string PONumber = "x_po_num";
		
		
		
		public bool ApiContainsKey (string key)
		{
				
			return ApiKeys.Contains (key);
		}
		
		
		
	}
}

