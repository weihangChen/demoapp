using Helpers.Annotations;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Reflection;
using DataAccess.DAL;
using Domain;
using Domain.Model;

namespace DataAccess.DAL
{
    [RegisterServiceAttribute()]
    public class UnitOfWork : IDisposable
    {
        private DbContext context;
        private ILog Logger;

        public UnitOfWork(DbContext context)
        {
            this.context = context;
            this.Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void setContext(DbContext context)
        {
            this.context = context;
        }


        private GenericRepository<Person> personRepository;


        public virtual GenericRepository<Person> PersonRepository
        {
            get
            {
                if (this.personRepository == null)
                {
                    this.personRepository = new GenericRepository<Person>(context);
                }
                return personRepository;
            }
        }

        public void Save()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Error("fail to save", e);
            }

        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
