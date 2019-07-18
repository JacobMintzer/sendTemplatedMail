using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.ComponentModel.Design;
using UiPath.Shared.Activities.Design.Editors;
using UiPathTeam.SendTemplatedMail.Activities.Design.Properties;

namespace UiPathTeam.SendTemplatedMail.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute =  new CategoryAttribute($"{Resources.Category}");
            builder.AddCustomAttributes(typeof(SendTemplatedSMTPMail), categoryAttribute);
            builder.AddCustomAttributes(typeof(SendTemplatedSMTPMail), new DesignerAttribute(typeof(SendMailDesigner)));
			builder.AddCustomAttributes(typeof(SendTemplatedSMTPMail), nameof(SendTemplatedSMTPMail.Attachments), new EditorAttribute(typeof(ArgumentCollectionEditor), typeof(DialogPropertyValueEditor)));
			builder.AddCustomAttributes(typeof(SendTemplatedOutlookMail), nameof(SendTemplatedOutlookMail.Attachments), new EditorAttribute(typeof(ArgumentCollectionEditor), typeof(DialogPropertyValueEditor)));
			builder.AddCustomAttributes(typeof(SendTemplatedOutlookMail), categoryAttribute);
            builder.AddCustomAttributes(typeof(SendTemplatedOutlookMail), new DesignerAttribute(typeof(SendMailDesigner)));
            builder.AddCustomAttributes(typeof(SendTemplatedOutlookMail), new HelpKeywordAttribute("https://go.uipath.com"));
            
			MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
