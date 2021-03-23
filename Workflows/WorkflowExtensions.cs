using System.Linq;
using MSTeamsConnector.Models;
using Sitecore.Data.Items;
using Sitecore.Security;
using Sitecore.Workflows;

namespace MSTeamsConnector.Workflows
{
    public static class WorkflowExtensions
    {
        private static WorkflowEvent[] GetHistory(Item item)
        {
            var workflowProvider = (item.Database.WorkflowProvider as Sitecore.Workflows.Simple.WorkflowProvider);

            return workflowProvider?.HistoryStore?.GetHistory(item);
        }

        public static TeamsMessage FillTeamsMessage(this TeamsMessage teamsMessage, Item item)
        {
            var workflowEvents = GetHistory(item);
            var oldState = item.GetLastActionOldState(workflowEvents);
            var newState = item.GetLastActionNewState(workflowEvents);

            teamsMessage.Author = item.GetAuthor();
            teamsMessage.User = item.GetUser(newState, workflowEvents);
            teamsMessage.WorkflowNewState = newState;
            teamsMessage.WorkflowOldState = oldState;

            return teamsMessage;
        }

        private static UserProfile GetUserProfile(this Item item)
        {
            var author = item.Statistics.CreatedBy;

            return Sitecore.Security.Accounts.User.FromName(author, false)?.Profile;
        }

        public static string GetUser(this Item item, string fromState, WorkflowEvent[] workflowEvents)
        {
            var user = workflowEvents.OrderBy(o => o.Date).LastOrDefault(w => w.OldState == fromState)?.User;

            if (string.IsNullOrEmpty(user))
            {
                return string.Empty;
            }

            var profile = Sitecore.Security.Accounts.User.FromName(user, false)?.Profile;

            return $"{profile?.FullName} - {profile?.Email}";
        }

        public static string GetAuthor(this Item item)
        {
            var authorProfile = item.GetUserProfile();

            return $"{authorProfile?.FullName} - {authorProfile?.Email}";
        }

        public static string GetLastActionOldState(this Item item, WorkflowEvent[] workflowEvents)
        {
            var state = workflowEvents.LastOrDefault()?.OldState;

            return string.IsNullOrEmpty(state) ? string.Empty : item.Database.GetItem(state).DisplayName;
        }

        public static string GetLastActionNewState(this Item item, WorkflowEvent[] workflowEvents)
        {
            var state = workflowEvents.LastOrDefault()?.NewState;

            return string.IsNullOrEmpty(state) ? string.Empty : item.Database.GetItem(state).DisplayName;
        }
    }
}