//create object of type o
function object(o) {
    function F() { }
    F.prototype = o;
    return new F();
}

function extend(subType, superType) {
    var prototype = object(superType.prototype);
    prototype.constructor = subType;
    subType.prototype = prototype;
}

