using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EBLIG.WebUI.Areas.Backend.Models
{
    public interface IPagingEntity
    {
        //public int TotalPages { get; set; }
        int PageSize { get; set; }
        int TotalRecords { get; set; }
        int CurrentPage { get; set; }
    }
    public interface IPagingEntity<T, T1>
    {
        IEnumerable<T> Result { get; set; }

        T1 Filtri { get; set; }

        //public int TotalPages { get; set; }
        int PageSize { get; set; }
        int TotalRecords { get; set; }
        int CurrentPage { get; set; }
    }
}