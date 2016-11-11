using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationContainer.Web
{
	public class StaticResourceRequestProcessor : RequestProcessor
	{
		public string IndexFileName { get; set;} = "index.html";
		public string FileNotFoundPageFileName { get; set;} = "404Page.html";
		public Func<string,bool> Return404Condition {get;set;} = (path)=>{return true;};

		public string RootPath {get; private set;}

		public StaticResourceRequestProcessor(string rootPath,
			Logging.ILogger logger) : base(logger){
			if (string.IsNullOrEmpty (rootPath))
				throw new ArgumentNullException (nameof (rootPath));

			if (!Directory.Exists (rootPath))
				throw new DirectoryNotFoundException (rootPath);

			this.RootPath = rootPath;
		}


		private String getFullPath(string requestPath){
			return Path.Combine (RootPath, requestPath.TrimStart(Path.DirectorySeparatorChar));
		}


		public override bool TryProcess(HttpListenerContext context)
		{
			var requestPath = context.Request.Url.AbsolutePath ?? string.Empty;

			if (requestPath.Last () == Path.DirectorySeparatorChar)
				requestPath = Path.Combine (requestPath, IndexFileName);

			if (!File.Exists (getFullPath (requestPath))){
				if (!Return404Condition(requestPath) &&
					!File.Exists (getFullPath (FileNotFoundPageFileName))) {
					return false;
				}
				logger.Log (this.GetType (), "Returning 404 page for request ["+requestPath+"]");
				requestPath = FileNotFoundPageFileName;
			}
				
			try{
				Task.Run( ()=>{
					// TODO send the MIME type
					// TODO read and write into a buffer - will be faster
					WriteAllBytesAndClose(File.ReadAllBytes(getFullPath (requestPath)),context.Response);
				});
			}catch{
				Send404AndClose (context.Response);
				logger.Error (this.GetType (), "Error processing request for '" + getFullPath (requestPath)+"'");
			}

			return true;
		}
	}
}

