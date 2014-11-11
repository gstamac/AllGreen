/// <reference path="reporter.ts" />
var AllGreenApp = null;

var AllGreen;
(function (AllGreen) {
    (function (SpecStatus) {
        SpecStatus[SpecStatus["Undefined"] = 0] = "Undefined";
        SpecStatus[SpecStatus["Running"] = 1] = "Running";
        SpecStatus[SpecStatus["Passed"] = 2] = "Passed";
        SpecStatus[SpecStatus["Failed"] = 3] = "Failed";
        SpecStatus[SpecStatus["Skipped"] = 4] = "Skipped";
    })(AllGreen.SpecStatus || (AllGreen.SpecStatus = {}));
    var SpecStatus = AllGreen.SpecStatus;

    var CompositeRunnerReporter = (function () {
        function CompositeRunnerReporter() {
            this.runnerReporters = [];
        }
        CompositeRunnerReporter.prototype.registerRunnerReporter = function (runnerReporter) {
            if (runnerReporter != null)
                this.runnerReporters.push(runnerReporter);
        };

        CompositeRunnerReporter.prototype.isReady = function () {
            return this.runnerReporters.every(function (runnerReporter) {
                return runnerReporter.isReady();
            });
        };

        CompositeRunnerReporter.prototype.reset = function () {
            this.runnerReporters.forEach(function (runnerReporter) {
                runnerReporter.reset();
            });
        };

        CompositeRunnerReporter.prototype.started = function (totalSpecs) {
            this.runnerReporters.forEach(function (runnerReporter) {
                runnerReporter.started(totalSpecs);
            });
        };

        CompositeRunnerReporter.prototype.specUpdated = function (spec) {
            this.runnerReporters.forEach(function (runnerReporter) {
                runnerReporter.specUpdated(spec);
            });
        };

        CompositeRunnerReporter.prototype.finished = function () {
            this.runnerReporters.forEach(function (runnerReporter) {
                runnerReporter.finished();
            });
        };
        return CompositeRunnerReporter;
    })();
    AllGreen.CompositeRunnerReporter = CompositeRunnerReporter;

    var App = (function () {
        function App() {
            this.serverReporter = null;
            this.adapterFactories = [];
            this.runnerLoaded = function () {
                this.delayRunTestsIfNeeded(this.compositeRunnerReporter);
            };
            this.reconnectEnabled = true;
            this.compositeRunnerReporter = new CompositeRunnerReporter();
        }
        App.prototype.setServerReporter = function (serverReporter) {
            if (serverReporter != null)
                this.serverReporter = serverReporter;
        };

        App.prototype.registerRunnerReporter = function (runnerReporter) {
            this.compositeRunnerReporter.registerRunnerReporter(runnerReporter);
        };

        App.prototype.registerAdapterFactory = function (adapterFactory) {
            var newName = adapterFactory.getName();
            if (this.adapterFactories.every(function (adapterFactory) {
                return adapterFactory.getName() != newName;
            }))
                this.adapterFactories.push(adapterFactory);
        };

        App.prototype.setServerStatus = function (status) {
            this.serverReporter.setServerStatus(status);
        };

        App.prototype.runTests = function () {
            this.adapterFactories = [];
            this.compositeRunnerReporter.reset();
            this.reloadRunner();
        };

        App.prototype.reloadRunner = function () {
            $('#runner-iframe').prop('src', '/~internal~/Client/runner.html');
        };

        App.prototype.delayRunTestsIfNeeded = function () {
            var _this = this;
            if (!this.compositeRunnerReporter.isReady()) {
                console.log('delaying test run');
                setTimeout(function () {
                    return _this.delayRunTestsIfNeeded();
                }, 10);
            } else {
                this.adapterFactories.forEach(function (adapterFactory) {
                    adapterFactory.create(_this.compositeRunnerReporter).runTests();
                });
            }
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
        return App;
    })();
    AllGreen.App = App;
})(AllGreen || (AllGreen = {}));
//# sourceMappingURL=allgreen.js.map
