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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

public interface IStartSession
{
    void StartSession(ISession session);
}

public class DistributedSessionStoreWithStart : ISessionStore
{
    DistributedSessionStore innerStore;
    IStartSession startSession;
    public DistributedSessionStoreWithStart(IDistributedCache cache, 
        ILoggerFactory loggerFactory, IStartSession startSession)
    {
        innerStore = new DistributedSessionStore(cache, loggerFactory);
        this.startSession = startSession;
    }

    public ISession Create(string sessionKey, TimeSpan idleTimeout, 
        TimeSpan ioTimeout, Func<bool> tryEstablishSession, 
        bool isNewSessionKey)
    {
        ISession session = innerStore.Create(sessionKey, idleTimeout, ioTimeout,
             tryEstablishSession, isNewSessionKey);
        if (isNewSessionKey)
        {
            startSession.StartSession(session);
        }
        return session;
    }
}
