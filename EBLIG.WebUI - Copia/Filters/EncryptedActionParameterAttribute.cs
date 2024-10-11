using Sediin.MVC.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedActionParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
            if (HttpContext.Current.Request.QueryString.Get("q") != null)
            {
                string encryptedQueryString = HttpContext.Current.Request.QueryString.Get("q");
                string decrptedString = Crypto.Decrypt(encryptedQueryString.ToString());
                string[] paramsArrs = decrptedString.Split('?');

                Dictionary<string, object> _dic = new Dictionary<string, object>();
                List<string> _addedParams = new List<string>();

                foreach (var item in paramsArrs)
                {
                    string[] pair = item.Split('=');
                    if (!_dic.ContainsKey(pair[0]) && !String.IsNullOrEmpty(pair[0]))
                    {

                        _dic.Add(pair[0], pair[1]);
                    }
                }

                if (filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper() == "POST")
                {
                    foreach (var item in HttpContext.Current.Request.Form)
                    {
                        if (item==null)
                        {
                            continue;
                        }

                        if (!_dic.ContainsKey(item.ToString()))
                        {
                            _dic.Add(item.ToString(), HttpContext.Current.Request.Form[item.ToString()]);
                        }
                    }
                }


                if (filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper() == "GET")
                {
                    foreach (var item in HttpContext.Current.Request.QueryString)
                    {
                        if (item == null)
                        {
                            continue;
                        }

                        if (!_dic.ContainsKey(item?.ToString()))
                        {
                            _dic.Add(item.ToString(), HttpContext.Current.Request.QueryString[item.ToString()]);
                        }
                    }
                }

                var actionParams = filterContext.ActionDescriptor.GetParameters();
                for (int i = 0; i < paramsArrs.Length; i++)
                {
                    string[] pair = paramsArrs[i].Split('=');
                    //decryptedParameters.Add(paramArr[0], Convert.ToString(paramArr[1]));

                    if (pair == null)
                        continue;

                    if (_addedParams.FirstOrDefault(x => x.ToUpper()== pair[0].ToUpper()) != null)
                        continue;

                    //Make sure the action has the parameter of the given name
                    var actionParam = actionParams.FirstOrDefault(o => o.ParameterName.ToUpper() == pair[0].ToUpper());
                    if (actionParam != null)
                    {
                        try
                        {
                            var nullType = Nullable.GetUnderlyingType(actionParam.ParameterType);

                            //If a nullable type, make sure to use changetype for that type instead; 
                            //nullable types are not supported
                            if (nullType != null)
                                filterContext.ActionParameters[pair[0]] =
                                     Convert.ChangeType(pair[1], nullType);
                            //Otherwise, assign and cast the value accordingly
                            else
                                filterContext.ActionParameters[pair[0]] =
                                     Convert.ChangeType(pair[1], actionParam.ParameterType);

                            _addedParams.Add(pair[0]);
                        }
                        catch 
                        {

                        }
                        
                    }


                }

                foreach (var item in actionParams)
                {

                    if (_addedParams.FirstOrDefault(x => x.ToUpper() == item.ParameterName.ToUpper()) != null)
                        continue;

                    var _param = _dic.FirstOrDefault(x => x.Key.ToUpper() == item.ParameterName.ToUpper());

                    if (_param.Key != null)
                    {
                        try
                        {
                            //if (_addedParams.FirstOrDefault(x => x.ToUpper()== _param.Key.ToUpper()) != null)
                            //    continue;

                            var nullType = Nullable.GetUnderlyingType(item.ParameterType);

                            //If a nullable type, make sure to use changetype for that type instead; 
                            //nullable types are not supported
                            if (nullType != null)
                                filterContext.ActionParameters[_param.Key] =
                                     Convert.ChangeType(_param.Value, nullType);
                            //Otherwise, assign and cast the value accordingly
                            else
                                filterContext.ActionParameters[_param.Key] =
                                     Convert.ChangeType(_param.Value, item.ParameterType);

                            _addedParams.Add(_param.Key);
                        }
                        catch (Exception)
                        {
                        }
                        
                    }
                    else
                    {
                        try
                        {

                            //parametri del model
                            var _fullname = item.ParameterType.FullName;

                            Type _type = Type.GetType(_fullname);

                            var T = Activator.CreateInstance(_type);

                            foreach (var dic in _dic)
                            {
                                var _prop = _type.GetProperty(dic.Key);

                                if (_prop == null)
                                {
                                    continue;
                                }

                                Reflection.SetValue(T, dic.Key, dic.Value);
                            }

                            if (T != null)
                                filterContext.ActionParameters[item.ParameterName] = T;
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

            }
            base.OnActionExecuting(filterContext);

        }
    }
}