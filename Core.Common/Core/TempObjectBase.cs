using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Core.Common.Contracts;
using FluentValidation;
using FluentValidation.Results;
using Core.Common.Utils;
using Core.Common.Extensions;
using System.Collections;

namespace Core.Common.Core
{
    public class TempObjectBase : INotifyPropertyChanged
    {
        private event PropertyChangedEventHandler _PropertyChanged;
        List<PropertyChangedEventHandler> _PropertyChangedSubscribers = new List<PropertyChangedEventHandler>();
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if(!_PropertyChangedSubscribers.Contains(value))
                {
                    _PropertyChanged += value;
                    _PropertyChangedSubscribers.Add(value);
                }
                
            }
            remove
            {          
                _PropertyChanged -= value;
                _PropertyChangedSubscribers.Remove(value);
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName, true);
        }
        protected virtual void OnPropertyChanged(string propertyName, bool makeDirty)
        {
            if (propertyName != null)
                _PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (makeDirty)
                _IsDirty = true;
        }

        public virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            OnPropertyChanged(propertyName);
        }

        bool _IsDirty;

        public bool IsDirty
        {
            get{return _IsDirty;}
        }
        protected void WalkObjectGraph(Func<ObjectBase, bool> snippetForObject,
                                       Action<IList> snippetForCollection,
                                       params string[] exemptProperties)
        {
            List<ObjectBase> visited = new List<ObjectBase>();
            Action<ObjectBase> walk = null;

            List<string> exemptions = new List<string>();
            if (exemptProperties != null)
                exemptions = exemptProperties.ToList();

            walk = (o) =>
            {
                if (o != null && !visited.Contains(o))
                {
                    visited.Add(o);

                    bool exitWalk = snippetForObject.Invoke(o);

                    if (!exitWalk)
                    {
                        PropertyInfo[] properties = o.GetBrowsableProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            if (!exemptions.Contains(property.Name))
                            {
                                if (property.PropertyType.IsSubclassOf(typeof(ObjectBase)))
                                {
                                    ObjectBase obj = (ObjectBase)(property.GetValue(o, null));
                                    walk(obj);
                                }
                                else
                                {
                                    IList coll = property.GetValue(o, null) as IList;
                                    if (coll != null)
                                    {
                                        snippetForCollection.Invoke(coll);

                                        foreach (object item in coll)
                                        {
                                            if (item is ObjectBase)
                                                walk((ObjectBase)item);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

    }
    public class MyClass: TempObjectBase
    {

    }
    public class TestClient
    {
        public TestClient()
        {
            MyClass obj = new MyClass();
            obj.PropertyChanged -= obj_PropertyChanged;
            obj.PropertyChanged += obj_PropertyChanged;
        }

        private void obj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }
    }
}
