using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.Para
{
    /// <summary>
    ///  功能：分页信息参数
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    public class PageInfoParam
    {
        #region   变量定义  

        private Dictionary<string, bool> m_sortInfo = null;

        #endregion

        #region  属性定义  

        /// <summary>
        ///     功能：页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        ///     功能：页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     功能：排序字段名称
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        ///     功能：排序方向(true:升序;false:降序)
        /// </summary>
        public bool SortDirection { get; set; }

        /// <summary>
        ///     功能：多字段排序信息
        /// </summary>
        public Dictionary<string, bool> SortInfo
        {
            get
            {
                if (null == m_sortInfo || m_sortInfo.Count == 0)
                {
                    m_sortInfo = new Dictionary<string, bool>();
                    m_sortInfo[SortField] = SortDirection;
                }

                return m_sortInfo;
            }

            set
            {
                m_sortInfo = value;
            }
        }

        #endregion

        #region  构造函数  

        /// <summary>
        ///     功能：创建一个默认的PageInfoParam
        /// </summary>
        public PageInfoParam()
        {
            PageIndex = 0;
            PageSize = 10;
            SortField = "FID";
            SortDirection = false;
        }

        /// <summary>
        ///     功能：创建一个 PageInfoParam
        /// </summary>
        /// <param name="pageIndex">当前页，从0开始</param>
        /// <param name="pageSize">分页信息</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortDirection">true, 升序。false，降序</param>
        public PageInfoParam(int pageIndex, int pageSize, string sortField, bool sortDirection)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            SortField = sortField;
            SortDirection = sortDirection;
        }

        #endregion

        #region   方法定义  

        /// <summary>
        ///     功能：设置排序信息
        /// </summary>
        /// <param name="sortField01">第一排序字段</param>
        /// <param name="sortDirection01">第一排序字段方向(true:升序;false:降序)</param>
        /// <param name="sortField02">第二排序字段</param>
        /// <param name="sortDirection02">第二排序字段方向(true:升序;false:降序)</param>
        public void SetSortInfo(string sortField01, bool sortDirection01, string sortField02, bool sortDirection02)
        {
            m_sortInfo = new Dictionary<string, bool>();
            m_sortInfo[sortField01] = sortDirection01;
            m_sortInfo[sortField02] = sortDirection02;
        }

        #endregion
    }

    /// <summary>
    ///     功能：分页信息参数
    /// </summary>
    [Serializable]
    public class PageInfoParam<T>
    {
        /// <summary>
        /// 获取一个修改过页大小的分页对象
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageInfoParam<T> ChangePageSize(int pageSize)
        {
            return new PageInfoParam<T>(PageIndex, pageSize, SortField, SortDirection);
        }

        #region   属性定义  

        /// <summary>
        ///     功能：页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        ///     功能：页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     功能：排序字段名称
        /// </summary>
        public T SortField { get; set; }

        /// <summary>
        ///     功能：排序方向(true:升序;false:降序)
        /// </summary>
        public bool SortDirection { get; set; }

        #endregion

        #region   构造函数  

        /// <summary>
        ///     功能：创建一个 PageInfoParam 类
        /// </summary>
        [DebuggerStepThrough]
        public PageInfoParam()
        {
            PageIndex = 0;
            PageSize = 10;
            SortDirection = false;
        }

        /// <summary>
        ///     功能：创建一个 PageInfoParam 类
        /// </summary>
        /// <param name="pageIndex">当前页，从0开始</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sortField">排序枚举</param>
        /// <param name="sortDirection">true, 升序; false, 降序</param>
        public PageInfoParam(int pageIndex, int pageSize, T sortField, bool sortDirection)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            SortField = sortField;
            SortDirection = sortDirection;
        }

        #endregion

        #region  方法定义  

        /// <summary>
        ///     功能：从泛型的 PageInfoParam 转换为非泛型的 PageInfoParam
        /// </summary>
        /// <returns></returns>
        public PageInfoParam ConvertToParam()
        {
            return new PageInfoParam(PageIndex, PageSize, SortField.ToString(), SortDirection);
        }

        #endregion
    }

}
