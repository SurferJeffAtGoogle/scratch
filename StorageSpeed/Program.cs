// Copyright(c) 2017 Google Inc.
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
using System.IO;
using CommandLine;
using Google.Cloud.Storage.V1;

[Verb("newfile", HelpText="Creates a big new file full of random data.")]
class NewFileOptions {
    [Value(0, HelpText="Name of the file to create", Required=true)]
    public string FileName {get; set;}

    [Option('g', HelpText="The size of the file to create, in gigabytes",
        Default=0.1)]
    public float Gigs { get; set; } 
}

[Verb("upload", HelpText="Uploads a file to Cloud Storage.")]
class UploadOptions {
    [Value(0, HelpText="Name of the file to upload", Required=true)]
    public string FileName { get; set; }

    [Value(1, HelpText="The name of the bucket to upload to", Required=true)]
    public string BucketName { get; set; }
}

namespace StorageSpeed
{
    class Program    
    {
        static object CreateLargeFile(string file, double gb)
        {
            int bufferSize = 512 * 1000;
            Random _rnd = new Random();
            Int64 size = (Int64)( 1000 * 1000 * 1000 * gb);
 
            Int64 bytesRemaining = size;
            byte[] buffer = new byte[bufferSize];
            using (Stream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                while (bytesRemaining > 0)
                {
                    int sizeOfChunkToWrite = (bytesRemaining > buffer.Length) ? buffer.Length : (int)bytesRemaining;
                    _rnd.NextBytes(buffer);
                    fileStream.Write(buffer, 0, sizeOfChunkToWrite);
                    bytesRemaining -= sizeOfChunkToWrite;
                }
                fileStream.Close();
            }
            return new FileInfo(file);
        }

        static object Upload(string fileName, string bucketName) {
            var client = StorageClient.Create();
            using (Stream fileStream =
                new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                client.UploadObject(bucketName, fileName, 
                    "application/octet-stream", fileStream);

            }
            return null;
        }
        
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                NewFileOptions, UploadOptions
                >(args).MapResult(
                    (NewFileOptions opts) => CreateLargeFile(opts.FileName, opts.Gigs),
                    (UploadOptions opts) => Upload(opts.FileName, opts.BucketName),
                    errs => 1);
        }
    }
}
