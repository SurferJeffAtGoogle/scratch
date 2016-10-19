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
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;
using Google.Apis.Services;
using Google.Datastore.V1;
using System;

namespace PubSubDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectId = "bookshelf-dotnet";
            string topicName = "book-process-queue";
            string topicPath = $"projects/{projectId}/topics/{topicName}";
            var db = DatastoreDb.Create(projectId);
            var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.
                GetApplicationDefaultAsync().Result;
            if (credentials.IsCreateScopedRequired)
            {
                credentials = credentials.CreateScoped(new[] { PubsubService.Scope.Pubsub });
            }
            PubsubService pubsub = new PubsubService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
            });
            try
            {
                pubsub.Projects.Topics.Create(new Topic() { Name = topicPath }, topicPath)
                    .Execute();
                Console.WriteLine($"Created topic {topicPath}.");
            }
            catch (Google.GoogleApiException e) when (e.Error.Code == 409)
            {
                Console.WriteLine($"The topic {topicPath} already exists.");
            }
        }
    }
}
