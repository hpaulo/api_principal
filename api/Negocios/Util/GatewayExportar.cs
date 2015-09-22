using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;

namespace api.Negocios.Util
{
    public class GatewayExportar
    {
        /// <summary>
        /// Converte uma array de objetos em CSV e retorna o array de BYTES
        /// </summary>
        /// <param name="Collection">Coleção de dados a ser Convertida</param>
        /// <returns></returns>
        public static byte[] CSV(dynamic[] Collection)
        {
            return Bibliotecas.Converter.ListToCSV(Collection);
        }

    }

}