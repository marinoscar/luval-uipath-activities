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
    [LocalizedDisplayName(nameof(Resources.ExtractEquipmentToInstallAndSummary_DisplayName))]
    [LocalizedDescription(nameof(Resources.ExtractEquipmentToInstallAndSummary_Description))]
    public class ExtractEquipmentToInstallAndSummary : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExtractEquipmentToInstallAndSummary_FileName_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractEquipmentToInstallAndSummary_FileName_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> FileName { get; set; }

        [LocalizedDisplayName(nameof(Resources.ExtractEquipmentToInstallAndSummary_Tables_DisplayName))]
        [LocalizedDescription(nameof(Resources.ExtractEquipmentToInstallAndSummary_Tables_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataSet> Tables { get; set; }

        #endregion


        #region Constructors

        public ExtractEquipmentToInstallAndSummary()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (FileName == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(FileName)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var fileName = FileName.Get(context);

            var tableExtracor = new CleanTableExtrator(fileName);
            var tables = new List<DataTable>();
            try
            {
                tables.AddRange(tableExtracor.GetTables());
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

