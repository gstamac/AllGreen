/// <reference path="reporter.ts" />

var AllGreenApp = null;

module AllGreen {

    export interface IServerReporter {
        setServerStatus(status: string);
    }

    export interface IRunnerReporter {
        isReady(): boolean;
        reset();
        started(totalSpecs: number);
        specUpdated(spec: ISpec);
        finished();
    }

    export enum SpecStatus {
        Undefined = 0,
        Running = 1,
        Passed = 2,
        Failed = 3,
        Skipped = 4
    }

    export interface ISpec {
        id: any;
        name: string;
        suite: ISuite;
        status: SpecStatus;
        time: number;
        steps: ISpecStep[];
    }

    export interface ISuite {
        id: any;
        name: string;
        parentSuite: ISuite;
        status: SpecStatus;
    }

    export interface ISpecStep {
        message: string;
        errorLocation: string;
        status: SpecStatus;
        trace: string;
    }

    export interface IAdapter {
        runTests();
    }

    export interface IAdapterFactory {
        create(reporter: IRunnerReporter): IAdapter;
        getName(): string;
    }

    export class CompositeRunnerReporter implements IRunnerReporter
    {
        private runnerReporters: IRunnerReporter[] = [];

        public registerRunnerReporter(runnerReporter: IRunnerReporter) {
            if (runnerReporter != null)
                this.runnerReporters.push(runnerReporter);
        }

        public isReady(): boolean {
            return this.runnerReporters.every((runnerReporter) => runnerReporter.isReady());
        }

        public reset() {
            this.runnerReporters.forEach((runnerReporter) => {
                runnerReporter.reset();
            });
        }

        public started(totalSpecs: number) {
            this.runnerReporters.forEach((runnerReporter) => {
                runnerReporter.started(totalSpecs);
            });
        }

        public specUpdated(spec: ISpec) {
            this.runnerReporters.forEach((runnerReporter) => {
                runnerReporter.specUpdated(spec);
            });
        }

        public finished() {
            this.runnerReporters.forEach((runnerReporter) => {
                runnerReporter.finished();
            });
        }
    }

    export class App {
        private serverReporter: IServerReporter = null;
        private compositeRunnerReporter: CompositeRunnerReporter;
        private adapterFactories: IAdapterFactory[] = [];

        constructor() {
            this.compositeRunnerReporter = new CompositeRunnerReporter();
        }

        public setServerReporter(serverReporter: IServerReporter) {
            if (serverReporter != null)
                this.serverReporter = serverReporter;
        }

        public registerRunnerReporter(runnerReporter: IRunnerReporter) {
            this.compositeRunnerReporter.registerRunnerReporter(runnerReporter);
        }

        public registerAdapterFactory(adapterFactory: IAdapterFactory) {
            var newName = adapterFactory.getName();
            if (this.adapterFactories.every((adapterFactory) => adapterFactory.getName() != newName))
                this.adapterFactories.push(adapterFactory);
        }

        public setServerStatus(status: string) {
            this.serverReporter.setServerStatus(status);
        }

        public runTests() {
            this.adapterFactories = [];
            this.compositeRunnerReporter.reset();
            this.reloadRunner();
        }

        private reloadRunner() {
            $('#runner-iframe').prop('src', '/~internal~/Client/runner.html');
        }

        public runnerLoaded = function () {
            this.delayRunTestsIfNeeded(this.compositeRunnerReporter);
        }

        private delayRunTestsIfNeeded() {
            if (!this.compositeRunnerReporter.isReady()) {
                console.log('delaying test run');
                setTimeout(() => this.delayRunTestsIfNeeded(), 10);
            }
            else {
                this.adapterFactories.forEach((adapterFactory) =>
                { adapterFactory.create(this.compositeRunnerReporter).runTests(); });
            }
        }

        public log(message?: any, ...optionalParams: any[]): void {
            console.log(message, optionalParams);
        }

        public reconnectEnabled: boolean = true;

        public enableReconnect(enabled: boolean) {
            this.log('Reconnect ' + (enabled ? 'enabled' : 'disabled'));
            this.reconnectEnabled = enabled;
        }
    }

}

