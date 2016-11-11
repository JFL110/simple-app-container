using System;
using ApplicationContainer.Web;
using ApplicationContainer.Logging;
using System.Linq;

namespace DemoApp
{
	/*
	 	Setup:
		Create a MYSQL schema called DemoApp, configure the user and password in App.config
		Configure the root directory and add an index.html and a 404page.html
		
		Start App with UpgradingBehaviour set to Initial
		Restart App with UpgradingBehaviour set to DropCreate or None
		Navigate to localhost:8181 to view index page
			    localhost:8181/countCats to view number of cats in Cat table
			    localhost:8181/insertCat?name=Bob to insert a cat named Bob
	*/
	class MainClass
	{
		private const string serverPort = "8181";
		private const string wwwRootDir ="../../wwwRoot";
		private const char exitKey = 'x';

		public static void Main (string[] args)
		{
			DemoAppDb.Initialise<DemoAppDb> (MySQLDatabase.UpgradingBehaviour.DropCreateIfModelChanges);

			var logger = new ConsoleLogger ();
			var catService = new CatService (logger);
			var server = new Server (serverPort,logger);

			var catCountingProcessor = new TextServiceProcessor ("countCats", (data) => {
				return catService.CountCats().ToString();
			}, logger);

			var catInsertingProcessor = new TextServiceProcessor ("insertCat", (data) => {
				String name;
				if(!data.TryGetValue("name", out name)){
					return "Please specify a name!";
				}
					
				return catService.insertCat(name).ToString();
			}, logger);

			var staticResourceProcessor = new StaticResourceRequestProcessor (wwwRootDir, logger);

			// Build the request processing queue.
			// The static processor should go last as it will try to return a 404 page for bad requests
			// Unless StaticResourceRequestProcessor.404Condtition is configured to do something smarter.
			server.AddNextProcessor(catCountingProcessor);
			server.AddNextProcessor(catInsertingProcessor);
			server.AddNextProcessor(staticResourceProcessor);
			server.Start ();

			while (true) {
				Console.WriteLine ("Enter '"+exitKey+"' to exit:");
				var key = Console.ReadKey ();
				Console.WriteLine (string.Empty);

				if (key.KeyChar == exitKey || key.KeyChar == exitKey)
					break;
			}
			server.Stop ();
		}
	}
}
