using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Luval.PDFTableReader.Activities.Design.Designers;
using Luval.PDFTableReader.Activities.Design.Properties;

namespace Luval.PDFTableReader.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(ExtractPDFTablesInPage), categoryAttribute);
            builder.AddCustomAttributes(typeof(ExtractPDFTablesInPage), new DesignerAttribute(typeof(ExtractPDFTablesInPageDesigner)));
            builder.AddCustomAttributes(typeof(ExtractPDFTablesInPage), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
