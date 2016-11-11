using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationContainer.Web
{
	// Http Server - takes requests asynchronously and tries to process them with its IRequestProcessors
	public class Server
	{
		private readonly Logging.ILogger logger;

		private HttpListener listener;
		private Thread listenerThread;
		private bool exitFlag;
		public string Port {get; private set;}

		private List<IRequestProcessor> _processors = new List<IRequestProcessor> ();
		public IEnumerable<IRequestProcessor> Processors => _processors.ToList();


		public void AddNextProcessor(IRequestProcessor Processor){
			if (Processor == null){
				throw new ArgumentNullException (nameof(Processor));
			}

			_processors.Add (Processor);
		}


		public Server(String port, Logging.ILogger logger){
			this.logger = logger;
			this.Port = port;

			listener = new HttpListener ();
			listener.Prefixes.Add ("http://*:"+port+"/");
			listenerThread = new Thread (Listen);
		}


		// Server Lifecycle
		public void Start(){
			if (exitFlag) {
				return;
			}
			
			exitFlag = false;

			logger.Log (this.GetType (), "Starting server on port [" + Port + "]");
			listenerThread.Start ();
		}


		public void Stop(){
			if (exitFlag) {
				return;
			}
			
			exitFlag = true;

			logger.Log (this.GetType (), "Stopping server");
			listenerThread.Abort ();
		}

		// Request Lifecycle
		private void Listen()
		{
			listener.Start ();

			try{
				while (!exitFlag){
					listener.GetContextAsync ()
						.ContinueWith (task => {
							Task.Run (() => {
								try{
									HandleRequest (task.Result);
								}catch(Exception e){
									logger.Error(this.GetType(),e);
								}
							});
						}).Wait(); 
					// Here we wait for a request to be recieved, then we dispatch
					// the HandleRequest method, but we dont wait for that to finish before we
					// we start trying to get the next context.
				}
			}finally{
				listener.Close ();
			}
		}

		private void HandleRequest(HttpListenerContext context){
			logger.Log (this.GetType (), "Request [" + context.Request.Url.AbsolutePath + "]");

			if (_processors.Any (p => p.TryProcess (context))) {
				return;
			}

			// Fallback if none of the processors could handle the request = do nothing
			logger.Error(this.GetType(),"Unknown/Failed Request: " + context.Request.Url.AbsolutePath);
			context.Response.Close ();
		}
	}
}