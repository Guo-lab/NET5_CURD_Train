'use strict';

window.addEventListener('load', function () {
    var oNodeList = document.querySelectorAll('[x-kit-data-toggle]');
    if (oNodeList) {
        Array.prototype.slice.call(oNodeList).forEach(function (value, index, array) {
            var sTargetElementId = value.getAttribute('x-kit-data-toggle');
            var oTargetElement = document.getElementById(sTargetElementId);

            oTargetElement.hasCssClass = function (sClassName) {
                if (typeof sClassName !== 'string') {
                    //throw Error('HTML DOM\'s class name must be string');
                    return;
                }
                sClassName = sClassName || '';
                if (sClassName.replace(/\s/g, '').length == 0)
                    return false;
                return new RegExp(' ' + sClassName + ' ').test(' ' + this.className + ' ');
            }

            oTargetElement.addCssClass = function (sClassName) {
                if (!this.hasCssClass(sClassName)) {
                    this.className = this.className == '' ? sClassName : this.className + ' ' + sClassName;
                }
            }

            oTargetElement.removeCssClass = function (sClassName) {
                if (this.hasCssClass(sClassName)) {
                    var newClass = ' ' + this.className.replace(/[\t\r\n]/g, '') + ' ';
                    while (newClass.indexOf(' ' + sClassName + ' ') >= 0) {
                        newClass = newClass.replace(' ' + sClassName + ' ', ' ');
                    }
                    this.className = newClass.replace(/^\s+|\s+$/g, '');
                }
            }


            if (!oTargetElement) {
                throw Error("x-kit-data-toggle\'s value is invalid");
            }
            value.addEventListener('click', function () {
                if (oTargetElement.hasCssClass('x-kit-actived')) {
                    oTargetElement.removeCssClass('x-kit-actived');
                }
                else{
                    oTargetElement.addCssClass('x-kit-actived');
                }
            });
        });
    }
});