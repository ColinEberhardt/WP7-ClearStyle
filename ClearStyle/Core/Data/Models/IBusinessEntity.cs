using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearStyle.Core.Providers.SQLite;

namespace ClearStyle.Core.Data.Models
{
	public interface IBusinessEntity
	{
		[PrimaryKey]
		int Id { get; set; }
	}
}
