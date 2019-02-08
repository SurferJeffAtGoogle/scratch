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
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        Program p = new Program();
        p.Run();
    }

    public void Run()
    {
        var host = CreateCompositionContainer();
        host.GetExport<IParentService>().DoSomething();
        host.GetExport<IParentService>().DoSomething();
    }

    private static CompositionHost CreateCompositionContainer()
    {
        var assemblies = new[] { typeof(Program).GetTypeInfo().Assembly };
        var configuration = new ContainerConfiguration()
           .WithAssembly(typeof(Program).GetTypeInfo().Assembly);
        return configuration.CreateContainer();
    }

    public interface IParentService
    {
        void DoSomething();
    }

    public interface IChildService
    {
        void DoSomething();
    }

    [Export(typeof(IParentService))]
    internal class ParentService : IParentService
    {
        Options _option;
        [ImportingConstructor]
        public ParentService([Import] Options options, IChildService secondSender)
        {
            _option = options;
            Console.WriteLine("Parent Service constructed.");
        }
        public void DoSomething()
        {
            Console.WriteLine("Parent Service is doing something!!");
        }
    }

    [Export(typeof(IChildService)), Shared]
    internal class ChildService : IChildService
    {
        Options _option;
        [ImportingConstructor]
        public ChildService([Import] Options options)
        {
            _option = options;
            Console.WriteLine("Child Service constructed.");
        }
        public void DoSomething()
        {
            Console.WriteLine("Child Service is doing something!!!");
        }
    }

    [Export(typeof(Options))]
    internal sealed class Options
    {
        internal List<string> StringList { get; set; }
        [ImportingConstructor]
        public Options()
        {
            StringList = new List<string>();
        }
    }
}
