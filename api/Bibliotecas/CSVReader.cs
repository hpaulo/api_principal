using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace api.Bibliotecas
{
    public class CSVFileira : List<string>
    {

        public string LinhaTexto { get; set; }

    }

    public class CSVReader: StreamReader

    {

        public CSVReader(Stream stream) : base(stream)

        {

        }



        public CSVReader(string arquivonome)
            : base(arquivonome)

        {

        }



        public bool LerLinha(CSVFileira fileira)

        {

            fileira.LinhaTexto = ReadLine();

            if (String.IsNullOrEmpty(fileira.LinhaTexto))

                return false;



            int pos = 0;

            int fileiras = 0;



            while (pos < fileira.LinhaTexto.Length)

            {

                string value;



                if (fileira.LinhaTexto[pos] == '"')

                {

                    pos++;



                    int inicio = pos;

                    while (pos < fileira.LinhaTexto.Length)

                    {

                        if (fileira.LinhaTexto[pos] == '"')

                        {

                            pos++;



                            if (pos >= fileira.LinhaTexto.Length || fileira.LinhaTexto[pos] != '"')

                            {

                                pos--;

                                break;

                            }

                        }

                        pos++;

                    }

                    value = fileira.LinhaTexto.Substring(inicio, pos - inicio);

                    value = value.Replace("\"\"", "\"");

                }

                else

                {

                    int inicio = pos;

                    while (pos < fileira.LinhaTexto.Length && fileira.LinhaTexto[pos] != ';')

                        pos++;

                    value = fileira.LinhaTexto.Substring(inicio, pos - inicio);

                }



                if (fileiras < fileira.Count)

                    fileira[fileiras] = value;

                else

                    fileira.Add(value);

                fileiras++;



                while (pos < fileira.LinhaTexto.Length && fileira.LinhaTexto[pos] != ';')

                    pos++;

                if (pos < fileira.LinhaTexto.Length)

                    pos++;

            }



            while (fileira.Count > fileiras)

                fileira.RemoveAt(fileiras);



            return (fileira.Count > 0);

        }
    }
}