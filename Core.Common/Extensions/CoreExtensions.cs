using Core.Common.Core;
using Core.Common.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    public static class CoreExtensions
    {
        public static void Merge<T>(this ObservableCollection<T> source, ObservableCollection<T> collection )
        {
            Merge(source, collection, false);
        }
        public static void Merge<T>(this ObservableCollection<T> source, ObservableCollection<T> collection, bool ignoreDuplicates)
        {
            if(collection!=null)
            {
                foreach (T item in collection)
                {
                    bool addItem = true;
                    if (ignoreDuplicates)
                        addItem = !source.Contains(item);
                    if (addItem)
                        source.Add(item);
                }
            }
        }

        public static bool IsNavigable(this PropertyInfo property)
        {
            bool navigable = true;
            object[] attributes = property.GetCustomAttributes(typeof(NotNavigableAttribute), true);//tra ve kieu cua mot lop
            if (attributes.Length > 0)
                navigable = false;
            return navigable;
        }
        
        public static bool IsNavigable(this ObjectBase obj, string propertyName)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);//tra ve kieu cua mot bien khi chay(runtime)
            return propertyInfo.IsNavigable();
        }

        public static bool Isnavigable<T>(this ObjectBase obj, Expression<Func<T>> propertyExpression)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo.IsNavigable();
        }
        static Dictionary<string, bool> BrowsableProperties = new Dictionary<string, bool>();
        static Dictionary<string, PropertyInfo[]> BrowsablePropertiesInfos = new Dictionary<string, PropertyInfo[]>();
        public static bool IsBrowsable(this object obj, PropertyInfo property)
        {
            string key = string.Format("{0},{1}", obj.GetType(), property.Name);
            if(!BrowsableProperties.ContainsKey(key))
            {
                bool browsable = property.IsNavigable();
                BrowsableProperties.Add(key, browsable);
            }
            return BrowsableProperties[key];
        }
        public static PropertyInfo[] GetBrowsableProperties(this object obj)
        {
            string key = obj.GetType().ToString();

            if(!BrowsablePropertiesInfos.ContainsKey(key))
            {
                List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
                PropertyInfo[] properties = obj.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if((property.PropertyType.IsSubclassOf(typeof(ObjectBase))
                        ||property.PropertyType.GetInterface("IList")!=null))
                    {
                        //chi them vao danh sach cua property khong danh dau [NotNavigable]
                        if (IsBrowsable(obj, property))
                            propertyInfoList.Add(property);
                    }
                }
                BrowsablePropertiesInfos.Add(key, propertyInfoList.ToArray());
            }

            return BrowsablePropertiesInfos[key];
        }
    }
}
