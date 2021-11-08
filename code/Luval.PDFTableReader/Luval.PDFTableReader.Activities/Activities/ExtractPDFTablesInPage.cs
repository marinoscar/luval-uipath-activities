using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Luval.PDFTableReader.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;
using System.Collections.Generic;

namespace Luval.PDFTableReader.Activities
{
    [LocalizedDisplayName(nameof(Resources.ExtractPDFTablesInPage_DisplayName))]
    [LocalizedDescription(nameof(Resources.ExtractPDFTablesInPage_Description))]
    public class ExtractPDFTablesInPage : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExtractPDFTablesInPage_FileName_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractPDFTablesInPage_FileName_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> FileName { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExtractPDFTablesInPage_PageNumber_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractPDFTablesInPage_PageNumber_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<int> PageNumber { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExtractPDFTablesInPage_Tables_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractPDFTablesInPage_Tables_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataSet> Tables { get; set; }

        #endregion


        #region Constructors

        public ExtractPDFTablesInPage()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (FileName == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(FileName)));
            if (PageNumber == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(PageNumber)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var fileName = FileName.Get(context);
            var pageNumber = PageNumber.Get(context);

            var tableExtracor = new TableExtractor(fileName);
            var tables = new List<DataTable>();
            try
            {
                tables.AddRange(tableExtracor.GetTablesInPage(pageNumber));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Unable to extract tables in file {0}", fileName), ex);
            }
            var ds = new DataSet("PDFTables");
            foreach (var t in tables)
            {
                ds.Tables.Add(t);
            }

            // Outputs
            return (ctx) => {
                Tables.Set(ctx, ds);
            };
        }

        #endregion
    }
}

