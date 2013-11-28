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
        status: SpecStatus;
        filename: string;
        lineNumber: number;
        trace: string;
    }

    export interface IAdapter {
        start();
    }

    export interface IAdapterFactory {
        create(reporter: IRunnerReporter): IAdapter;
        getName(): string;
    }

    export class App implements IServerReporter, IRunnerReporter {
        constructor() { }

        private serverReporter: IServerReporter = null;
        private runnerReporters: IRunnerReporter[] = [];
        private adapterFactories: IAdapterFactory[] = [];

        public setServerReporter(serverReporter: IServerReporter) {
            if (serverReporter != null)
                this.serverReporter = serverReporter;
        }

        public registerRunnerReporter(runnerReporter: IRunnerReporter) {
            if (runnerReporter != null)
                this.runnerReporters.push(runnerReporter);
        }

        public registerAdapterFactory(adapterFactory: IAdapterFactory) {
            var newName = adapterFactory.getName();
            var found: boolean = false;
            this.adapterFactories.forEach((adapterFactory: IAdapterFactory) =>
            { found = found || adapterFactory.getName() == newName; });
            if (!found)
                this.adapterFactories.push(adapterFactory);
        }

        public runTests = function () {
            this.delayStartIfNeeded(this, this.adapterFactories);
        }

        private delayStartIfNeeded(app: App, adapterFactories: IAdapterFactory[]) {
            if (!app.isReady()) {
                console.log('delaying test run');
                setTimeout(() => this.delayStartIfNeeded(app, adapterFactories), 10);
            }
            else {
                adapterFactories.forEach((adapterFactory: IAdapterFactory) =>
                { adapterFactory.create(app).start(); });
            }
        }

        public setServerStatus(status: string) {
            this.serverReporter.setServerStatus(status);
        }

        public isReady(): boolean {
            for (var i = 0; i < this.runnerReporters.length; i++) {
                if (!this.runnerReporters[i].isReady()) {
                    return false;
                }
            }
            return true;
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

        public reload() {
            this.adapterFactories = [];
            this.reset();
            $('#runner-iframe').prop('src', '/~internal~/Client/runner.html');
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

