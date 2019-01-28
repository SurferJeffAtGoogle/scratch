// Copyright (c) 2019 Google LLC.
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

namespace InitConstruct
{
    class C 
    {
        private string _s;
        public string S
        {
            get { return _s;}
            set 
            { 
                Console.WriteLine("Setting S to " + value);
                _s  = value;
            }
        }
        
        public C(string s) { 
            _s = s; 
            Console.WriteLine("Constructing C with " + s);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var c = new C("A") {
                S = "B"
            };
            Console.WriteLine("c.S: " + c.S);
        }
    }
}
