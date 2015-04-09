using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

		public async Task<TableEntityModelCollection> List(string tableName, string partitionKey, int segmentCount = 20, string tokenStr = null)
		{
			CloudTable table = await this.GetTable(tableName);
			var query = new TableQuery<DynamicTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
			query.TakeCount = segmentCount;

			TableContinuationToken token = null;
			if (!string.IsNullOrWhiteSpace(tokenStr))
			{
				token = new TableContinuationToken();
				using (var reader = new StringReader(tokenStr))
				{
					token.ReadXml(XmlReader.Create(reader));
				}
			}

			TableQuerySegment<DynamicTableEntity> result = await table.ExecuteQuerySegmentedAsync<DynamicTableEntity>(query, token);

			var resultCollection = new TableEntityModelCollection();

			if (result.ContinuationToken != null)
			{
				StringBuilder sb = new StringBuilder();
				using (var xmlWriter = XmlWriter.Create(sb))
				{
					result.ContinuationToken.WriteXml(xmlWriter);
				}

				resultCollection.ContinuationToken = sb.ToString();
			}

			foreach (var item in result.Results)
			{
				var entity = new TableEntityModel()
				{
					PartitionKey = item.PartitionKey,
					RowKey = item.RowKey,
					EntityProperties = item.Properties
				};

				resultCollection.Entities.Add(entity);
			}

			return resultCollection;
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
