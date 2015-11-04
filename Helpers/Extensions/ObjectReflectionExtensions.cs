using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Extensions
{
    public static class ObjectReflectionExtensions
    {
        public static object GetPropertyValue(this object instance, string propertyName)
        {
            return instance.GetType().InvokeMember(propertyName, BindingFlags.GetProperty,
                null, instance, new object[] { });
        }

        public static void SetPropertyValue(this object instance, string propertyName, object propertySetValue)
        {
            instance.GetType().InvokeMember(propertyName, BindingFlags.SetProperty,
                null, instance, new object[] { Convert.ChangeType(propertySetValue, propertySetValue.GetType()) });
        }


        public static TEntity ShallowCopyEntity<TEntity>(TEntity source) where TEntity : class, new()
        {

            // Get properties from EF that are read/write and not marked witht he NotMappedAttribute
            var sourceProperties = typeof(TEntity)
                                    .GetProperties()
                                    .Where(p => p.CanRead && p.CanWrite &&
                                                p.GetCustomAttributes(typeof(NotMappedAttribute), true).Length == 0);
            var newObj = new TEntity();
            foreach (var property in sourceProperties)
            {
                property.SetValue(newObj, property.GetValue(source, null), null);
            }
            return newObj;
        }



        public static object RunStaticMethod(this Type T, string strMethod,
 object[] aobjParams)
        {
            BindingFlags eFlags =
             BindingFlags.Static | BindingFlags.Public |
             BindingFlags.NonPublic;
            return RunMethod(T, strMethod,
             null, aobjParams, eFlags);
        }

        public static object RunInstanceMethod(this object objInstance, Type T, string strMethod,
 object[] aobjParams)
        {
            BindingFlags eFlags = BindingFlags.Instance | BindingFlags.Public |
             BindingFlags.NonPublic;
            return RunMethod(T, strMethod,
             objInstance, aobjParams, eFlags);
        }

        private static object RunMethod(Type T, string
 strMethod, object objInstance, object[] aobjParams, BindingFlags eFlags)
        {
            MethodInfo m;
            try
            {
                m = T.GetMethod(strMethod, eFlags);
                if (m == null)
                {
                    throw new ArgumentException("There is no method '" +
                     strMethod + "' for type '" + T.ToString() + "'.");
                }

                object objRet = m.Invoke(objInstance, aobjParams);
                return objRet;
            }
            catch
            {
                throw;
            }
        }
    }
}
