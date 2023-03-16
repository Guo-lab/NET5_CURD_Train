(function (String, angular) {
    'use strict';

    var pbm = angular.module('projectbase');

    //<----------------custom validators------------------------------------------------------------
    pbm.directive('pbCompareto', ['$parse', 'pb', function ($parse, pb) {
        return {
            restrict: "A",
            require: "?ngModel",
            link: function (scope, ele, attrs, ngModelController) {
                pb.RegisterGroupedValidator(scope, ele, attrs, ngModelController, 'pbCompareto', function (modelValue, viewValue) {
                    var value = modelValue || viewValue;
                    if (!value) return true;
                    var otherProp = attrs["pbCompareto"];
                    if (otherProp.indexOf('.') < 0) {
                        otherProp = 'c.vm.Input.' + otherProp;
                    }
                    var otherValue = $parse(otherProp)(scope);
                    var params = JSON.parse(attrs["pbComparetoParams"]);
                    if (params.isDate) {
                        value = new Date(value);
                    }
                    var compare = params.compare;
                    if (compare == 0) return value == otherValue;
                    if (compare == 1) return value > otherValue;
                    if (compare == -1) return value < otherValue;
                    if (compare == 10) return value >= otherValue;
                    if (compare == -10) return value <= otherValue;
                });
            }
        };
    }]);

    pbm.directive('vminner_', ['$parse', 'pb', function ($parse, pb) {
        return {
            restrict: "A",
            require: "?ngModel",
            link: function (scope, ele, attrs, ngModelController) {
                pb.RegisterGroupedValidator(scope, ele, attrs, ngModelController, 'vminner', function (modelValue, viewValue,formIndex) {
                    var value = modelValue || viewValue;
                    var funcName = attrs["vminner_"];
                    var registeredMap = ngModelController ? ngModelController.$$parentForm.pbVmInnerValidators : null;//看angularForm上有没有
                    if (!registeredMap) {
                        var formName = ele.context.form.name;
                        registeredMap = $parse(formName)(scope).pbVmInnerValidators;//看kendoForm上有没有
                        if (!registeredMap) {
                            var controller = pb.GetMyController($(ele));
                            if (controller.pbFormArrayVmValidatorRegistra && controller.pbFormArrayVmValidatorRegistra[formName]) {
                                registeredMap = controller.pbFormArrayVmValidatorRegistra[formName].pbVmInnerValidators;////看是否表单数组对应上有没有
                            }
                        }
                    }
                    var valfunc = registeredMap[funcName];
                    return valfunc(modelValue, viewValue, formIndex);
                });
            }
        };
    }]);
    pbm.directive('pbGrequired', ['pb', function (pb) {
        return {
            restrict: "A",
            require: "?ngModel",
            link: function (scope, ele, attrs, ngModelController) {
                pb.RegisterGroupedValidator(scope, ele, attrs, ngModelController, 'pbGrequired', function (modelValue, viewValue) {
                    return !!viewValue;
                });
            }
        };
    }]);
    pbm.directive('pbRange', ['pb', function (pb) {
        return {
            restrict: "A",
            require: "?ngModel",
            link: function (scope, ele, attrs, ngModelController) {
                pb.RegisterGroupedValidator(scope, ele, attrs, ngModelController, 'pbRange', function (modelValue, viewValue) {
                    var value = modelValue || viewValue;
                    if (!value) return true;
                    var params = JSON.parse(attrs["pbRangeParams"]);
                    var min = params.min;
                    var max = params.max;
                    if (min != null && value < min)  return false;
                    if (max != null) return value <= max;
                    return true;
                });
            }
        };
    }]);
    pbm.directive('pbStringLength', ['pb', function (pb) {
        return {
            restrict: "A",
            require: "?ngModel",
            link: function (scope, ele, attrs, ngModelController) {
                pb.RegisterGroupedValidator(scope, ele, attrs, ngModelController, 'pbStringLength', function (modelValue, viewValue) {
                    var value = modelValue || viewValue;
                    if (!value) return true;
                    var params = JSON.parse(attrs["pbStringLengthParams"]);
                    var min = params.min;
                    var max = params.max;
                    return value.length >= min && value.length <= max;
                });
            }
        };
    }]);
    pbm.directive('pbRegex', ['pb', function (pb) {
        return {
            restrict: "A",
            require: "?ngModel",
            link: function (scope, ele, attrs, ngModelController) {
                pb.RegisterGroupedValidator(scope, ele, attrs, ngModelController, 'pbRegex', function (modelValue, viewValue) {
                    var value = modelValue || viewValue;
                    if (!value) return true;
                    var pattern = attrs["pbRegex"];
                    return new RegExp(pattern).test(value);
                });
            }
        };
    }]);
    pbm.directive('pbTrim', ['pb', function (pb) {
        return {
            restrict: "A",
            require: "?ngModel",
            link: function (scope, ele, attrs, ngModelController) {
                ngModelController.$validators['pbTrim']= function (modelValue, viewValue) {
                    var value = modelValue || viewValue;
                    if (!value) return true;
                    if (typeof value=='string' && value != value.trim()) {
                        ngModelController.$setViewValue(value.trim());
                        ngModelController.$render();
                    }
                    return true;
                };
            }
        };
    }]);
    pbm.directive('pbValidateWhen', ['pb', function (pb) {
        return {
            restrict: "A",
            require: "?ngModel",
            link: function (scope, ele, attrs, ngModelController) {
                var jqForm = $(ele[0].form);
                ele.bind('blur', function () {
                    pb.Validate(jqForm);
                });
            }
        };
    }]);
    //pbm.directive('pbEqualto', ['$parse', function ($parse) {
    //    return {
    //        restrict: "A",
    //        require: "ngModel",
    //        link: function (scope, ele, attrs, ngModelController) {
    //            ngModelController.$validators.equalto = function (modelValue, viewValue) {
    //                var value = modelValue || viewValue;
    //                return value == $parse(attrs["pbEqualto"])(scope);
    //            };
    //        }
    //    };
    //}]);
    //pbm.directive('pbMaxByteLength', [function () {
    //    return {
    //        restrict: "A",
    //        require: "ngModel",
    //        link: function (scope, ele, attrs, ngModelController) {
    //            ngModelController.$validators.maxByteLength = function (modelValue, viewValue) {
    //                var value = modelValue || viewValue;
    //                return value == '' || value.getBytesLength() <= attrs["pbMaxByteLength"];
    //            };
    //        }
    //    };
    //}]);
    //pbm.directive('pbUnique', ['pb', 'App_UniqueCheckerUri', '$q', function (pb, App_UniqueCheckerUri, $q) {
    //    return {
    //        restrict: "A",
    //        require: "ngModel",
    //        link: function (scope, ele, attrs, ngModelController) {
    //            ngModelController.$asyncValidators.unique = function (modelValue, viewValue) {
    //                var value = modelValue || viewValue;
    //                return pb.AjaxSubmit(null, { value: value }, { "ajax-url": App_UniqueCheckerUri + attrs["unique"] })
    //                    .then(function (response) {
    //                        return response.data ? true : $q.reject();
    //                    }, function () {
    //                        return $q.reject();
    //                    });
    //            };
    //        }
    //    };
    //}]);

    //this function need the parameter stringvalue to be something in order of year month day no matter what the delimeter is.
    var StringToStandardDateString = function (stringvalue) {
        var year = stringvalue.substr(0, 4);
        var month = stringvalue.substr(5, 2);
        var day;
        if (isNaN(month.substr(1, 1))) {
            month = month.substr(0, 1);
            day = stringvalue.toString().substring(7, stringvalue.length);
        } else {
            day = stringvalue.toString().substring(8, stringvalue.length);
        }
        if (month.substr(0, 1) == '0') month = month.substr(1, 1);
        if (day.substr(0, 1) == '0') day = day.substr(1, 1);

        return year + '/' + month + '/' + day;
    }
    var IsValidDate = function IsValidDate(stringvalue, isStandard) {
        var tDateString = stringvalue;
        if (!isStandard) {
            tDateString = StringToStandardDateString(tDateString);
        }
        var tempDate = new Date(tDateString);
        if (isNaN(tempDate) == false) {
            if (((tempDate.getFullYear()).toString() == year) && (tempDate.getMonth() == parseInt(month) - 1) && (tempDate.getDate() == parseInt(day)))
                return true;
        }
        return false;
    }

}(String, angular));                         //end pack