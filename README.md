### Welcome to Simple Azure Storage Protocol.
A library that ease your way to interact with Azure Storage. Currently support Table storage only.
Where you don`t need to worry about data type, schema (very close to JSON).

### NuGet
You can find NuGet package from [here](https://www.nuget.org/packages/SimpleAzureStorage/)


### Sample Code
````C#
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
````
