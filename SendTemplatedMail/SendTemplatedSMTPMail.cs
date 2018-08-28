using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Office.Interop.Outlook;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Net.Mail;
using System.Data;
using System.Net.Mime;
using CDO;
using ADODB;

namespace SendTemplatedMail
{
	public class SendTemplatedSMTPMail : CodeActivity
	{

		[RequiredArgument()]
		[Category("Logon")]
		public InArgument<string> Email { get; set; }

		 
		[RequiredArgument()]
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
		public InArgument<DataTable> DT { get; set; }

		[Category("Email")]
		public InArgument<string> Subject { get; set; }

		[Category("Email")]
		public InArgument<string> Body { get; set; }

		

		protected override void Execute(CodeActivityContext context)
		{
			String TO, CC, BCC, SUBJECT, BODY;
			if (TemplatePath.Get(context).Contains(".eml"))
			{
				CDO.Message emlMessage = ReadMessage(TemplatePath.Get(context).Contains(":\\") ? TemplatePath.Get(context) : System.IO.Directory.GetCurrentDirectory() + '\\' + TemplatePath.Get(context));
				TO = emlMessage.To;
				CC = emlMessage.CC;
				BCC = emlMessage.BCC;
				SUBJECT = emlMessage.Subject;
				BODY = String.IsNullOrEmpty(emlMessage.HTMLBody)?emlMessage.TextBody:emlMessage.HTMLBody;
				

			}
			else if (TemplatePath.Get(context).Contains(".oft") || TemplatePath.Get(context).Contains(".msg"))
			{
				try
				{
					Application app = new Application();
					Outlook.MailItem template = app.CreateItemFromTemplate(TemplatePath.Get(context).Contains(":\\") ? TemplatePath.Get(context) : System.IO.Directory.GetCurrentDirectory() + '\\' + TemplatePath.Get(context)) as Outlook.MailItem;
					TO = template.To;
					CC = template.CC;
					BCC = template.BCC;
					SUBJECT = template.Subject;
					BODY = String.IsNullOrEmpty(template.HTMLBody) ? template.Body : template.HTMLBody;
				}
				catch
				{
					throw new System.Exception("Error, .oft and .msg files can only be read if you have Outlook installed, try using a .eml as a template");
				}
			}
			else
			{
				throw new System.Exception("Invalid template format, please use a '.oft', '.msg', or '.eml' template.");
			}
			
			SUBJECT = String.IsNullOrEmpty(Subject.Get(context)) ? (String.IsNullOrEmpty(SUBJECT) ? "Untitled" : SUBJECT) : Subject.Get(context);
			TO = String.IsNullOrEmpty(To.Get(context)) ? TO : To.Get(context);
			CC = String.IsNullOrEmpty(Cc.Get(context)) ? CC : Cc.Get(context);
			BCC = String.IsNullOrEmpty(Bcc.Get(context)) ? BCC : Bcc.Get(context);
			BODY = BODY.Replace("{message}", String.IsNullOrEmpty(Body.Get(context))?"":Body.Get(context));
			BODY=BODY.Replace("{table}", DT.Get(context) != null ? (DT.Get(context).Rows.Count > 0 ? GetHTMLTable(DT.Get(context)) : "") : "");
			MailMessage mail = new MailMessage(From.Get(context), TO, SUBJECT, BODY);
			mail.Bcc.Add(BCC);
			mail.IsBodyHtml = true;
			mail.CC.Add(CC);
			SmtpClient client = new SmtpClient
			{
				Port = Port.Get(context),
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				EnableSsl = EnableSSL,
				Host = Server.Get(context),
				Credentials = new System.Net.NetworkCredential(Email.Get(context), Password.Get(context)),
				Timeout = 10000
			};
			client.Send(mail);
		}
		protected CDO.Message ReadMessage(String emlFileName)
		{
			CDO.Message msg = new CDO.MessageClass();
			ADODB.Stream stream = new ADODB.StreamClass();
			stream.Open(Type.Missing,
						   ADODB.ConnectModeEnum.adModeUnknown,
						   ADODB.StreamOpenOptionsEnum.adOpenStreamUnspecified,
						   String.Empty,
						   String.Empty);
			stream.LoadFromFile(emlFileName);
			stream.Flush();
			msg.DataSource.OpenObject(stream, "_Stream");
			msg.DataSource.Save();
			return msg;
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
