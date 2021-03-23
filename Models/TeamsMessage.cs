namespace MSTeamsConnector.Models
{
    public class TeamsMessage
    {
        public string Author { get; set; }
        public string User { get; set; }
        public string Language { get; set; }
        public string Comments { get; set; }
        public string HostUrl { get; set; }
        public string ItemPath { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string WorkflowOldState { get; set; }
        public string WorkflowNewState { get; set; }
        public string ItemUrl { get; set; }
        public string Site { get; set; }
        public string Command { get; set; }
        public string MSTeamsConnectionString { get; set; }

    }
}