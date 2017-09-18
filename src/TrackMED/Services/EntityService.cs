using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrackMED.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Http;

namespace TrackMED.Services
{
    public class EntityService<T>: IEntityService<T> where T: IEntity
    {
        private readonly Settings _settings;
        private string uri = null;

        // https://docs.asp.net/en/latest/mvc/controllers/dependency-injection.html#accessing-settings-from-a-controller
        public EntityService(IOptions<Settings> optionsAccessor)
        {
            _settings = optionsAccessor.Value; // reads appsettings.json
        }

        //public async Task<List<T>> GetEntitiesAsync(CancellationToken cancelToken = default(CancellationToken))
        public async Task<List<T>> GetEntitiesAsync(string id = null, CancellationToken cancelToken = default(CancellationToken))
        {
            //uri = getServiceUri(typeof(T).Name);
            uri = id == null?getServiceUri(typeof(T).Name):getServiceUri(typeof(T).Name + "/" + id);

            // HttpClient Class: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient%28v=vs.118%29.aspx?f=255&MSPPError=-2147217396
            using (HttpClient httpClient = new HttpClient())
            {
                // From http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(uri, cancelToken);
                //var response = await httpClient.GetAsync(uri, cancelToken);

                // needs "Microsoft.AspNet.WebApi.Client": "5.2.3" in order to work
                return (await response.Content.ReadAsAsync<List<T>>());
            }
        }

        /*
        public async Task<List<T>> GetEntitiesByIdAsync(string id = null, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = id == null?getServiceUri(typeof(T).Name):getServiceUri(typeof(T).Name + "/" + id);

            // HttpClient Class: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient%28v=vs.118%29.aspx?f=255&MSPPError=-2147217396
            using (HttpClient httpClient = new HttpClient())
            {
                // From http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(uri, cancelToken);
                //var response = await httpClient.GetAsync(uri, cancelToken);

                // needs "Microsoft.AspNet.WebApi.Client": "5.2.3" in order to work
                return (await response.Content.ReadAsAsync<List<T>>());
            }
        }
        */

        public async Task<List<T>> GetEntitiesManyAsync(List<string> ids, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/multiples" + "/" + ids);
            // HttpClient Class: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient%28v=vs.118%29.aspx?f=255&MSPPError=-2147217396
            using (HttpClient httpClient = new HttpClient())
            {
                // From http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(uri, cancelToken);
                //var response = await httpClient.GetAsync(uri, cancelToken);

                return (await response.Content.ReadAsAsync<List<T>>());
            }
        }

        public async Task<List<T>> GetSelectedEntitiesAsync(string tableID, string id, CancellationToken cancelToken = default(CancellationToken))
        {
            // http://www.c-sharpcorner.com/UploadFile/2b481f/pass-multiple-parameter-in-url-in-web-api/
            uri = getServiceUri(typeof(T).Name + "/" + tableID + "/" + id);
            // HttpClient Class: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient%28v=vs.118%29.aspx?f=255&MSPPError=-2147217396
            using (HttpClient httpClient = new HttpClient())
            {
                // From http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(uri, cancelToken);
                //var response = await httpClient.GetAsync(uri, cancelToken);

                return (await response.Content.ReadAsAsync<List<T>>());
            }
        }

        public async Task<T> GetEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + id);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);
                return (await response.Content.ReadAsAsync<T>());
            }
        }

        public async Task<T> GetEntityAsyncByDescription(string Description,
            CancellationToken cancelToken = default(CancellationToken))
        {
            // String.IndexOfAny Method https://msdn.microsoft.com/en-us/library/11w09h50(v=vs.110).aspx
            char[] chars = { '&', '/', '%', '$' };
            //int indexSlash = Description.IndexOf("/");
            int indexSpecial = Description.IndexOfAny(chars);
            if (indexSpecial >= 0)
            {
                // http://stackoverflow.com/questions/24978885/asp-c-special-characters-cant-pass-trough-url-parameter
                Description = Uri.EscapeDataString(Description);
            }

            uri = getServiceUri(typeof(T).Name + "/Desc/" + Description);
            using (HttpClient httpClient = new HttpClient()) 
            {
                var response = await httpClient.GetAsync(uri, cancelToken);
                return (await response.Content.ReadAsAsync<T>());
            }
        }
        
        public async Task<T> GetEntityAsyncByFieldID(string fieldID, string id, string tableID, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + fieldID + "/" + id + "/" + tableID);
            using (HttpClient httpClient = new HttpClient()) 
            {
                var response = await httpClient.GetAsync(uri, cancelToken);
                return (await response.Content.ReadAsAsync<T>());
            }
        }         
        
        public async Task<bool> DeleteEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + id);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(uri, cancelToken);
                return (response.IsSuccessStatusCode);
            }
        }

        public async Task<HttpResponseMessage> EditEntityAsync(T Entity, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PutAsJsonAsync(uri, Entity, cancelToken);
                return response.EnsureSuccessStatusCode();
            }
        }

        /*
        public async Task<HttpResponseMessage> EditEntitiesAsync(List<string> ids, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/multiples/" + ids);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PutAsJsonAsync(uri, Entity, cancelToken);
                return response.EnsureSuccessStatusCode();
            }
        }
        */

        //public async Task<HttpResponseMessage> PostEntityAsync(Entity Entity,
        public async Task<T> PostEntityAsync( T Entity, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsJsonAsync(uri, Entity);
                //return response.EnsureSuccessStatusCode();
                return (await response.Content.ReadAsAsync<T>());
            }
        }

        public async Task<T> PostEntitiesAsync(List<T> Entities, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/multiples/" + Entities);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsJsonAsync(uri, Entities);
                return (await response.Content.ReadAsAsync<T>());
            }
        }

        public async Task<T> VerifyEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + id);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);
                return (await response.Content.ReadAsAsync<T>());
                //var response = await httpClient.GetAsync(uri, cancelToken);
                //return (response.IsSuccessStatusCode);
            }
        }

        public async Task<bool> DropDatabaseAsync(CancellationToken cancelToken = default(CancellationToken))
        {
            uri = _settings.TrackMEDApi;
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PutAsJsonAsync(uri, cancelToken);
                return (response.IsSuccessStatusCode);
            }
        }

        public string getServiceUri(string srv)
        {
            return _settings.TrackMEDApi + "api/" + srv;
        }
    }
}