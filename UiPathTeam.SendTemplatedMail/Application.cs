using System;
using System.Activities;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using CDO;
using Newtonsoft.Json.Linq;
using UiPathTeam.SendTemplatedMail.Enums;
using UiPathTeam.SendTemplatedMail.Properties;
using UiPathTeam.SendTemplatedMail.Tools;
using static UiPathTeam.SendTemplatedMail.Enums.TokenSource;
using static UiPathTeam.SendTemplatedMail.Enums.AuthScheme;
using Stream = ADODB.Stream;

namespace UiPathTeam.SendTemplatedMail
{
    /// <summary>
    /// The Application class holds an HTTP Client and list of API calls shared amongst the scope and child activities.
    /// </summary>
    public class Application
	{
		string TO, CC, BCC, SUBJECT, BODY, FROM, HOST, EMAIL, PASSWORD;
		private int Port;
		private bool SSL;
		private List<string> Inputs;

		public Application()
		{

		}
		public Application(string templatePath, string subject, string to, string cc, string bcc, string body, string from, int port, DataTable dt, bool ssl, string host, string email, string password, List<string> inputs)
		{
			EMAIL = email;
			PASSWORD = password;
			HOST = host;
			SSL = ssl;
			Port = port;
			FROM = from;
			Inputs = inputs;
			if (templatePath.Contains(".eml"))
			{
				Message emlMessage = ReadMessage(templatePath.Contains(":\\") ? templatePath : System.IO.Directory.GetCurrentDirectory() + '\\' + templatePath);
				TO = emlMessage.To;
				CC = emlMessage.CC;
				BCC = emlMessage.BCC;
				SUBJECT = emlMessage.Subject;
				BODY = String.IsNullOrEmpty(emlMessage.HTMLBody) ? emlMessage.TextBody : emlMessage.HTMLBody;


			}
			else if (templatePath.Contains(".oft") || templatePath.Contains(".msg"))
			{
				try
				{
					Outlook.Application app = new Outlook.Application();
					Outlook.MailItem template = app.CreateItemFromTemplate(templatePath.Contains(":\\")
						? templatePath
						: System.IO.Directory.GetCurrentDirectory() + '\\' + templatePath) as Outlook.MailItem;
					TO = template.To;
					CC = template.CC;
					BCC = template.BCC;
					SUBJECT = template.Subject;
					BODY = String.IsNullOrEmpty(template.HTMLBody) ? template.Body : template.HTMLBody;
				}
				catch (IOException)
				{
					throw;
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

			SUBJECT = String.IsNullOrEmpty(subject) ? (String.IsNullOrEmpty(SUBJECT) ? "Untitled" : SUBJECT) : subject;
			TO = String.IsNullOrEmpty(to) ? TO : to;
			CC = String.IsNullOrEmpty(cc)? CC : cc;
			BCC = String.IsNullOrEmpty(bcc) ? BCC :bcc;
			BODY = BODY.Replace("{message}", String.IsNullOrEmpty(body) ? "" : body);
			BODY = BODY.Replace("{table}", dt != null ? (dt.Rows.Count > 0 ? GetHTMLTable(dt) : "") : "");
			
		}
		public void SendOutlook(string template, string subject, string to, string cc, string bcc, string body, DataTable dt, List<string> attachments)
		{
			Outlook.Application app = new Outlook.Application();
			app.ActiveWindow();
			Outlook.MailItem mail;
			mail = app.CreateItemFromTemplate(template.Contains(@":\")
				? template
				: System.IO.Directory.GetCurrentDirectory() + '\\' + template) as Outlook.MailItem;
			mail.Subject = String.IsNullOrEmpty(subject) ? (String.IsNullOrEmpty(mail.Subject) ? "Untitled" : mail.Subject) : subject;
			mail.To = String.IsNullOrEmpty(to) ? mail.To : to;
			mail.CC = String.IsNullOrEmpty(cc) ? mail.CC : cc;
			mail.BCC = String.IsNullOrEmpty(bcc) ? mail.BCC : bcc;
			attachments.ForEach(x=>mail.Attachments.Add(x));
			if (String.IsNullOrEmpty(mail.To) && String.IsNullOrEmpty(mail.CC) && String.IsNullOrEmpty(mail.BCC))
			{
				throw new System.Exception("Error, there is no recepients specified");
			}

			mail.HTMLBody = mail.HTMLBody.Replace("{message}", body);
			mail.HTMLBody = mail.HTMLBody.Replace("{table}", dt != null ? (dt.Rows.Count > 0 ? GetHTMLTable(dt) : "") : "");
			mail.Send();
			app.GetNamespace("MAPI").SendAndReceive(true);

			var releaseResult = Marshal.ReleaseComObject(app);
		}

		public void SendMail()
	    {
		    MailMessage mail = new MailMessage(FROM, TO, SUBJECT, BODY) ;
		    if (!string.IsNullOrEmpty(BCC))
				mail.Bcc.Add(BCC);
		    mail.IsBodyHtml = true;
			if(!string.IsNullOrEmpty(CC))
			    mail.CC.Add(CC);
			Inputs.ForEach(x=>mail.Attachments.Add(new Attachment(x)));
		    SmtpClient client = new SmtpClient
		    {
			    Port = Port,
			    DeliveryMethod = SmtpDeliveryMethod.Network,
			    UseDefaultCredentials = false,
			    EnableSsl = SSL,
			    Host =HOST,
			    Credentials = new System.Net.NetworkCredential(EMAIL, PASSWORD),
			    Timeout = 30000
		    };
		    client.Send(mail);
		}

	    protected Message ReadMessage(String emlFileName)
	    {
		    Message msg = new Message();
		    Stream stream = new Stream();
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
