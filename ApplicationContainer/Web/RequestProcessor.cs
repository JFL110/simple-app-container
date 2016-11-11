using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ApplicationContainer;
using System.Linq;
using System.IO;

namespace ApplicationContainer.Web
{
	public abstract class RequestProcessor :IRequestProcessor
	{
		protected Logging.ILogger logger { get; private set; }

		protected RequestProcessor(Logging.ILogger logger){
			this.logger = logger;
		}
			

		public abstract bool TryProcess (HttpListenerContext context);


		protected void WriteAllBytesAndClose(object obj,HttpListenerResponse response)
		{
			WriteAllBytesAndClose (System.Text.Encoding.ASCII.GetBytes (obj?.ToString() ?? string.Empty), response);
		}


		protected void WriteAllBytesAndClose(byte[] bytes,HttpListenerResponse response)
		{
			if (response == null) {
				throw new ArgumentNullException (nameof (response));
			}

			bytes = bytes ?? new byte[0];

			// Write
			try{
				response.ContentLength64 = bytes.Length;
				response.OutputStream.Write (bytes, 0, bytes.Length);
				response.OutputStream.Flush ();
				response.Close ();
			}catch(Exception e){
				logger.Error (this.GetType(),e);
			}// Continue
		}


		protected void Send404AndClose(HttpListenerResponse response){
			response.StatusCode = (int)HttpStatusCode.InternalServerError;
			response.ContentLength64 = 0;
			response.Close ();
		}


		protected IDictionary<string,string> GetData(HttpListenerRequest request)
		{
			return request.QueryString
				.AllKeys
				.ToDictionary((key)=>{return key;},
				(key)=>{return request.QueryString[key];});
		}


		protected string GetData(string key,HttpListenerRequest request){
			return request.QueryString.Get (key);
		}

	}
}

