using System;
using System.Linq;

namespace QLog.Components.Repository
{
    internal partial class AzureTableRepository
    {
        private string GetTableName()
        {
            //Customized to save all logs to single_table
            string tableName = String.Format(DEFAULT_TABLE_NAME, string.Empty);
            if (!String.IsNullOrWhiteSpace(ComponentsService.Config.GetDataSourcePostfix()))
            {
                tableName = String.Format(POSTFIX_TABLE_NAME, ComponentsService.Config.GetDataSourcePostfix().ToLower(), string.Empty);
            }
            return tableName;
        }
    }
}
