/*
 * Copyright (c) 2018 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;

namespace StackOverflow53251345
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceAccounts(File.ReadAllText(args[0]), args[1]);
        }

        static IamService s_service;

        private static void ServiceAccounts(string credentialsJson, string userEmail)
        {
            LoginAsServiceAccountWithJson(credentialsJson);

            IList<ServiceAccountKey> serviceAccountKeys = ListKeysOfServiceAccount(userEmail);

            ServiceAccountKey newKey = CreateKeyOfServiceAccount(userEmail);

            //DeleteKeyOfServiceAccount(newKey);

            // LoginWithServiceAccountKey(newKey, credentialsJson);
        }

        public static void LoginAsServiceAccountWithJson(string credentialsJson)
        {
            // Create credentials from the JSON file that we receive from GCP.
            GoogleCredential credential = GoogleCredential.FromJson(credentialsJson)
            .CreateScoped(IamService.Scope.CloudPlatform);

            s_service = new IamService(new IamService.Initializer
            {
                HttpClientInitializer = credential
            });

            ListRolesResponse response = s_service.Roles.List().Execute();
        }

        public static ServiceAccountKey CreateKeyOfServiceAccount(string userEmail)
        {
            var newKeyDetails = s_service.Projects.ServiceAccounts.Keys.Create(
                new CreateServiceAccountKeyRequest(), "projects/-/serviceAccounts/" + userEmail);

            ServiceAccountKey key = newKeyDetails.Execute();
            return key;
        }

        public static IList<ServiceAccountKey> ListKeysOfServiceAccount(string userEmail)
        {

            IList<ServiceAccountKey> keys = s_service.Projects.ServiceAccounts.Keys
                .List($"projects/-/serviceAccounts/{userEmail}").Execute().Keys;

            return keys;
        }
    }
}
