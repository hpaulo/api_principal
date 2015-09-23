using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace api.Bibliotecas
{
    public static class Converter
    {
        public static byte[] ListToCSV<T>(T[] ArrayToSave)
        {
            MemoryStream stream = new MemoryStream();
                string line = String.Empty;
                string head = String.Empty;
                bool header = true;
                int l = 0;
                foreach (T item in ArrayToSave)
                {
                    foreach (var itemProp in item.GetType().GetProperties())
                    {
                        string chave = itemProp.Name;
                        object v = itemProp.GetValue(item, null);
                        string valor = v != null ? v.ToString() : String.Empty;

                        if(header)
                            head = head.Length > 0 ? head + ";" + chave : chave;
                        line = line.Length > 0 ? line + ";" + valor : valor;
                    }
                    if (header)
                        head = head + Environment.NewLine;
                    header = false;
                    line = line + Environment.NewLine;
                }

                StreamWriter file = new StreamWriter(stream, Encoding.UTF8, 512);
                file.Write(head.Replace("\r\n;", "\r\n"));
                file.Write(line.Replace("\r\n;", "\r\n"));
                file.Flush();
                file.Close();
                return stream.GetBuffer();
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            
            foreach (T item in data)
            {
                foreach (var itemProp in item.GetType().GetProperties())
                    table.Columns.Add(itemProp.Name, Nullable.GetUnderlyingType(itemProp.PropertyType) ?? itemProp.PropertyType);
                break;
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();

                foreach (var itemProp in item.GetType().GetProperties())
                    row[itemProp.Name] = itemProp.GetValue(item, null) ?? DBNull.Value;

                table.Rows.Add(row);
            }
            return table;
        }

    }
}
