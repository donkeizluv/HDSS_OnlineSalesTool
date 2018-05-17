﻿using Microsoft.AspNetCore.Http;

namespace OnlineSalesTool.ApiParameter
{
    public class ListingParams
    {
        public int Id { get; set; }
        public int Page { get; set; }
        public string Filter { get; set; }
        public string Contain { get; set; }
        public string OrderBy { get; set; }
        public bool Asc { get; set; }

        public class ParamBuilder
        {
            private int _id;
            private int _page;
            private string _type;
            private string _contain;
            private string _orderBy;
            private bool _asc;

            public ParamBuilder SetId(int value)
            {
                _id = value;
                return this;
            }
            public ParamBuilder SetPage(int value)
            {
                _page = value < 1 ? 1 : value;
                return this;
            }
            public ParamBuilder SetType(string value)
            {
                _type = value;
                return this;
            }
            public ParamBuilder SetContain(string value)
            {
                _contain = value;
                return this;
            }
            public ParamBuilder SetOrderBy(string value)
            {
                _orderBy = value;
                return this;
            }
            public ParamBuilder SetAsc(bool value)
            {
                _asc = value;
                return this;
            }
            public ListingParams Build()
            {
                return new ListingParams()
                {
                    Id = _id,
                    Page = _page,
                    Filter = _type,
                    Contain = _contain,
                    OrderBy = _orderBy,
                    Asc = _asc,
                };
            }
        }
    }
}
