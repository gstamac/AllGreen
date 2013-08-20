/// <reference path="reporter.ts" />

module AllGreen {

    export function startApp() {
        App.startApp();
    }

    export interface IAdapter {
        start();
    }

    export interface IAdapterFactory {
        create(reporter: IReporter): IAdapter;
    }

    export class App {
        constructor() { }

        private static currentApp: App = null;

        public static startApp() {
            if (this.currentApp == null) this.currentApp = new App();
        }

        public static getCurrent(): App {
            return this.currentApp;
        }

        private reporter: IReporter;
        private adapterFactories: IAdapterFactory[] = [];

        public setReporter(newReporter: IReporter) {
            if (newReporter != null)
                this.reporter = newReporter;
        }

        public registerAdapterFactory(adapterFactory: IAdapterFactory) {
            this.adapterFactories.push(adapterFactory);
        }

        public start = function () {
            $.each(this.adapterFactories,
                (index, adapterFactory: IAdapterFactory) =>
                { adapterFactory.create(this.reporter).start(); });
        }

        public setServerStatus(status: string) {
            this.reporter.setServerStatus(status);
        }

        public reset() {
            this.adapterFactories = [];
            this.reporter.reset();
            $('#runner').prop('src', 'about:blank');
        }

        public reload() {
            this.reset();
            $('#runner').prop('src', 'runner.html');
        }
    }

}

