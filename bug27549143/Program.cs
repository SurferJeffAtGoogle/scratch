using Google.Apis.Datastore.v1beta2;
using Google.Apis.Datastore.v1beta2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bug27549143
{
    class Program
    {
        static void Main(string[] args)
        {
            var projectId = args[1];
            // Use Application Default Credentials.
            var credentials = Google.Apis.Auth.OAuth2.GoogleCredential
                .GetApplicationDefaultAsync().Result;
            credentials = credentials.CreateScoped(new[] {
                DatastoreService.Scope.Datastore
            });
            // Create our connection to datastore.
            var datastore = new DatastoreService(new Google.Apis.Services
                .BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
            });

            var query = new Query()
            {
                Limit = 100,
                Kinds = new[] { new KindExpression() { Name = "Book" } },
            };

            var datastoreRequest = datastore.Datasets.RunQuery(
                datasetId: projectId,
                body: new RunQueryRequest() { Query = query }
            );

            var response = datastoreRequest.Execute();
            var results = response.Batch.EntityResults;
            foreach (var result in results)
            {
                Property value;
                bool found = result.Entity.Properties.TryGetValue("Title", out value);
                Console.WriteLine(value.StringValue);
            }
        }
    }
}
