using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllGreen.WebServer.Core
{
    public class Spec
    {
        /*
        id: any;
        name: string;
        suite: ISuite;
        status: SpecStatus;
        steps: ISpecStep[];
        */

        public object Id { get; set; }
        public string Name { get; set; }
        public Suite Suite { get; set; }
        public SpecStatus Status { get; set; }
        public SpecStep[] Steps { get; set; }
    }

    public class Suite
    {
        /*
        id: any;
        name: string;
        parentSuite: ISuite;
        status: SpecStatus;
        */
        public object Id { get; set; }
        public string Name { get; set; }
        public Suite ParentSuite { get; set; }
        public SpecStatus Status { get; set; }
    }

    public class SpecStep
    {
        /*
        message: string;
        status: SpecStatus;
        trace: string;
        */
        public string Message { get; set; }
        public SpecStatus Status { get; set; }
        public string Trace { get; set; }
    }

    public enum SpecStatus
    {
        Running = 0,
        Failed = 1,
        Undefined = 2,
        Passed = 3,
        Skipped = 4
    }
}
