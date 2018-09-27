using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;

namespace UiPathTeam.SendTemplatedMail.Activities
{
	public class DesignerMetadata : IRegisterMetadata

	{

		public void Register()

		{

			AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();



			attributeTableBuilder.AddCustomAttributes(typeof(SendTemplatedOutlookMail), new DesignerAttribute(typeof(SendMailDesigner)));
			attributeTableBuilder.AddCustomAttributes(typeof(SendTemplatedSMTPMail), new DesignerAttribute(typeof(SendMailDesigner)));



			MetadataStore.AddAttributeTable(attributeTableBuilder.CreateTable());

		}

	}


}
