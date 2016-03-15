using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace api.Negocios.Util
{
    public class SimpleDataBaseQuery
    {
        public string[] select { get; set; }
        public Dictionary<string, string> join { get; set; }
        public string from { get; set; }
        public string[] where { get; set; }
        public string[] groupby { get; set; }
        public string[] orderby { get; set; }
        public int take { get; set; }
        public bool readUncommited { get; set; }
        public bool readDistinct { get; set; }

        public SimpleDataBaseQuery(SimpleDataBaseQuery simpleDataBaseQuery)
        {
            this.select = simpleDataBaseQuery.select;
            this.join = simpleDataBaseQuery.join != null ? simpleDataBaseQuery.join : new Dictionary<string, string>(); ;
            this.from = simpleDataBaseQuery.from;
            this.where = simpleDataBaseQuery.where;
            this.groupby = simpleDataBaseQuery.groupby;
            this.orderby = simpleDataBaseQuery.orderby;
            this.take = simpleDataBaseQuery.take;
            this.readUncommited = simpleDataBaseQuery.readUncommited;
            this.readDistinct = simpleDataBaseQuery.readDistinct;
        }

        public SimpleDataBaseQuery()
        {
            this.from = String.Empty;
        }

        public SimpleDataBaseQuery(string[] select, string from, Dictionary<string, string> join, string[] where, string[] groupby, string[] orderby, int take = 0, bool readUncommited = false, bool readDistinct = false, Dictionary<string, string> left_join = null)
        {
            this.select = select;
            this.join = join != null ? join : new Dictionary<string, string>(); ;
            this.from = from;
            this.where = where;
            this.groupby = groupby;
            this.orderby = orderby;
            this.take = take;
            this.readUncommited = readUncommited;
            this.readDistinct = readDistinct;
        }

        public void AddWhereClause(string clause)
        {
            if (clause == null) return;
            string[] aux = this.where;
            this.where = new string[aux.Length + 1];
            int k = 0;
            for (; k < aux.Length; k++)
                this.where[k] = aux[k];
            this.where[k] = clause;
        }

        public string ScriptForWhereClause()
        {
            string script = String.Empty;
            if (where != null && where.Length > 0)
            {
                foreach (string w in where)
                    script += "(" + w + ") AND ";
                // Remove o último AND
                script = script.Remove(script.Length - 4);
            }
            return script;
        }

        public string ScriptForOrderBy()
        {
            string script = String.Empty;
            if (orderby != null && orderby.Length > 0)
            {
                foreach (string o in orderby)
                    script += o + ", ";
                // Remove a última vírgula
                script = script.Remove(script.Length - 2);
            }
            return script;
        }

        public string Script()
        {
            string script = "SELECT ";
            if (readDistinct)
                script += " DISTINCT ";
            if (take > 0)
                script += "TOP(" + take + ") ";
            if (select != null)
            {
                foreach (string s in select)
                    script += s + ", ";
                // Remove a última vírgula
                script = script.Remove(script.Length - 2);
            }
            script += " FROM " + from;
            if (readUncommited) script += " (NOLOCK)";
            if(join != null)
            {
                script += " ";
                foreach (string j in join.Select(x => x.Key + " " + (readUncommited ? "(NOLOCK) " : "") + x.Value).ToArray())
	                script += j + " ";
            }
            if (where != null && where.Length > 0)
            {
                script += " WHERE " + ScriptForWhereClause();
                //foreach (string w in where)
                //    script += "(" + w + ") AND ";
                //// Remove o último AND
                //script = script.Remove(script.Length - 4);
            }
            if (groupby != null && groupby.Length > 0)
            {
                script += " GROUP BY ";
                foreach (string g in groupby)
                    script += g + ", ";
                // Remove a última vírgula
                script = script.Remove(script.Length - 2);
            }
            if (orderby != null && orderby.Length > 0)
            {
                script += " ORDER BY " + ScriptForOrderBy(); ;
                //foreach (string o in orderby)
                //    script += o + ", ";
                //// Remove a última vírgula
                //script = script.Remove(script.Length - 2);
            }
            return script; 
        }
    }

    public class DataBaseQueries
    {

        public static string GetDate(DateTime data)
        {
            if (data == null) return "";
            string ano = data.Year.ToString("0000");
            string mes = data.Month.ToString("00");
            string dia = data.Day.ToString("00");
            return ano + "-" + mes + "-" + dia;
        }

        public static int GetBoolean(bool valor)
        {
            return valor ? 1 : 0;
        }



        public static List<IDataRecord> SqlQuery(string script, SqlConnection outterConnection = null)
        {
            SqlConnection connection;

            if (outterConnection == null)
                connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString);
            else
                connection = outterConnection;

            // Abre a conexão
            if (!connection.State.Equals(ConnectionState.Open))
            {
                try
                {
                    connection.Open();
                }
                catch
                {
                    return null;
                }
            }

            SqlCommand command = new SqlCommand(script, connection);
            command.CommandTimeout = 60; // 1 minuto

            List<IDataRecord> queryResult = new List<IDataRecord>();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                try
                {
                    queryResult = reader.Cast<IDataRecord>().ToList<IDataRecord>();
                }
                catch { }
            }

            if (outterConnection == null)
            {
                // Fecha a conexão
                connection.Close();
            }

            return queryResult;
        }

    }
}