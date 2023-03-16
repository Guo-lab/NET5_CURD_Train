Object.prototype.hasCssClass = function (sClassName) {
    if (typeof sClassName !== 'string') {
        //throw Error('HTML DOM\'s class name must be string');
        return;
    }
    sClassName = sClassName || '';
    if (sClassName.replace(/\s/g, '').length == 0)
        return false;
    return new RegExp(' ' + sClassName + ' ').test(' ' + this.className + ' ');
}

Object.prototype.addCssClass = function (sClassName) {
    if (!this.hasCssClass(sClassName)) {
        this.className = this.className == '' ? sClassName : this.className + ' ' + sClassName;
    }
}

Object.prototype.removeCssClass = function(sClassName) {
    if (this.hasCssClass(sClassName)) {
        var newClass = ' ' + this.className.replace(/[\t\r\n]/g, '') + ' ';
        while (newClass.indexOf(' ' + sClassName + ' ') >= 0) {
            newClass = newClass.replace(' ' + sClassName + ' ', ' ');
        }
        this.className = newClass.replace(/^\s+|\s+$/g, '');
    }
}