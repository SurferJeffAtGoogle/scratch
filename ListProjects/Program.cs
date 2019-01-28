using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudResourceManager.v1;
using Google.Apis.Services;
using System;

namespace ListProjects
{
    class Program
    {
        static void Main(string[] args)
        {
 
             GoogleCredential credential =
                GoogleCredential.GetApplicationDefault();
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    CloudResourceManagerService.Scope.CloudPlatformReadOnly
                });
            }
            var crmService = new CloudResourceManagerService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });
            var request = new ProjectsResource.ListRequest(crmService);
            while (true)
            {
                var result = request.Execute();
                foreach (var project in result.Projects)
                {
                    Console.WriteLine(project.ProjectId);
                }
                if (string.IsNullOrEmpty(result.NextPageToken))
                {
                    break;
                }
                request.PageToken = result.NextPageToken;
            }
        }
    }
}
