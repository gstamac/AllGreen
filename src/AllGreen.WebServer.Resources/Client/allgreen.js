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

    var App = (function () {
        function App() {
            this.serverReporter = null;
            this.runnerReporters = [];
            this.adapterFactories = [];
            this.runTests = function () {
                this.delayStartIfNeeded(this, this.adapterFactories);
            };
            this.reconnectEnabled = true;
        }
        App.prototype.setServerReporter = function (serverReporter) {
            if (serverReporter != null)
                this.serverReporter = serverReporter;
        };

        App.prototype.registerRunnerReporter = function (runnerReporter) {
            if (runnerReporter != null)
                this.runnerReporters.push(runnerReporter);
        };

        App.prototype.registerAdapterFactory = function (adapterFactory) {
            var newName = adapterFactory.getName();
            var found = false;
            this.adapterFactories.forEach(function (adapterFactory) {
                found = found || adapterFactory.getName() == newName;
            });
            if (!found)
                this.adapterFactories.push(adapterFactory);
        };

        App.prototype.delayStartIfNeeded = function (app, adapterFactories) {
            var _this = this;
            if (!app.isReady()) {
                console.log('delaying test run');
                setTimeout(function () {
                    return _this.delayStartIfNeeded(app, adapterFactories);
                }, 10);
            } else {
                adapterFactories.forEach(function (adapterFactory) {
                    adapterFactory.create(app).start();
                });
            }
        };

        App.prototype.setServerStatus = function (status) {
            this.serverReporter.setServerStatus(status);
        };

        App.prototype.isReady = function () {
            for (var i = 0; i < this.runnerReporters.length; i++) {
                if (!this.runnerReporters[i].isReady()) {
                    return false;
                }
            }
            return true;
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
            $('#runner-iframe').prop('src', '/~internal~/Client/runner.html');
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
