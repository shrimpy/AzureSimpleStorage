using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureSimpleStorage
{
	public class TableEntityModelCollection
	{
		public List<TableEntityModel> Entities { get; set; }
		public string ContinuationToken { get; set; }

		public TableEntityModelCollection()
		{
			this.Entities = new List<TableEntityModel>();
		}
	}
}
