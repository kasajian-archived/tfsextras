using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace tfextras
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private static void Run(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"tfsextras [command] [options...]");
                Console.WriteLine();
                Console.WriteLine("Commands:");
                Console.WriteLine("    linkparent");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("    -collection      --> tfs url");
                Console.WriteLine("    -project         --> teamproject");
                Console.WriteLine("    -workitemidfrom  --> id of the first work item");
                Console.WriteLine("    -workitemidto    --> id of the second work item");
                Console.WriteLine();
                Console.WriteLine(@"Example:");
                Console.WriteLine(
                    @"tfsextras linkparent -collection http://yourtfs:8080/tfs/ProjectCollection -project teamprojectname -workitemidfrom childWorkItemId -workitemidto parentWorkItemId");
                return;
            }

            var collectionUri = GetArgument(args, "-collection");
            var teamProjectName = GetArgument(args, "-project");
            var workitemidfrom = int.Parse(GetArgument(args, "-workitemidfrom"));
            var workitemidto = int.Parse(GetArgument(args, "-workitemidto"));

            TfsExtras(collectionUri, teamProjectName, workitemidfrom, workitemidto);
        }

        private static string GetArgument(IEnumerable<string> args, string option)
        {
            return args.SkipWhile(i => i != option).Skip(1).Take(1).FirstOrDefault();
        }

        private static void TfsExtras(string collectionUri, string teamProjectName, int childWorkItemId, int parentWorkItemId)
        {
            using (var tpc = new TfsTeamProjectCollection(new Uri(collectionUri)))
            {
                var workItemStore = tpc.GetService<WorkItemStore>();
                var linkType = workItemStore.WorkItemLinkTypes[CoreLinkTypeReferenceNames.Hierarchy];
                var link = new WorkItemLink(linkType.ReverseEnd, parentWorkItemId);

                var workitem = workItemStore.GetWorkItem(childWorkItemId);
                workitem.WorkItemLinks.Add(link);
                workitem.Save();
            }
        }
    }
}
