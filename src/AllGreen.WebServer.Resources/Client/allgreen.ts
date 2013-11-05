/// <reference path="reporter.ts" />

module AllGreen {

    export function startApp() {
        App.startApp();
    }

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
        trace: string;
    }

    export interface IAdapter {
        start();
    }

    export interface IAdapterFactory {
        create(reporter: IRunnerReporter): IAdapter;
    }

    export class App implements IServerReporter, IRunnerReporter {
        constructor() { }

        private static currentApp: App = null;

        public static startApp() {
            if (this.currentApp == null) this.currentApp = new App();
        }

        public static getCurrent(): App {
            return this.currentApp;
        }

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
            this.adapterFactories.push(adapterFactory);
        }

        public start = function () {
            this.delayStartIfNeeded(this, this.adapterFactories);
        }

        private delayStartIfNeeded(reporter: App, adapterFactories: IAdapterFactory[]) {
            if (!reporter.isReady()) {
                setTimeout(() =>  this.delayStartIfNeeded(reporter, adapterFactories), 10);
            }
            else {
                adapterFactories.forEach((adapterFactory: IAdapterFactory) =>
                    { adapterFactory.create(reporter).start(); });
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
            $('#runner').prop('src', 'Client/runner.html');
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

