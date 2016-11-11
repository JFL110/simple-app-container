using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ApplicationContainer.Web
{
	public class TextServiceProcessor : RequestProcessor
	{
		public Func<IDictionary<String,String>,String> Function { get; private set; }
		public String RequestPath { get; private set; }

		public TextServiceProcessor(string requestPath,
			Func<IDictionary<String,String>,String> function,
			Logging.ILogger logger) : base(logger){
			this.RequestPath = requestPath;
			this.Function = function;
		}


		public override bool TryProcess(HttpListenerContext context)
		{
			var path = (context.Request.Url.AbsolutePath ?? string.Empty).Trim(Path.DirectorySeparatorChar);
			logger.Log (this.GetType(), path);
			if (path != RequestPath) {
				return false;
			}

			try{
				Task.Run( ()=>{
					// TODO send the MIME type
					WriteAllBytesAndClose(Function(GetData(context.Request)),context.Response);
				});
			}catch{
				Send404AndClose (context.Response);
				logger.Error (this.GetType (), "Error processing request for '" + path +"'");
			}

			return true;
		}
	}
}

