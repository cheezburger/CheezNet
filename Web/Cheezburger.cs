using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using RestSharp;

namespace Cheezburger.StarterKit
{
    public static class Cheezburger
    {
        private class AccessTokenResponse
        {
            public string AccessToken { get; set; }
            public string ExpiresIn { get; set; }
            public string RefreshToken { get; set; }
        }

        private class CheezburgerAuthenticator : IAuthenticator
        {
            private readonly AccessToken accessToken;

            public CheezburgerAuthenticator(AccessToken accessToken)
            {
                this.accessToken = accessToken;
            }

            public void Authenticate(IRestClient client, IRestRequest request)
            {
                request.AddParameter("access_token", accessToken.Value);
            }
        }

        private static readonly string Hostname = "api.cheezburger.com";
        private static readonly string BaseUrl = String.Format("https://{0}/v1/", Hostname);
        private static readonly string AuthorizeUrl = String.Format("https://{0}/oauth/authorize", Hostname);
        private static readonly string AccessTokenUrl = String.Format("https://{0}/oauth/access_token", Hostname);

        private static readonly string ClientId = ConfigurationManager.AppSettings["Cheezburger.ClientId"];
        private static readonly string ClientSecret = ConfigurationManager.AppSettings["Cheezburger.ClientSecret"];
        private static readonly string RedirectUri = ConfigurationManager.AppSettings["Cheezburger.RedirectUri"];

        private static RestClient CreateClient(AccessToken accessToken)
        {
            return new RestClient(BaseUrl) { Authenticator = new CheezburgerAuthenticator(accessToken) };
        }

        public static RedirectResult RedirectToAuthorize()
        {
            var request = new RestRequest(AuthorizeUrl);
            request.AddParameter("response_type", "code");
            request.AddParameter("client_id", ClientId);
            request.AddParameter("redirect_uri", RedirectUri);

            return new RedirectResult(new RestClient().BuildUri(request).ToString());
        }

        public static AccessToken GetAccessToken(string code)
        {
            var request = new RestRequest(AccessTokenUrl, Method.POST);
            request.AddParameter("client_id", ClientId);
            request.AddParameter("client_secret", ClientSecret);
            request.AddParameter("code", code);
            request.AddParameter("grant_type", "authorization_code");

            var response = new RestClient().Execute<AccessTokenResponse>(request);

            return new AccessToken(Int32.Parse(response.Data.ExpiresIn)) { Value = response.Data.AccessToken, RefreshToken = response.Data.RefreshToken };
        }

        public static User GetCurrentUser(AccessToken accessToken)
        {
            var request = new RestRequest("/me", Method.GET);
            var response = CreateClient(accessToken).Execute<Wrapper<User>>(request);

            return response.Data.Items.Single();
        }
    }

    public class Wrapper<T>
    {
        public List<T> Items { get; set; }

        public int? ErrorId { get; set; }
        public string ErrorName { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class User
    {
        public long Id { get; set; }
        public string Link { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class AccessToken
    {
        public AccessToken() : this(0)
        {
        }

        public AccessToken(int expiresIn)
        {
            CreatedAt = DateTimeOffset.Now;
            ExpiresAt = CreatedAt.AddSeconds(expiresIn);
        }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public string RefreshToken { get; set; }

        public string Value { get; set; }

        public static implicit operator string(AccessToken accessToken)
        {
            return accessToken.Value;
        }
    }
}