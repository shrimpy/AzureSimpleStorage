using System;
using System.Threading.Tasks;
using AzureSimpleStorage;
using Xunit;

namespace AzureSimpleStorageTests
{
	public class TableProtocolTests : IDisposable
	{
		private string StorageAccountName = Environment.GetEnvironmentVariable("JsonAzureStorage-AccountName");
		private string StorageAccountKey = Environment.GetEnvironmentVariable("JsonAzureStorage-AccountKey");
		private const string TestTable = "jsonazurestorageteststableprotocol";

		[Fact]
		public async Task CanInsertOrUpdate()
		{
			string partKey = Guid.NewGuid().ToString();
			string rowkey = Guid.NewGuid().ToString();
			var proto = new TableProtocol(StorageAccountName, StorageAccountKey);
			var entity = new TableEntityModel
			{
				PartitionKey = partKey,
				RowKey = rowkey
			};
			entity["val1"] = 1;
			entity["val12"] = 2.0;
			entity["val13"] = DateTime.Now;

			await proto.InsertOrUpdate(TestTable, entity);

			TableEntityModel getResult = await proto.Get(TestTable, partKey, rowkey);
			foreach (var item in entity)
			{
				Assert.Equal(item.Value, entity[item.Key]);
			}
		}

		public void Dispose()
		{
			var proto = new TableProtocol(StorageAccountName, StorageAccountKey);
			proto.DeleteTable(TestTable).Wait();
		}
	}
}
