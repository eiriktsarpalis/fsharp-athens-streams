using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Nessos.MBrace.Azure.Runtime;
using Nessos.MBrace.Azure.Store;
using Nessos.MBrace.Azure.Runtime.Common;
using Nessos.MBrace.Store;

namespace MBraceWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private Service _svc;

        public override void Run()
        {
            _svc.Start();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            bool result = base.OnStart();

            var config = Configuration.Default
                            .WithStorageConnectionString("")
                            .WithServiceBusConnectionString("");

            var blobStore = new BlobStore(config.StorageConnectionString);
            var storeConfig = new CloudFileStoreConfiguration(blobStore, defaultDirectory: "mbracestore");

            _svc = new Service(config, maxTasks: 10, storeConfig: storeConfig, serviceId: RoleEnvironment.CurrentRoleInstance.Id);
            _svc.AttachLogger(new CustomLogger(s => Trace.WriteLine(s)));

            return result;
        }

        public override void OnStop()
        {
            base.OnStop();
        }
    }
}
