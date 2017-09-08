using System;
namespace AuthorizeNet {
    public interface IGatewayRequest {
        RequestAction ApiAction { get; }
        void Queue(string key, string value);
        System.Collections.Generic.Dictionary<string, string> Post { get; set; }
        //Fluent API stuff
        IGatewayRequest AddCustomer(string ID, string first, string last, string address, string state, string zip);
        IGatewayRequest AddShipping(string ID, string first, string last, string address, string state, string zip);
        IGatewayRequest AddMerchantValue(string key, string value);
        IGatewayRequest AddInvoice(string invoiceNumber);

        string ToPostString();
    }
}
