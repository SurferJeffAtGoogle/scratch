// Copyright(c) 2016 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Google.Apis.Datastore.v1beta2;
using Google.Apis.Datastore.v1beta2.Data;
using System;

namespace bug27549143
{
    internal class Program
    {
        static readonly string _usage = @"Usage:
    {0} google-project-id

Attempts to read some entities from datastore.

When I set GOOGLE_APPLICATION_CREDENTIALS to my downloaded service account .json file,
I get the following error:

Unhandled Exception: Google.GoogleApiException: Google.Apis.Requests.RequestError
Invalid Credentials [401]
Errors [
        Message[Invalid Credentials] Location[Authorization - header] Reason[authError] Domain[global]
]";
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                string binPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                Console.WriteLine(String.Format(_usage, System.IO.Path.GetFileName(binPath)));
                return;
            }
            var projectId = args[0];
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