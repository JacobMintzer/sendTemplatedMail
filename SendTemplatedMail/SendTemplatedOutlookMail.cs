using System;
using System.Activities;
using System.Collections.Generic;
using Microsoft.Office.Interop.Outlook;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Data;

namespace UiPathTeam.SendTemplatedMail.Activities
{
    public class SendTemplatedOutlookMail : CodeActivity
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
		
		public InArgument<Dictionary<string,string>> Attachments { get; set; }
		
		protected override void Execute(CodeActivityContext context)
		{
			Application app = new Application();

			app.ActiveWindow();
			Outlook.MailItem mail;
			if (!(TemplatePath.Get(context).Contains(".oft") || TemplatePath.Get(context).Contains(".msg"))){
				throw new System.Exception("Invalid template format, please use a '.oft' or '.msg' template.");
			}
			mail = TemplatePath.Get(context)!=""?(app.CreateItemFromTemplate(TemplatePath.Get(context).Contains(":\\") ? TemplatePath.Get(context) : System.IO.Directory.GetCurrentDirectory()+'\\'+TemplatePath.Get(context)) as Outlook.MailItem):new MailItem();
			
			mail.Subject = String.IsNullOrEmpty(Subject.Get(context))?(String.IsNullOrEmpty(mail.Subject)?"Untitled":mail.Subject):Subject.Get(context);
			mail.To = String.IsNullOrEmpty(To.Get(context))?mail.To:To.Get(context);
			mail.CC = String.IsNullOrEmpty(Cc.Get(context))?mail.CC:Cc.Get(context);
			mail.BCC = String.IsNullOrEmpty(Bcc.Get(context))?mail.BCC:Bcc.Get(context);
			if (String.IsNullOrEmpty(mail.To)&& String.IsNullOrEmpty(mail.CC) && String.IsNullOrEmpty(mail.BCC))
			{
				throw new System.Exception("Error, there is no recepients specified");
			}
			
			mail.HTMLBody = mail.HTMLBody.Replace("{message}", Body.Get(context));
			mail.HTMLBody = mail.HTMLBody.Replace("{table}", DT.Get(context)!=null?(DT.Get(context).Rows.Count>0?GetHTMLTable(DT.Get(context)):""):"");
			mail.Send();
			app.GetNamespace("MAPI").SendAndReceive(true);
			
			var releaseResult = Marshal.ReleaseComObject(app);
		}
		
		private string GetHTMLTable(DataTable dt)
		{
			string html = "<table style='border:1px solid black;border-collapse: collapse;'>";
			//add header row
			html += "<tr>";
			for (int i = 0; i < dt.Columns.Count; i++)
				html += "<th style='border:1px solid black'>" + dt.Columns[i].ColumnName + "</th>";
			html += "</tr>";
			//add rows
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				html += "<tr>";
				for (int j = 0; j < dt.Columns.Count; j++)
					html += "<td style='border:1px solid black'>" + dt.Rows[i][j].ToString() + "</td>";
				html += "</tr>";
			}
			html += "</table>";
			return html;
		}
	}
}