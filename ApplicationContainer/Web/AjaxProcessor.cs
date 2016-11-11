using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace ApplicationContainer.Web
{
	public class AjaxProcessor : RequestProcessor
	{
		public string AjaxSuffix {get;set;} =".ajax";
		public int UpdateCheckInterval { get; set; } =  150;
		public TimeSpan MaxRequestTime { get; set; } =  new TimeSpan (0, 0, 10);

		public AjaxProcessor(Logging.ILogger logger) :base(logger){}
			
		public override bool TryProcess(HttpListenerContext Context)
		{
			return false;
		}
	}
}