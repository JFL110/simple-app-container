using System;
using System.Data.Entity;

namespace DemoApp
{
	public abstract class MySQLDatabase : DbContext
	{
		public MySQLDatabase (String dbName) : base(dbName){}

		public static void Initialise<T>(UpgradingBehaviour upgradingBehaviour) where T: MySQLDatabase{
			if (upgradingBehaviour == UpgradingBehaviour.InitialCreation) {
				Database.SetInitializer<T>(new DropCreateDatabaseAlways<T>());
			} else if (upgradingBehaviour == UpgradingBehaviour.DropCreateIfModelChanges) {
				Database.SetInitializer<T>(new DropCreateDatabaseIfModelChanges<T>());
			}
		}

		public enum UpgradingBehaviour
		{
			InitialCreation,
			DropCreateIfModelChanges,
			None
		}

		static MySQLDatabase()
		{
			DbConfiguration.SetConfiguration (new MySql.Data.Entity.MySqlEFConfiguration ());
		}
			
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
			base.OnModelCreating(modelBuilder);
		}
	}
}