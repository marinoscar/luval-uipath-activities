using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Luval.GoogleTextAnalyticsParser.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Luval.GoogleTextAnalyticsParser.Activities
{
    [LocalizedDisplayName(nameof(Resources.ParseEntities_DisplayName))]
    [LocalizedDescription(nameof(Resources.ParseEntities_Description))]
    public class ParseEntities : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.ParseEntities_Json_DisplayName))]
        [LocalizedDescription(nameof(Resources.ParseEntities_Json_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<string> Json { get; set; }

        [LocalizedDisplayName(nameof(Resources.ParseEntities_Result_DisplayName))]
        [LocalizedDescription(nameof(Resources.ParseEntities_Result_Description))]
        [LocalizedCategory(nameof(Resources.Output_Category))]
        public OutArgument<DataTable> Result { get; set; }

        #endregion


        #region Constructors

        public ParseEntities()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (Json == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(Json)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var json = Json.Get(context);

            var dt = EntityParser.GetEntities(json);

            // Outputs
            return (ctx) => {
                Result.Set(ctx, dt);
            };
        }

        #endregion
    }
}

