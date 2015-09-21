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
        public static byte[] ListToCSV<T>(T[]ArrayToSave)
        {
            MemoryStream stream = new MemoryStream();
            using (StreamWriter file = new StreamWriter(stream))
            {
                    string line = String.Empty;
                    string head = String.Empty;
                    bool header = true;
                    int l = 0;
                    foreach (T item in ArrayToSave)
                    {


                        var obj = item.ToString();
                        var prop = obj.Replace("{", "").Replace("}", "").Split(',');
                        
                        foreach (var itemProp in prop)
                        {


                            var collection = itemProp.Split('=');
                            int i = 0;
                            foreach (var itemCollection in collection)
                            {
                                if (i == 0)
                                {
                                    if (header)
                                        head = (head.Length == 0) ? itemCollection : head + ";" + itemCollection;
                                }
                                else
                                {
                                    line = (l == 0 ) ? itemCollection : line + ";" + itemCollection;
                                    l++;
                                }
                                i++;
                            }
                        
                        }
                        
                        
                        if (header)
                            head = head + Environment.NewLine;
                        header = false;

                        line = line + Environment.NewLine;
                }
                file.Write(head.Replace(" ",""));
                file.Write(line.Replace("\r\n;", "\r\n").Replace(" ", ""));
                //file.Write(Environment.NewLine);

                return stream.GetBuffer();
            }
        }
    }
}
