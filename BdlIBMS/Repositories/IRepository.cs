using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdlIBMS.Repositories
{
    /// <summary>
    /// 仓库接口。提供对模型数据的增删改查。
    /// </summary>
    /// <typeparam name="K">数据模型的主键Key。</typeparam>
    /// <typeparam name="T">数据模型对象。</typeparam>
    public interface IRepository<K, T> where T : class
    {
        /// <summary>
        /// 获取全部数据列表。
        /// </summary>
        /// <returns></returns>
        DbSet<T> GetAll();

        /// <summary>
        /// 通过UUID获取指定数据项。
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(K uuid);

        /// <summary>
        /// 添加数据项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task AddAsync(T item);

        /// <summary>
        /// 更新数据项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task PutAsync(T item);

        /// <summary>
        /// 删除数据项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task DeleteAsync(T item);

        /// <summary>
        /// 判断指定UUID的数据项是否存在。
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        bool IsExist(K uuid);
    }
}
