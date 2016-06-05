System.register([], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var App;
    return {
        setters:[],
        execute: function() {
            App = (function () {
                function App() {
                }
                App.prototype.configureRouter = function (config, router) {
                    config.title = 'Aurelia';
                    config.map([
                        { route: ['', 'home'], name: 'home', moduleId: './views/home', nav: true, title: 'CSharp' },
                    ]);
                    this.router = router;
                };
                return App;
            }());
            exports_1("App", App);
        }
    }
});
//# sourceMappingURL=app.js.map