// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Donut.Extensions
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Donut.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    internal static class ProxyExtensions
    {
        private const int StreamCopyBufferSize = 81920;

        /// <summary>
        /// Forwards current request to the specified destination uri.
        /// </summary>
        /// <param name="context">The Http Context in action method</param>
        /// <param name="destinationUri">Destination Uri</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        public static async Task ProxyRequest(this HttpContext context, Uri destinationUri)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (destinationUri == null)
            {
                throw new ArgumentNullException(nameof(destinationUri));
            }

            var proxyService = context.RequestServices.GetRequiredService<ProxyService>();

            using (var requestMessage = context.CreateProxyHttpRequest(destinationUri))
            {
                using (var responseMessage = await context.SendProxyHttpRequest(requestMessage))
                {
                    await context.CopyProxyHttpResponse(responseMessage);
                }
            }
        }

        /// <summary>
        /// Send proxy http request
        /// </summary>
        /// <param name="context">The Http Context in action method.</param>
        /// <param name="requestMessage">The request message to send.</param>
        /// <returns>A <see cref="HttpResponseMessage"/> received from the destination server</returns>
        public static Task<HttpResponseMessage> SendProxyHttpRequest(this HttpContext context, HttpRequestMessage requestMessage)
        {
            if (requestMessage == null)
            {
                throw new ArgumentNullException(nameof(requestMessage));
            }

            var proxyService = context.RequestServices.GetRequiredService<ProxyService>();

            return proxyService.Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
        }

        public static HttpRequestMessage CreateProxyHttpRequest(this HttpContext context, Uri uri)
        {
            var request = context.Request;

            var requestMessage = new HttpRequestMessage();
            var requestMethod = request.Method;
            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(request.Body);
                requestMessage.Content = streamContent;
            }

            // Copy the request headers
            foreach (var header in request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            requestMessage.Headers.Host = uri.Authority;
            requestMessage.RequestUri = uri;
            requestMessage.Method = new HttpMethod(request.Method);

            return requestMessage;
        }

        public static async Task CopyProxyHttpResponse(this HttpContext context, HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            var response = context.Response;

            response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
            response.Headers.Remove("transfer-encoding");

            using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
            {
                await responseStream.CopyToAsync(response.Body, StreamCopyBufferSize, context.RequestAborted);
            }
        }
    }
}
