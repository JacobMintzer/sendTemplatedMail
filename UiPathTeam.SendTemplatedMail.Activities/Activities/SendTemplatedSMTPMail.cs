using System;
using System.Activities;
using System.Collections.Generic;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.ComponentModel;
using System.Net.Mail;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CDO;
using ADODB;

namespace UiPathTeam.SendTemplatedMail.Activities
{
	public class SendTemplatedSMTPMail : AsyncTaskCodeActivity
	{

		[Category("Logon")]
		public InArgument<string> Email { get; set; }

		 
		[Category("Logon")]
		public InArgument<string> Password { get; set; }


		[RequiredArgument()]
		[Category("Host")]
		public InArgument<string> Server { get; set; }

		[RequiredArgument()]
		[Category("Host")]
		public InArgument<int> Port { get; set; }

		[Category("Account")]
		public bool EnableSSL { get; set; }


		[RequiredArgument()]
		[Category("Sender")]
		public InArgument<string> From { get; set; }

		[RequiredArgument()]
		[Category("Sender")]
		public InArgument<string> Name { get; set; }
		
		[Category("Input")]
		public InArgument<string> TemplatePath { get; set; }

		[Category("Reciever")]
		public InArgument<string> To { get; set; }

		[Category("Reciever")]
		public InArgument<string> Cc { get; set; }

		[Category("Reciever")]
		public InArgument<string> Bcc { get; set; }

		[Category("Email")]
		[DisplayName("DataTable")]
		public InArgument<DataTable> DT { get; set; }

		[Category("Email")]
		public InArgument<string> Subject { get; set; }

		[Category("Email")]
		public InArgument<string> Body { get; set; }

		[Category("Email")]
		[Browsable(true)]
		public InArgument<string[]> Attachments { get; set; }

		public SendTemplatedSMTPMail()
		{
			//Attachments = new List<InArgument<string>>(){new InArgument<string>()};
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
			}
			*/
		}

		protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync (AsyncCodeActivityContext context,
			CancellationToken cancellationToken, Application client)
		{
			List<string> inputs =Attachments.Get(context).ToList();
			
			var app = new Application(TemplatePath.Get(context), Subject?.Get(context), To?.Get(context), Cc?.Get(context), Bcc?.Get(context), Body?.Get(context), From?.Get(context), Port.Get(context),DT?.Get(context),EnableSSL,Server.Get(context),Email.Get(context),Password.Get(context),inputs);
			await Task.Run(() => app.SendMail());
		return f => { };
		}
	}
}
