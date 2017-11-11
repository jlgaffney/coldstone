var edge = require("electron-edge-js");
var path = require("path");

module.exports = function (pathToDll) {
    var self = this;
    var dirName = path.dirname(pathToDll);

    var edgeMvvmBase = (function () {

        //public Task<object> Initialize(dynamic obj);
        var initialize = edge.func({
            assemblyFile: dirName + "/Electron.Edge.Mvvm.dll",
            typeName: "Electron.Edge.Mvvm.Binder",
            methodName: "Initialize"
        });

        //public Task<object> CreateViewModel(dynamic obj);
        var createViewModel = edge.func({
            assemblyFile: dirName + "/Electron.Edge.Mvvm.dll",
            typeName: "Electron.Edge.Mvvm.Binder",
            methodName: "CreateViewModel"
        });

        //public Task<object> GetPropertyValue(dynamic obj);
        var getPropertyValue = edge.func({
            assemblyFile: dirName + "/Electron.Edge.Mvvm.dll",
            typeName: "Electron.Edge.Mvvm.Binder",
            methodName: "GetPropertyValue"
        });

        //public Task<object> SetPropertyValue(dynamic obj);
        var setPropertyValue = edge.func({
            assemblyFile: dirName + "/Electron.Edge.Mvvm.dll",
            typeName: "Electron.Edge.Mvvm.Binder",
            methodName: "SetPropertyValue"
        });

        //public Task<object> BindToProperty(dynamic obj);
        var bindToProperty = edge.func({
            assemblyFile: dirName + "/Electron.Edge.Mvvm.dll",
            typeName: "Electron.Edge.Mvvm.Binder",
            methodName: "BindToProperty"
        });

        //public Task<object> ExecuteCommand(dynamic obj);
        var executeCommand = edge.func({
            assemblyFile: dirName + "/Electron.Edge.Mvvm.dll",
            typeName: "Electron.Edge.Mvvm.Binder",
            methodName: "ExecuteCommand"
        });

        //public Task<object> GetPropertyAsViewModel(dynamic obj);
        var getPropertyAsViewModel = edge.func({
            assemblyFile: dirName + "/Electron.Edge.Mvvm.dll",
            typeName: "Electron.Edge.Mvvm.Binder",
            methodName: "GetPropertyAsViewModel"
        });

        return {
            initialize: initialize,
            createViewModel: createViewModel,
            getPropertyValue: getPropertyValue,
            setPropertyValue: setPropertyValue,
            bindToProperty: bindToProperty,
            executeCommand: executeCommand,
            getPropertyAsViewModel: getPropertyAsViewModel
        };
    })();

    function checkResult(result) {
        if (!result.ok) {
            throw result.result;
        } else {
            return result.result;
        }
    };

    function setup() {
        var ViewModel = function (id) {
            var self = this;
            var _id = id;

            self.bindInput = function (propertyName, node) {
                checkResult(edgeMvvmBase.bindToProperty({
                    id: _id,
                    property: propertyName,
                    onChanged: function (input, callback) {
                        node.value = input;
                        callback(null, null);
                    }
                }, true));

                edgeMvvmBase.getPropertyValue({ id: _id, property: propertyName }, function (err, result) {
                    node.value = checkResult(result);
                });

                node.addEventListener("input", function (e) {
                    edgeMvvmBase.setPropertyValue({ id: _id, property: propertyName, value: node.value },
                        function (err, result) { checkResult(result); });
                });
            };

            self.bindCommand = function (commandName, node) {
                node.addEventListener("click", function (e) {
                    edgeMvvmBase.executeCommand({ id: _id, command: commandName },
                        function (err, result) { checkResult(result); });
                });
            };

            self.bindText = function (propertyName, node) {
                edgeMvvmBase.bindToProperty({
                    id: _id,
                    property: propertyName,
                    onChanged: function (input, callback) {
                        node.innerText = input;
                        callback(null, null);
                    }
                }, true);

                edgeMvvmBase.getPropertyValue({ id: _id, property: propertyName }, function (err, result) {
                    node.innerText = checkResult(result);
                });
            };

            self.getChildAsViewModel = function (propertyName) {
                return new ViewModel(checkResult(edgeMvvmBase.getPropertyAsViewModel({ id: _id, property: propertyName }, true)));
            }
        };

        function isDescendant(parent, child) {
            var node = child.parentNode;
            while (node != null) {
                if (node == parent) {
                    return true;
                }
                node = node.parentNode;
            }
            return false;
        }

        self.bind = function (idOrNode, typeOrVM) {

            var viewModel = null;
            var root = null;
            if (typeof idOrNode === "string") {
                viewModel = new ViewModel(checkResult(edgeMvvmBase.createViewModel({ name: typeOrVM }, true)));
                root = document.getElementById(idOrNode);
            } else {
                viewModel = typeOrVM;
                root = idOrNode;
            }

            var skipChildren = [];
            var bindings = root.querySelectorAll("[data-bind]");

            for (var i = 0; i < bindings.length; i++) {
                var node = bindings[i];

                var skip = false;
                for (var j = 0; j < skipChildren.length; j++) {
                    if (isDescendant(skipChildren[j], node)) {
                        skip = true;
                    }
                }

                if (skip)
                    continue;

                var binding = node.dataset.bind;
                var bindingParts = binding.split(" ");
                var bindingType = bindingParts[0];
                var propertyName = bindingParts[1];
                if (bindingType === "value:") {
                    viewModel.bindInput(propertyName, node);
                } else if (bindingType === "command:") {
                    viewModel.bindCommand(propertyName, node);
                } else if (bindingType === "text:") {
                    viewModel.bindText(propertyName, node);
                } else if (bindingType === "with:") {
                    var child = viewModel.getChildAsViewModel(propertyName);
                    self.bind(node, child);
                    skipChildren.push(node);
                }
            }
        };
    }

    edgeMvvmBase.initialize({ path: pathToDll }, function (error, result) {
        var errorMsg = null;

        // Check for errors
        if (error) {
            errorMsg = "Initialization failed! " + error;
        }
        else if (result == null) {
            errorMsg = "Initialization failed!";
        }
        else if (!result.ok) {
            errorMsg = "Initialization failed! " + result.result;
        }

        if (errorMsg != null) {
            throw errorMsg;
        }

        // Initialization successful
        setup();
    });
}