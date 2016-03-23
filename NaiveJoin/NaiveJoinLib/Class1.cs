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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    public class NaiveJoiner
    {
        DatastoreService _datastore;
        int _batchSize = 100;
        NaiveJoiner(DatastoreService datastore)
        {
            _datastore = datastore;
        }

        async Task<LookupResponse> ExecuteAsync(string datasetId, RunQueryRequest[] requests)
        {            
            var requestIndices = new Dictionary<Task<RunQueryResponse>, int>();
            // Maps an entity key to the requests that found it.
            // The value is a bitmap of indices of requests that found the entity.
            var foundFlags = new Dictionary<Key, int>();
            Debug.Assert(requests.Count() < 8 * sizeof(int) - 1,
                $"Really?  You want to join more than {sizeof(int) - 1} queries?");
            int foundAll = (1 << requests.Count()) - 1;
            int limit = 0;
            QueryResultBatch resultBatch = new QueryResultBatch()
            {
                EntityResults = new List<EntityResult>()
            };
            var matchingKeys = new List<Key>(); 

            int requestIndex = 0;
            foreach (RunQueryRequest request in requests)
            {
                request.Query.Projection = new PropertyExpression[]
                {
                    new PropertyExpression()
                    {
                        Property = new PropertyReference()
                        {
                            Name = "__key__",
                        }
                    }
                };
                request.Query.Limit = _batchSize;
                requestIndices[_datastore.Datasets.RunQuery(request, datasetId).ExecuteAsync()] = requestIndex++;
            }
            Task<RunQueryResponse> task = await Task.WhenAny(requestIndices.Keys);
            var batch = task.Result.Batch;
            requestIndex = requestIndices[task];
            requestIndices.Remove(task);
            foreach (var entity in batch.EntityResults)
            {
                int flags = 0;
                foundFlags.TryGetValue(entity.Entity.Key, out flags);
                if (flags == foundAll)
                    continue;  // Already found and added to the results
                flags |= requestIndex;
                if (flags == foundAll)
                {
                    matchingKeys.Add(entity.Entity.Key);
                    if (matchingKeys.Count >= limit)
                    {
                        return await _datastore.Datasets.Lookup(new LookupRequest()
                        {
                            Keys = matchingKeys
                        }, datasetId).ExecuteAsync();
                    }
                }
            }
            if (batch.EntityResults.Count() > 0)
            {
                requests[requestIndex].Query.StartCursor = batch.EndCursor;
                requestIndices[_datastore.Datasets.RunQuery(requests[requestIndex], datasetId).ExecuteAsync()] = requestIndex;
            }
        }
    }
}
