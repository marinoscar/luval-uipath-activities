using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Luval.Pdf2Image.Activities.Design.Designers;
using Luval.Pdf2Image.Activities.Design.Properties;

namespace Luval.Pdf2Image.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(ConvertPDF2Images), categoryAttribute);
            builder.AddCustomAttributes(typeof(ConvertPDF2Images), new DesignerAttribute(typeof(ConvertPDF2ImagesDesigner)));
            builder.AddCustomAttributes(typeof(ConvertPDF2Images), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
