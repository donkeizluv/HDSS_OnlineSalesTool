using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.ViewModels
{
    public interface IListingViewModel
    {
        //For debug purposes
        int OnPage { get; set; }
        string FilterBy { get; set; }
        string FilterString { get; set; }
        string OrderBy { get; set; }
        bool OrderAsc { get; set; }
        int ItemPerPage { get; }
        int TotalPages { get; }
        int TotalRows{ get; }
    }
    public abstract class ListingViewModel<T> : IListingViewModel
    {
        public virtual int OnPage { get; set; }
        public virtual string FilterBy { get; set; }
        public virtual string FilterString { get; set; }
        public virtual string OrderBy { get; set; }
        public virtual bool OrderAsc { get; set; }
        public abstract int ItemPerPage { get; }
        public virtual List<T> Items { get; set; }
        public virtual Dictionary<string, List<string>> Claims { get; set; }
        //update these every time add record
        public int TotalPages { get; private set; }
        private int _totalRows;
        public int TotalRows
        {
            get
            {
                return _totalRows;
            }
            set
            {
                _totalRows = value;
                TotalPages = (_totalRows + ItemPerPage - 1) / ItemPerPage;
                if (TotalPages < 1)
                    TotalPages = 1;
            }
        }
    }
}
