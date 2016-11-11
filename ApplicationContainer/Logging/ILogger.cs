using System;

namespace ApplicationContainer.Logging
{
	public interface ILogger
	{
		void Error(Type type,String message);
		void Error(Type type,Exception e);
		void Log(Type type,Object obj);
		void Log (Type type,String format, params Object[] objs);
	}
}

