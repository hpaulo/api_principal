using api.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace api.Negocios.SignalR
{
    public static class GatewayMonitorCargas
    {
        public static List<Models.LogExecution> list = null;
        private static IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ServerHub>();

        public static void GetData(SqlNotificationInfo Info = SqlNotificationInfo.Unknown)
        {

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["painel_taxservices_dbContext"].ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(
                            //_db.LogExecutions.Select(e => e).AsQueryable().ToString()
                            @"
                            SELECT
                            pos.LogExecution.id,
                            pos.LogExecution.idOperadora,
                            pos.LogExecution.dtaExecution,
                            pos.LogExecution.dtaFiltroTransacoes,
                            pos.LogExecution.qtdTransacoes,
                            pos.LogExecution.vlTotalTransacoes,
                            pos.LogExecution.statusExecution,
                            pos.LogExecution.idLoginOperadora,
                            pos.LogExecution.dtaExecucaoInicio,
                            pos.LogExecution.dtaExecucaoFim,
                            pos.LogExecution.dtaFiltroTransacoesFinal,
                            pos.LogExecution.dtaExecucaoProxima

                            FROM
                            pos.LogExecution
                            WHERE pos.LogExecution.dtaFiltroTransacoes BETWEEN '2015-08-21 00:00:00' AND '2015-08-21 23:59:59'
                            "

                            , connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    List<Models.LogExecution> teste = new List<Models.LogExecution>();
                    DateTime dtOut = DateTime.Now;

                    using (var reader = command.ExecuteReader())
                    {

                        List<dynamic> retorno = reader.Cast<dynamic>()
                                   .Select(x => x).ToList<dynamic>();

                        if (list != null)
                        {
                            if (Info.Equals(SqlNotificationInfo.Delete))
                            {
                                List<Models.LogExecution> r = retorno.Select(e => new Models.LogExecution { id = e["id"], statusExecution = e["statusExecution"], dtaExecucaoFim = e["dtaExecucaoFim"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtaExecucaoFim"] }).ToList<Models.LogExecution>();
                                teste = list.Where(e => !r.Any(l => l.id == e.id && l.statusExecution.Equals(e.statusExecution))).ToList<Models.LogExecution>();
                            }
                            else
                                teste = retorno.Select(e => new Models.LogExecution { id = e["id"], statusExecution = e["statusExecution"], dtaExecucaoFim = e["dtaExecucaoFim"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtaExecucaoFim"] }).Where(e => !list.Any(l => l.id == e.id && l.statusExecution.Equals(e.statusExecution) && l.dtaExecucaoFim.Equals(e.dtaExecucaoFim))).ToList<Models.LogExecution>();
                         
                            list.Clear();
                        }
                        else
                            teste = retorno.Select(e => new Models.LogExecution { id = e["id"], statusExecution = e["statusExecution"], dtaExecucaoFim = e["dtaExecucaoFim"].Equals( DBNull.Value ) ? (DateTime?)null : (DateTime)e["dtaExecucaoFim"] }).ToList<Models.LogExecution>();

                        list = retorno.Select(e => new Models.LogExecution { id = e["id"], statusExecution = e["statusExecution"], dtaExecucaoFim = e["dtaExecucaoFim"].Equals(DBNull.Value) ? (DateTime?)null : (DateTime)e["dtaExecucaoFim"] }).ToList<Models.LogExecution>();

                        
                        context.Clients.All.notifyCarga(teste);
                    }

                }
            }
        }

        public static void enviaLista(string id)
        {
            context.Clients.Client(id).notifyCarga(list);
        }

        private static void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {

            if (e.Info.Equals(SqlNotificationInfo.Insert) || e.Info.Equals(SqlNotificationInfo.Update))
            {
                SqlNotificationInfo Info = e.Info;
                SqlNotificationSource Statement = e.Source;
                SqlNotificationType Subscribe = e.Type;
                ServerHub.Show();

                //ISynchronizeInvoke i = (ISynchronizeInvoke)this;
                //if (i.InvokeRequired)
                //{
                //    OnChangeEventHandler tempDelegate =
                //    new OnChangeEventHandler(dependency_OnChange);
                //    object[] args = { sender, e };
                //    i.BeginInvoke(tempDelegate, args);
                //    return;
                //}
                //SqlDependency dependency = (SqlDependency)sender;
                //dependency.OnChange -= dependency_OnChange;
                GetData(Info);
            }
        }
    }
}