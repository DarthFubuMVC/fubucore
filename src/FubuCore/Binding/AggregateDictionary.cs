using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public enum RequestDataSource
    {
        Route,
        Request,
        RequestProperty,
        File,
        Header,
        Other
    }

    public class AspNetAggregateDictionary : AggregateDictionary
    {
        private static readonly Cache<string, Func<HttpRequestBase, object>> _requestProperties =
            new Cache<string, Func<HttpRequestBase, object>>();

        private static readonly IList<PropertyInfo> _systemProperties = new List<PropertyInfo>();

        static AspNetAggregateDictionary()
        {
            AddRequestProperty(r => r.AcceptTypes);
            AddRequestProperty(r => r.ApplicationPath);
            AddRequestProperty(r => r.AppRelativeCurrentExecutionFilePath);
            AddRequestProperty(r => r.Browser);
            AddRequestProperty(r => r.ClientCertificate);
            AddRequestProperty(r => r.ContentEncoding);
            AddRequestProperty(r => r.ContentLength);
            AddRequestProperty(r => r.ContentType);
            AddRequestProperty(r => r.Cookies);
            AddRequestProperty(r => r.CurrentExecutionFilePath);
            AddRequestProperty(r => r.FilePath);
            AddRequestProperty(r => r.Files);
            AddRequestProperty(r => r.Filter);
            AddRequestProperty(r => r.Form);
            AddRequestProperty(r => r.Headers);
            AddRequestProperty(r => r.HttpMethod);
            AddRequestProperty(r => r.IsAuthenticated);
            AddRequestProperty(r => r.IsLocal);
            AddRequestProperty(r => r.IsSecureConnection);
            AddRequestProperty(r => r.LogonUserIdentity);
            AddRequestProperty(r => r.Params);
            AddRequestProperty(r => r.Path);
            AddRequestProperty(r => r.PathInfo);
            AddRequestProperty(r => r.PhysicalApplicationPath);
            AddRequestProperty(r => r.PhysicalPath);
            AddRequestProperty(r => r.QueryString);
            AddRequestProperty(r => r.RawUrl);
            AddRequestProperty(r => r.RequestType);
            AddRequestProperty(r => r.ServerVariables);
            AddRequestProperty(r => r.TotalBytes);
            AddRequestProperty(r => r.Url);
            AddRequestProperty(r => r.UrlReferrer);
            AddRequestProperty(r => r.UserAgent);
            AddRequestProperty(r => r.UserHostAddress);
            AddRequestProperty(r => r.UserHostName);
            AddRequestProperty(r => r.UserLanguages);
        }

        private AspNetAggregateDictionary()
        {
        }

        public AspNetAggregateDictionary(RequestContext context)
        {
            Func<string, object> locator = key =>
            {
                object found;
                context.RouteData.Values.TryGetValue(key, out found);
                return found;
            };


            AddLocator(RequestDataSource.Route, locator, () => context.RouteData.Values.Keys);

            var @base = context.HttpContext;

            configureForRequest(@base);
        }

        private void configureForRequest(HttpContextBase @base)
        {
            var request = @base.Request;

            AddLocator(RequestDataSource.Request, key => request[key], () => keysForRequest(request));

            AddLocator(RequestDataSource.File,
                       key => request.Files[key],
                       () => request.Files.AllKeys);
            AddLocator(RequestDataSource.Header, key => request.Headers[key], () => request.Headers.AllKeys);
            AddLocator(RequestDataSource.RequestProperty, key => GetRequestProperty(request, key),
                       () => _requestProperties.GetAllKeys());
        }

        public static AspNetAggregateDictionary ForHttpContext(HttpContextWrapper context)
        {
            var dict = new AspNetAggregateDictionary();
            dict.configureForRequest(context);

            return dict;
        }

        public static bool IsSystemProperty(PropertyInfo property)
        {
            return
                _systemProperties.Any(
                    x => property.PropertyType.IsAssignableFrom(x.PropertyType) && x.Name == property.Name);
        }

        public static void AddRequestProperty(Expression<Func<HttpRequestBase, object>> expression)
        {
            var property = ReflectionHelper.GetProperty(expression);
            _systemProperties.Add(property);

            _requestProperties[property.Name] = expression.Compile();
        }


        private static IEnumerable<string> keysForRequest(HttpRequestBase request)
        {
            foreach (var key in request.QueryString.AllKeys)
            {
                yield return key;
            }

            foreach (var key in request.Form.AllKeys)
            {
                yield return key;
            }
        }


        private static object GetRequestProperty(HttpRequestBase request, string key)
        {
            return _requestProperties.Has(key) ? _requestProperties[key](request) : null;
        }
    }

    public class AggregateDictionary
    {
        private readonly IList<Locator> _locators = new List<Locator>();

        public bool HasAnyValuePrefixedWith(string key)
        {
            return _locators.Any(x => x.StartsWith(key));
        }

        public void Value(string key, Action<string, object> callback)
        {
            _locators.Any(x => x.Locate(key, callback));
        }

        public AggregateDictionary AddLocator(RequestDataSource source, Func<string, object> locator,
                                              Func<IEnumerable<string>> allKeys)
        {
            return AddLocator(source.ToString(), locator, allKeys);
        }

        public AggregateDictionary AddLocator(string source, Func<string, object> locator,
                                              Func<IEnumerable<string>> allKeys)
        {
            _locators.Add(new Locator
            {
                Getter = locator,
                Source = source,
                AllKeys = allKeys
            });

            return this;
        }

        public AggregateDictionary AddDictionary(IDictionary<string, object> dictionary)
        {
            AddLocator(RequestDataSource.Other, key => dictionary.ContainsKey(key) ? dictionary[key] : null,
                       () => dictionary.Keys);
            return this;
        }

        public IEnumerable<string> GetAllKeys()
        {
            return _locators.SelectMany(locator => locator.AllKeys()).Distinct();
        }
    }

    public class Locator
    {
        public Func<IEnumerable<string>> AllKeys { get; set; }
        public string Source { get; set;}
        
        public Func<string, object> Getter { get; set; }

        public bool Locate(string key, Action<string, object> callback)
        {
            var value = Getter(key);
            if (value != null)
            {
                callback(Source, value);
                return true;
            }

            return false;
        }

        public bool StartsWith(string key)
        {
            return AllKeys().Any(x => x.StartsWith(key));
        }
    }
}