using System;

namespace ApplicationContainer.Logging
{
	public class ConsoleLogger : ILogger
	{
		public void Log(Type source, Object obj){
			Console.WriteLine (source?.Name+" : "+obj);
		}

		public void Error(Type source,Exception e){
			Console.WriteLine (source?.Name+" : "+e);
		}

		public void Error(Type source,String message){
			Console.WriteLine (source?.Name+" : "+message);
		}

		public void Log(Type source,String format, params Object[] objs){
			Console.WriteLine (source?.Name+" : "+format, objs);
	}
}
}
