using ServerRA_AspnetCore.Model.Basket;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;

namespace ServerRA_AspnetCore.Enternal.TempHTTPReq
{
    public class HttpServerComunication : ServerCommunication
    {

        private string ConvertToJsonString(BasketEntryModel[] items)
        {
            List<List<object>> lst = new List<List<object>>(items.Length);

            foreach(var item in items)
            {
                lst.Add(new List<object> { item.productId, item.count });
            }

            Dictionary<string, object> dict = new Dictionary<string, object> ();
            dict.Add("component_list", new List<object>() { lst });
            var json = JsonSerializer.Serialize(dict);
            return json;
        }

        public async Task<bool> areAllAvailable(BasketEntryModel[] items)
        {

            string json_msg = ConvertToJsonString(items);

            var httpRequestMsg = new HttpRequestMessage(HttpMethod.Get, "https://server-parts-ada.herokuapp.com/arecomponentsavailable");
            
            httpRequestMsg.Content = new StringContent(json_msg, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();

            var response = httpClient.Send(httpRequestMsg);
            if (response != null && response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                return bool.Parse(str);
            }

            return false;
        }

        private BasketExtendedEntryModel[]? convert(Stream s)
        {
            StreamReader rs = new StreamReader(s);
            var serializer = new DataContractJsonSerializer(typeof(BasketExtendedEntryModel[]));

            var result = serializer.ReadObject(rs.BaseStream) as BasketExtendedEntryModel[];
            return result;
        }

        public Task<BasketExtendedEntryModel[]> getAvailability(BasketEntryModel[] items)
        {
            string json_msg = ConvertToJsonString(items);

            var httpRequestMsg = new HttpRequestMessage(HttpMethod.Get, "https://server-parts-ada.herokuapp.com/getcomponentsinfo");
            
            httpRequestMsg.Content = new StringContent(json_msg, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();

            var response = httpClient.Send(httpRequestMsg);
            if (response != null && response.IsSuccessStatusCode)
            {
                var cvt = convert(response.Content.ReadAsStream());
                if(cvt != null)
                    return Task.FromResult(cvt);
            }

            return Task.FromResult(Array.Empty<BasketExtendedEntryModel>());
        }

        public async Task<bool> returnProducts(BasketEntryModel[] items)
        {
            string json_msg = ConvertToJsonString(items);

            var httpRequestMsg = new HttpRequestMessage(HttpMethod.Get, "https://server-parts-ada.herokuapp.com/return_components");

            httpRequestMsg.Content = new StringContent(json_msg, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();

            var response = httpClient.Send(httpRequestMsg);
            if (response != null && response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                return bool.Parse(str);
            }

            return false;
        }
    }
}
