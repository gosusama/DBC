using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BTS.API.SERVICE.BuildQuery
{
    public class PagedObj : PagedObj<object>
    {
    }
    public class PagedObj<T>
    {
        private List<T> _data;

        public List<T> Data
        {
            get { return _data ?? (_data = new List<T>()); }
            set { _data = value; }
        }

        public bool TakeAll { get; set; }

        public int ItemsPerPage
        {
            get
            {
                return TakeAll
                    ? 0
                    : _itemsPerPage > 0 ? _itemsPerPage : DefaultPageSize;
            }
            set { _itemsPerPage = value; }
        }

        private int _itemsPerPage;

        public int CurrentPage
        {
            get { return _currentPage > 0 ? _currentPage : 1; }
            set { _currentPage = value; }
        }

        private int _currentPage;

        public int TotalPages
        {
            get
            {
                return ItemsPerPage > 0
                    ? (TotalItems / ItemsPerPage) + (TotalItems % ItemsPerPage == 0 ? 0 : 1)
                    : 1;
            }
        }

        public int TotalItems { get; set; }

        public int FromItem
        {
            get { return (CurrentPage - 1) * ItemsPerPage + 1; }
            internal set
            {
                CurrentPage = (value > 0 && ItemsPerPage > 0)
                    ? (value - 1) / ItemsPerPage + 1
                    : 1;
            }
        }

        public int ToItem
        {
            get
            {
                var result = TotalItems;
                if (ItemsPerPage > 0)
                {
                    result = CurrentPage * ItemsPerPage;
                    if (result > TotalItems)
                    {
                        result = TotalItems;
                    }
                }
                return result;
            }
        }


        public static int DefaultPageSize = 10;

        public IQueryBuilder ToQueryBuilder()
        {
            return new QueryBuilder
            {
                TakeAll = TakeAll,
                Take = ItemsPerPage,
                Skip = FromItem - 1
            };
        }

        public PagedObj<T> FromQueryBuilder(IQueryBuilder queryBuilder)
        {
            TakeAll = queryBuilder.TakeAll;
            ItemsPerPage = queryBuilder.Take;
            FromItem = queryBuilder.Skip + 1;
            return this;
        }
    }
    [Serializable]
    [DataContract]
    public class PagedObjTranf<T>
    {

        [DataMember]
        public List<T> Data
        {
            get;
            set;
        }
        [DataMember]
        public bool TakeAll { get; set; }
        [DataMember]
        public int ItemsPerPage
        {
            get;
            set;
        }


        [DataMember]
        public int CurrentPage
        {
            get;
            set;
        }


        [DataMember]
        public int TotalPages
        {
            get;
            set;
        }
        [DataMember]
        public int TotalItems { get; set; }
        [DataMember]
        public int FromItem
        {
            get;
            set;
        }
        [DataMember]
        public int ToItem
        {
            get;
            set;
        }

    }
}