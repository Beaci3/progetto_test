using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sediin.MVC.HtmlHelpers
{
    public class BsGrid : Controller
    {
        public bool? UseDebug_jquery_bs_grid { get; set; } = false;

        //public string CustomButton { get; set; }

        public string AjaxFetchDataURL { get; set; }

        public string GridId { get; set; }

        public string SortName { get; set; }

        public string SortField { get; set; }

        public string SortOrder { get; set; }

        public string PlaceholderRicerca { get; set; }

        public List<BsGridFields> BsFields { get; set; }

        public BsGridNewButton BsGridButton { get; set; }

        public Dictionary<string, string> FilterParameter { get; set; }

        RequestContext CurrentContext { get; set; }

        public HttpRequestBase RequestBase { get; set; }

        Uri CurrentUrl { get; set; }

        public BsGrid()
        {
            SortOrder = "descending";
            GridId = Guid.NewGuid().ToString("N").Substring(0, 10);
        }

        public BsGrid(HttpRequestBase httpRequestBase)
        {
            SortOrder = "descending";
            GridId = Guid.NewGuid().ToString("N").Substring(0, 10);
            CurrentContext = httpRequestBase.RequestContext;
            CurrentUrl = httpRequestBase.UrlReferrer;
        }

        public class BsGridFields
        {
            public string field { get; set; }

            public string header { get; set; }

            public bool? visible { get; set; }

        }

        public class BsGridNewButton
        {
            public string Action { get; set; }

            public string Text { get; set; }

            public string AjaxSuccess { get; set; }

            public string AjaxBegin { get; set; }

            public string AjaxFailure { get; set; }

            //public string Css { get; set; }
        }

        private class page_settings<T>
        {
            public int total_rows { get; set; }
            public string error { get; set; }
            public List<string> debug_message { get; set; }
            public List<string> filter_error { get; set; }
            public IEnumerable<T> page_data { get; set; }
        }

        public class current_page_settings
        {
            public int page_num { get; set; }

            public int rows_per_page { get; set; }

            public string sortingName { get; set; }

            public string sortingOrder { get; set; }

            public string sortingField { get; set; }

            public string filter { get; set; }

            public int skip { get; set; }

            public int take { get; set; }

            public Dictionary<string, string> filterParameter { get; set; }

        }

        public JsonResult GetBsGrid<T>(IEnumerable<T> list, int totalRows)
        {
            try
            {
                page_settings<T> a = new page_settings<T>();
                a.page_data = list;
                a.total_rows = totalRows;
                a.debug_message = new List<string>();
                a.filter_error = new List<string>();

                SetCurrentPagingSettings();

                return Json(a, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal void SetIsPostBack()
        {
            try
            {
                IsNewController();

                if (CurrentContext.HttpContext.Session["BsGridPagingDataHolder"] != null)
                    CurrentContext.HttpContext.Session["BsGridPaging"] = CurrentContext.HttpContext.Session["BsGridPagingDataHolder"];

            }
            catch
            {
            }
        }

        internal current_page_settings GetCurrent_page_settings()
        {
            return (current_page_settings)CurrentContext.HttpContext.Session["BsGridPaging"];
        }

        private void SetCurrentPagingSettings()
        {
            //CheckController();

            current_page_settings c = new current_page_settings();
            c.page_num = ToInt(GetUnvalidated("page_num"));
            c.rows_per_page = ToInt(GetUnvalidated("rows_per_page"));
            c.sortingName = GetUnvalidated("sorting[0][sortingName");
            c.sortingField = GetUnvalidated("sorting[0][field]");
            c.sortingOrder = GetUnvalidated("sorting[0][order]");
            c.filter = GetQueryString("Cerca");

            c.filterParameter = new Dictionary<string, string>();

            //foreach (var item in CurrentContext.HttpContext.Request.Form)
            //{
            //    if (item.ToString().StartsWith("filterParameter_", StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        c.filterParameter.Add(item.ToString().Replace("filterParameter_", ""), GetUnvalidated(item.ToString()));
            //    }
            //}
            foreach (var item in CurrentContext.HttpContext.Request.Params)
            {
                if (item.ToString().StartsWith("filterParameter_", StringComparison.InvariantCultureIgnoreCase))
                {
                    c.filterParameter.Add(item.ToString().Replace("filterParameter_", ""), GetUnvalidated(item.ToString()));
                }
            }


            //c.parameters
            CurrentContext.HttpContext.Session["BsGridPagingDataHolder"] = c;
            CurrentContext.HttpContext.Session["BsGridPagingController"] = CurrentContext.RouteData.Values["controller"];
            CurrentContext.HttpContext.Session["BsGridPaging"] = null;
        }

        private bool IsNewController()
        {
            try
            {
                if (CurrentContext.RouteData.Values["controller"] != null && CurrentContext.HttpContext.Session["BsGridPagingController"] != null)
                {
                    if (CurrentContext.RouteData.Values["controller"].ToString().ToLower() != CurrentContext.HttpContext.Session["BsGridPagingController"].ToString().ToLower())
                    {
                        CurrentContext.HttpContext.Session["BsGridPagingDataHolder"] = null;
                        CurrentContext.HttpContext.Session["BsGridPagingController"] = null;
                        CurrentContext.HttpContext.Session["BsGridPaging"] = null;
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public current_page_settings GetCurrentPagingSettings()
        {
            try
            {
                current_page_settings _current_page_settings = null;

                var page_num = 1;
                var rows_per_page = 10;
                var sortingName = "";
                var sortingField = "";
                var sortingOrder = "";
                var filter = "";

                if (!IsNewController())
                {
                    _current_page_settings = (current_page_settings)GetCurrent_page_settings();
                    page_num = _current_page_settings != null ? _current_page_settings.page_num : ToInt(GetUnvalidated("page_num"));
                    rows_per_page = _current_page_settings != null ? _current_page_settings.rows_per_page : ToInt(GetUnvalidated("rows_per_page"));
                    sortingName = _current_page_settings != null ? _current_page_settings.sortingName : GetUnvalidated("sorting[0][sortingName]");
                    sortingField = _current_page_settings != null ? _current_page_settings.sortingField : GetUnvalidated("sorting[0][field]");
                    sortingOrder = _current_page_settings != null ? _current_page_settings.sortingOrder : GetUnvalidated("sorting[0][order]");
                    filter = _current_page_settings != null ? _current_page_settings.filter : trim(GetQueryString("Cerca"));
                }

                if (sortingOrder != null)
                {
                    if (sortingOrder == "none")
                        sortingOrder = "";
                }

                int skip = ((page_num - 1) * rows_per_page);// page_num > 1 ? ((page_num - 1) * rows_per_page) + 1 : 0;
                int take = rows_per_page;// page_num > 1 ? (skip + rows_per_page) - 1 : rows_per_page;

                _current_page_settings = new current_page_settings();
                _current_page_settings.page_num = page_num;
                _current_page_settings.rows_per_page = rows_per_page;
                _current_page_settings.sortingName = trim(sortingName);
                _current_page_settings.sortingField = trim(sortingField);
                _current_page_settings.sortingOrder = trim(sortingOrder);
                _current_page_settings.filter = trim(filter);
                _current_page_settings.skip = skip;
                _current_page_settings.take = take;

                return _current_page_settings;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SetBsGrid(HttpRequestBase Request)
        {
            BsGrid h = new BsGrid();
            try
            {
                if (Request.ServerVariables["HTTP_REFERER"] == null || Request.Url == null)
                {
                    h.SetIsPostBack();
                }
                else
                {
                    Uri U = new Uri(Request.ServerVariables["HTTP_REFERER"]);
                    Uri SU = new Uri(Request.Url.ToString());

                    h.CurrentContext = Request.RequestContext;

                    if (!U.Equals(SU))
                    {
                        h.SetIsPostBack();
                    }
                }

            }
            catch (Exception)
            {
                h.SetIsPostBack();
            }
        }

        #region MyRegion

        private string GetUnvalidated(string p)
        {
            try
            {
                return CurrentContext.HttpContext.Request.Unvalidated[p];
                //return CurrentContext.HttpContext.Request.Params[p];
            }
            catch
            {
                return "";
            }
        }

        private string GetQueryString(string p)
        {
            try
            {
                return CurrentContext.HttpContext.Request.QueryString[p];
            }
            catch
            {
                return "";
            }
        }

        string trim(string p)
        {
            try
            {
                return p.TrimEnd().TrimStart();
            }
            catch (Exception)
            {
                return "";
            }
        }

        int ToInt(object p)
        {
            try
            {
                return Convert.ToInt32(p);
            }
            catch
            {
                return 0;
            }
        }

        //string NotNull(string p)
        //{
        //    try
        //    {
        //        return Request.QueryString[p].Trim().TrimStart().TrimEnd();
        //    }
        //    catch (Exception)
        //    {
        //        return "";
        //    }
        //}

        #endregion
    }

    public static class Helpers
    {
        //public static MvcHtmlString DataGrid(this HtmlHelper helper)
        //{

        //    StringBuilder sb = new StringBuilder();
        //    UrlHelper h = new UrlHelper(HttpContext.Current.Request.RequestContext);

        //    var _folder = h.Content("~/bs_grid/");
        //    sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}{1}\"></script>", _folder, "");

        //    sb.AppendFormat("<script type=\"text/javascript\">function loadDataBrid(){}</script>", _folder, "");


        //    return MvcHtmlString.Create("");
        //}

        public static MvcHtmlString BsGrid(this HtmlHelper helper, BsGrid model)
        {
            var _jquery_bs_grid = "bs_grid/jquery.bs_grid.min.js";

            if (model.UseDebug_jquery_bs_grid.GetValueOrDefault())
            {
                _jquery_bs_grid = "bs_grid/jquery.bs_grid.js";
            }

            UrlHelper h = new UrlHelper(HttpContext.Current.Request.RequestContext);

            var _folder = h.Content("~/bs_grid/");

            var _jsFiles = new string[] {
                "jquery-ui-timepicker-addon.min.js",
                "jquery.ui.touch-punch.min.js",
                "bs_pagination/jquery.bs_pagination.min.css",
                "bs_pagination/jquery.bs_pagination.min.js",
                "bs_pagination/localization/en.min.js",
                "jui_filter_rules/jquery.jui_filter_rules.min.js",
                "jui_filter_rules/localization/en.min.js",
                "moment.min.js",
                _jquery_bs_grid,
                //"bs_grid/jquery.bs_grid.min.js",
                "bs_grid/localization/en.min.js"
            };

            var _cssFiles = new string[] {
                "jquery-ui-timepicker-addon.min.css",
                "bs_pagination/jquery.bs_pagination.min.css",
                "jui_filter_rules/jquery.jui_filter_rules.bs.min.css",
                "bs_grid/jquery.bs_grid.min.css"
            };

            StringBuilder sb = new StringBuilder();

            foreach (var item in _cssFiles)
            {
                sb.AppendFormat("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}{1}\"/>", _folder, item);
            }

            foreach (var item in _jsFiles)
            {
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}{1}\"></script>", _folder, item);
            }

            sb.Append("<script type=\"text/javascript\">");

            //ready
            sb.Append("$().ready(function () {");
            sb.Append("loadGrid();");
            sb.Append("});");


            //loadgrid
            sb.Append("function loadGrid(pagenum) {");

            sb.AppendFormat("$(\"#btnSubmit_{0}\").addClass(\"disabled\");", model.GridId);

            Sediin.MVC.HtmlHelpers.BsGrid.current_page_settings a =
                (Sediin.MVC.HtmlHelpers.BsGrid.current_page_settings)HttpContext.Current.Session["BsGridPaging"];

            if (a == null)
            {
                a = new Sediin.MVC.HtmlHelpers.BsGrid.current_page_settings();
                a.page_num = 1;
                a.rows_per_page = 10;
            }

            sb.AppendFormat("var _pagenum = \"{0}\";", a.page_num);
            sb.Append("_pagenum = pagenum != undefined ? pagenum : (_pagenum == \"\" ? 1 : _pagenum);");

            sb.AppendFormat("var _rows_per_page = \"{0}\";", a.rows_per_page);

            sb.AppendFormat("var _sortingName = \"{0}\";", a.sortingName);
            sb.AppendFormat("_sortingName = _sortingName == \"\" ? \"{0}\" : _sortingName;", model.SortName);

            sb.AppendFormat("var _sortingField = \"{0}\";", a.sortingField);
            sb.AppendFormat("_sortingField = _sortingField == \"\" ? \"{0}\" : _sortingField;", model.SortField);

            sb.AppendFormat("var _filter = \"{0}\";", a.filter);
            sb.AppendFormat("_filter = $(\"#cerca_{0}\").val() != undefined ? $(\"#cerca_{0}\").val() : _filter;", model.GridId);
            sb.AppendFormat("$(\"#cerca_{0}\").val(_filter);", model.GridId);

            sb.AppendFormat("var _sortingOrder = \"{0}\";", a.sortingOrder);
            sb.AppendFormat("_sortingOrder = _sortingOrder == \"\" ? \"{0}\" : _sortingOrder;", model.SortOrder);


            sb.AppendFormat("$(\"#{0}\")", model.GridId);

            sb.Append(".bs_grid({");
            sb.Append("onDatagridError: function(event, data) {");
            sb.Append("alert(data[\"err_description\"] + ' (' + data[\"err_code\"] + ')');");
            sb.Append("},");

            sb.AppendFormat("ajaxFetchDataURL: \"{0}?cerca=\" + _filter,", model.AjaxFetchDataURL);
            sb.Append("row_primary_key: \"PraticaID\",");
            sb.Append("pageNum: parseInt(_pagenum),");
            sb.Append("rowsPerPage: parseInt(_rows_per_page),");
            sb.Append("showRowNumbers: true,");
            sb.Append("useFilters: false,");
            sb.Append("rowSelectionMode: false,");
            sb.Append("useSortableLists: false,");
            sb.Append("showSortingIndicator: true,");

            sb.Append("columns: [");

            foreach (var item in model.BsFields)
            {
                sb.Append("{");
                sb.AppendFormat("field: \"{0}\", header: \"{1}\", visible: \"{2}\"", item.field, item.header, !item.visible.HasValue || item.visible.GetValueOrDefault() ? "yes" : "no");
                sb.Append("},");
            }

            sb.Append("],");

            sb.Append("sorting: [");
            sb.Append("{ sortingName: _sortingName, field: _sortingField, order: _sortingOrder }");
            sb.Append("],");


            //fine bsgrid
            sb.Append("});");

            sb.AppendFormat("var _testoPlaceholder = \"{0}\";", model.PlaceholderRicerca);

            sb.Append("var _search = \"\";");

            sb.Append("_search += \"<div class='col-lg-8' style='margin-left:-15px'>\";");
            sb.Append("_search += \"<div class='input-group'>\";");

            sb.AppendFormat("_search += \"<input type='text' class='form-control' placeholder='\"+ _testoPlaceholder + \"' name='cerca_{0}' id='cerca_{0}' value='\" + _filter + \"'>\";", model.GridId);
            sb.Append("_search += \"<span class='input-group-btn'>\";");
            sb.AppendFormat("_search += \"<button id='btnSubmit_{0}'  name='btnSubmit_{0}' class='btn btn-default disabled' type='button' onclick='return false' title='Cerca'><span class='glyphicon glyphicon-search'></span></button>\";", model.GridId);
            sb.Append("_search += \"</span>\";");

            sb.Append("_search += \"</div>\";");
            sb.Append("_search += \"</div>\";");

            if (model.BsGridButton != null)
            {

                var _buttonNew = "<a id='btnNew_" + model.GridId + "' name='btnNew_" + model.GridId + "' href='" + model.BsGridButton.Action + "' title='" + model.BsGridButton.Text + "' ";
                _buttonNew += "class='btn btn-default dropdown pull-right' ";
                _buttonNew += "data-ajax='true' data-ajax-success='" + model.BsGridButton.AjaxSuccess + "' data-ajax-begin='" + model.BsGridButton.AjaxBegin + "' data-ajax-failure='" + model.BsGridButton.AjaxFailure + "' >";
                _buttonNew += "<span class='glyphicon glyphicon-plus text-danger'></span><span class=''>&nbsp;" + model.BsGridButton.Text + "</span></a>";


                sb.Append("_search += \"" + _buttonNew + "\";");
            }

            sb.AppendFormat("$(\"#tools_{0}\").append(_search);", model.GridId);


            sb.AppendFormat("$(\"#cerca_{0}\").keyup(function (event) ", model.GridId);

            sb.Append("{");

            sb.Append("if (event.keyCode == 13) {");
            sb.AppendFormat("$(\"#btnSubmit_{0}\").click();", model.GridId);
            sb.Append("}");
            sb.Append("});");

            sb.AppendFormat("$('#filter_list_{0}').click(function (e)", model.GridId);
            sb.Append("{");
            sb.Append("e.preventDefault();");
            sb.Append("e.stopPropagation();");
            sb.Append("});");

            sb.AppendFormat("$(\"#btnSubmit_{0}\").click(function (e)", model.GridId);
            sb.Append("{");
            sb.Append("e.preventDefault();");
            sb.Append("e.stopPropagation();");
            sb.Append("loadGrid(1);");
            sb.Append("});");


            //fine function loadgrid
            sb.Append("}");



            sb.Append("</script>");



            sb.AppendFormat("<div id=\"{0}\"></div>", model.GridId);


            return MvcHtmlString.Create(sb.ToString());

        }
    }
}


