using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace Sediin.MVC.HtmlHelpers
{
    public class MVCPagingModel
    {
        public int Page { get; set; } = 1;

        public int Rows { get; set; }

        public int PageSize { get; set; } = 10;

        public int PagingStep { get; set; } = 10;


    }

    public static class MVCPaging
    {
        public static MvcHtmlString PagingAjax(this HtmlHelper htmlHelper, int pageSize = 10, int pageIndex = 1, int? totalRows = 10, string action = null, object model = null, AjaxOptions ajaxOptions = null, string actionEsporta = null)
        {
            Func<string> getAjaxOption = delegate ()
            {
                string _ajaxoption = "";

                if (!string.IsNullOrWhiteSpace(ajaxOptions?.HttpMethod))
                {
                    _ajaxoption += " data-ajax-method=\"" + ajaxOptions?.HttpMethod + "\" ";
                }

                if (!string.IsNullOrWhiteSpace(ajaxOptions?.OnBegin))
                {
                    _ajaxoption += " data-ajax-begin=\"" + ajaxOptions?.OnBegin + "\" ";
                }
                else
                {
                    _ajaxoption += " data-ajax-begin=\"gotoPagePaging()\" ";
                }

                if (!string.IsNullOrWhiteSpace(ajaxOptions?.OnSuccess))
                {
                    _ajaxoption += " data-ajax-success=\"" + ajaxOptions?.OnSuccess + "\" ";
                }

                if (!string.IsNullOrWhiteSpace(ajaxOptions?.OnFailure))
                {
                    _ajaxoption += " data-ajax-failure=\"" + ajaxOptions?.OnFailure + "\" ";
                }

                if (!string.IsNullOrWhiteSpace(ajaxOptions?.UpdateTargetId))
                {
                    _ajaxoption += " data-ajax-mode=\"replace\" data-ajax-update=\"#" + ajaxOptions?.UpdateTargetId + "\" ";
                }

                return _ajaxoption;
            };

            pageSize = pageSize == 0 ? 10 : pageSize;

            var _route = new RouteValueDictionary(model);
            _route.Remove("Page");
            //_route.Remove("PageIndex");
            //_route.Remove("PageSize");
            //_route.Remove("Rows");
            //_route.Remove("TotalRecords");
            //_route.Remove("CurrentPage");

            List<string> _querystrings = new List<string>();

            foreach (var item in _route)
            {
                if (item.Value != null)
                {
                    var _c = HttpUtility.UrlDecode(item.Value.ToString());

                    //_querystrings.Add(string.Concat(item.Key, "=",item.Value.ToString()));
                    _querystrings.Add(string.Concat(item.Key, "=", HttpUtility.UrlEncode(_c)));
                    //   _querystrings.Add(string.Concat(item.Key, "=", HttpUtility.UrlEncode(item.Value.ToString())));

                }

            }

            // string _querystring = string.Join("&", _querystrings.ToArray());
            //string _querystring = string.Join("?", _querystrings.ToArray());

            string getUriAndQueryString(string url, int? page = null)
            {
                var _q = new List<string>();

                _q.AddRange(_querystrings);

                if (page.HasValue)
                {
                    _q.Add("page=" + page);
                }

                string _querystring = string.Join("?", _q.ToArray());

                return url + "?" + "q=" + Crypto.Encrypt(_querystring);

            }

            //var _url = action;// + (_querystrings.Count() > 0 ? "?" + _querystring + "&amp;" : "?");

            var _href = "";

            pageIndex = pageIndex == 0 ? 1 : pageIndex;

            //int pageSize = pageSize;// pagingStep.GetValueOrDefault() == 0 ? 10 : pagingStep.GetValueOrDefault();

            var totalpages = (int)Math.Ceiling((decimal)totalRows / pageSize);

            pageIndex = pageIndex > totalpages ? totalpages : pageIndex;

            int totalesteps = (int)Math.Ceiling((decimal)totalpages / pageSize);

            int currentstep = (int)Math.Ceiling((decimal)pageIndex / pageSize);

            //int e = (currentstep * _pagingStep) - _pagingStep;

            int start = ((currentstep * pageSize) - pageSize) + 1;// (int)Math.Ceiling((decimal)totalpages / _pagingStep);

            int end = start + pageSize - 1;

            end = end > totalpages ? totalpages : end;

            var _loader1 = "<div id=\"gotoPageLoaderPaging\" style=\"display:none; width: 16px;background-image: url('data:image/gif;base64,R0lGODlhEAALAPQAAD3A9QAAADSk0jGdyDiw4QEEBQAAAAsiLB9ifRdIXCyMswgZIBE3RyFohRhLYC2PtwkcJAADAxI6Szet3TOj0Dq46w0qNjSm1Dq26SuJryZ4mjCYwjmz5QAAAAAAAAAAACH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCwAAACwAAAAAEAALAAAFLSAgjmRpnqSgCuLKAq5AEIM4zDVw03ve27ifDgfkEYe04kDIDC5zrtYKRa2WQgAh+QQJCwAAACwAAAAAEAALAAAFJGBhGAVgnqhpHIeRvsDawqns0qeN5+y967tYLyicBYE7EYkYAgAh+QQJCwAAACwAAAAAEAALAAAFNiAgjothLOOIJAkiGgxjpGKiKMkbz7SN6zIawJcDwIK9W/HISxGBzdHTuBNOmcJVCyoUlk7CEAAh+QQJCwAAACwAAAAAEAALAAAFNSAgjqQIRRFUAo3jNGIkSdHqPI8Tz3V55zuaDacDyIQ+YrBH+hWPzJFzOQQaeavWi7oqnVIhACH5BAkLAAAALAAAAAAQAAsAAAUyICCOZGme1rJY5kRRk7hI0mJSVUXJtF3iOl7tltsBZsNfUegjAY3I5sgFY55KqdX1GgIAIfkECQsAAAAsAAAAABAACwAABTcgII5kaZ4kcV2EqLJipmnZhWGXaOOitm2aXQ4g7P2Ct2ER4AMul00kj5g0Al8tADY2y6C+4FIIACH5BAkLAAAALAAAAAAQAAsAAAUvICCOZGme5ERRk6iy7qpyHCVStA3gNa/7txxwlwv2isSacYUc+l4tADQGQ1mvpBAAIfkECQsAAAAsAAAAABAACwAABS8gII5kaZ7kRFGTqLLuqnIcJVK0DeA1r/u3HHCXC/aKxJpxhRz6Xi0ANAZDWa+kEAA7AAAAAAAAAAAA'); height: 11px; background-repeat: no-repeat;margin-left:20px; margin-top:12px\"></div>";
            var _loader2 = "<span id=\"gotoPageLoader\" style=\"display:none; width: 16px;background-image: url('data:image/gif;base64,R0lGODlhEAALAPQAAD3A9QAAADSk0jGdyDiw4QEEBQAAAAsiLB9ifRdIXCyMswgZIBE3RyFohRhLYC2PtwkcJAADAxI6Szet3TOj0Dq46w0qNjSm1Dq26SuJryZ4mjCYwjmz5QAAAAAAAAAAACH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCwAAACwAAAAAEAALAAAFLSAgjmRpnqSgCuLKAq5AEIM4zDVw03ve27ifDgfkEYe04kDIDC5zrtYKRa2WQgAh+QQJCwAAACwAAAAAEAALAAAFJGBhGAVgnqhpHIeRvsDawqns0qeN5+y967tYLyicBYE7EYkYAgAh+QQJCwAAACwAAAAAEAALAAAFNiAgjothLOOIJAkiGgxjpGKiKMkbz7SN6zIawJcDwIK9W/HISxGBzdHTuBNOmcJVCyoUlk7CEAAh+QQJCwAAACwAAAAAEAALAAAFNSAgjqQIRRFUAo3jNGIkSdHqPI8Tz3V55zuaDacDyIQ+YrBH+hWPzJFzOQQaeavWi7oqnVIhACH5BAkLAAAALAAAAAAQAAsAAAUyICCOZGme1rJY5kRRk7hI0mJSVUXJtF3iOl7tltsBZsNfUegjAY3I5sgFY55KqdX1GgIAIfkECQsAAAAsAAAAABAACwAABTcgII5kaZ4kcV2EqLJipmnZhWGXaOOitm2aXQ4g7P2Ct2ER4AMul00kj5g0Al8tADY2y6C+4FIIACH5BAkLAAAALAAAAAAQAAsAAAUvICCOZGme5ERRk6iy7qpyHCVStA3gNa/7txxwlwv2isSacYUc+l4tADQGQ1mvpBAAIfkECQsAAAAsAAAAABAACwAABS8gII5kaZ7kRFGTqLLuqnIcJVK0DeA1r/u3HHCXC/aKxJpxhRz6Xi0ANAZDWa+kEAA7AAAAAAAAAAAA'); height: 11px; background-repeat: no-repeat;margin-left: 12px\"></span>";

            var _paging = "";

            _paging += "<div class=\"card mt-2 mb-4\">";
            _paging += "<div class=\"card-block ml-2 mr-2 mt-3\">";
            _paging += "<div class=\"row\">";

            _paging += "<div class=\"col-md-6 table-responsive\" style=\"margin-top:-10px\">";
            #region MyRegion

            _paging += "<nav aria-label=\"navigation\">";
            _paging += "<ul class=\"pagination\">";

            if (start > 1)
            {
                //_href = _url + "rows=" + rows + "&amp;page=" + (start - 1);
                _href = getUriAndQueryString(action, (start - 1));

                _paging += "<li class=\"page-item\">";
                _paging += "<a data-ajax=\"true\" ";
                _paging += getAjaxOption();
                _paging += " class=\"page-link\" href=\"" + _href + "\" aria-label=\"Precedente\">";

                _paging += "<span aria-hidden=\"true\">&laquo;</span>";
                _paging += "<span class=\"sr-only\">Precedente</span>";
                _paging += "</a>";
                _paging += "</li>";
            }
            else
            {
                _paging += "<li class=\"page-item disabled\">";
                _paging += "<a class=\"page-link disabled\" href=\"#\" aria-label=\"Precedente\" onclick=\"return false;\">";
                _paging += "<span aria-hidden=\"true\">&laquo;</span>";
                _paging += "<span class=\"sr-only\">Precedente</span>";
                _paging += "</a>";
                _paging += "</li>";
            }

            for (int i = start; i <= end; i++)
            {
                if (i == pageIndex)
                {
                    _paging += "<li class=\"page-item active\">";
                    _paging += "<a class=\"page-link\" href=\"#\" onclick=\"return false;\" style=\"cursor:default\">" + i + "</a>";
                    _paging += "</li>";
                }
                else
                {
                    _href = getUriAndQueryString(action, i);
                    //_href = _url + "rows=" + rows + "&amp;page=" + i;

                    _paging += "<li class=\"page-item\">";
                    _paging += "<a data-ajax=\"true\" ";
                    _paging += getAjaxOption();
                    _paging += " class=\"page-link\" href=\"" + _href + "\">" + i + "</a>";
                    _paging += "</li>";
                }
            }

            if (end < totalpages)
            {
                _href = getUriAndQueryString(action, (end + 1));

                _paging += "<li class=\"page-item\">";
                _paging += "<a data-ajax=\"true\" ";
                _paging += getAjaxOption();
                _paging += " class=\"page-link\" href=\"" + _href + "\" aria-label=\"Prossima\">";
                //_paging += "<a class=\"page-link\" href=\"#\" aria-label=\"Next\">";
                _paging += "<span aria-hidden=\"true\">&raquo;</span>";
                _paging += "<span class=\"sr-only\">Prossima</span>";
                _paging += "</a>";
                _paging += "</li>";
            }
            else
            {
                _paging += "<li class=\"page-item disabled\">";
                _paging += "<a class=\"page-link disabled\" href=\"#\" aria-label=\"Prossima\" onclick=\"return false;\">";
                _paging += "<span aria-hidden=\"true\">&raquo;</span>";
                _paging += "<span class=\"sr-only\">Prossima</span>";
                _paging += "</a>";
                _paging += "</li>";
            }


            _paging += "<li>" + _loader1 + "</li>";

            _paging += "</ul>";
            _paging += "</nav>";

            #endregion
            _paging += "</div>"; //col-md-6

            var _col = "4";
            var _urlesporta = getUriAndQueryString(actionEsporta);
            _col = "2";
            _paging += "<div class=\"col-md-2\">";

            if (!string.IsNullOrWhiteSpace(actionEsporta))
            {
                _paging += "<a href=\"" + _urlesporta + "\" target=\"_blank\" class=\"btn btn-primary btn-sm\" style=\"margin-top:-10px\"><i class=\"fas fa-file-excel mr-2\"></i>Esporta excel</a>";

            }
            _paging += "</div>";

            //if (!string.IsNullOrWhiteSpace(totaleSomma))
            //{
            //    _col = "2";
            //    _paging += "<div  style=\"margin-top:-10px\" class=\"col-md-2 font-weight-bold\" > Totale importo: <div class=\"doubleunderline text-success\" > "+ totaleSomma + " &euro;</div> </div>";

            //}

            _paging += "<div class=\"col-md-1 form-inline text-right pull-right\" style=\"margin-top:-10px\">";

            _paging += "<input onfocus=\"this.select()\" type=\"number\" id=\"sediinmvcpaging\" name=\"sediinmvcpaging\" class=\"form-control form-control-sm col-md-6\" placeholder=\"Pagina\" min=\"" + 1 + "\" max=\"" + totalpages + "\" value=\"" + pageIndex + "\">";
            _paging += "</div>"; //col-md-6

            _paging += "<div class=\"col-md-1 form-inline text-right pull-right\" style=\"margin-top:-10px\">";

            _paging += "<button type=\"button\" class=\"btn btn-sm btn-primary ml-1\" onclick=\"gotoPage()\">Vai</button>" + _loader2;
            _paging += "</div>"; //col-md-6

            _paging += "<div class=\"col-md-" + _col + " text-right pull-right mb-2\" style=\"margin-top:-20px\">";

            _paging += "<div><small>";
            _paging += "Record <strong>" + (((pageIndex - 1) * pageSize) + 1) + "</strong>/<strong>" + (pageIndex * pageSize > totalRows ? totalRows : (pageIndex * pageSize)) + "</strong>";
            _paging += "</small></div>";

            _paging += "<div><small>";
            _paging += "Totale Record: <strong>" + totalRows + "</strong>";
            _paging += "</small></div>";

            _paging += "<div><small>";
            _paging += "Totale pagine: <strong>" + totalpages + "</strong>";
            _paging += "</small></div>";

            _paging += "</div>"; //col-md-6
            _paging += "</div>"; //row
            _paging += "</div>"; //card-block
            _paging += "</div>"; //card

            _paging += "<script>";

            _paging += "$(document).ready(function(){ $('[data-bs-toggle=\"tooltip\"]').tooltip();});";

            _paging += "function gotoPagePaging()";
            _paging += "{";
            _paging += "$(\"#gotoPageLoaderPaging\").show();";
            _paging += "}";


            _paging += "function gotoPage()";
            _paging += "{";
            _paging += "if ($(\"#sediinmvcpaging\").val() > " + totalpages + " || $(\"#sediinmvcpaging\").val() <= 0){ return; }";

            var _m = "get";

            if (ajaxOptions.HttpMethod?.ToUpper() == "POST")
            {
                _m = "post";
            }
            _paging += "$(\"#gotoPageLoader\").show();";
            _paging += "$." + _m + "('" + getUriAndQueryString(action) + "', { \"page\": $(\"#sediinmvcpaging\").val() }, function(data){ $(\"#" + ajaxOptions?.UpdateTargetId + "\").html(data); $(\"#gotoPageLoader\").hide();});";
            _paging += "}";
            _paging += "</script>";

            return new MvcHtmlString(_paging);
        }

        public static int StartRow(int currentPage, int? pageSize = null)
        {
            pageSize = pageSize.GetValueOrDefault() == 0 ? 10 : pageSize.GetValueOrDefault();
            return currentPage >= 1 ? ((currentPage - 1) * pageSize.GetValueOrDefault()) : 0;
        }

        public static int EndtRow(int startRow, int? pageSize = null)
        {
            pageSize = pageSize.GetValueOrDefault() == 0 ? 10 : pageSize.GetValueOrDefault();
            return startRow + pageSize.GetValueOrDefault();
        }

        public static int RowCount(this HtmlHelper htmlHelper, int currentPage, int pageSize, int rowIndex)
        {
            return (currentPage == 0 ? currentPage : currentPage - 1) * pageSize + rowIndex;
        }

        public static int EndtRow()
        {
            return 10;
        }

        public static int EndtRow(int startRow)
        {
            return (EndtRow() + startRow) - 1;
        }
    }
}
