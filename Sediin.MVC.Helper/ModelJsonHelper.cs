using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sediin.MVC.HtmlHelpers
{
    public class ModelJsonHelper
    {
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
       
        public static DataTable ModelToDataTable<T>(IEnumerable<T> models)
        {
            try
            {
                // creating a data table instance and typed it as our incoming model   
                // as I make it generic, if you want, you can make it the model typed you want.  
                DataTable dataTable = new DataTable(typeof(T).Name);

                //Get all the properties of that model  
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Loop through all the properties              
                // Adding Column name to our datatable  
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names    
                    dataTable.Columns.Add(prop.Name);
                }
                // Adding Row and its value to our dataTable  
                foreach (T item in models)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows    
                        values[i] = Props[i].GetValue(item, null);
                    }
                    // Finally add value to datatable    
                    dataTable.Rows.Add(values);
                }
                return dataTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string DataTableToJson(DataTable dt)
        {
            try
            {
                string Json;

                if (dt != null)
                {
                    Json = JsonConvert.SerializeObject(dt, Formatting.Indented);
                    return Json;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T DataTableToModel<T>(DataTable dt)
        {
            try
            {
                string Json;

                if (dt != null)
                {
                    Json = JsonConvert.SerializeObject(dt, Formatting.Indented);

                    return JsonToModel<T>(Json);

                }

                return default;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static T JsonToModel<T>(string json)// where T : new()
        {
            try
            {
                if (json != null)
                {
                    var settings = new JsonSerializerSettings();
                    //settings.DateFormatString = "YYYY-MM-DD hh24:mm:ss";
                    //JsonConverter[] converters = { new Db2TimestampConverter()};

                    var jsonSerializerSettings = new JsonSerializerSettings();
                    jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                    jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    //jsonSerializerSettings.Error = HandleDeserializationError;
                    //jsonSerializerSettings.Converters = converters;

                    return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
                }

                return default;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        private static void HandleDeserializationError(object sender, ErrorEventArgs e)
        {
            var currentError = e.ErrorContext.Error.Message;
            e.ErrorContext.Handled = false;
        }

        public static string ModelToJson(object model)
        {
            try
            {
                if (model != null)
                {
                    return JsonConvert.SerializeObject(model);
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static T ModelToModel<T>(object model) where T : new()
        {
            try
            {
                if (model != null)
                {
                    var _model = JsonConvert.SerializeObject(model);
                    return JsonToModel<T>(_model);
                }

                return default;
            }
            catch
            {
                return default;
            }
        }  
    }
}
