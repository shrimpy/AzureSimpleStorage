using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureSimpleStorage
{
	public class TableEntityModel : Dictionary<string, object>
	{
		private const string PartitionKeyStr = "partition-key";
		private const string RowKeyStr = "row-key";

		public string PartitionKey
		{
			get
			{
				return (string)this[PartitionKeyStr];
			}
			set
			{
				this[PartitionKeyStr] = value;
			}
		}

		public string RowKey
		{
			get
			{
				return (string)this[RowKeyStr];
			}
			set
			{
				this[RowKeyStr] = value;
			}
		}

		public IDictionary<string, EntityProperty> EntityProperties
		{
			get
			{
				IDictionary<string, EntityProperty> result = new Dictionary<string, EntityProperty>();
				foreach (var item in this)
				{
					if (!string.Equals(PartitionKeyStr, item.Key) && !string.Equals(RowKeyStr, item.Key))
					{
						result.Add(item.Key, EntityProperty.CreateEntityPropertyFromObject(item.Value));
					}
				}

				return result;
			}
			set
			{
				foreach (var item in value)
				{
					this[item.Key] = item.Value.PropertyAsObject;
				}
			}
		}
	}
}
