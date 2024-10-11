using Dapper;
using EBLIG.DOM.Data;
using EBLIG.DOM.Entitys;
using LambdaSqlBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace EBLIG.DOM.DAL
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal EBLIGDbContext context;
        internal DbSet<TEntity> dbSet;

        internal string connectionString;

        public GenericRepository(EBLIGDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
            this.connectionString = context.Database.Connection.ConnectionString;
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            try
            {
                IQueryable<TEntity> query = dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (orderBy != null)
                {
                    return orderBy(query).ToList();
                }
                else
                {
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<TEntity> Get(ref int totalRows, SqlLam<TEntity> filter = null, string orderBy = null, int? page = 1, int? pageSize = 10)
        {
            return Get(ref totalRows, filter, orderBy, null, page, pageSize);
        }

        public IEnumerable<TEntity> Get(ref int totalRows, SqlLam<TEntity> filter = null, string orderBy = null, List<(string, string, object)> whereParamters = null, int? page = 1, int? pageSize = 10)
        {
            try
            {
                if (page.GetValueOrDefault() <= 0)
                {
                    page = 1;
                }

                int startRow = 1 + ((page.GetValueOrDefault() - 1) * pageSize.GetValueOrDefault());
                int endRow = page.GetValueOrDefault() * pageSize.GetValueOrDefault();

                var orderColumn = string.IsNullOrWhiteSpace(orderBy) ?
                    typeof(TEntity).GetProperties().FirstOrDefault().Name + " asc" : orderBy;

                var key = typeof(TEntity).GetProperties().Where(c => c.CustomAttributes.Any(x => x.AttributeType == typeof(KeyAttribute))).FirstOrDefault();

                //var objectContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext;
                //ObjectSet<TEntity> set = objectContext.CreateObjectSet<TEntity>();
                ////Act
                //IEnumerable<string> keyNames = set.EntitySet.ElementType.KeyMembers.Select(k => k.Name);
                //Console.WriteLine("{0}", string.Join(",", keyNames.ToArray()));

                var query = new SqlLam<TEntity>();

                var tablename = "[dbo].[" + query.SqlBuilder.TableNames.FirstOrDefault() + "]";

                var _where = "";

                if (filter != null)
                {
                    query = filter;// new SqlLam<TEntity>(filter);

                    if (query.SqlBuilder.WhereConditions.Count() > 0)
                    {
                        _where = $" {string.Join(" ", query.SqlBuilder.WhereConditions)}".Trim();
                    }
                }

                if (whereParamters?.Count() > 0)
                {
                    for (int i = 0; i < whereParamters.Count; i++)
                    {
                        if (_where != "")
                        {
                            _where += $" {whereParamters[i].Item2} {tablename}.[{whereParamters[i].Item1}] = '{whereParamters[i].Item3}'";
                        }
                        else
                        {
                            _where += $" {tablename}.[{whereParamters[i].Item1}] = '{whereParamters[i].Item3}'";
                        }

                    }
                }

                if (_where != "")
                {
                    _where = $" where {_where}";
                }

                var _baseSql = $"Select {key.Name} from (Select {tablename}.*,ROW_NUMBER() OVER(ORDER BY {HttpUtility.UrlDecode(orderColumn)}) AS Row From {tablename} {_where}) as tb where row >= {startRow} and row <= {endRow}";
                var _countSql = $"Select Count({key.Name}) From {tablename} {_where}";

                var count = Task.Run(() => GetData<int>(_countSql, query.QueryParameters).FirstOrDefault());
                var results = Task.Run(() => GetData<int>(_baseSql, query.QueryParameters));

                //File.WriteAllText(@"C:\temp\base.txt", _baseSql);
                ////File.WriteAllText(@"C:\temp\count.txt", _countSql);
                //File.WriteAllText(@"C:\temp\query.txt", query.QueryParameters.FirstOrDefault().ToString());

                List<Task> _tasklist = new List<Task>
            {
                count,
                results
            };

                Task.WhenAll(_tasklist);

                totalRows = count.Result;

                List<TEntity> a = new List<TEntity>();

                foreach (var item in results.Result)
                {
                    a.Add(dbSet.Find(item));
                }

                return a;// results.Result;

            }
            catch (Exception ex)
            {
                File.WriteAllText(@"C:\temp\error.txt", ex.Message + " " + ex.InnerException?.InnerException?.Message);

                throw;
            }
        }

        IEnumerable<T> GetData<T>(string sql, IDictionary<string, object> queryParameters)// where T : new()
        {
            using (var connection = new SqlConnection(context.Database.Connection.ConnectionString.ToString()))
            {
                return (IEnumerable<T>)connection.Query<T>(sql, queryParameters);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await Task.FromResult(orderBy(query).ToList());
            }
            else
            {
                return await Task.FromResult(query.ToList());
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void InsertOrUpdate(TEntity entity)
        {
            dbSet.AddOrUpdate(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        //public object Get(ref int totalRows, SqlLam<Logs> sqlLam, object ordine, int? page, object pageSize)
        //{
        //    throw new NotImplementedException();
        //}

        //public object Get(ref int totalRows, SqlLam<Logs> sqlLam, object ordine, int? page, int? pageSize)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
