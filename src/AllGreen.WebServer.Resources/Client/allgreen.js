/// <reference path="reporter.ts" />
var AllGreen;
(function (AllGreen) {
    var AllGreenEnv = (function () {
        function AllGreenEnv() {
            this.adapterFactories = [];
            this.start = function () {
                var _this = this;
                $.each(this.adapterFactories, function (index, adapterFactory) {
                    adapterFactory.create(_this.reporter).start();
                });
            };
        }
        AllGreenEnv.getCurrent = function () {
            if (this.currentEnv == null)
                this.currentEnv = new AllGreenEnv();
            return this.currentEnv;
        };

        AllGreenEnv.prototype.setReporter = function (newReporter) {
            if (newReporter != null)
                this.reporter = newReporter;
        };

        AllGreenEnv.prototype.registerAdapterFactory = function (adapterFactory) {
            this.adapterFactories.push(adapterFactory);
        };

        AllGreenEnv.prototype.setServerStatus = function (status) {
            this.reporter.setServerStatus(status);
        };

        AllGreenEnv.prototype.reset = function () {
            this.adapterFactories = [];
            this.reporter.reset();
            $('#runner').prop('src', 'about:blank');
        };

        AllGreenEnv.prototype.reload = function () {
            this.reset();
            $('#runner').prop('src', 'runner.html');
        };
        AllGreenEnv.currentEnv = null;
        return AllGreenEnv;
    })();
    AllGreen.AllGreenEnv = AllGreenEnv;
})(AllGreen || (AllGreen = {}));
