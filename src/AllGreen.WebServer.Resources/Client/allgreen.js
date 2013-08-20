/// <reference path="reporter.ts" />
var AllGreen;
(function (AllGreen) {
    function startApp() {
        App.startApp();
    }
    AllGreen.startApp = startApp;

    var App = (function () {
        function App() {
            this.adapterFactories = [];
            this.start = function () {
                var _this = this;
                $.each(this.adapterFactories, function (index, adapterFactory) {
                    adapterFactory.create(_this.reporter).start();
                });
            };
        }
        App.startApp = function () {
            if (this.currentApp == null)
                this.currentApp = new App();
        };

        App.getCurrent = function () {
            return this.currentApp;
        };

        App.prototype.setReporter = function (newReporter) {
            if (newReporter != null)
                this.reporter = newReporter;
        };

        App.prototype.registerAdapterFactory = function (adapterFactory) {
            this.adapterFactories.push(adapterFactory);
        };

        App.prototype.setServerStatus = function (status) {
            this.reporter.setServerStatus(status);
        };

        App.prototype.reset = function () {
            this.adapterFactories = [];
            this.reporter.reset();
            $('#runner').prop('src', 'about:blank');
        };

        App.prototype.reload = function () {
            this.reset();
            $('#runner').prop('src', 'runner.html');
        };
        App.currentApp = null;
        return App;
    })();
    AllGreen.App = App;
})(AllGreen || (AllGreen = {}));
