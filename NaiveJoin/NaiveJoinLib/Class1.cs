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
        async Task<RunQueryResponse> ExecuteAsync(string datasetId, IEnumerable<RunQueryRequest> requests)
        {            
            var tasks = new Dictionary<Task<RunQueryResponse>, RunQueryRequest>();
            // TODO: Use some kind of bit or flag value instead of int.  Otherwise,
            // cursor boundaries can cause bugs.
            var entityCounts = new Dictionary<Key, int>();

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
                tasks[_datastore.Datasets.RunQuery(request, datasetId).ExecuteAsync()] = request;
            }
            Task<RunQueryResponse> task = await Task.WhenAny(tasks.Keys);
            var batch = task.Result.Batch;
            var completeRequest = tasks[task];
            tasks.Remove(task);
            foreach (var entity in batch.EntityResults)
            {
                int count = 0;
                entityCounts.TryGetValue(entity.Entity.Key, out count);
                entityCounts[entity.Entity.Key] = ++count;
                if (count == requests.Count())
                    ; // yield result.
            }
            if (batch.EntityResults.Count() > 0)
            {
                completeRequest.Query.StartCursor = batch.EndCursor;
                tasks[_datastore.Datasets.RunQuery(completeRequest, datasetId).ExecuteAsync()] = completeRequest;
            }
        }
    }
}
