// Copyright (c) 2018 Google LLC.
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

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

public class CacheWithStart : IDistributedCache
{
    public CacheWithStart(IDistributedCache innerCache, Func<byte[]> start)
    {
        InnerCache = innerCache;
        Start = start;
    }

    public IDistributedCache InnerCache { get; }
    public Func<byte[]> Start { get; }

    public byte[] Get(string key)
    {
        var value = InnerCache.Get(key);
        if (value == null)
            value = Start();
        return value;
    }

    public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
    {
        var value = await InnerCache.GetAsync(key, token);
        return value == null ? Start() : value;
    }

    public void Refresh(string key)
    {
        InnerCache.Refresh(key);
    }

    public Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
    {
        return InnerCache.RefreshAsync(key, token);
    }

    public void Remove(string key)
    {
        InnerCache.Remove(key);
    }

    public Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
    {
        return InnerCache.RemoveAsync(key, token);
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        InnerCache.Set(key, value, options);
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
    {
        return InnerCache.SetAsync(key, value, options, token);
    }
}
