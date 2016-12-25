﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using OpenShare.Net.Library.Common;

namespace OpenShare.Net.Library.Services
{
    public class HttpService : IHttpService
    {
        public CookieContainer Container { get; set; }

        private readonly SecureString _basicAuthenticaiton;

        public HttpService()
        {
            Container = new CookieContainer();

            _basicAuthenticaiton = new SecureString();
            _basicAuthenticaiton.MakeReadOnly();
        }

        /// <summary>
        /// Constructor for Basic Authentication
        /// </summary>
        /// <param name="username">Username as SecureString</param>
        /// <param name="password">Password as SecureString</param>
        public HttpService(
            SecureString username,
            SecureString password)
        {
            Container = new CookieContainer();

            _basicAuthenticaiton = $"{username.ToUnsecureString()}:{password.ToUnsecureString()}"
                .ToBase64String()
                .ToSecureString();

            if (!username.IsReadOnly())
                username.Clear();
            if (!password.IsReadOnly())
                password.Clear();
        }

        public virtual async Task<HttpResponseMessage> LoginAsync(
            string url,
            Dictionary<string, string> content = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                if (content != null)
                    httpRequestMessage.Content = new FormUrlEncodedContent(content);
                    //Content = new FormUrlEncodedContent(
                    //    new Dictionary<string, string>
                    //    {
                    //        {"Username", Username},
                    //        {"Password", Password}
                    //    })
                
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    return response.EnsureSuccessStatusCode();
                }
            }
        }

        public virtual async Task<HttpResponseMessage> LoginJsonAsync(
            string url, Dictionary<string, string> content = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                if (content != null)
                    httpRequestMessage.Content = new StringContent(
                        new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 }.Serialize(content),
                        Encoding.UTF8,
                        "application/json");

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    return response.EnsureSuccessStatusCode();
                }
            }
        }

        public virtual async Task<HttpResponseMessage> LoginJsonAsync<T>(
            string url, T content,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                if (content != null)
                    httpRequestMessage.Content = new StringContent(
                        new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 }.Serialize(content),
                        Encoding.UTF8,
                        "application/json");

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    return response.EnsureSuccessStatusCode();
                }
            }
        }

        public virtual async Task<string> RequestAsync(
            HttpMethod httpMethod, string url, Dictionary<string, string> content = null, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new FormUrlEncodedContent(content);

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                        return await response.Content.ReadAsStringAsync();

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public virtual async Task<string> RequestJsonAsync(
            HttpMethod httpMethod, string url, Dictionary<string, string> content = null, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new StringContent(
                        new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 }.Serialize(content),
                        Encoding.UTF8,
                        "application/json");

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                        return await response.Content.ReadAsStringAsync();

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public virtual async Task<string> RequestJsonAsync<T>(
            HttpMethod httpMethod, string url, T content, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new StringContent(
                        new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 }.Serialize(content),
                        Encoding.UTF8,
                        "application/json");

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                        return await response.Content.ReadAsStringAsync();

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public virtual async Task<byte[]> RequestBytesAsync(
            HttpMethod httpMethod, string url, Dictionary<string, string> content = null, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new FormUrlEncodedContent(content);

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                        return await response.Content.ReadAsByteArrayAsync();

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }

        public virtual async Task<byte[]> RequestBytesJsonAsync(
            HttpMethod httpMethod, string url, Dictionary<string, string> content = null, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new StringContent(
                        new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 }.Serialize(content),
                        Encoding.UTF8,
                        "application/json");

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                        return await response.Content.ReadAsByteArrayAsync();

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }

        public virtual async Task<byte[]> RequestBytesJsonAsync<T>(
            HttpMethod httpMethod, string url, T content, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new StringContent(
                        new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 }.Serialize(content),
                        Encoding.UTF8,
                        "application/json");

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                        return await response.Content.ReadAsByteArrayAsync();

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }

        public virtual async Task<Stream> RequestStreamAsync(
            HttpMethod httpMethod, string url, Dictionary<string, string> content = null, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new FormUrlEncodedContent(content);

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                        return await response.Content.ReadAsStreamAsync();

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    return await response.Content.ReadAsStreamAsync();
                }
            }
        }

        public virtual async Task<Stream> RequestStreamJsonAsync(
            HttpMethod httpMethod, string url, Dictionary<string, string> content = null, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new StringContent(
                        new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 }.Serialize(content),
                        Encoding.UTF8,
                        "application/json");

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                        return await response.Content.ReadAsStreamAsync();

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    return await response.Content.ReadAsStreamAsync();
                }
            }
        }

        public virtual async Task<Stream> RequestStreamJsonAsync<T>(
            HttpMethod httpMethod, string url, T content, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new StringContent(
                        new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 }.Serialize(content),
                        Encoding.UTF8,
                        "application/json");

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                        return await response.Content.ReadAsStreamAsync();

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    return await response.Content.ReadAsStreamAsync();
                }
            }
        }

        public virtual async Task RequestStreamAsync(
            HttpMethod httpMethod, string url, Stream stream, Dictionary<string, string> content = null, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new FormUrlEncodedContent(content);

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                    {
                        await response.Content.CopyToAsync(stream);
                        return;
                    }

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    await response.Content.CopyToAsync(stream);
                }
            }
        }

        public virtual async Task RequestStreamJsonAsync(
            HttpMethod httpMethod, string url, Stream stream, Dictionary<string, string> content = null, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new StringContent(
                        new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 }.Serialize(content),
                        Encoding.UTF8,
                        "application/json");

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                    {
                        await response.Content.CopyToAsync(stream);
                        return;
                    }

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    await response.Content.CopyToAsync(stream);
                }
            }
        }

        public virtual async Task RequestStreamJsonAsync<T>(
            HttpMethod httpMethod, string url, Stream stream, T content, bool skipStatusCheck = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception($"Invalid url value: {url}");

            using (var httpClientHandler = new HttpClientHandler { UseProxy = false, CookieContainer = Container })
            {
                var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
                if (httpMethod != HttpMethod.Get && content != null)
                    httpRequestMessage.Content = new StringContent(
                        new JavaScriptSerializer { MaxJsonLength = int.MaxValue, RecursionLimit = 100 }.Serialize(content),
                        Encoding.UTF8,
                        "application/json");

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    if (_basicAuthenticaiton.Length > 0)
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthenticaiton.ToUnsecureString());

                    var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    if (skipStatusCheck)
                    {
                        await response.Content.CopyToAsync(stream);
                        return;
                    }

                    response.EnsureSuccessStatusCode();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error fetching data from Url: {url}");

                    await response.Content.CopyToAsync(stream);
                }
            }
        }
    }
}
