using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SignableReadAPIDemo
{
    class Program
    {
        static string getListEnvelopes = "https://api.signable.co.uk/v1/envelopes?offset={0}&limit={1}";
        static int recordsPerPage = 5;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter your API Key from  https://app.signable.co.uk/api");
            string apiKey = Console.ReadLine();
            var webClient = CreateWebClient(apiKey);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string rootJSON;

            Rootobject root = new Rootobject() { next = string.Format(getListEnvelopes, 0, recordsPerPage) };
            do
            {
                rootJSON = GetDetail(webClient, root.next);
                root = javaScriptSerializer.Deserialize<Rootobject>(rootJSON);
                foreach (var envelope in root.envelopes.Where(x => x.envelope_status == "signed"))
                {
                    using (WebClient client = new WebClient())
                    {

                        client.DownloadFile(envelope.envelope_signed_pdf, string.Format(@"D:\Ashley\Desktop\{0}.pdf", envelope.envelope_fingerprint));
                    }
                }
            } while (root.next != null);

        }

        private static WebClient CreateWebClient(string apiKey)
        {
            WebClient webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKey + ":x"));
            var authorization = string.Format("Basic {0}", credentials);
            webClient.Headers[HttpRequestHeader.Authorization] = authorization;
            return webClient;
        }

        private static string GetDetail(WebClient webClient, string apiCommand)
        {
            var results = webClient.DownloadString(apiCommand);
            return results;
        }
    }


    public class Rootobject
    {
        public int http { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public string total_envelopes { get; set; }
        public string prev { get; set; }
        public string next { get; set; }
        public Envelope[] envelopes { get; set; }
    }

    public class Envelope
    {
        public string envelope_fingerprint { get; set; }
        public string envelope_title { get; set; }
        public string envelope_status { get; set; }
        public string envelope_redirect_url { get; set; }
        public DateTime envelope_created { get; set; }
        public DateTime envelope_sent { get; set; }
        public object envelope_processed { get; set; }
        public string envelope_signed_pdf { get; set; }
    }


}
