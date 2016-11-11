using System;
using System.Net;

namespace ApplicationContainer.Web
{
	public interface IRequestProcessor
	{
		// Returns true if the request was process, otherwise false
		bool TryProcess (HttpListenerContext context);
	}
}

