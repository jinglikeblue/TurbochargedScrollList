using System.Collections.Generic;

namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 滚动列表接口
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    interface IScrollList<TData>
    {
        /// <summary>
        /// 添加一组数据
        /// </summary>
        /// <param name="collection"></param>
        void AddRange(IEnumerable<TData> collection);

        /// <summary>
        /// 添加一个数据
        /// </summary>
        /// <param name="data"></param>
        void Add(TData data);

        /// <summary>
        /// 在指定位置，插入一个数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        void Insert(int index, TData data);

        /// <summary>
        /// 移除列表找到的第一个和data数据相同的项
        /// </summary>
        /// <param name="data"></param>
        /// <returns>有数据被移除，返回true</returns>
        bool Remove(TData data);

        /// <summary>
        /// 移除指定索引位置的数据
        /// </summary>
        /// <param name="index"></param>
        void RemoveAt(int index);

        /// <summary>
        /// 清空列表，实际上是将数据清除，并将对象放入对象池
        /// </summary>
        void Clear();
    }
}
