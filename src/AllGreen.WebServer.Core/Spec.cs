using System;

namespace AllGreen.WebServer.Core
{
    public class Spec
    {
        /*
        id: string;
        name: string;
        suite: ISuite;
        status: SpecStatus;
        time: number;
        steps: ISpecStep[];
        */

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Suite Suite { get; set; }
        public SpecStatus Status { get; set; }
        public UInt64 Time { get; set; }
        public SpecStep[] Steps { get; set; }

        public string GetFullName()
        {
            string fullName = Name;
            Suite suite = this.Suite;
            while (suite != null)
            {
                fullName = suite.Name + " -> " + fullName;
                suite = suite.ParentSuite;
            }
            return fullName;
        }
    }

    public class Suite
    {
        /*
        id: string;
        name: string;
        parentSuite: ISuite;
        status: SpecStatus;
        */
        public Guid Id { get; set; }
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
        Undefined = 0,
        Running = 1,
        Passed = 2,
        Failed = 3,
        Skipped = 4
    }
}
