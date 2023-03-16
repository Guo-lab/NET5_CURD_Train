using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBase.Utils
{
    [Serializable]
    public class Pager
    {
        public static int DefaultPageSize { get; set; } = 5;

        protected int itemCount = 0;

        protected int pageSize = DefaultPageSize;

        protected int pageIndex = 0;

        protected int goPageIndex = 0;

        public Pager() { }

        public Pager(int pageSize)
        {
            this.PageSize = pageSize;
        }
        public Pager(int pageSize,int pageNum)
        {
            this.PageSize = pageSize;
            this.PageNum = pageNum;
        }

        /**
         * if the currently required page is the last page
         */
        protected Boolean lastPage = false;

        public int ItemCount
        {
            get { return itemCount; }
            set
            {
                itemCount = value;
                //adjust the pageIndex to the actual number of the last page
                if (lastPage == true || pageIndex > PageCount - 1)
                {
                    pageIndex = PageCount - 1;
                    goPageIndex = pageIndex;
                }
                if (PageCount >= 0 && pageIndex < 0)
                {
                    pageIndex = 0;
                    goPageIndex = pageIndex;
                }
            }
        }

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        [JsonIgnore]
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }

        public Boolean IsLastPage
        {
            set { lastPage = value; }
        }

        public int PageNum
        {
            get { return pageIndex + 1; }
            set { pageIndex=value-1; }
        }

        public int GoPageNum
        {
            get { return goPageIndex + 1; }
            set { goPageIndex = value - 1; }
        }

        public int PageCount
        {
            get
            {
                if (itemCount==0) return 0;
                return pageSize==0?1:((int)Math.Ceiling((double)itemCount / pageSize));
            }
        }

        public int FromRowNum
        {
            get {return pageSize==0?1:(pageIndex * pageSize+1); }
        }

        public int ToRowNum
        {
            get { return pageSize==0?ItemCount:((pageIndex + 1) * pageSize); }
        }
        [JsonIgnore]
        public int FromRowIndex
        {
            get { return pageSize==0?0:(pageIndex * pageSize); }
        }

        [JsonIgnore]
        public int ToRowIndex
        {
            get { return (pageSize == 0 ? ItemCount : (pageIndex + 1)*pageSize) - 1; }
        }
    }

}
