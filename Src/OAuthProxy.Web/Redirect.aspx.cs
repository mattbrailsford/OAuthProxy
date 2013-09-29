using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace OAuthProxy.Web
{
    public partial class Redirect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Grab querystring parameters
                var parameters = Request.QueryString;

                // Parse the url param
                var encodedUrl = parameters["url"];
                var bytes = HttpServerUtility.UrlTokenDecode(encodedUrl);
                var url = new UTF8Encoding().GetString(bytes);

                // Make sure url starts with http
                if(!Regex.IsMatch(url, "^https?://", RegexOptions.IgnoreCase))
                    throw new ArgumentException("Invalid url", "url");

                // Remove the url param from parms collection
                parameters.Remove("url");

                // Construct final redirect url
                var redirectUrl = url +
                    (url.IndexOf("?") >= 0 ? "&" : "?") +
                    ConstructQueryString(parameters);

                // Perform redirect
                Response.StatusCode = 301;
                Response.Status = "301 Moved Permanently";
                Response.AddHeader("Location", redirectUrl);

            }
            catch (Exception)
            {
                // Something went wrong so just return bad request
                Response.StatusCode = 400;
                Response.Status = "400 Bad Request";
            }
        }

        public static string ConstructQueryString(NameValueCollection parameters)
        {
            return string.Join("&", 
                parameters.AllKeys.Select(name => 
                    string.Concat(name, "=", HttpUtility.UrlEncode(parameters[name])))
                    .ToArray());
        }
    }
}