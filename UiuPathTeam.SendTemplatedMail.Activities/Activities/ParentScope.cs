using System;
using System.Activities;
using System.ComponentModel;
using System.Activities.Statements;
using UiPathTeam.SendTemplatedMail.Activities.Properties;

namespace UiPathTeam.SendTemplatedMail.Activities
{

    [LocalizedDescription(nameof(Resources.ParentScopeDescription))]
    [LocalizedDisplayName(nameof(Resources.ParentScope))]
    public class ParentScope : NativeActivity
    {
        #region Properties

        [Browsable(false)]
        public ActivityAction<Application> Body { get; set; }

        [LocalizedCategory(nameof(Resources.Authentication))]
        [LocalizedDisplayName(nameof(Resources.ParentScopeUsernameDisplayName))]
        [LocalizedDescription(nameof(Resources.ParentScopeUsernameDescription))]
        public InArgument<string> Username { get; set; }

        [LocalizedCategory(nameof(Resources.Authentication))]
        [LocalizedDisplayName(nameof(Resources.ParentScopePasswordDisplayName))]
        [LocalizedDescription(nameof(Resources.ParentScopePasswordDescription))]
        public InArgument<string> Password { get; set; }

        [LocalizedCategory(nameof(Resources.Authentication))]
        [LocalizedDisplayName(nameof(Resources.ParentScopeURLDisplayName))]
        [LocalizedDescription(nameof(Resources.ParentScopeURLDescription))]
        public InArgument<string> URL { get; set; }

        internal static string ParentContainerPropertyTag => "ParentScope";

        #endregion


        #region Constructors

        public ParentScope()
        {

            Body = new ActivityAction<Application>
            {
                Argument = new DelegateInArgument<Application>(ParentContainerPropertyTag),
                Handler = new Sequence { DisplayName = "Do" }
            };
        }

        #endregion


        #region Private Methods

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }

        protected override void Execute(NativeActivityContext context)
        {
            //var username = Username.Get(context);
            //var password = Password.Get(context);
            //var url = URL.Get(context);
            //var application = new Application(username, password, url);
            
            //if (Body != null)
            //{
            //    context.ScheduleAction<Application>(Body, application, OnCompleted, OnFaulted);
            //}
        }

        private void OnFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            //TODO
        }

        private void OnCompleted(NativeActivityContext context, ActivityInstance completedInstance)
        {
            //TODO
        }

        #endregion


        #region Helpers
        
        #endregion
    }
}
