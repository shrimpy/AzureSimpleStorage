using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureSimpleStorage;
using Xunit;

namespace AzureSimpleStorageTests
{
	public class TableProtocolTests
	{
		private string StorageAccountName = Environment.GetEnvironmentVariable("JsonAzureStorage-AccountName");
		private string StorageAccountKey = Environment.GetEnvironmentVariable("JsonAzureStorage-AccountKey");

		[Fact]
		public async Task CanInsertOrUpdate()
		{
			string tableName = GetRandomTableName();

			try
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

				await proto.InsertOrUpdate(tableName, entity);

				TableEntityModel getResult = await proto.Get(tableName, partKey, rowkey);
				Assert.Equal(5, getResult.Count);
				foreach (var item in entity)
				{
					Assert.Equal(item.Value, entity[item.Key]);
				}

				entity["val1"] = 100;
				entity["val4"] = "foo";
				await proto.InsertOrUpdate(tableName, entity);

				getResult = await proto.Get(tableName, partKey, rowkey);
				Assert.Equal(6, getResult.Count);
				foreach (var item in entity)
				{
					Assert.Equal(item.Value, entity[item.Key]);
				}
			}
			finally
			{
				var proto = new TableProtocol(StorageAccountName, StorageAccountKey);
				proto.DeleteTable(tableName).Wait();
			}
		}

		[Fact]
		public async Task CanList()
		{
			string tableName = GetRandomTableName();

			try
			{
				string partKey = Guid.NewGuid().ToString();
				var proto = new TableProtocol(StorageAccountName, StorageAccountKey);
				List<Task> tasks = new List<Task>();
				for (int i = 0; i < 10; i++)
				{
					var entity = new TableEntityModel
					{
						PartitionKey = partKey,
						RowKey = Guid.NewGuid().ToString()
					};
					entity["val1"] = 1;
					entity["val12"] = 2.0;
					entity["val13"] = DateTime.Now;

					tasks.Add(proto.InsertOrUpdate(tableName, entity));
				}

				await Task.WhenAll(tasks);

				TableEntityModelCollection results = await proto.List(tableName, partKey, 8);
				Assert.NotNull(results.ContinuationToken);
				Assert.Equal(8, results.Entities.Count);

				results = await proto.List(tableName, partKey, 8, results.ContinuationToken);
				Assert.Null(results.ContinuationToken);
				Assert.Equal(2, results.Entities.Count);
			}
			finally
			{
				var proto = new TableProtocol(StorageAccountName, StorageAccountKey);
				proto.DeleteTable(tableName).Wait();
			}
		}

		private static string GetRandomTableName()
		{
			return "t" + Guid.NewGuid().ToString("N").ToLower();
		}
	}
}
