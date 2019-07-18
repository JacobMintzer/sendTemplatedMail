using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using UiPathTeam.SendTemplatedMail.Activities.Properties;

namespace UiPathTeam.SendTemplatedMail.Activities
{
    [LocalizedDisplayName(nameof(Resources.ChildActivityDisplayName))]
    [LocalizedDescription(nameof(Resources.ChildActivityDescription))]
    public class ChildActivity : AsyncTaskCodeActivity<int>
    {
        [LocalizedDisplayName(nameof(Resources.ChildActivityFirstValueDisplayName))]
        [LocalizedDescription(nameof(Resources.ChildActivityFirstValueDescription))]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<int> FirstValue { get; set; }

        [LocalizedDisplayName(nameof(Resources.ChildActivitySecondValueDisplayName))]
        [LocalizedDescription(nameof(Resources.ChildActivitySecondValueDescription))]
        [LocalizedCategory(nameof(Resources.Input))]
        public InArgument<int> SecondValue { get; set; }

        [LocalizedDisplayName(nameof(Resources.ChildActivitySumDisplayName))]
        [LocalizedDescription(nameof(Resources.ChildActivitySumDescription))]
        [LocalizedCategory(nameof(Resources.Output))]
        public OutArgument<int> Sum { get; set; }

        /// <inheritdoc />
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);

            if (FirstValue == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError, nameof(FirstValue)));
            if (SecondValue == null) metadata.AddValidationError(string.Format(Resources.MetadataValidationError, nameof(SecondValue)));
        }

        protected override Task<int> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken, Application client)
        {
            var firstValue = FirstValue.Get(context);
            var secondValue = SecondValue.Get(context);
            return client.SumValues(firstValue, secondValue);
        }

        protected override void OutputResult(AsyncCodeActivityContext context, int result)
        {
            Sum.Set(context, result);
        }
    }
}
