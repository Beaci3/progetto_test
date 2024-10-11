using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using System.Runtime.Remoting.Contexts;

namespace Sediin.MVC.HtmlHelpers
{
    public class Reflection
    {
        //public static DataTable Tabulate(string json)
        //{
        //    var jsonLinq = JObject.Parse(json);

        //    // Find the first array using Linq
        //    var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
        //    var trgArray = new JArray();
        //    foreach (JObject row in srcArray.Children<JObject>())
        //    {
        //        var cleanRow = new JObject();
        //        foreach (JProperty column in row.Properties())
        //        {
        //            // Only include JValue types
        //            if (column.Value is JValue)
        //            {
        //                cleanRow.Add(column.Name, column.Value);
        //            }
        //        }

        //        trgArray.Add(cleanRow);
        //    }

        //    return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
        //}

        //public static DataTable JsonToDataTable(string json)
        //{
        //    var jsonLinq = JObject.Parse(json);

        //    // Find the first array using Linq
        //    var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
        //    var trgArray = new JArray();
        //    foreach (JObject row in srcArray.Children<JObject>())
        //    {
        //        var cleanRow = new JObject();
        //        foreach (JProperty column in row.Properties())
        //        {
        //            // Only include JValue types
        //            if (column.Value is JValue)
        //            {
        //                cleanRow.Add(column.Name, column.Value);
        //            }
        //        }

        //        trgArray.Add(cleanRow);
        //    }

        //    return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());

        //}


        public static DataTable ListToDataTable<T>(IList<T> data)
        {
            //return (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data), (typeof(DataTable)));

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable("Grid");

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                Type itemType = prop.PropertyType;

                if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    continue;
                }

                if (Nullable.GetUnderlyingType(itemType) != null)
                {
                    itemType = itemType?.GenericTypeArguments[0];
                }

                //if (prop.PropertyType.IsClass && !prop.PropertyType.FullName.StartsWith("System."))
                //{
                //    // do something with property
                //}

                table.Columns.Add(prop.Name, itemType);
            }

            object[] values = new object[table.Columns.Count];

            DateTime? isMinDate(object val)
            {
                try
                {
                    DateTime.TryParse(val.ToString(), out DateTime _d);

                    if (_d == DateTime.MinValue || _d.Date.Year < 1900)
                    {
                        return null;
                    }

                    return (DateTime)val;
                }
                catch
                {
                    return null;
                }
            };

            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    var _p = props[i].GetValue(item);

                    if (_p != null)
                    {
                        if (props[i].PropertyType == typeof(DateTime) || props[i].PropertyType == typeof(DateTime?))
                        {
                            _p = isMinDate(_p);
                        }
                    }

                    values[i] = _p;// props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static decimal ToDecimal(object val)
        {
            if (val == null)
            {
                return 0;
            }

            decimal.TryParse(val.ToString(), out decimal result);

            return result;
        }

        public static void SetValue(object obj, string propertyName, object propertyValue)
        {
            try
            {
                //find out the type 
                Type type = obj.GetType();

                //get the property information based on the type
                PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                    return;

                //find the property type
                Type propertyType = propertyInfo.PropertyType;

                //Convert.ChangeType does not handle conversion to nullable types
                //if the property type is nullable, we need to get the underlying type of the property
                var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

                //Returns an System.Object with the specified System.Type and whose value is
                //equivalent to the specified object.
                propertyValue = propertyValue == null || string.IsNullOrWhiteSpace(propertyValue?.ToString())
                    ? null : Convert.ChangeType(propertyValue, targetType);

                //Set the value of the property
                propertyInfo.SetValue(obj, propertyValue, null);

            }
            catch (Exception ex)
            {

            }
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static PropertyInfo GetProperty<T>(string propertyName)
        {
            try
            {
                return typeof(T).GetProperty(propertyName);

            }
            catch (Exception)
            {
                return default;
            }
        }

        public static T CreateModel<T>(object objNew, object objOld) where T : new()
        {
            var _model = new T();

            //find out the type 
            Type type = objNew.GetType();

            foreach (var item in type.GetProperties())
            {
                var _x = item.GetValue(objNew);

                if (_x == null)
                {
                    SetValue(objNew, item.Name, item.GetValue(objOld));
                }

                SetValue(_model, item.Name, item.GetValue(objNew));
            }

            return _model;
        }

        public static T CreateModel<T>(object obj) where T : new()
        {
            var _model = new T();

            if (obj == null)
            {
                return _model;
            }

            var type = obj.GetType();

            foreach (var item in type.GetProperties())// typeof(T).GetProperties())
            {
                SetValue(_model, item.Name, item.GetValue(obj));// type.GetProperty(item.Name).GetValue(obj));
            }

            return _model;
        }

        public static List<T1> CreateModel_List<T, T1>(IEnumerable<T> model) where T1 : new()
        {
            List<T1> _obj = new List<T1>();

            if (model != null)
            {
                foreach (object item in model)
                {
                    _obj.Add(Reflection.CreateModel<T1>(item));
                }
            }

            return _obj;
        }

        public static T MergeModels<T>(object[] models) where T : new()
        {
            try
            {
                var _model = new T();

                foreach (var item in models)
                {
                    foreach (var p in item.GetType().GetProperties())
                    {
                        //var _val = _model.GetType().GetProperty(p.Name);

                        //if (_val.GetValue(_model) != null)
                        //    continue;

                        Reflection.SetValue(_model, p.Name, p.GetValue(item));
                    }
                }

                return _model;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public static float? GetFloat(object val)
        {
            try
            {
                return (float)Convert.ToDecimal(val);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static decimal? GetDecimal(object val)
        {
            try
            {
                return Convert.ToDecimal(val);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DateTime? GetDateTime(object val)
        {
            try
            {
                return Convert.ToDateTime(val);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int? GetInt(object val)
        {
            try
            {
                return Convert.ToInt32(val);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
