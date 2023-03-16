if (typeof JSON.decycle !== "function") {

    function isPlainObject(obj) { //鏄惁鏄函绮瑰璞�
        if (typeof obj !== 'object' || obj === null) return false

        var proto = obj
        while (Object.getPrototypeOf(proto) !== null) {
            proto = Object.getPrototypeOf(proto)
        }

        return Object.getPrototypeOf(obj) === proto
    }

    JSON.decycle = function decycle(object, replacer) {
        "use strict";
        var objects = new WeakMap(); // object to path mappings

        return (function derez(value, path) {
            var old_path; // The path of an earlier occurance of value
            var nu; // The new object or array
            if (replacer !== undefined) {
                value = replacer(value);
            }
            if (isPlainObject(value) || Array.isArray(value)) {//绾�

                old_path = objects.get(value);
                //				console.log('aaa','path=',path,' old_path=',old_path);
                if (old_path !== undefined) {
                    return {
                        $ref: old_path
                    };
                }

                objects.set(value, path);
                if (Array.isArray(value)) {
                    nu = [];
                    value.forEach(function (element, i) {
                        nu[i] = derez(element, path + "[" + i + "]");
                    });
                } else {
                    nu = {};
                    Object.keys(value).forEach(function (name) {
                        nu[name] = derez(
                            value[name],
                            path + "[" + JSON.stringify(name) + "]"
                        );
                    });
                }
                return nu;
            }
            return value;
        }(object, "$"));
    };
}

if (typeof JSON.retrocycle !== "function") {
    JSON.retrocycle = function retrocycle($) {
        "use strict";
        var px = /^\$(?:\[(?:\d+|"(?:[^\\"\u0000-\u001f]|\\(?:[\\"\/bfnrt]|u[0-9a-zA-Z]{4}))*")\])*$/;
        (function rez(value) {

            if (value && typeof value === "object") {
                if (Array.isArray(value)) {
                    value.forEach(function (element, i) {
                        if (typeof element === "object" && element !== null) {
                            var path = element.$ref;
                            if (typeof path === "string" && px.test(path)) {

                                value[i] = eval(path);
                            } else {
                                rez(element);
                            }
                        }
                    });
                } else {
                    Object.keys(value).forEach(function (name) {
                        var item = value[name];
                        if (typeof item === "object" && item !== null) {
                            var path = item.$ref;
                            if (typeof path === "string" && px.test(path)) {

                                value[name] = eval(path);
                            } else {
                                rez(item);
                            }
                        }
                    });
                }
            }
        }($));
        return $;
    };
}