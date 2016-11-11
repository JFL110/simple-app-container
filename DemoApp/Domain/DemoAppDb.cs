using System;
using System.Data.Entity;

namespace DemoApp
{
	public class DemoAppDb : MySQLDatabase
	{
		public DemoAppDb () : base("DemoApp"){}

		public DbSet<Cat> Cats {get;set;}

	}
}