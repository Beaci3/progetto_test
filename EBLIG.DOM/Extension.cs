using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace EBLIG.DOM
{
    public static class Extension
    {
        public static string TrimAll(this object val)
        {
            try
            {
                var _val = val;

                if (_val != null)
                {
                    _val = _val.ToString().Trim();
                    _val = _val.ToString().TrimStart();
                    _val = _val.ToString().TrimEnd();
                    _val = _val.ToString().Replace("  ", " ");
                }

                return _val?.ToString();
            }
            catch
            {
                return val?.ToString();
            }
        }
        public static string RemoveWhiteSpace(this object val)
        {
            try
            {
                var _val = val;

                if (_val != null)
                {
                  return  Regex.Replace(_val.ToString(), @"\s+", "");
                }

                return _val?.ToString();
            }
            catch
            {
                return val?.ToString();
            }
        }


    }
}
