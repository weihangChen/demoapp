using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAL
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal DbContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }




        public bool IsEmpty()
        {
            return dbSet.Count() > 0 ? false : true;
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }


        public List<TEntity> GetAsList(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string orderByWithPropertyString = "",
            string includeProperties = "")
        {
            var query = GetAsQuery(filter, orderBy, includeProperties: includeProperties);
            if (dbSet != null && query != null)
                return query.ToList();
            return new List<TEntity>();
        }


        public List<T> GetAsListViewModel<T>(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string orderByWithPropertyString = "",
           string includeProperties = "")
        {
            var query = GetAsQuery(filter, orderBy, includeProperties: includeProperties);
            if (dbSet != null && query != null)
            {
                var dbResultSet = query.ToList();
                //convert to view level entity 
                var viewModelResultSet = dbResultSet.Select(x => Mapper.Map<TEntity, T>(x)).ToList();
                return viewModelResultSet;
            }
            return new List<T>();
        }
        public IQueryable<TEntity> GetAsQuery(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          string orderByWithPropertyString = "",
          string includeProperties = "")
        {
            if (dbSet == null)
                return null;
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
                return orderBy(query);
            }


            return query;

        }



        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            if (entityToDelete != null)
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




        public virtual IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters).ToList();
        }

        public virtual bool IsAttachedTo(ObjectContext context, object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            ObjectStateEntry entry;

            if (context.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
            {
                return (entry.State != EntityState.Detached);
            }
            return false;
        }

        public virtual void RemoveDuplicationInContext(object entity)
        {
            bool isPresent = IsAttachedTo(((IObjectContextAdapter)context).ObjectContext, entity);
            if (isPresent)
                ((IObjectContextAdapter)context).ObjectContext.Detach(entity);
        }
    }
}
