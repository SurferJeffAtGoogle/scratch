﻿// Copyright (c) 2019 Google LLC.
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

namespace GuessWord
{
    /// <summary>
    /// Options loaded from appsettings.json for configuring data protection.
    /// This class isn't strictly necessary - it just means you only need a single call
    /// accessing the Configuration to fetch all the values.
    /// </summary>
    public class DataProtectionOptions
    {
        public string Bucket { get; set; }
        public string Object { get; set; }
        public string KmsKeyName { get; set; }
    }
}
