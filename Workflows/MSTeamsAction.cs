using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using MSTeamsConnector.Models;
using MSTeamsConnector.Services;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Sites;
using Sitecore.Web;
using Sitecore.Workflows.Simple;

namespace MSTeamsConnector.Workflows
{
    /// <summary>Represents an Email Action.</summary>
    public class SendNotification
    {
        IItemSiteResolver _siteResolver;

        public SendNotification(IItemSiteResolver siteResolver)
        {
            _siteResolver = siteResolver;
        }

        public SendNotification()
        {
            _siteResolver = ServiceLocator.ServiceProvider.GetService<IItemSiteResolver>();
        }

        /// <summary>Runs the processor.</summary>
        /// <param name="args">The arguments.</param>
        public async Task Process(WorkflowPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));

            var processorItem = args.ProcessorItem;

            if (processorItem == null)
            {
                return;
            }

            var item = args.DataItem;
            var site = _siteResolver.ResolveSite(item);
            var teamsMessage = new TeamsMessage
            {
                HostUrl = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}",
                ItemId = HttpUtility.UrlEncode(item.ID.ToString()),
                ItemName = item.DisplayName,
                ItemPath = item.Paths.FullPath,
                Language = item.Language.Name,
                ItemUrl = GetItemUrl(item, site),
                Site = site.Name,
                Command = processorItem.InnerItem.Parent.Name,
                Comments = args.CommentFields["Comments"]
            };

            teamsMessage.FillTeamsMessage(item);

            await new FunctionService().DoPostAsync(teamsMessage);
        }

        private static string GetItemUrl(Item item, SiteInfo site)
        {
            using (new SiteContextSwitcher(SiteContext.GetSite(site.Name)))
            {
                var urlOptions = LinkManager.GetDefaultUrlBuilderOptions();

                urlOptions.AlwaysIncludeServerUrl = true;
                urlOptions.ShortenUrls = true;
                urlOptions.SiteResolving = true;
                
                return LinkManager.GetItemUrl(item, urlOptions);
            }
        }
    }
}
