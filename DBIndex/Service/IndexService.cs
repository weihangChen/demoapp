using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseIndex.Entities;

namespace DatabaseIndex.Service
{
    public class IndexService<TEntity>
    {
        private IEnumerable<TEntity> _tableData;
        private Dictionary<Type, IndexHandler<IComparable, TEntity>> _IndexeHandlers;

        public IndexService(IEnumerable<TEntity> tableData)
        {
            this._tableData = tableData;
            this._IndexeHandlers = new Dictionary<Type, IndexHandler<IComparable, TEntity>>();
        }


        public void CreateIndex(string key, Func<TEntity, IComparable> propertyFunc)
        {
            var item = propertyFunc(_tableData.First());
            Type itemType = item.GetType();

            if (!_IndexeHandlers.ContainsKey(itemType))
            {
                //_IndexeHandlers.Add(String, new IndexHandler<String, Person>(Persons));
                _IndexeHandlers.Add(itemType, new IndexHandler<IComparable, TEntity>(_tableData));
                _IndexeHandlers[itemType].CreateIndex(key, propertyFunc);
            }

            else
            {
                IndexHandler<IComparable, TEntity> handler = _IndexeHandlers[itemType];
                handler.CreateIndex(key, propertyFunc);
            }
        }


        public bool DropIndex(string indexkey)
        {
            var handler = _IndexeHandlers.Values.FirstOrDefault(x => x.ContainsKey(indexkey));
            return handler != null ? handler.DropIndex(indexkey) : false;

        }

        /// <summary>
        /// Retrieve data range using indexes
        /// </summary>
        /// <param name="indexkey">index key</param>
        /// <param name="from">from value</param>
        /// <param name="to">to value</param>
        /// <returns>every row found in the values range</returns>
        public IEnumerable<TEntity> RetrieveData(string indexkey, IComparable from, IComparable to) //Match query on a single field, several values
        {
            var handler = _IndexeHandlers.Values.FirstOrDefault(x => x.ContainsKey(indexkey));
            return handler != null ? handler.Retrieve(indexkey, from, to) : new List<TEntity>();
        }

        /// <summary>
        /// Retrieve data using indexes
        /// </summary>
        /// <param name="indexkey">index key</param>
        /// <param name="value">value to find</param>
        /// <returns>every row contains the value</returns>
        public IEnumerable<TEntity> RetrieveData(string indexkey, IComparable value) //Match query on a single field, one value
        {
            return RetrieveData(indexkey, value, value);
        }

        public IEnumerable<TEntity> RetrieveData(string firstField, string secondField, IComparable value, SearchOperation operation)
        {
            switch (operation)
            {
                case SearchOperation.Or:
                    return OrMatchOnTwoFields(firstField, secondField, value);
                case SearchOperation.And:

                default:
                    return OrMatchOnTwoFields(firstField, secondField, value);
            }
        }

        private IEnumerable<TEntity> OrMatchOnTwoFields(string firstField, string secondField, IComparable value) //An OR of a match query on 2 different fields, one value
        {
            List<TEntity> result = RetrieveData(firstField, value).ToList();
            result.InsertRange(0, RetrieveData(secondField, value));
            result = result.Distinct().ToList(); //remove duplicates
            return result;
        }




    }
}
