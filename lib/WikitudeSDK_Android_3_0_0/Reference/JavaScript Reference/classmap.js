YAHOO.env.classMap = {"GeoLocation": "AR", "Drawable2D": "AR", "CONST": "AR", "Style": "AR", "Label": "AR", "radar": "AR", "Animation": "AR", "Location": "AR", "ActionRange": "AR", "Circle": "AR", "Drawable": "AR", "GeoObject": "AR", "ARObject": "AR", "ARchitectObject": "AR", "AnimationGroup": "AR", "ModelAnimation": "AR", "RelativeLocation": "AR", "BoundingRectangle": "AR", "PropertyAnimation": "AR", "Sound": "AR", "ImageDrawable": "AR", "Trackable2DObject": "AR", "ImageResource": "AR", "Model": "AR", "ActionArea": "AR", "logger": "AR", "HtmlDrawable": "AR", "Tracker": "AR", "context": "AR", "AnimatedImageDrawable": "AR"};

YAHOO.env.resolveClass = function(className) {
    var a=className.split('.'), ns=YAHOO.env.classMap;

    for (var i=0; i<a.length; i=i+1) {
        if (ns[a[i]]) {
            ns = ns[a[i]];
        } else {
            return null;
        }
    }

    return ns;
};
