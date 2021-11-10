using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Luval.GoogleTextAnalyticsParser.Activities.Design.Designers;
using Luval.GoogleTextAnalyticsParser.Activities.Design.Properties;

namespace Luval.GoogleTextAnalyticsParser.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(ParseEntities), categoryAttribute);
            builder.AddCustomAttributes(typeof(ParseEntities), new DesignerAttribute(typeof(ParseEntitiesDesigner)));
            builder.AddCustomAttributes(typeof(ParseEntities), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
