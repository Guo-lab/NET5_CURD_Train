app.factory('myCustomErrorMessageResolver', ['pbTranslate','$q', 'defaultErrorMessageResolver', '$translate', '$window', '$log',
    function (pbTranslate,$q, defaultErrorMessageResolver, $translate, $window, $log) {
        /**
        * @ngdoc function
        * @name defaultErrorMessageResolver#resolve
        * @methodOf defaultErrorMessageResolver
        *
        * @description
        * Resolves a validate error type into a user validation error message
        *
        * @param {String} errorType - The type of validation error that has occurred.
        * @param {Element} el - The input element that is the source of the validation error.
        * @returns {Promise} A promise that is resolved when the validation message has been produced.
        */
        var resolve = function (errorType, el,tooltipCtrl) {
            return resolveMessage(errorType, el).then(function (msg) {
                //el.kendoTooltip({ content: msg});
                if (tooltipCtrl) tooltipCtrl.attr("title", msg);
                return msg;
            });
        }

        var resolveMessage = function (errorType, el) {
            return pbTranslate.TranslateVal(errorType, el);
        }
        return {
            resolve: resolve,
            resolveMessage: resolveMessage
        };
    }
]);
app.factory('myCustomElementModifier', [function () {
    var /**
             * @ngdoc function
             * @name myCustomElementModifier#makeValid
             * @methodOf myCustomElementModifier
             *
             * @description
             * Makes an element appear valid by apply custom styles and child elements.
             *
             * @param {Element} el - The input control element that is the target of the validation.
             */
        makeValid = function (el) {
            el.css('border-color', '#cccccc');
            el.attr("title", "");
        },

        /**
        * @ngdoc function
        * @name myCustomElementModifier#makeInvalid
        * @methodOf myCustomElementModifier
        *
        * @description
        * Makes an element appear invalid by apply custom styles and child elements.
        *
        * @param {Element} el - The input control element that is the target of the validation.
        * @param {String} errorMsg - The validation error message to display to the user.
        */
        makeInvalid = function (el, errorMsg) {
            el.css('border-color', 'red');
            el.attr("title", errorMsg);
        },
        makeDefault = function (el) {
            el.css('border-color', '#cccccc');
        };

    return {
        makeValid: makeValid,
        makeInvalid: makeInvalid,
        makeDefault: makeDefault,
        key: 'myCustomModifierKey'
    };
}]);
