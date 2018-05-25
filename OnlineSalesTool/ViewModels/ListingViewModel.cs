using OnlineSalesTool.ApiParameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.ViewModels
{
    /// <summary>
    /// Generic listing model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ListingViewModel<T>
    {
        public int OnPage { get; protected set; }
        public string FilterBy { get; protected set; }
        public string FilterString { get; protected set; }
        public string OrderBy { get; protected set; }
        public bool OrderAsc { get; protected set; }
        //public int ItemPerPage { get; protected set; } = 10;
        public IEnumerable<T> Items { get; protected set; }
        //update these every time add record
        public int ItemPerPage { get; protected set; }
        public int TotalPages { get; protected set; }
        private int _totalRows;
        public int TotalRows
        {
            get
            {
                return _totalRows;
            }
            set
            {
                _totalRows = value < 1? 1 : value;
                TotalPages = (_totalRows + ItemPerPage - 1) / ItemPerPage;
                if (TotalPages < 1)
                    TotalPages = 1;
            }
        }
        //Interesting, why would C# even allow abstract class ctor to be public :/
        protected ListingViewModel(ListingParams param)
        {
            if (param == null) throw new ArgumentNullException();
            FilterBy = string.IsNullOrEmpty(param.Filter) ? string.Empty : param.Filter;
            FilterString = param.Contain;
            OnPage = param.Page;
            OrderAsc = param.Asc;
            OrderBy = param.OrderBy;
        }
        /// <summary>
        /// Set items for this VM
        /// </summary>
        /// <param name="item">Item for this VM</param>
        /// <param name="total">Paging purposes</param>
        public void SetItems(IEnumerable<T> items, int itemPerPage, int total)
        {
            Items = items ?? throw new ArgumentNullException();
            ItemPerPage = itemPerPage;
            TotalRows = total;
        }
    }
}
