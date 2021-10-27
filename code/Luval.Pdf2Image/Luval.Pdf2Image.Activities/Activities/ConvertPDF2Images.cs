using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Luval.Pdf2Image.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Luval.Pdf2Image.Activities
{
    [LocalizedDisplayName(nameof(Resources.ConvertPDF2Images_DisplayName))]
    [LocalizedDescription(nameof(Resources.ConvertPDF2Images_Description))]
    public class ConvertPDF2Images : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.ConvertPDF2Images_PDFFileName_DisplayName))]
        [LocalizedDescription(nameof(Resources.ConvertPDF2Images_PDFFileName_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> PDFFileName { get; set; }

        [LocalizedDisplayName(nameof(Resources.ConvertPDF2Images_OutputFolder_DisplayName))]
        [LocalizedDescription(nameof(Resources.ConvertPDF2Images_OutputFolder_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> OutputFolder { get; set; }

        [LocalizedDisplayName(nameof(Resources.ConvertPDF2Images_FileDataTable_DisplayName))]
        [LocalizedDescription(nameof(Resources.ConvertPDF2Images_FileDataTable_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataTable> FileDataTable { get; set; }

        #endregion


        #region Constructors

        public ConvertPDF2Images()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (PDFFileName == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(PDFFileName)));
            if (OutputFolder == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(OutputFolder)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var pdffilename = PDFFileName.Get(context);
            var outputfolder = OutputFolder.Get(context);
            DataTable dt;

            try
            {
                dt = Converter.ConvertIntoDatatable(pdffilename, outputfolder);
            }
            catch (Exception ex)
            {

                throw new Exception(string.Format("Unable to execute method Converter.ConvertIntoDatatable for input: {0} - {1} error message: {2}", pdffilename, outputfolder, ex.Message), ex);
            }

            // Outputs
            return (ctx) => {
                FileDataTable.Set(ctx, dt);
            };
        }

        #endregion
    }
}

