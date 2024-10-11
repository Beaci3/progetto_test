using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Sediin.MVC.HtmlHelpers
{
    public static class Helper
    {
        public static string ResizeImage(this HtmlHelper html, string path, int width, int height)
        {
            WebImage image = new WebImage(HostingEnvironment.MapPath(path));

            return "data:image/png;base64," + Convert.ToBase64String(image.Resize(width, height, true).GetBytes());
        }

        public static MvcHtmlString AlertWarning(this HtmlHelper helper, string text = null)
        {
            text = text ?? "Nessun dato trovato";
            var _result = "<div class=\"alert alert-warning\">";
            _result += "<span class=\"fas fa-exclamation-circle mr-3\" style=\"font-size:22px; margin-right:10px\"></span>";
            _result += text;
            _result += "</div>";

            return new MvcHtmlString(_result);
        }

        public static MvcHtmlString AlertDanger(this HtmlHelper helper, string text)
        {
            var _result = "<div class=\"alert alert-danger\">";
            _result += "<span class=\"fas fa-exclamation-triangle mr-3\" style=\"font-size:22px; margin-right:10px\"></span>";
            _result += text;
            _result += "</div>";

            return new MvcHtmlString(_result);
        }

        public static MvcHtmlString AlertSuccess(this HtmlHelper helper, string text)
        {
            var _result = "<div class=\"alert alert-success\">";
            _result += "<span class=\"fas fa-check-circle mr-3\" style=\"font-size:22px; margin-right:10px\"></span>";
            _result += text;
            _result += "</div>";

            return new MvcHtmlString(_result);
        }

        public static MvcHtmlString AlertInfo(this HtmlHelper helper, string text)
        {
            var _result = "<div class=\"alert alert-info\">";
            _result += "<span class=\"fas fa-info-circle mr -3\" style=\"font-size:22px; margin-right:10px\"></span>";
            _result += text;
            _result += "</div>";

            return new MvcHtmlString(_result);
        }

        public static List<SelectListItem> SelectListPageSize(this HtmlHelper helper, bool? insertFirst = false)
        {
            List<SelectListItem> _selectlist = new List<SelectListItem>();

            var _a = new int[] { 10, 25, 50, 75, 100 };

            foreach (var item in _a)
            {
                _selectlist.Add(new SelectListItem { Text = item.ToString(), Value = item.ToString() });

            }

            if (insertFirst.GetValueOrDefault())
            {
                var _s = true;
                if (_selectlist != null)
                {
                    _s = _selectlist.FirstOrDefault(s => s.Selected) == null;
                }

                _selectlist.Insert(0, new SelectListItem { Value = "", Text = "[Selezionare una voce]", Selected = _s });
            }

            return _selectlist;

        }

        public static List<SelectListItem> SelectListOrderBy(this HtmlHelper helper, Dictionary<string, string> a, bool? insertFirst = false)
        {
            List<SelectListItem> _selectlist = new List<SelectListItem>();

            foreach (var item in a)
            {
                var _asc = " CASE WHEN "+ item.Key + " IS NULL or "+ item.Key + "  = '' THEN '' ELSE 'ZZZZZ' END DESC, "+ item.Key + " asc";
                var _desc = " CASE WHEN "+ item.Key + "  IS NULL or "+ item.Key + "  = '' THEN '' ELSE 'ZZZZZ' END DESC, " + item.Key + " desc";
               
                _selectlist.Add(new SelectListItem { Text = item.Value + " ascendente", Value = _asc });
                _selectlist.Add(new SelectListItem { Text = item.Value + " discendente", Value = _desc });
            }

            if (insertFirst.GetValueOrDefault())
            {
                var _s = true;
                if (_selectlist != null)
                {
                    _s = _selectlist.FirstOrDefault(s => s.Selected) == null;
                }

                _selectlist.Insert(0, new SelectListItem { Value = "", Text = "[Selezionare una voce]", Selected = _s });
            }

            return _selectlist;

        }

        public static List<SelectListItem> SelectList(this HtmlHelper helper, IEnumerable<object> a, string value, string text, bool? insertFirst = false)
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            var _selectlist = (from item in a
                               select new SelectListItem
                               {
                                   Text = item?.GetType()?.GetProperty(text)?.GetValue(item)?.ToString(),
                                   Value = item?.GetType()?.GetProperty(value)?.GetValue(item)?.ToString(),
                                   Selected = Convert.ToBoolean(item?.GetType()?.GetProperty("Selected")?.GetValue(item)?.ToString()),
                               }).ToList();

            if (insertFirst.GetValueOrDefault())
            {
                var _s = true;
                if (_selectlist != null)
                {
                    _s = _selectlist.FirstOrDefault(s => s.Selected) == null;
                }

                _selectlist.Insert(0, new SelectListItem { Value = "", Text = "[Selezionare una voce]", Selected = _s });
            }

            return _selectlist;
        }

        public static MvcHtmlString ColorString(this HtmlHelper helper, object text, object pattern)
        {
            try
            {
                if (text != null && pattern != null)
                {
                    Regex reg = new Regex(pattern.ToString().Replace("(", "").Replace(")", "").Trim().TrimStart().TrimEnd(), RegexOptions.IgnoreCase);
                    text = reg.Replace(text.ToString().Trim().TrimStart().TrimEnd(), new MatchEvaluator(ReplaceKeywords));
                }
                if (text == null)
                    text = "";
            }
            catch
            {
            }
            return MvcHtmlString.Create(text.ToString());
        }

        private static string ReplaceKeywords(Match m)
        {
            return "<span style='color: red; background-color: #FFFF00;'>" + m.Value + "</span>";
        }

        public static MvcHtmlString CropText(this HtmlHelper helper, string text, int lenght, string opzional = "...")
        {
            try
            {
                var _text = text;

                if (text?.Length > lenght)// && (text.Length - lenght != 1))
                {
                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");

                    _text = rx.Replace(text, "");

                    var _leng = !string.IsNullOrWhiteSpace(opzional) ? lenght + opzional.Length : lenght;

                    if (_text.Length > (_leng + 1))
                    {
                        _text = _text.Substring(0, _leng) + opzional;

                        _text = "<span title=\"" + text + "\" data-bs-toggle=\"tooltip\" data-bs-title=\"" + text + "\">" + _text + "</span>";
                    }
                }
                return MvcHtmlString.Create(_text);
            }
            catch
            {
                return MvcHtmlString.Create(text);
            }
        }

        public static string GetIPAddress(this HtmlHelper htmlHelper)
        {
            return GetIPAddress(htmlHelper.ViewContext.RequestContext.HttpContext);
        }

        public static string GetIPAddress(HttpContextBase request)
        {
            try
            {
                if (request?.Request?.Headers["CF-CONNECTING-IP"] != null)
                {
                    return request.Request.Headers["CF-CONNECTING-IP"].ToString();
                }

                if (request.Request.ServerVariables.AllKeys.Contains("HTTP_X_FORWARDED_FOR"))
                {
                    string ipAddress = request.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        string[] addresses = ipAddress.Split(',');
                        if (addresses.Length != 0)
                        {
                            return addresses[0];
                        }
                    }
                }

                return request?.Request?.UserHostAddress;
            }
            catch
            {
                return "";
            }
        }


        #region Encoded url "Backend" per Filter "EncryptedActionParameterAttribute.cs"

        public static MvcHtmlString EncodedAction(this HtmlHelper htmlHelper, string actionName)
        {
            return EncodedAction(htmlHelper, actionName, null, null);
        }

        public static MvcHtmlString EncodedAction(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            return EncodedAction(htmlHelper, actionName, controllerName, null);
        }

        public static MvcHtmlString EncodedAction(this HtmlHelper htmlHelper, string actionName, object routeValues = null)
        {
            return EncodedAction(htmlHelper, actionName, null, routeValues);
        }

        public static MvcHtmlString EncodedAction(this HtmlHelper htmlHelper, string actionName, string controllerName = null, object routeValues = null)
        {
            var _areas = ((Route)htmlHelper.ViewContext.RequestContext.RouteData.Route).DataTokens.FirstOrDefault(c => c.Key == "area");
            var _area = _areas.Value;

            string queryString = string.Empty;

            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    if (d.Keys.ElementAt(i) == "area")
                    {
                        _area = d.Values.ElementAt(i);
                        continue;
                    }

                    if (i > 0)
                    {
                        queryString += "?";
                    }
                    queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            StringBuilder ancor = new StringBuilder();

            ancor.Append("/" + _area);
            // ancor.Append("/Backend");

            if (controllerName != string.Empty)
            {
                ancor.Append("/" + controllerName);
            }

            if (actionName != "Index")
            {
                ancor.Append("/" + actionName);
            }

            if (queryString != string.Empty)
            {
                ancor.Append("?q=" + Crypto.Encrypt(queryString));
            }

            return new MvcHtmlString(ancor.ToString());
        }

        public static MvcHtmlString EncodedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName = null, object routeValues = null, object htmlAttributes = null)
        {
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;

            if (htmlAttributes != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(htmlAttributes);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    htmlAttributesString += " " + d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }

            StringBuilder ancor = new StringBuilder();
            ancor.Append("<a ");
            if (htmlAttributesString != string.Empty)
            {
                ancor.Append(htmlAttributesString);
            }
            ancor.Append(" href='");

            ancor.Append(EncodedAction(null, actionName, controllerName, routeValues).ToHtmlString());

            ancor.Append("'");
            ancor.Append(">");
            ancor.Append(linkText);
            ancor.Append("</a>");
            return new MvcHtmlString(ancor.ToString());
        }

        public static MvcHtmlString EncodedAjaxActionLink(this AjaxHelper htmlHelper, string linkText, string actionName, string controllerName = null, object routeValues = null, AjaxOptions ajaxoptions = null, object htmlAttributes = null)
        {
            var _areas = ((Route)htmlHelper.ViewContext.RequestContext.RouteData.Route).DataTokens.FirstOrDefault(c => c.Key == "area");
            var _area = _areas.Value;

            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var anchor = new TagBuilder("a");
            anchor.InnerHtml = linkText;

            anchor.Attributes["href"] = EncodedAction(new HtmlHelper(htmlHelper.ViewContext, htmlHelper.ViewDataContainer)
            {
            }, actionName, controllerName, routeValues).ToHtmlString();// url.ToString();// urlHelper.Action(actionName, controllerName, routeValues);

            anchor.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            anchor.MergeAttributes((ajaxoptions ?? new AjaxOptions()).ToUnobtrusiveHtmlAttributes());

            //anchor.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(anchor.ToString());
        }

        #endregion

        public static MvcHtmlString GetMonthName(this HtmlHelper html, object monthNumber)
        {
            try
            {
                if (monthNumber == null)
                {
                    return null;
                }

                int.TryParse(monthNumber.ToString(), out int _month);

                if (_month > 0 && _month <= 12)
                {
                    return MvcHtmlString.Create(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_month));
                }

                return MvcHtmlString.Create(monthNumber.ToString());
            }
            catch (Exception ex)
            {
                return MvcHtmlString.Create(ex.Message);
            }
        }


        #region MyRegion

        public static MvcHtmlString TextBoxForReadOnly<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, string format = null, bool? readOnly = null)
        {
            var _readonly = readOnly.HasValue ? readOnly.Value : typeof(TModel)?.GetProperty("ReadOnly")?.GetValue(helper.ViewData.Model);
            var _name = ExpressionHelper.GetExpressionText(expression);
            //   var _value = typeof(TModel)?.GetProperty(_name)?.GetValue(helper.ViewData.Model);

            object _value = null;

            if (helper.ViewData.Model != null)
                _value = typeof(TModel)?.GetProperty(_name)?.GetValue(helper.ViewData.Model);

            if (format != null)
            {
                _value = string.Format(format, _value);
            }

            // copy htmlAttributes object to Dictionary
            Dictionary<string, object> dicHtmlAttributes = new Dictionary<string, object>();
            if (htmlAttributes != null)
            {
                foreach (var prop in htmlAttributes?.GetType().GetProperties())
                {
                    if (prop.Name == "style")
                    {
                        continue;
                    }
                    dicHtmlAttributes.Add(prop.Name.Contains("_") ? prop.Name.Replace("_", "-") : prop.Name, prop.GetValue(htmlAttributes));
                }
            }
            //add custom attribute
            dicHtmlAttributes.Add("value", _value);

            if (_readonly != null)
            {
                bool.TryParse(_readonly.ToString(), out bool a);
                if (a)
                {
                    var _style = "";
                    if (htmlAttributes != null)
                    {
                        foreach (var prop in htmlAttributes?.GetType().GetProperties())
                        {
                            if (prop.Name == "style")
                            {
                                _style = prop.GetValue(htmlAttributes).ToString();
                                break;
                            }
                        }
                    }

                    var _hidden = helper.Hidden(_name, _value, dicHtmlAttributes);

                    return MvcHtmlString.Create("<div id=\"" + _name + "_readOnly\" class=\"w-100 mt-1\" style=\"" + _style + "\">" + _value + _hidden.ToHtmlString() + "</div>");
                }
            }

            // return helper.TextBoxFor(expression, dicHtmlAttributes);
            return helper.TextBox(_name, _value, dicHtmlAttributes);
        }

        public static MvcHtmlString TextBoxForReadOnlyAsSpan<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, string format = null, bool? readOnly = null)
        {
            var _readonly = readOnly.HasValue ? readOnly.Value : typeof(TModel)?.GetProperty("ReadOnly")?.GetValue(helper.ViewData.Model);
            var _name = ExpressionHelper.GetExpressionText(expression);
            //   var _value = typeof(TModel)?.GetProperty(_name)?.GetValue(helper.ViewData.Model);

            object _value = null;

            if (helper.ViewData.Model != null)
                _value = typeof(TModel)?.GetProperty(_name)?.GetValue(helper.ViewData.Model);

            if (format != null)
            {
                _value = string.Format(format, _value);
            }

            // copy htmlAttributes object to Dictionary
            Dictionary<string, object> dicHtmlAttributes = new Dictionary<string, object>();
            if (htmlAttributes != null)
            {
                foreach (var prop in htmlAttributes?.GetType().GetProperties())
                {
                    if (prop.Name == "style")
                    {
                        continue;
                    }
                    dicHtmlAttributes.Add(prop.Name.Contains("_") ? prop.Name.Replace("_", "-") : prop.Name, prop.GetValue(htmlAttributes));
                }
            }
            //add custom attribute
            dicHtmlAttributes.Add("value", _value);

            if (_readonly != null)
            {
                bool.TryParse(_readonly.ToString(), out bool a);
                if (a)
                {
                    var _style = "";
                    if (htmlAttributes != null)
                    {
                        foreach (var prop in htmlAttributes?.GetType().GetProperties())
                        {
                            if (prop.Name == "style")
                            {
                                _style = prop.GetValue(htmlAttributes).ToString();
                                break;
                            }
                        }
                    }

                    var _hidden = helper.Hidden(_name, _value, dicHtmlAttributes);

                    return MvcHtmlString.Create("<span id=\"" + _name + "_readOnly\" class=\"\" style=\"" + _style + "\">" + _value + _hidden.ToHtmlString() + "</span>");
                }
            }

            // return helper.TextBoxFor(expression, dicHtmlAttributes);
            return helper.TextBox(_name, _value, dicHtmlAttributes);
        }

        public static MvcHtmlString TextAreaForReadOnly<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, int rows, int cols, object htmlAttributes, string format = null, bool? readOnly = null)
        {
            var _readonly = readOnly.HasValue ? readOnly.Value : typeof(TModel)?.GetProperty("ReadOnly")?.GetValue(helper.ViewData.Model);
            var _name = ExpressionHelper.GetExpressionText(expression);
            object _value = null;

            if (helper.ViewData.Model != null)
                _value = typeof(TModel)?.GetProperty(_name)?.GetValue(helper.ViewData.Model);

            if (format != null)
            {
                _value = string.Format(format, _value);
            }

            if (_readonly != null)
            {
                bool.TryParse(_readonly.ToString(), out bool a);
                if (a)
                {
                    var _style = "";
                    if (htmlAttributes != null)
                    {
                        foreach (var prop in htmlAttributes?.GetType().GetProperties())
                        {
                            if (prop.Name == "style")
                            {
                                _style = prop.GetValue(htmlAttributes).ToString();
                                break;
                            }
                        }
                    }
                    return MvcHtmlString.Create("<div id=\"" + _name + "_readOnly\"  class=\"w-100 mt-1 text-uppercase\" style=\"" + _style + "\">" + _value + "</div>");
                }
            }

            // copy htmlAttributes object to Dictionary
            Dictionary<string, object> dicHtmlAttributes = new Dictionary<string, object>();
            if (htmlAttributes != null)
            {
                foreach (var prop in htmlAttributes?.GetType().GetProperties())
                {
                    if (prop.Name == "style")
                    {
                        continue;
                    }
                    dicHtmlAttributes.Add(prop.Name.Contains("_") ? prop.Name.Replace("_", "-") : prop.Name, prop.GetValue(htmlAttributes));
                }
            }
            //add custom attribute
            //dicHtmlAttributes.Add("value", _value);
            // return helper.TextBoxFor(expression, dicHtmlAttributes);
            return helper.TextArea(_name, _value != null ? _value.ToString() : "", rows, cols, dicHtmlAttributes);
        }

        public static MvcHtmlString DropDownListForReadOnly<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, List<SelectListItem> values, object htmlAttributes, bool? readOnly = null)
        {
            var _readonly = readOnly.HasValue ? readOnly.Value : typeof(TModel).GetProperty("ReadOnly")?.GetValue(helper.ViewData.Model);
            var _name = ExpressionHelper.GetExpressionText(expression);
            //var _value = typeof(TModel)?.GetProperty(_name)?.GetValue(helper.ViewData.Model);
            object _value = null;

            if (helper.ViewData.Model != null)
                _value = typeof(TModel)?.GetProperty(_name)?.GetValue(helper.ViewData.Model);

            if (_readonly != null)
            {
                bool.TryParse(_readonly.ToString(), out bool a);
                if (a)
                {
                    _value = values?.FirstOrDefault(c => c.Value?.ToString() == _value?.ToString())?.Text;
                    return MvcHtmlString.Create("<div id=\"" + _name + "_readOnly\" class=\"w-100 mt-1 text-uppercase\">" + _value + "</div>");
                }
            }

            List<SelectListItem> _list = new List<SelectListItem>();
            foreach (var item in values)
            {
                SelectListItem selItem = new SelectListItem
                {
                    Value = item.Value,
                    Text = item.Text,
                    Selected = item.Selected ? true : item.Value?.ToString() == _value?.ToString()
                };
                _list.Add(selItem);
            }

            // copy htmlAttributes object to Dictionary
            Dictionary<string, object> dicHtmlAttributes = new Dictionary<string, object>();
            if (htmlAttributes != null)
            {
                foreach (var prop in htmlAttributes?.GetType().GetProperties())
                {
                    if (prop.Name == "style")
                    {
                        continue;
                    }
                    dicHtmlAttributes.Add(prop.Name.Contains("_") ? prop.Name.Replace("_", "-") : prop.Name, prop.GetValue(htmlAttributes));
                }
            }

            return helper.DropDownList(_name, _list, dicHtmlAttributes);
        }


        #endregion
    }
}
