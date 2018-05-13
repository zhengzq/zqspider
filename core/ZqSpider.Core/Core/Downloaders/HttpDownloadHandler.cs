using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ZqSpider.Core.Spiders;

namespace ZqSpider.Core.Downloaders
{
    public class HttpDownloadHandler : DownloadHandler
    {
        public override async Task<Response> Download(Request request, Response response, ISpider spider)
        {
            try
            {
                using (var client = CreateHttpClient(request))
                {
                    var message = default(HttpResponseMessage);
                    if (request.Method == "GET")
                    {
                        message = await client.GetAsync(request.Url);
                    }
                    else
                    {
                        var kvp = new List<KeyValuePair<string, string>>();
                        if (request.Body != null)
                            foreach (var item in request.Body)
                                kvp.Add(new KeyValuePair<string, string>(item.Key, item.Value));
                        var content = new FormUrlEncodedContent(kvp);
                        message = await client.PostAsync(request.Url, content);
                    }
                    message.EnsureSuccessStatusCode();
                    response.Html = await message.Content.ReadAsStringAsync();
                    response.Body = await message.Content.ReadAsByteArrayAsync();
                    response.RootUrl = string.Format("{0}://{1}", message.RequestMessage.RequestUri.Scheme, message.RequestMessage.RequestUri.Host);
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new DownLoaderException(string.Format("{0}\n URL {1}\n {2}", ex.Message, request.Url, ex.ToString()));
            }
        }

        private HttpClient CreateHttpClient(Request request)
        {
            var handler = new Handler() { AllowAutoRedirect = request.AllowAutoRedirect };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("UserAgent", request.UserAgent);
            return client;
        }
    }

    internal class Handler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (this.AllowAutoRedirect)
                response = await AutoRedirect(request, cancellationToken, response);
            return response;
        }
        private HttpRequestMessage CopyRequest(HttpRequestMessage oldRequest)
        {
            var newrequest = new HttpRequestMessage(oldRequest.Method, oldRequest.RequestUri);

            foreach (var header in oldRequest.Headers)
            {
                newrequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            foreach (var property in oldRequest.Properties)
            {
                newrequest.Properties.Add(property);
            }
            if (oldRequest.Content != null) newrequest.Content = new StreamContent(oldRequest.Content.ReadAsStreamAsync().Result);
            return newrequest;
        }
        private async Task<HttpResponseMessage> AutoRedirect(HttpRequestMessage request, CancellationToken cancellationToken, HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.MovedPermanently
                   || response.StatusCode == HttpStatusCode.Moved
                   || response.StatusCode == HttpStatusCode.Redirect
                   || response.StatusCode == HttpStatusCode.Found
                   || response.StatusCode == HttpStatusCode.SeeOther
                   || response.StatusCode == HttpStatusCode.RedirectKeepVerb
                   || response.StatusCode == HttpStatusCode.TemporaryRedirect
                   || (int)response.StatusCode == 308)
            {
                var newRequest = CopyRequest(response.RequestMessage);
                if (response.StatusCode == HttpStatusCode.Redirect
                    || response.StatusCode == HttpStatusCode.Found
                    || response.StatusCode == HttpStatusCode.SeeOther)
                {
                    newRequest.Content = request.Content;
                    newRequest.Method = request.Method;
                }
                newRequest.RequestUri = response.Headers.Location;
                var r = await base.SendAsync(newRequest, cancellationToken);
                response = await AutoRedirect(request, cancellationToken, r);
            }
            return response;
        }
    }
}
