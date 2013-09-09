/// <reference path="reporter.ts" />
var AllGreen;
(function (AllGreen) {
    function startApp() {
        App.startApp();
    }
    AllGreen.startApp = startApp;

    (function (SpecStatus) {
        SpecStatus[SpecStatus["Running"] = 0] = "Running";
        SpecStatus[SpecStatus["Failed"] = 1] = "Failed";
        SpecStatus[SpecStatus["Undefined"] = 2] = "Undefined";
        SpecStatus[SpecStatus["Passed"] = 3] = "Passed";
        SpecStatus[SpecStatus["Skipped"] = 4] = "Skipped";
    })(AllGreen.SpecStatus || (AllGreen.SpecStatus = {}));
    var SpecStatus = AllGreen.SpecStatus;

    var App = (function () {
        function App() {
            this.serverReporter = null;
            this.runnerReporters = [];
            this.adapterFactories = [];
            this.start = function () {
                var reporter = this;
                this.adapterFactories.forEach(function (adapterFactory) {
                    adapterFactory.create(reporter).start();
                });
            };
            this.reconnectEnabled = true;
        }
        App.startApp = function () {
            if (this.currentApp == null)
                this.currentApp = new App();
        };

        App.getCurrent = function () {
            return this.currentApp;
        };

        App.prototype.setServerReporter = function (serverReporter) {
            if (serverReporter != null)
                this.serverReporter = serverReporter;
        };

        App.prototype.registerRunnerReporter = function (runnerReporter) {
            if (runnerReporter != null)
                this.runnerReporters.push(runnerReporter);
        };

        App.prototype.registerAdapterFactory = function (adapterFactory) {
            this.adapterFactories.push(adapterFactory);
        };

        App.prototype.setServerStatus = function (status) {
            this.serverReporter.setServerStatus(status);
        };

        App.prototype.reset = function () {
            this.runnerReporters.forEach(function (runnerReporter) {
                runnerReporter.reset();
            });
        };

        App.prototype.started = function (totalSpecs) {
            this.runnerReporters.forEach(function (runnerReporter) {
                runnerReporter.started(totalSpecs);
            });
        };

        App.prototype.specUpdated = function (spec) {
            this.runnerReporters.forEach(function (runnerReporter) {
                runnerReporter.specUpdated(spec);
            });
        };

        App.prototype.finished = function () {
            this.runnerReporters.forEach(function (runnerReporter) {
                runnerReporter.finished();
            });
        };

        App.prototype.reload = function () {
            this.adapterFactories = [];
            this.reset();
            $('#runner').prop('src', 'runner.html');
        };

        App.prototype.log = function (message) {
            var optionalParams = [];
            for (var _i = 0; _i < (arguments.length - 1); _i++) {
                optionalParams[_i] = arguments[_i + 1];
            }
            console.log(message, optionalParams);
        };

        App.prototype.enableReconnect = function (enabled) {
            this.log('Reconnect ' + (enabled ? 'enabled' : 'disabled'));
            this.reconnectEnabled = enabled;
        };
        App.currentApp = null;
        return App;
    })();
    AllGreen.App = App;
})(AllGreen || (AllGreen = {}));
