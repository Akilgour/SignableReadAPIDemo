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
        static string getEnvelopesHeaders = "https://api.signable.co.uk/v1/envelopes?offset=1&limit=1";
        static string getListEnvelopes = "https://api.signable.co.uk/v1/envelopes?offset={0}&limit={1}";

        static void Main(string[] args)
        {
            Console.WriteLine("Enter your API Key from  https://app.signable.co.uk/api");
            string apiKey = "99b18594ed0144188a0efe109043702a";//Console.ReadLine();

            var webClient = CreateWebClient(apiKey);

            var envelopesHeaderJSON = GetDetail(webClient, getEnvelopesHeaders);

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Rootobject envelopesHeader = javaScriptSerializer.Deserialize<Rootobject>(envelopesHeaderJSON);

            int recordsPerPage = 5;
            int pageCount = (int.Parse(envelopesHeader.total_envelopes) + recordsPerPage - 1) / recordsPerPage;

            var envelopeFingerprints = new List<Guid>();
            string rootJSON;
             
            for (int i = 0; i < pageCount; i++)
            {
                rootJSON = GetDetail(webClient, string.Format(getListEnvelopes, i * recordsPerPage, recordsPerPage));
                Rootobject root = javaScriptSerializer.Deserialize<Rootobject>(rootJSON);
                foreach (var envelope in root.envelopes)
                {
                    envelopeFingerprints.Add(new Guid(envelope.envelope_fingerprint));
                }
            }

             
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
            //Console.WriteLine(results);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadLine();
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
    }


}
