using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AuthorizeNet;

namespace SampleAuthorization {
    class Program {
        static void Main(string[] args) {

            //step 1 - create the request
            var request = new AuthorizationRequest("4111111111111111", "1216", 10.00M, "Test Transaction");

            //step 2 - create the gateway, sending in your credentials and setting the Mode to Test (boolean flag)
            //which is true by default
            //this login and key are the shared dev account - you should get your own if you 
            //want to do more testing
            var gate = new Gateway("75sqQ96qHEP8", "7r83Sb4HUd58Tz5p",true);
            
            //step 3 - make some money
            var response = gate.Send(request);

            Console.WriteLine("{0}: {1}",response.ResponseCode, response.Message);
            Console.Read();
        }
    }
}
