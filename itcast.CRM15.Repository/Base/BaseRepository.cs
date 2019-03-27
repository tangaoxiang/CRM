using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.CRM15.Repository
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq.Expressions;
    using IRepository;
    using System.Runtime.Remoting.Messaging;

    /// <summary>
    /// 统一父类，负责所有表的CRUD操作，分页，排序，连表
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        //1.0 实例化EF上下文容器对象类
        //缺点：如果一个控制器中有多个服务接口则会在当前请求线程中产生相应个数的EF容器对象
        //造成每次都要使用此业务逻辑对应的EF容器来进行数据库的访问，容易出错，并且性能会降低
        //BaseDBContext db = new BaseDBContext();

        //为了解决1.0步骤中的缺陷，则应该使用线程缓存来存储当前线程中的EF容器对象，保证此线程的EF容器对象唯一，同时，此线程销毁以后，EF容器跟着销毁
        BaseDBContext db
        {
            get
            {
                //1.0 先从线程缓存CallContext中根据key查找EF容器对象，如果没有则创建,同时保存到缓存中
                object obj = CallContext.GetData(typeof(BaseDBContext).FullName);
                if (obj == null)
                {
                    //1.0.1 例化EF的上下文容器对象
                    obj = new BaseDBContext();

                    //1.0.2 将EF的上下文容器对象存入线程缓存CallContext中
                    CallContext.SetData(typeof(BaseDBContext).FullName, obj);
                }

                //2.0 将当前的EF上下文对象返回
                return obj as BaseDBContext;
            }
        }

        DbSet<TEntity> _dbset;
        public BaseRepository()
        {
            _dbset = db.Set<TEntity>();
        }

        #region 2.0  查询相关方法

        /// <summary>
        /// 根据labmda表达式进行查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<TEntity> QueryWhere(Expression<Func<TEntity, bool>> where)
        {
            return _dbset.Where(where).ToList();
        }

        /// <summary>
        /// 连表操作
        /// </summary>
        /// <param name="where"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        public List<TEntity> QueryJoin(Expression<Func<TEntity, bool>> where, string[] tableNames)
        {
            if (tableNames == null || tableNames.Any() == false)
            {
                throw new Exception("连表操作的表名称至少要有一个");
            }

            DbQuery<TEntity> query = _dbset;

            foreach (var tablename in tableNames)
            {
                query = query.Include(tablename);
            }

            return query.Where(where).ToList();

            //return _dbset.Include("A").Include("B")
        }

        /// <summary>
        /// 按照条件查询出数据以后，根据外部指定的字段进行升序排列
        /// </summary>
        /// <typeparam name="TKey">表示从TEntity中获取的属性类型</typeparam>
        /// <param name="where">条件</param>
        /// <param name="order">排序lambda表达式</param>
        /// <returns></returns>
        public List<TEntity> QueryOrderBy<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> order)
        {
            return _dbset.Where(where).OrderBy(order).ToList();
        }

        /// <summary>
        /// 按照条件查询出数据以后，根据外部指定的字段进行倒序排列
        /// </summary>
        /// <typeparam name="TKey">表示从TEntity中获取的属性类型</typeparam>
        /// <param name="where">条件</param>
        /// <param name="order">排序lambda表达式</param>
        /// <returns></returns>
        public List<TEntity> QueryOrderByDescending<TKey>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> order)
        {
            return _dbset.Where(where).OrderByDescending(order).ToList();
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <typeparam name="TKey">要指定的排序属性名称 Tentity.property</typeparam>
        /// <param name="pageindex">分页页码</param>
        /// <param name="pagesize">页容量</param>
        /// <param name="rowcount">总行数</param>
        /// <param name="order">排序lambda表达式</param>
        /// <param name="where">查询条件lambda表达式</param>
        /// <returns></returns>
        public List<TEntity> QueryByPage<TKey>(int pageindex, int pagesize, out int rowcount, Expression<Func<TEntity, TKey>> order, Expression<Func<TEntity, bool>> where)
        {
            //1.0 计算当前分页要跳过的数据行数
            int skipCount = (pageindex - 1) * pagesize;

            //2.0 获取当前满足条件的所有数据总条数
            rowcount = _dbset.Count(where);

            //3.0 获取分页数据
            return _dbset.Where(where).OrderByDescending(order).Skip(skipCount).Take(pagesize).ToList();
        }

        #endregion

        #region 3.0  编辑相关方法

        public void Edit(TEntity model, string[] propertys)
        {
            if (model == null)
            {
                throw new Exception("实体不能为空");
            }

            if (propertys.Any() == false)
            {
                throw new Exception("要修改的属性至少要有一个");
            }

            //2.0 将model追击到EF容器
            System.Data.Entity.Infrastructure.DbEntityEntry entry = db.Entry(model);
            entry.State = System.Data.EntityState.Unchanged;
            foreach (var item in propertys)
            {
                entry.Property(item).IsModified = true;
            }

            //3.0 关闭EF对于实体的合法性验证参数
            db.Configuration.ValidateOnSaveEnabled = false;
        }

        #endregion

        #region 4.0  删除相关方法

        public void Delete(TEntity model, bool isadded)
        {
            //(!isadded)表示当前model没有追加到EF容器中
            if (!isadded)
            {
                _dbset.Attach(model);
            }
            _dbset.Remove(model);
        }

        #endregion

        #region 5.0  新增相关方法

        public void Add(TEntity model)
        {
            _dbset.Add(model);
        }

        #endregion

        #region 7.0 调用存储过程返回一个自己指定的类型TResult
        public List<TResult> RunProc<TResult>(string sql, params object[] pamrs)
        {
            return db.Database.SqlQuery<TResult>(sql, pamrs).ToList();
        }

        #endregion

        #region 6.0  统一提交

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
        #endregion

    }
}
