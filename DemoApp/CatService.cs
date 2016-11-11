using System;
using System.Linq;
using ApplicationContainer.Logging;

namespace DemoApp
{
	public class CatService
	{
		private ILogger logger;

		public CatService(ILogger logger){
			this.logger = logger;
		}


		public int CountCats(){
			using (var db = new DemoAppDb ()) {
				return db.Cats.Count();
			}
		}


		public Cat insertCat(string name){
			using (var db = new DemoAppDb ()) {
				logger.Log(this.GetType(),"Inserting cat with name ["+name+"]");

				var cat = new Cat (){ Name = name };
				db.Cats.Add (cat);
				db.SaveChanges ();

				return cat;
			}
		}
	}
}

