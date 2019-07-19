using System;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Office.Interop.Outlook;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UiPathTeam.SendTemplatedMail.Activities
{
    public class SendTemplatedOutlookMail : AsyncTaskCodeActivity
	{
		[RequiredArgument()]
		[Category("Input")]
		public InArgument<string> TemplatePath { get; set; }

		[Category("Receiver")]
		public InArgument<string> To { get; set; }

		[Category("Receiver")]
		public InArgument<string> Cc { get; set; }

		[Category("Receiver")]
		public InArgument<string> Bcc { get; set; }
		
		[Category("Email")]
		public InArgument<DataTable> DT { get; set; }
		
		[Category("Email")]
		public InArgument<string> Subject { get; set; }
		
		[Category("Email")]
		public InArgument<string> Body { get; set; }
		
		public InArgument<string[]> Attachments { get; set; }

		public SendTemplatedOutlookMail()
		{
			//Attachments = new List<InArgument<string>>() { new InArgument<string>() };
		}
		protected override void CacheMetadata(CodeActivityMetadata metadata)
		{
			base.CacheMetadata(metadata);
			/*int index = 1;
			foreach (var item in Attachments)
			{
				string name = "attachmentArg" + ++index;
				var runtimeArg = new RuntimeArgument(name, typeof(string), ArgumentDirection.In);
				metadata.Bind(item, runtimeArg);
				metadata.AddArgument(runtimeArg);
			}*/
		}

		protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken, Application client)
		{

			List<string> inputs = Attachments.Get(context).ToList();
			if (!(TemplatePath.Get(context).Contains(".oft") || TemplatePath.Get(context).Contains(".msg")))
			{
				throw new System.Exception("Invalid template format, please use a '.oft' or '.msg' template.");
			}
			Application app = new Application();
			app.SendOutlook(TemplatePath.Get(context),Subject.Get(context), To.Get(context), Cc.Get(context), Bcc.Get(context), Body.Get(context), DT.Get(context), inputs);
			return ctx => { };
		}


		
		
		
	}
}