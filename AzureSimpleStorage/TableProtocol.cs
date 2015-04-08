using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureSimpleStorage
{
	public class TableProtocol
	{
		private CloudStorageAccount _acc;
		private CloudTableClient _client;
		public TableProtocol(string storageAccountName, string storageAccountKey, bool useHttps = true)
		{
			var cred = new StorageCredentials(storageAccountName, storageAccountKey);
			_acc = new CloudStorageAccount(cred, useHttps);
			_client = _acc.CreateCloudTableClient();
		}

		public async Task InsertOrUpdate(string tableName, TableEntityModel entity)
		{
			CloudTable table = await this.GetTable(tableName);
			var dataEntity = new DynamicTableEntity(entity.PartitionKey, entity.RowKey, null, entity.EntityProperties);
			await table.ExecuteAsync(TableOperation.InsertOrReplace(dataEntity));
		}

		public async Task Delete(string tableName, string partitionKey, string rowKey)
		{
			CloudTable table = await this.GetTable(tableName);
			TableResult result = await table.ExecuteAsync(TableOperation.Retrieve(partitionKey, rowKey));
			await table.ExecuteAsync(TableOperation.Delete(result.Result as ITableEntity));
		}

		public async Task<TableEntityModel> Get(string tableName, string partitionKey, string rowKey)
		{
			CloudTable table = await this.GetTable(tableName);
			TableResult result = await table.ExecuteAsync(TableOperation.Retrieve(partitionKey, rowKey));
			var data = result.Result as DynamicTableEntity;
			var model = new TableEntityModel();
			model.PartitionKey = data.PartitionKey;
			model.RowKey = data.RowKey;
			model.EntityProperties = data.Properties;
			return model;
		}

		public async Task<bool> DeleteTable(string tableName)
		{
			CloudTable table = _client.GetTableReference(tableName);
			return await table.DeleteIfExistsAsync();
		}

		private async Task<CloudTable> GetTable(string tableName)
		{
			CloudTable table = _client.GetTableReference(tableName);
			await table.CreateIfNotExistsAsync();
			return table;
		}
	}
}
