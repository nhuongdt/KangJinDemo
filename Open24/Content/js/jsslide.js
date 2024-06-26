﻿

! function (j, h, n, g, d, k, e) {
    new (function () { });
    var c = {
        C: n.PI,
        k: n.max,
        j: n.min,
        I: n.ceil,
        P: n.floor,
        m: n.abs,
        rb: n.sin,
        Kb: n.cos,
        xd: n.tan,
        Af: n.atan,
        Mb: n.sqrt,
        q: n.pow,
        td: n.random,
        $Round: n.round
    },
        f = j.$Jease$ = {
            $Swing: function (a) {
                return -c.Kb(a * c.C) / 2 + .5
            },
            $Linear: function (a) {
                return a
            },
            $InQuad: function (a) {
                return a * a
            },
            $OutQuad: function (a) {
                return -a * (a - 2)
            },
            $InOutQuad: function (a) {
                return (a *= 2) < 1 ? 1 / 2 * a * a : -1 / 2 * (--a * (a - 2) - 1)
            },
            $InCubic: function (a) {
                return a * a * a
            },
            $OutCubic: function (a) {
                return (a -= 1) * a * a + 1
            },
            $InOutCubic: function (a) {
                return (a *= 2) < 1 ? 1 / 2 * a * a * a : 1 / 2 * ((a -= 2) * a * a + 2)
            },
            $InQuart: function (a) {
                return a * a * a * a
            },
            $OutQuart: function (a) {
                return -((a -= 1) * a * a * a - 1)
            },
            $InOutQuart: function (a) {
                return (a *= 2) < 1 ? 1 / 2 * a * a * a * a : -1 / 2 * ((a -= 2) * a * a * a - 2)
            },
            $InQuint: function (a) {
                return a * a * a * a * a
            },
            $OutQuint: function (a) {
                return (a -= 1) * a * a * a * a + 1
            },
            $InOutQuint: function (a) {
                return (a *= 2) < 1 ? 1 / 2 * a * a * a * a * a : 1 / 2 * ((a -= 2) * a * a * a * a + 2)
            },
            $InSine: function (a) {
                return 1 - c.Kb(c.C / 2 * a)
            },
            $OutSine: function (a) {
                return c.rb(c.C / 2 * a)
            },
            $InOutSine: function (a) {
                return -1 / 2 * (c.Kb(c.C * a) - 1)
            },
            $InExpo: function (a) {
                return a == 0 ? 0 : c.q(2, 10 * (a - 1))
            },
            $OutExpo: function (a) {
                return a == 1 ? 1 : -c.q(2, -10 * a) + 1
            },
            $InOutExpo: function (a) {
                return a == 0 || a == 1 ? a : (a *= 2) < 1 ? 1 / 2 * c.q(2, 10 * (a - 1)) : 1 / 2 * (-c.q(2, -10 * --a) + 2)
            },
            $InCirc: function (a) {
                return -(c.Mb(1 - a * a) - 1)
            },
            $OutCirc: function (a) {
                return c.Mb(1 - (a -= 1) * a)
            },
            $InOutCirc: function (a) {
                return (a *= 2) < 1 ? -1 / 2 * (c.Mb(1 - a * a) - 1) : 1 / 2 * (c.Mb(1 - (a -= 2) * a) + 1)
            },
            $InElastic: function (a) {
                if (!a || a == 1) return a;
                var b = .3,
                    d = .075;
                return -(c.q(2, 10 * (a -= 1)) * c.rb((a - d) * 2 * c.C / b))
            },
            $OutElastic: function (a) {
                if (!a || a == 1) return a;
                var b = .3,
                    d = .075;
                return c.q(2, -10 * a) * c.rb((a - d) * 2 * c.C / b) + 1
            },
            $InOutElastic: function (a) {
                if (!a || a == 1) return a;
                var b = .45,
                    d = .1125;
                return (a *= 2) < 1 ? -.5 * c.q(2, 10 * (a -= 1)) * c.rb((a - d) * 2 * c.C / b) : c.q(2, -10 * (a -= 1)) * c.rb((a - d) * 2 * c.C / b) * .5 + 1
            },
            $InBack: function (a) {
                var b = 1.70158;
                return a * a * ((b + 1) * a - b)
            },
            $OutBack: function (a) {
                var b = 1.70158;
                return (a -= 1) * a * ((b + 1) * a + b) + 1
            },
            $InOutBack: function (a) {
                var b = 1.70158;
                return (a *= 2) < 1 ? 1 / 2 * a * a * (((b *= 1.525) + 1) * a - b) : 1 / 2 * ((a -= 2) * a * (((b *= 1.525) + 1) * a + b) + 2)
            },
            $InBounce: function (a) {
                return 1 - f.$OutBounce(1 - a)
            },
            $OutBounce: function (a) {
                return a < 1 / 2.75 ? 7.5625 * a * a : a < 2 / 2.75 ? 7.5625 * (a -= 1.5 / 2.75) * a + .75 : a < 2.5 / 2.75 ? 7.5625 * (a -= 2.25 / 2.75) * a + .9375 : 7.5625 * (a -= 2.625 / 2.75) * a + .984375
            },
            $InOutBounce: function (a) {
                return a < 1 / 2 ? f.$InBounce(a * 2) * .5 : f.$OutBounce(a * 2 - 1) * .5 + .5
            },
            $GoBack: function (a) {
                return 1 - c.m(2 - 1)
            },
            $InWave: function (a) {
                return 1 - c.Kb(a * c.C * 2)
            },
            $OutWave: function (a) {
                return c.rb(a * c.C * 2)
            },
            $OutJump: function (a) {
                return 1 - ((a *= 2) < 1 ? (a = 1 - a) * a * a : (a -= 1) * a * a)
            },
            $InJump: function (a) {
                return (a *= 2) < 1 ? a * a * a : (a = 2 - a) * a * a
            },
            $Early: c.I,
            $Late: c.P
        };
    var b = j.$Jssor$ = new function () {
        var i = this,
            Ab = /\S+/g,
            N = 1,
            jb = 2,
            mb = 3,
            lb = 4,
            pb = 5,
            O, t = 0,
            l = 0,
            u = 0,
            A = 0,
            B = 0,
            E = navigator,
            ub = E.appName,
            o = E.userAgent,
            z = h.documentElement,
            q = parseFloat;

        function Ib() {
            if (!O) {
                O = {
                    Mg: "ontouchstart" in j || "createTouch" in h
                };
                var a;
                if (E.pointerEnabled || (a = E.msPointerEnabled)) O.Zd = a ? "msTouchAction" : "touchAction"
            }
            return O
        }

        function w(g) {
            if (!t) {
                t = -1;
                if (ub == "Microsoft Internet Explorer" && !!j.attachEvent && !!j.ActiveXObject) {
                    var e = o.indexOf("MSIE");
                    t = N;
                    u = q(o.substring(e + 5, o.indexOf(";", e))); /*@cc_on A=@_jscript_version@*/;
                    l = h.documentMode || u
                } else if (ub == "Netscape" && !!j.addEventListener) {
                    var d = o.indexOf("Firefox"),
                        b = o.indexOf("Safari"),
                        f = o.indexOf("Chrome"),
                        c = o.indexOf("AppleWebKit");
                    if (d >= 0) {
                        t = jb;
                        l = q(o.substring(d + 8))
                    } else if (b >= 0) {
                        var i = o.substring(0, b).lastIndexOf("/");
                        t = f >= 0 ? lb : mb;
                        l = q(o.substring(i + 1, b))
                    } else {
                        var a = /Trident\/.*rv:([0-9]{1,}[\.0-9]{0,})/i.exec(o);
                        if (a) {
                            t = N;
                            l = u = q(a[1])
                        }
                    }
                    if (c >= 0) B = q(o.substring(c + 12))
                } else {
                    var a = /(opera)(?:.*version|)[ \/]([\w.]+)/i.exec(o);
                    if (a) {
                        t = pb;
                        l = q(a[2])
                    }
                }
            }
            return g == t
        }

        function r() {
            return w(N)
        }

        function ib() {
            return r() && (l < 6 || h.compatMode == "BackCompat")
        }

        function Bb() {
            return w(jb)
        }

        function kb() {
            return w(mb)
        }

        function Eb() {
            return w(lb)
        }

        function ob() {
            return w(pb)
        }

        function eb() {
            return kb() && B > 534 && B < 535
        }

        function I() {
            w();
            return B > 537 || l > 42 || t == N && l >= 11
        }

        function gb() {
            return r() && l < 9
        }

        function fb(a) {
            var b, c;
            return function (g) {
                if (!b) {
                    b = d;
                    var f = a.substr(0, 1).toUpperCase() + a.substr(1);
                    n([a].concat(["WebKit", "ms", "Moz", "O", "webkit"]), function (h, d) {
                        var b = a;
                        if (d) b = h + f;
                        if (g.style[b] != e) return c = b
                    })
                }
                return c
            }
        }

        function db(b) {
            var a;
            return function (c) {
                a = a || fb(b)(c) || b;
                return a
            }
        }
        var P = db("transform");

        function tb(a) {
            return {}.toString.call(a)
        }
        var qb = {};
        n(["Boolean", "Number", "String", "Function", "Array", "Date", "RegExp", "Object"], function (a) {
            qb["[object " + a + "]"] = a.toLowerCase()
        });

        function n(b, d) {
            var a, c;
            if (tb(b) == "[object Array]") {
                for (a = 0; a < b.length; a++)
                    if (c = d(b[a], a, b)) return c
            } else
                for (a in b)
                    if (c = d(b[a], a, b)) return c
        }

        function H(a) {
            return a == g ? String(a) : qb[tb(a)] || "object"
        }

        function rb(a) {
            for (var b in a) return d
        }

        function C(a) {
            try {
                return H(a) == "object" && !a.nodeType && a != a.window && (!a.constructor || {}.hasOwnProperty.call(a.constructor.prototype, "isPrototypeOf"))
            } catch (b) { }
        }

        function p(a, b) {
            return {
                x: a,
                y: b
            }
        }

        function yb(b, a) {
            setTimeout(b, a || 0)
        }

        function D(b, d, c) {
            var a = !b || b == "inherit" ? "" : b;
            n(d, function (c) {
                var b = c.exec(a);
                if (b) {
                    var d = a.substr(0, b.index),
                        e = a.substr(b.index + b[0].length + 1, a.length - 1);
                    a = d + e
                }
            });
            a && (c += (!a.indexOf(" ") ? "" : " ") + a);
            return c
        }

        function U(b, a) {
            if (l < 9) b.style.filter = a
        }

        function vb(a, b) {
            if (a === e) a = b;
            return a
        }
        i.$Device = Ib;
        i.$IsBrowserIE = r;
        i.$IsBrowserIeQuirks = ib;
        i.$IsBrowserFireFox = Bb;
        i.$IsBrowserSafari = kb;
        i.$IsBrowserChrome = Eb;
        i.$IsBrowserOpera = ob;
        i.yg = I;
        fb("transform");
        i.$BrowserVersion = function () {
            return l
        };
        i.$BrowserEngineVersion = function () {
            return u || l
        };
        i.$WebKitVersion = function () {
            w();
            return B
        };
        i.$Delay = yb;
        i.Y = vb;
        i.Gg = function (a, b) {
            b.call(a);
            return G({}, a)
        };

        function Z(a) {
            a.constructor === Z.caller && a.jc && a.jc.apply(a, Z.caller.arguments)
        }
        i.jc = Z;
        i.$GetElement = function (a) {
            if (i.Fg(a)) a = h.getElementById(a);
            return a
        };

        function v(a) {
            return a || j.event
        }
        i.$EvtSrc = function (b) {
            b = v(b);
            var a = b.target || b.srcElement || h;
            if (a.nodeType == 3) a = i.Cc(a);
            return a
        };
        i.de = function (a) {
            a = v(a);
            return {
                x: a.pageX || a.clientX || 0,
                y: a.pageY || a.clientY || 0
            }
        };
        i.$WindowSize = function () {
            var a = h.body;
            return {
                x: a.clientWidth || z.clientWidth,
                y: a.clientHeight || z.clientHeight
            }
        };

        function x(c, d, a) {
            if (a !== e) c.style[d] = a == e ? "" : a;
            else {
                var b = c.currentStyle || c.style;
                a = b[d];
                if (a == "" && j.getComputedStyle) {
                    b = c.ownerDocument.defaultView.getComputedStyle(c, g);
                    b && (a = b.getPropertyValue(d) || b[d])
                }
                return a
            }
        }

        function bb(b, c, a, d) {
            if (a === e) {
                a = q(x(b, c));
                isNaN(a) && (a = g);
                return a
            }
            if (a == g) a = "";
            else d && (a += "px");
            x(b, c, a)
        }

        function m(c, a) {
            var d = a ? bb : x,
                b;
            if (a & 4) b = db(c);
            return function (e, f) {
                return d(e, b ? b(e) : c, f, a & 2)
            }
        }

        function Db(b) {
            if (r() && u < 9) {
                var a = /opacity=([^)]*)/.exec(b.style.filter || "");
                return a ? q(a[1]) / 100 : 1
            } else return q(b.style.opacity || "1")
        }

        function Fb(b, a, f) {
            if (r() && u < 9) {
                var h = b.style.filter || "",
                    i = new RegExp(/[\s]*alpha\([^\)]*\)/g),
                    e = c.$Round(100 * a),
                    d = "";
                if (e < 100 || f) d = "alpha(opacity=" + e + ") ";
                var g = D(h, [i], d);
                U(b, g)
            } else b.style.opacity = a == 1 ? "" : c.$Round(a * 100) / 100
        }
        var Q = {
            $Rotate: ["rotate"],
            $RotateX: ["rotateX"],
            $RotateY: ["rotateY"],
            $SkewX: ["skewX"],
            $SkewY: ["skewY"]
        };
        if (!I()) Q = G(Q, {
            $ScaleX: ["scaleX", 2],
            $ScaleY: ["scaleY", 2],
            $TranslateZ: ["translateZ", 1]
        });

        function R(d, a) {
            var c = "";
            if (a) {
                if (r() && l && l < 10) {
                    delete a.$RotateX;
                    delete a.$RotateY;
                    delete a.$TranslateZ
                }
                b.$Each(a, function (d, b) {
                    var a = Q[b];
                    if (a) {
                        var e = a[1] || 0;
                        if (S[b] != d) c += " " + a[0] + "(" + d + (["deg", "px", ""])[e] + ")"
                    }
                });
                if (I()) {
                    if (a.$TranslateX || a.$TranslateY || a.$TranslateZ != e) c += " translate3d(" + (a.$TranslateX || 0) + "px," + (a.$TranslateY || 0) + "px," + (a.$TranslateZ || 0) + "px)";
                    if (a.$ScaleX == e) a.$ScaleX = 1;
                    if (a.$ScaleY == e) a.$ScaleY = 1;
                    if (a.$ScaleX != 1 || a.$ScaleY != 1) c += " scale3d(" + a.$ScaleX + ", " + a.$ScaleY + ", 1)"
                }
            }
            d.style[P(d)] = c
        }
        i.dg = m("transformOrigin", 4);
        i.gg = m("backfaceVisibility", 4);
        i.jg = m("transformStyle", 4);
        i.ig = m("perspective", 6);
        i.hg = m("perspectiveOrigin", 4);
        i.cg = function (b, a) {
            if (r() && u < 9 || u < 10 && ib()) b.style.zoom = a == 1 ? "" : a;
            else {
                var c = P(b),
                    f = a == 1 ? "" : "scale(" + a + ")",
                    e = b.style[c],
                    g = new RegExp(/[\s]*scale\(.*?\)/g),
                    d = D(e, [g], f);
                b.style[c] = d
            }
        };
        i.$AddEvent = function (a, c, d, b) {
            a = i.$GetElement(a);
            if (a.addEventListener) {
                c == "mousewheel" && a.addEventListener("DOMMouseScroll", d, b);
                a.addEventListener(c, d, b)
            } else if (a.attachEvent) {
                a.attachEvent("on" + c, d);
                b && a.setCapture && a.setCapture()
            }
        };
        i.fb = function (a, c, d, b) {
            a = i.$GetElement(a);
            if (a.removeEventListener) {
                c == "mousewheel" && a.removeEventListener("DOMMouseScroll", d, b);
                a.removeEventListener(c, d, b)
            } else if (a.detachEvent) {
                a.detachEvent("on" + c, d);
                b && a.releaseCapture && a.releaseCapture()
            }
        };
        i.$FireEvent = function (c, b) {
            var a;
            if (h.createEvent) {
                a = h.createEvent("HTMLEvents");
                a.initEvent(b, k, k);
                c.dispatchEvent(a)
            } else {
                var d = "on" + b;
                a = h.createEventObject();
                c.fireEvent(d, a)
            }
        };
        i.$CancelEvent = function (a) {
            a = v(a);
            a.preventDefault && a.preventDefault();
            a.cancel = d;
            a.returnValue = k
        };
        i.$StopEvent = function (a) {
            a = v(a);
            a.stopPropagation && a.stopPropagation();
            a.cancelBubble = d
        };
        i.$CreateCallback = function (d, c) {
            var a = [].slice.call(arguments, 2),
                b = function () {
                    var b = a.concat([].slice.call(arguments, 0));
                    return c.apply(d, b)
                };
            return b
        };
        i.$InnerText = function (a, b) {
            if (b == e) return a.textContent || a.innerText;
            var c = h.createTextNode(b);
            i.Dc(a);
            a.appendChild(c)
        };
        i.$InnerHtml = function (a, b) {
            if (b == e) return a.innerHTML;
            a.innerHTML = b
        };
        i.$ClearInnerHtml = function (a) {
            a.innerHTML = ""
        };
        i.$Children = function (d, c) {
            for (var b = [], a = d.firstChild; a; a = a.nextSibling)(c || a.nodeType == 1) && b.push(a);
            return b
        };

        function sb(a, c, e, b) {
            b = b || "u";
            for (a = a ? a.firstChild : g; a; a = a.nextSibling)
                if (a.nodeType == 1) {
                    if (M(a, b) == c) return a;
                    if (!e) {
                        var d = sb(a, c, e, b);
                        if (d) return d
                    }
                }
        }
        i.$FindChild = sb;

        function X(a, d, f, b) {
            b = b || "u";
            var c = [];
            for (a = a ? a.firstChild : g; a; a = a.nextSibling)
                if (a.nodeType == 1) {
                    M(a, b) == d && c.push(a);
                    if (!f) {
                        var e = X(a, d, f, b);
                        if (e.length) c = c.concat(e)
                    }
                }
            return c
        }

        function nb(a, c, d) {
            for (a = a ? a.firstChild : g; a; a = a.nextSibling)
                if (a.nodeType == 1) {
                    if (a.tagName == c) return a;
                    if (!d) {
                        var b = nb(a, c, d);
                        if (b) return b
                    }
                }
        }
        i.sg = nb;
        i.ug = function (b, a) {
            return b.getElementsByTagName(a)
        };
        i.Wb = function (a, f, d) {
            d = d || "u";
            var e;
            do {
                if (a.nodeType == 1) {
                    var c = b.$AttributeEx(a, d);
                    if (c && c == vb(f, c)) {
                        e = a;
                        break
                    }
                }
                a = b.Cc(a)
            } while (a && a != h.body);
            return e
        };

        function G() {
            var f = arguments,
                d, c, b, a, h = 1 & f[0],
                g = 1 + h;
            d = f[g - 1] || {};
            for (; g < f.length; g++)
                if (c = f[g])
                    for (b in c) {
                        a = c[b];
                        if (a !== e) {
                            a = c[b];
                            var i = d[b];
                            d[b] = h && (C(i) || C(a)) ? G(h, {}, i, a) : a
                        }
                    }
            return d
        }
        i.B = G;

        function ab(f, g) {
            var d = {},
                c, a, b;
            for (c in f) {
                a = f[c];
                b = g[c];
                if (a !== b) {
                    var e;
                    if (C(a) && C(b)) {
                        a = ab(a, b);
                        e = !rb(a)
                    } !e && (d[c] = a)
                }
            }
            return d
        }
        i.kd = function (a) {
            return H(a) == "function"
        };
        i.Fg = function (a) {
            return H(a) == "string"
        };
        i.mc = function (a) {
            return !isNaN(q(a)) && isFinite(a)
        };
        i.$Each = n;
        i.sd = C;

        function V(a) {
            return h.createElement(a)
        }
        i.$CreateElement = V;
        i.$CreateDiv = function () {
            return V("DIV")
        };
        i.wg = function () {
            return V("SPAN")
        };
        i.ed = function () { };

        function F(b, c, a) {
            if (a == e) return b.getAttribute(c);
            b.setAttribute(c, a)
        }

        function M(a, b) {
            return F(a, b) || F(a, "data-" + b)
        }
        i.$Attribute = F;
        i.$AttributeEx = M;
        i.db = function (d, b, c) {
            var a = i.vg(M(d, b));
            if (isNaN(a)) a = c;
            return a
        };

        function y(b, a) {
            return F(b, "class", a) || ""
        }

        function xb(b) {
            var a = {};
            n(b, function (b) {
                if (b != e) a[b] = b
            });
            return a
        }

        function zb(b, a) {
            return b.match(a || Ab)
        }

        function T(b, a) {
            return xb(zb(b || "", a))
        }
        i.dd = xb;
        i.qg = zb;

        function cb(b, c) {
            var a = "";
            n(c, function (c) {
                a && (a += b);
                a += c
            });
            return a
        }

        function K(a, c, b) {
            y(a, cb(" ", G(ab(T(y(a)), T(c)), T(b))))
        }
        i.Cc = function (a) {
            return a.parentNode
        };
        i.eb = function (a) {
            i.ac(a, "none")
        };
        i.J = function (a, b) {
            i.ac(a, b ? "none" : "")
        };
        i.kg = function (b, a) {
            b.removeAttribute(a)
        };
        i.pg = function (d, a) {
            if (a) d.style.clip = "rect(" + c.$Round(a.$Top || a.F || 0) + "px " + c.$Round(a.$Right) + "px " + c.$Round(a.$Bottom) + "px " + c.$Round(a.$Left || a.G || 0) + "px)";
            else if (a !== e) {
                var h = d.style.cssText,
                    g = [new RegExp(/[\s]*clip: rect\(.*?\)[;]?/i), new RegExp(/[\s]*cliptop: .*?[;]?/i), new RegExp(/[\s]*clipright: .*?[;]?/i), new RegExp(/[\s]*clipbottom: .*?[;]?/i), new RegExp(/[\s]*clipleft: .*?[;]?/i)],
                    f = D(h, g, "");
                b.$CssCssText(d, f)
            }
        };
        i.cb = function () {
            return +new Date
        };
        i.$AppendChild = function (b, a) {
            b.appendChild(a)
        };
        i.Nb = function (b, a, c) {
            (c || a.parentNode).insertBefore(b, a)
        };
        i.fc = function (b, a) {
            a = a || b.parentNode;
            a && a.removeChild(b)
        };
        i.og = function (a, b) {
            n(a, function (a) {
                i.fc(a, b)
            })
        };
        i.Dc = function (a) {
            i.og(i.$Children(a, d), a)
        };
        i.Bc = function (a, b) {
            var c = i.Cc(a);
            b & 1 && i.R(a, (i.$CssWidth(c) - i.$CssWidth(a)) / 2);
            b & 2 && i.U(a, (i.$CssHeight(c) - i.$CssHeight(a)) / 2)
        };
        var W = {
            $Top: g,
            $Right: g,
            $Bottom: g,
            $Left: g,
            D: g,
            E: g
        };
        i.ng = function (a) {
            var b = i.$CreateDiv();
            s(b, {
                bc: "block",
                sb: i.tb(a),
                $Top: 0,
                $Left: 0,
                D: 0,
                E: 0
            });
            var d = i.nd(a, W);
            i.Nb(b, a);
            i.$AppendChild(b, a);
            var e = i.nd(a, W),
                c = {};
            n(d, function (b, a) {
                if (b == e[a]) c[a] = b
            });
            s(b, W);
            s(b, c);
            s(a, {
                $Top: 0,
                $Left: 0
            });
            return c
        };
        i.vg = q;

        function Y(d, c, b) {
            var a = d.cloneNode(!c);
            !b && i.kg(a, "id");
            return a
        }
        i.$CloneNode = Y;
        i.Pb = function (e, f) {
            var a = new Image;

            function b(e, d) {
                i.fb(a, "load", b);
                i.fb(a, "abort", c);
                i.fb(a, "error", c);
                f && f(a, d)
            }

            function c(a) {
                b(a, d)
            }
            if (ob() && l < 11.6 || !e) b(!e);
            else {
                i.$AddEvent(a, "load", b);
                i.$AddEvent(a, "abort", c);
                i.$AddEvent(a, "error", c);
                a.src = e
            }
        };
        i.qe = function (d, a, e) {
            var c = d.length + 1;

            function b(b) {
                c--;
                if (a && b && b.src == a.src) a = b;
                !c && e && e(a)
            }
            n(d, function (a) {
                i.Pb(a.src, b)
            });
            b()
        };
        i.Id = function (a, g, i, h) {
            if (h) a = Y(a);
            var c = X(a, g);
            if (!c.length) c = b.ug(a, g);
            for (var f = c.length - 1; f > -1; f--) {
                var d = c[f],
                    e = Y(i);
                y(e, y(d));
                b.$CssCssText(e, d.style.cssText);
                b.Nb(e, d);
                b.fc(d)
            }
            return a
        };

        function Gb(a) {
            var d = this,
                p = "",
                r = ["av", "pv", "ds", "dn"],
                f = [],
                q, m = 0,
                k = 0,
                g = 0;

            function l() {
                K(a, q, (f[g || k & 2 || k] || "") + " " + (f[m] || ""));
                b.$Css(a, "pointer-events", g ? "none" : "")
            }

            function c() {
                m = 0;
                l();
                i.fb(h, "mouseup", c);
                i.fb(h, "touchend", c);
                i.fb(h, "touchcancel", c)
            }

            function j(a) {
                if (g) i.$CancelEvent(a);
                else {
                    m = 4;
                    l();
                    i.$AddEvent(h, "mouseup", c);
                    i.$AddEvent(h, "touchend", c);
                    i.$AddEvent(h, "touchcancel", c)
                }
            }
            d.zd = function (a) {
                if (a === e) return k;
                k = a & 2 || a & 1;
                l()
            };
            d.$Enable = function (a) {
                if (a === e) return !g;
                g = a ? 0 : 3;
                l()
            };
            d.$Elmt = a = i.$GetElement(a);
            F(a, "data-jssor-button", "1");
            var o = b.qg(y(a));
            if (o) p = o.shift();
            n(r, function (a) {
                f.push(p + a)
            });
            q = cb(" ", f);
            f.unshift("");
            i.$AddEvent(a, "mousedown", j);
            i.$AddEvent(a, "touchstart", j)
        }
        i.lc = function (a) {
            return new Gb(a)
        };
        i.$Css = x;
        i.Zb = m("overflow");
        i.U = m("top", 2);
        i.xg = m("right", 2);
        i.Je = m("bottom", 2);
        i.R = m("left", 2);
        i.$CssWidth = m("width", 2);
        i.$CssHeight = m("height", 2);
        i.Bg = m("marginLeft", 2);
        i.Ag = m("marginTop", 2);
        i.tb = m("position");
        i.ac = m("display");
        i.H = m("zIndex", 1);
        i.Fc = function (b, a, c) {
            if (a != e) Fb(b, a, c);
            else return Db(b)
        };
        i.$CssCssText = function (a, b) {
            if (b != e) a.style.cssText = b;
            else return a.style.cssText
        };
        i.pe = function (b, a) {
            if (a === e) {
                a = x(b, "backgroundImage") || "";
                var c = /\burl\s*\(\s*["']?([^"'\r\n,]+)["']?\s*\)/gi.exec(a) || [];
                return c[1]
            }
            x(b, "backgroundImage", a ? "url('" + a + "')" : "")
        };
        var L;
        i.ze = L = {
            $Opacity: i.Fc,
            $Top: i.U,
            $Right: i.xg,
            $Bottom: i.Je,
            $Left: i.R,
            D: i.$CssWidth,
            E: i.$CssHeight,
            sb: i.tb,
            bc: i.ac,
            $ZIndex: i.H
        };
        i.nd = function (c, b) {
            var a = {};
            n(b, function (d, b) {
                if (L[b]) a[b] = L[b](c)
            });
            return a
        };

        function s(h, l) {
            var f = gb(),
                b = I(),
                d = eb(),
                j = P(h);

            function k(b, d, a) {
                var e = b.zb(p(-d / 2, -a / 2)),
                    f = b.zb(p(d / 2, -a / 2)),
                    g = b.zb(p(d / 2, a / 2)),
                    h = b.zb(p(-d / 2, a / 2));
                b.zb(p(300, 300));
                return p(c.j(e.x, f.x, g.x, h.x) + d / 2, c.j(e.y, f.y, g.y, h.y) + a / 2)
            }

            function a(d, a) {
                a = a || {};
                var n = a.$TranslateZ || 0,
                    p = (a.$RotateX || 0) % 360,
                    q = (a.$RotateY || 0) % 360,
                    u = (a.$Rotate || 0) % 360,
                    l = a.$ScaleX,
                    m = a.$ScaleY,
                    g = a.th;
                if (l == e) l = 1;
                if (m == e) m = 1;
                if (g == e) g = 1;
                if (f) {
                    n = 0;
                    p = 0;
                    q = 0;
                    g = 0
                }
                var c = new Cb(a.$TranslateX, a.$TranslateY, n);
                c.$Scale(l, m, g);
                c.Ne(a.$SkewX, a.$SkewY);
                c.$RotateX(p);
                c.$RotateY(q);
                c.Pe(u);
                if (b) {
                    c.$Move(a.G, a.F);
                    d.style[j] = c.Ge()
                } else if (!A || A < 9) {
                    var o = "",
                        h = {
                            x: 0,
                            y: 0
                        };
                    if (a.$OriginalWidth) h = k(c, a.$OriginalWidth, a.$OriginalHeight);
                    i.Ag(d, h.y);
                    i.Bg(d, h.x);
                    o = c.Be();
                    var s = d.style.filter,
                        t = new RegExp(/[\s]*progid:DXImageTransform\.Microsoft\.Matrix\([^\)]*\)/g),
                        r = D(s, [t], o);
                    U(d, r)
                }
            }
            s = function (f, c) {
                c = c || {};
                var j = c.G,
                    k = c.F,
                    h;
                n(L, function (a, b) {
                    h = c[b];
                    h !== e && a(f, h)
                });
                i.pg(f, c.$Clip);
                if (!b) {
                    j != e && i.R(f, (c.Kd || 0) + j);
                    k != e && i.U(f, (c.Jd || 0) + k)
                }
                if (c.Oe)
                    if (d) yb(i.$CreateCallback(g, R, f, c));
                    else a(f, c)
            };
            i.ic = R;
            if (d) i.ic = s;
            if (f) i.ic = a;
            else if (!b) a = R;
            i.N = s;
            s(h, l)
        }
        i.ic = s;
        i.N = s;

        function Cb(j, k, o) {
            var d = this,
                b = [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, j || 0, k || 0, o || 0, 1],
                i = c.rb,
                h = c.Kb,
                l = c.xd;

            function f(a) {
                return a * c.C / 180
            }

            function n(a, b) {
                return {
                    x: a,
                    y: b
                }
            }

            function m(b, c, f, g, i, l, n, o, q, t, u, w, y, A, C, F, a, d, e, h, j, k, m, p, r, s, v, x, z, B, D, E) {
                return [b * a + c * j + f * r + g * z, b * d + c * k + f * s + g * B, b * e + c * m + f * v + g * D, b * h + c * p + f * x + g * E, i * a + l * j + n * r + o * z, i * d + l * k + n * s + o * B, i * e + l * m + n * v + o * D, i * h + l * p + n * x + o * E, q * a + t * j + u * r + w * z, q * d + t * k + u * s + w * B, q * e + t * m + u * v + w * D, q * h + t * p + u * x + w * E, y * a + A * j + C * r + F * z, y * d + A * k + C * s + F * B, y * e + A * m + C * v + F * D, y * h + A * p + C * x + F * E]
            }

            function e(c, a) {
                return m.apply(g, (a || b).concat(c))
            }
            d.$Scale = function (a, c, d) {
                if (a != 1 || c != 1 || d != 1) b = e([a, 0, 0, 0, 0, c, 0, 0, 0, 0, d, 0, 0, 0, 0, 1])
            };
            d.$Move = function (a, c, d) {
                b[12] += a || 0;
                b[13] += c || 0;
                b[14] += d || 0
            };
            d.$RotateX = function (c) {
                if (c) {
                    a = f(c);
                    var d = h(a),
                        g = i(a);
                    b = e([1, 0, 0, 0, 0, d, g, 0, 0, -g, d, 0, 0, 0, 0, 1])
                }
            };
            d.$RotateY = function (c) {
                if (c) {
                    a = f(c);
                    var d = h(a),
                        g = i(a);
                    b = e([d, 0, -g, 0, 0, 1, 0, 0, g, 0, d, 0, 0, 0, 0, 1])
                }
            };
            d.Pe = function (c) {
                if (c) {
                    a = f(c);
                    var d = h(a),
                        g = i(a);
                    b = e([d, g, 0, 0, -g, d, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1])
                }
            };
            d.Ne = function (a, c) {
                if (a || c) {
                    j = f(a);
                    k = f(c);
                    b = e([1, l(k), 0, 0, l(j), 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1])
                }
            };
            d.zb = function (c) {
                var a = e(b, [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, c.x, c.y, 0, 1]);
                return n(a[12], a[13])
            };
            d.Ge = function () {
                return "matrix3d(" + b.join(",") + ")"
            };
            d.Be = function () {
                return "progid:DXImageTransform.Microsoft.Matrix(M11=" + b[0] + ", M12=" + b[4] + ", M21=" + b[1] + ", M22=" + b[5] + ", SizingMethod='auto expand')"
            }
        }
        new (function () {
            var a = this;

            function b(d, g) {
                for (var j = d[0].length, i = d.length, h = g[0].length, f = [], c = 0; c < i; c++)
                    for (var k = f[c] = [], b = 0; b < h; b++) {
                        for (var e = 0, a = 0; a < j; a++) e += d[c][a] * g[a][b];
                        k[b] = e
                    }
                return f
            }
            a.$ScaleX = function (b, c) {
                return a.ce(b, c, 0)
            };
            a.$ScaleY = function (b, c) {
                return a.ce(b, 0, c)
            };
            a.ce = function (a, c, d) {
                return b(a, [
                    [c, 0],
                    [0, d]
                ])
            };
            a.zb = function (d, c) {
                var a = b(d, [
                    [c.x],
                    [c.y]
                ]);
                return p(a[0][0], a[1][0])
            }
        });
        var S = {
            Kd: 0,
            Jd: 0,
            G: 0,
            F: 0,
            $Zoom: 1,
            $ScaleX: 1,
            $ScaleY: 1,
            $Rotate: 0,
            $RotateX: 0,
            $RotateY: 0,
            $TranslateX: 0,
            $TranslateY: 0,
            $TranslateZ: 0,
            $SkewX: 0,
            $SkewY: 0
        };
        i.Gc = function (c, d) {
            var a = c || {};
            if (c)
                if (b.kd(c)) a = {
                    Y: a
                };
                else if (b.kd(c.$Clip)) a.$Clip = {
                    Y: c.$Clip
                };
            a.Y = a.Y || d;
            if (a.$Clip) a.$Clip.Y = a.$Clip.Y || d;
            return a
        };

        function wb(c, a) {
            var b = {};
            n(c, function (c, d) {
                var f = c;
                if (a[d] != e)
                    if (i.mc(c)) f = c + a[d];
                    else f = wb(c, a[d]);
                b[d] = f
            });
            return b
        }
        i.ve = wb;
        i.Rd = function (n, j, s, t, B, C, o) {
            var a = j;
            if (n) {
                a = {};
                for (var i in j) {
                    var D = C[i] || 1,
                        z = B[i] || [0, 1],
                        h = (s - z[0]) / z[1];
                    h = c.j(c.k(h, 0), 1);
                    h = h * D;
                    var x = c.P(h);
                    if (h != x) h -= x;
                    var k = t.Y || f.$Linear,
                        m, E = n[i],
                        q = j[i];
                    if (b.mc(q)) {
                        k = t[i] || k;
                        var A = k(h);
                        m = E + q * A
                    } else {
                        m = b.B({
                            hc: {}
                        }, n[i]);
                        var y = t[i] || {};
                        b.$Each(q.hc || q, function (d, a) {
                            k = y[a] || y.Y || k;
                            var c = k(h),
                                b = d * c;
                            m.hc[a] = b;
                            m[a] += b
                        })
                    }
                    a[i] = m
                }
                var w = b.$Each(j, function (b, a) {
                    return S[a] != e
                });
                w && b.$Each(S, function (c, b) {
                    if (a[b] == e && n[b] !== e) a[b] = n[b]
                });
                if (w) {
                    if (a.$Zoom) a.$ScaleX = a.$ScaleY = a.$Zoom;
                    a.$OriginalWidth = o.$OriginalWidth;
                    a.$OriginalHeight = o.$OriginalHeight;
                    if (r() && l >= 11 && (j.G || j.F) && s != 0 && s != 1) a.$Rotate = a.$Rotate || 1e-8;
                    a.Oe = d
                }
            }
            if (j.$Clip && o.$Move) {
                var p = a.$Clip.hc,
                    v = (p.$Top || 0) + (p.$Bottom || 0),
                    u = (p.$Left || 0) + (p.$Right || 0);
                a.$Left = (a.$Left || 0) + u;
                a.$Top = (a.$Top || 0) + v;
                a.$Clip.$Left -= u;
                a.$Clip.$Right -= u;
                a.$Clip.$Top -= v;
                a.$Clip.$Bottom -= v
            }
            if (a.$Clip && !a.$Clip.$Top && !a.$Clip.$Left && !a.$Clip.F && !a.$Clip.G && a.$Clip.$Right == o.$OriginalWidth && a.$Clip.$Bottom == o.$OriginalHeight) a.$Clip = g;
            return a
        }
    };

    function p() {
        var a = this,
            e = [];

        function k(a, b) {
            e.push({
                ad: a,
                Vc: b
            })
        }

        function i(a, c) {
            b.$Each(e, function (b, d) {
                b.ad == a && b.Vc === c && e.splice(d, 1)
            })
        }
        a.$On = a.addEventListener = k;
        a.$Off = a.removeEventListener = i;
        a.l = function (a) {
            var c = [].slice.call(arguments, 1);
            b.$Each(e, function (b) {
                b.ad == a && b.Vc.apply(j, c)
            })
        }
    }
    var l = function (A, D, g, L, O, J) {
        A = A || 0;
        var a = this,
            p, m, n, t, B = 0,
            H, I, G, C, z = 0,
            h = 0,
            l = 0,
            y, i, e, f, o, x, v = [],
            w;

        function P(a) {
            e += a;
            f += a;
            i += a;
            h += a;
            l += a;
            z += a
        }

        function s(p) {
            var j = p;
            if (o)
                if (!x && (j >= f || j < e) || x && j >= e) j = ((j - e) % o + o) % o + e;
            if (!y || t || h != j) {
                var k = c.j(j, f);
                k = c.k(k, e);
                if (!y || t || k != l) {
                    if (J) {
                        var m = (k - i) / (D || 1);
                        if (g.$Reverse) m = 1 - m;
                        var n = b.Rd(O, J, m, H, G, I, g);
                        if (w) b.$Each(n, function (b, a) {
                            w[a] && w[a](L, b)
                        });
                        else b.N(L, n)
                    }
                    a.Lc(l - i, k - i);
                    var r = l,
                        q = l = k;
                    b.$Each(v, function (b, c) {
                        var a = !y && x || j <= h ? v[v.length - c - 1] : b;
                        a.L(l - z)
                    });
                    h = j;
                    y = d;
                    a.Ac(r, q)
                }
            }
        }

        function E(a, b, d) {
            b && a.$Shift(f);
            if (!d) {
                e = c.j(e, a.pc() + z);
                f = c.k(f, a.yb() + z)
            }
            v.push(a)
        }
        var u = j.requestAnimationFrame || j.webkitRequestAnimationFrame || j.mozRequestAnimationFrame || j.msRequestAnimationFrame;
        if (b.$IsBrowserSafari() && b.$BrowserVersion() < 7 || !u) u = function (a) {
            b.$Delay(a, g.$Interval)
        };

        function K() {
            if (p) {
                var d = b.cb(),
                    e = c.j(d - B, g.Ld),
                    a = h + e * n;
                B = d;
                if (a * n >= m * n) a = m;
                s(a);
                if (!t && a * n >= m * n) M(C);
                else u(K)
            }
        }

        function r(g, i, j) {
            if (!p) {
                p = d;
                t = j;
                C = i;
                g = c.k(g, e);
                g = c.j(g, f);
                m = g;
                n = m < h ? -1 : 1;
                a.Sd();
                B = b.cb();
                u(K)
            }
        }

        function M(b) {
            if (p) {
                t = p = C = k;
                a.Od();
                b && b()
            }
        }
        a.$Play = function (a, b, c) {
            r(a ? h + a : f, b, c)
        };
        a.ie = r;
        a.Db = M;
        a.Fe = function (a) {
            r(a)
        };
        a.jb = function () {
            return h
        };
        a.he = function () {
            return m
        };
        a.ob = function () {
            return l
        };
        a.L = s;
        a.Ae = function () {
            s(f, d)
        };
        a.$Move = function (a) {
            s(h + a)
        };
        a.$IsPlaying = function () {
            return p
        };
        a.le = function (a) {
            o = a
        };
        a.$Shift = P;
        a.W = function (a, b) {
            E(a, 0, b)
        };
        a.Pc = function (a) {
            E(a, 1)
        };
        a.ke = function (a) {
            f += a
        };
        a.pc = function () {
            return e
        };
        a.yb = function () {
            return f
        };
        a.Ac = a.Sd = a.Od = a.Lc = b.ed;
        a.Hc = b.cb();
        g = b.B({
            $Interval: 16,
            Ld: 50
        }, g);
        o = g.Qc;
        x = g.se;
        w = g.ye;
        e = i = A;
        f = A + D;
        I = g.$Round || {};
        G = g.$During || {};
        H = b.Gc(g.$Easing)
    };
    var m = {
        Sb: "data-scale",
        Ic: "data-scale-ratio",
        Hb: "data-autocenter"
    },
        o = new function () {
            var a = this;
            a.Z = function (c, a, e, d) {
                (d || !b.$Attribute(c, a)) && b.$Attribute(c, a, e)
            };
            a.kc = function (a) {
                var c = b.db(a, m.Hb);
                b.Bc(a, c)
            }
        },
        r = j.$JssorSlideshowFormations$ = new function () {
            var h = this,
                b = 0,
                a = 1,
                f = 2,
                e = 3,
                s = 1,
                r = 2,
                t = 4,
                q = 8,
                w = 256,
                x = 512,
                v = 1024,
                u = 2048,
                j = u + s,
                i = u + r,
                o = x + s,
                m = x + r,
                n = w + t,
                k = w + q,
                l = v + t,
                p = v + q;

            function y(a) {
                return (a & r) == r
            }

            function z(a) {
                return (a & t) == t
            }

            function g(b, a, c) {
                c.push(a);
                b[a] = b[a] || [];
                b[a].push(c)
            }
            h.$FormationStraight = function (f) {
                for (var d = f.$Cols, e = f.$Rows, s = f.$Assembly, t = f.oc, r = [], a = 0, b = 0, p = d - 1, q = e - 1, h = t - 1, c, b = 0; b < e; b++)
                    for (a = 0; a < d; a++) {
                        switch (s) {
                            case j:
                                c = h - (a * e + (q - b));
                                break;
                            case l:
                                c = h - (b * d + (p - a));
                                break;
                            case o:
                                c = h - (a * e + b);
                            case n:
                                c = h - (b * d + a);
                                break;
                            case i:
                                c = a * e + b;
                                break;
                            case k:
                                c = b * d + (p - a);
                                break;
                            case m:
                                c = a * e + (q - b);
                                break;
                            default:
                                c = b * d + a
                        }
                        g(r, c, [b, a])
                    }
                return r
            };
            h.$FormationSwirl = function (q) {
                var x = q.$Cols,
                    y = q.$Rows,
                    B = q.$Assembly,
                    w = q.oc,
                    A = [],
                    z = [],
                    u = 0,
                    c = 0,
                    h = 0,
                    r = x - 1,
                    s = y - 1,
                    t, p, v = 0;
                switch (B) {
                    case j:
                        c = r;
                        h = 0;
                        p = [f, a, e, b];
                        break;
                    case l:
                        c = 0;
                        h = s;
                        p = [b, e, a, f];
                        break;
                    case o:
                        c = r;
                        h = s;
                        p = [e, a, f, b];
                        break;
                    case n:
                        c = r;
                        h = s;
                        p = [a, e, b, f];
                        break;
                    case i:
                        c = 0;
                        h = 0;
                        p = [f, b, e, a];
                        break;
                    case k:
                        c = r;
                        h = 0;
                        p = [a, f, b, e];
                        break;
                    case m:
                        c = 0;
                        h = s;
                        p = [e, b, f, a];
                        break;
                    default:
                        c = 0;
                        h = 0;
                        p = [b, f, a, e]
                }
                u = 0;
                while (u < w) {
                    t = h + "," + c;
                    if (c >= 0 && c < x && h >= 0 && h < y && !z[t]) {
                        z[t] = d;
                        g(A, u++, [h, c])
                    } else switch (p[v++ % p.length]) {
                        case b:
                            c--;
                            break;
                        case f:
                            h--;
                            break;
                        case a:
                            c++;
                            break;
                        case e:
                            h++
                    }
                    switch (p[v % p.length]) {
                        case b:
                            c++;
                            break;
                        case f:
                            h++;
                            break;
                        case a:
                            c--;
                            break;
                        case e:
                            h--
                    }
                }
                return A
            };
            h.$FormationZigZag = function (p) {
                var w = p.$Cols,
                    x = p.$Rows,
                    z = p.$Assembly,
                    v = p.oc,
                    t = [],
                    u = 0,
                    c = 0,
                    d = 0,
                    q = w - 1,
                    r = x - 1,
                    y, h, s = 0;
                switch (z) {
                    case j:
                        c = q;
                        d = 0;
                        h = [f, a, e, a];
                        break;
                    case l:
                        c = 0;
                        d = r;
                        h = [b, e, a, e];
                        break;
                    case o:
                        c = q;
                        d = r;
                        h = [e, a, f, a];
                        break;
                    case n:
                        c = q;
                        d = r;
                        h = [a, e, b, e];
                        break;
                    case i:
                        c = 0;
                        d = 0;
                        h = [f, b, e, b];
                        break;
                    case k:
                        c = q;
                        d = 0;
                        h = [a, f, b, f];
                        break;
                    case m:
                        c = 0;
                        d = r;
                        h = [e, b, f, b];
                        break;
                    default:
                        c = 0;
                        d = 0;
                        h = [b, f, a, f]
                }
                u = 0;
                while (u < v) {
                    y = d + "," + c;
                    if (c >= 0 && c < w && d >= 0 && d < x && typeof t[y] == "undefined") {
                        g(t, u++, [d, c]);
                        switch (h[s % h.length]) {
                            case b:
                                c++;
                                break;
                            case f:
                                d++;
                                break;
                            case a:
                                c--;
                                break;
                            case e:
                                d--
                        }
                    } else {
                        switch (h[s++ % h.length]) {
                            case b:
                                c--;
                                break;
                            case f:
                                d--;
                                break;
                            case a:
                                c++;
                                break;
                            case e:
                                d++
                        }
                        switch (h[s++ % h.length]) {
                            case b:
                                c++;
                                break;
                            case f:
                                d++;
                                break;
                            case a:
                                c--;
                                break;
                            case e:
                                d--
                        }
                    }
                }
                return t
            };
            h.$FormationStraightStairs = function (q) {
                var u = q.$Cols,
                    v = q.$Rows,
                    e = q.$Assembly,
                    t = q.oc,
                    r = [],
                    s = 0,
                    c = 0,
                    d = 0,
                    f = u - 1,
                    h = v - 1,
                    x = t - 1;
                switch (e) {
                    case j:
                    case m:
                    case o:
                    case i:
                        var a = 0,
                            b = 0;
                        break;
                    case k:
                    case l:
                    case n:
                    case p:
                        var a = f,
                            b = 0;
                        break;
                    default:
                        e = p;
                        var a = f,
                            b = 0
                }
                c = a;
                d = b;
                while (s < t) {
                    if (z(e) || y(e)) g(r, x - s++, [d, c]);
                    else g(r, s++, [d, c]);
                    switch (e) {
                        case j:
                        case m:
                            c--;
                            d++;
                            break;
                        case o:
                        case i:
                            c++;
                            d--;
                            break;
                        case k:
                        case l:
                            c--;
                            d--;
                            break;
                        case p:
                        case n:
                        default:
                            c++;
                            d++
                    }
                    if (c < 0 || d < 0 || c > f || d > h) {
                        switch (e) {
                            case j:
                            case m:
                                a++;
                                break;
                            case k:
                            case l:
                            case o:
                            case i:
                                b++;
                                break;
                            case p:
                            case n:
                            default:
                                a--
                        }
                        if (a < 0 || b < 0 || a > f || b > h) {
                            switch (e) {
                                case j:
                                case m:
                                    a = f;
                                    b++;
                                    break;
                                case o:
                                case i:
                                    b = h;
                                    a++;
                                    break;
                                case k:
                                case l:
                                    b = h;
                                    a--;
                                    break;
                                case p:
                                case n:
                                default:
                                    a = 0;
                                    b++
                            }
                            if (b > h) b = h;
                            else if (b < 0) b = 0;
                            else if (a > f) a = f;
                            else if (a < 0) a = 0
                        }
                        d = b;
                        c = a
                    }
                }
                return r
            };
            h.$FormationRectangle = function (f) {
                var d = f.$Cols || 1,
                    e = f.$Rows || 1,
                    h = [],
                    a, b, i;
                i = c.$Round(c.j(d / 2, e / 2)) + 1;
                for (a = 0; a < d; a++)
                    for (b = 0; b < e; b++) g(h, i - c.j(a + 1, b + 1, d - a, e - b), [b, a]);
                return h
            };
            h.$FormationRandom = function (d) {
                for (var e = [], a, b = 0; b < d.$Rows; b++)
                    for (a = 0; a < d.$Cols; a++) g(e, c.I(1e5 * c.td()) % 13, [b, a]);
                return e
            };
            h.$FormationCircle = function (d) {
                for (var e = d.$Cols || 1, f = d.$Rows || 1, h = [], a, i = e / 2 - .5, j = f / 2 - .5, b = 0; b < e; b++)
                    for (a = 0; a < f; a++) g(h, c.$Round(c.Mb(c.q(b - i, 2) + c.q(a - j, 2))), [a, b]);
                return h
            };
            h.$FormationCross = function (d) {
                for (var e = d.$Cols || 1, f = d.$Rows || 1, h = [], a, i = e / 2 - .5, j = f / 2 - .5, b = 0; b < e; b++)
                    for (a = 0; a < f; a++) g(h, c.$Round(c.j(c.m(b - i), c.m(a - j))), [a, b]);
                return h
            };
            h.$FormationRectangleCross = function (f) {
                for (var h = f.$Cols || 1, i = f.$Rows || 1, j = [], a, d = h / 2 - .5, e = i / 2 - .5, k = c.k(d, e) + 1, b = 0; b < h; b++)
                    for (a = 0; a < i; a++) g(j, c.$Round(k - c.k(d - c.m(b - d), e - c.m(a - e))) - 1, [a, b]);
                return j
            }
        };
    j.$JssorSlideshowRunner$ = function (m, s, o, u, z, A) {
        var a = this,
            v, h, e, y = 0,
            x = u.$TransitionsOrder,
            q, i = 8;

        function t(a) {
            if (a.$Top) a.F = a.$Top;
            if (a.$Left) a.G = a.$Left;
            b.$Each(a, function (a) {
                b.sd(a) && t(a)
            })
        }

        function j(h, e, g) {
            var a = {
                $Interval: e,
                $Duration: 1,
                $Delay: 0,
                $Cols: 1,
                $Rows: 1,
                $Opacity: 0,
                $Zoom: 0,
                $Clip: 0,
                $Move: k,
                $SlideOut: k,
                $Reverse: k,
                $Formation: r.$FormationRandom,
                $Assembly: 1032,
                $ChessMode: {
                    $Column: 0,
                    $Row: 0
                },
                $Easing: f.$Linear,
                $Round: {},
                dc: [],
                $During: {}
            };
            b.B(a, h);
            if (a.$Rows == 0) a.$Rows = c.$Round(a.$Cols * g);
            t(a);
            a.oc = a.$Cols * a.$Rows;
            a.$Easing = b.Gc(a.$Easing, f.$Linear);
            a.me = c.I(a.$Duration / a.$Interval);
            a.re = function (c, b) {
                c /= a.$Cols;
                b /= a.$Rows;
                var f = c + "x" + b;
                if (!a.dc[f]) {
                    a.dc[f] = {
                        D: c,
                        E: b
                    };
                    for (var d = 0; d < a.$Cols; d++)
                        for (var e = 0; e < a.$Rows; e++) a.dc[f][e + "," + d] = {
                            $Top: e * b,
                            $Right: d * c + c,
                            $Bottom: e * b + b,
                            $Left: d * c
                        }
                }
                return a.dc[f]
            };
            if (a.$Brother) {
                a.$Brother = j(a.$Brother, e, g);
                a.$SlideOut = d
            }
            return a
        }

        function n(z, i, a, v, n, l) {
            var y = this,
                t, u = {},
                h = {},
                m = [],
                f, e, r, p = a.$ChessMode.$Column || 0,
                q = a.$ChessMode.$Row || 0,
                g = a.re(n, l),
                o = B(a),
                C = o.length - 1,
                s = a.$Duration + a.$Delay * C,
                w = v + s,
                j = a.$SlideOut,
                x;
            w += 50;

            function B(a) {
                var b = a.$Formation(a);
                return a.$Reverse ? b.reverse() : b
            }
            y.rd = w;
            y.cc = function (d) {
                d -= v;
                var e = d < s;
                if (e || x) {
                    x = e;
                    if (!j) d = s - d;
                    var f = c.I(d / a.$Interval);
                    b.$Each(h, function (a, e) {
                        var d = c.k(f, a.j);
                        d = c.j(d, a.length - 1);
                        if (a.Ed != d) {
                            if (!a.Ed && !j) b.J(m[e]);
                            else d == a.k && j && b.eb(m[e]);
                            a.Ed = d;
                            b.N(m[e], a[d])
                        }
                    })
                }
            };
            i = b.$CloneNode(i);
            A(i, 0, 0);
            b.$Each(o, function (i, m) {
                b.$Each(i, function (G) {
                    var I = G[0],
                        H = G[1],
                        v = I + "," + H,
                        o = k,
                        s = k,
                        x = k;
                    if (p && H % 2) {
                        if (p & 3) o = !o;
                        if (p & 12) s = !s;
                        if (p & 16) x = !x
                    }
                    if (q && I % 2) {
                        if (q & 3) o = !o;
                        if (q & 12) s = !s;
                        if (q & 16) x = !x
                    }
                    a.$Top = a.$Top || a.$Clip & 4;
                    a.$Bottom = a.$Bottom || a.$Clip & 8;
                    a.$Left = a.$Left || a.$Clip & 1;
                    a.$Right = a.$Right || a.$Clip & 2;
                    var C = s ? a.$Bottom : a.$Top,
                        z = s ? a.$Top : a.$Bottom,
                        B = o ? a.$Right : a.$Left,
                        A = o ? a.$Left : a.$Right;
                    a.$Clip = C || z || B || A;
                    r = {};
                    e = {
                        F: 0,
                        G: 0,
                        $Opacity: 1,
                        D: n,
                        E: l
                    };
                    f = b.B({}, e);
                    t = b.B({}, g[v]);
                    if (a.$Opacity) e.$Opacity = 2 - a.$Opacity;
                    if (a.$ZIndex) {
                        e.$ZIndex = a.$ZIndex;
                        f.$ZIndex = 0
                    }
                    var K = a.$Cols * a.$Rows > 1 || a.$Clip;
                    if (a.$Zoom || a.$Rotate) {
                        var J = d;
                        if (J) {
                            e.$Zoom = a.$Zoom ? a.$Zoom - 1 : 1;
                            f.$Zoom = 1;
                            var N = a.$Rotate || 0;
                            e.$Rotate = N * 360 * (x ? -1 : 1);
                            f.$Rotate = 0
                        }
                    }
                    if (K) {
                        var i = t.hc = {};
                        if (a.$Clip) {
                            var w = a.$ScaleClip || 1;
                            if (C && z) {
                                i.$Top = g.E / 2 * w;
                                i.$Bottom = -i.$Top
                            } else if (C) i.$Bottom = -g.E * w;
                            else if (z) i.$Top = g.E * w;
                            if (B && A) {
                                i.$Left = g.D / 2 * w;
                                i.$Right = -i.$Left
                            } else if (B) i.$Right = -g.D * w;
                            else if (A) i.$Left = g.D * w
                        }
                        r.$Clip = t;
                        f.$Clip = g[v]
                    }
                    var L = o ? 1 : -1,
                        M = s ? 1 : -1;
                    if (a.x) e.G += n * a.x * L;
                    if (a.y) e.F += l * a.y * M;
                    b.$Each(e, function (a, c) {
                        if (b.mc(a))
                            if (a != f[c]) r[c] = a - f[c]
                    });
                    u[v] = j ? f : e;
                    var D = a.me,
                        y = c.$Round(m * a.$Delay / a.$Interval);
                    h[v] = new Array(y);
                    h[v].j = y;
                    h[v].k = y + D - 1;
                    for (var F = 0; F <= D; F++) {
                        var E = b.Rd(f, r, F / D, a.$Easing, a.$During, a.$Round, {
                            $Move: a.$Move,
                            $OriginalWidth: n,
                            $OriginalHeight: l
                        });
                        E.$ZIndex = E.$ZIndex || 1;
                        h[v].push(E)
                    }
                })
            });
            o.reverse();
            b.$Each(o, function (a) {
                b.$Each(a, function (c) {
                    var f = c[0],
                        e = c[1],
                        d = f + "," + e,
                        a = i;
                    if (e || f) a = b.$CloneNode(i);
                    b.N(a, u[d]);
                    b.Zb(a, "hidden");
                    b.tb(a, "absolute");
                    z.te(a);
                    m[d] = a;
                    b.J(a, !j)
                })
            })
        }

        function w() {
            var a = this,
                b = 0;
            l.call(a, 0, v);
            a.Ac = function (c, a) {
                if (a - b > i) {
                    b = a;
                    e && e.cc(a);
                    h && h.cc(a)
                }
            };
            a.Xc = q
        }
        a.ue = function () {
            var a = 0,
                b = u.$Transitions,
                d = b.length;
            if (x) a = y++ % d;
            else a = c.P(c.td() * d);
            b[a] && (b[a].Cb = a);
            return b[a]
        };
        a.Ce = function (x, y, k, l, b, t) {
            a.Bb();
            q = b;
            b = j(b, i, t);
            var g = l.Cd,
                f = k.Cd;
            g["no-image"] = !l.qc;
            f["no-image"] = !k.qc;
            var p = g,
                r = f,
                w = b,
                d = b.$Brother || j({}, i, t);
            if (!b.$SlideOut) {
                p = f;
                r = g
            }
            var u = d.$Shift || 0;
            h = new n(m, r, d, c.k(u - d.$Interval, 0), s, o);
            e = new n(m, p, w, c.k(d.$Interval - u, 0), s, o);
            h.cc(0);
            e.cc(0);
            v = c.k(h.rd, e.rd);
            a.Cb = x
        };
        a.Bb = function () {
            m.Bb();
            h = g;
            e = g
        };
        a.Ie = function () {
            var a = g;
            if (e) a = new w;
            return a
        };
        if (z && b.$WebKitVersion() < 537) i = 16;
        p.call(a);
        l.call(a, -1e7, 1e7)
    };
    var q = {
        tc: 1
    };
    j.$JssorBulletNavigator$ = function (a, E) {
        var f = this;
        p.call(f);
        a = b.$GetElement(a);
        var u, C, B, t, l = 0,
            e, n, j, y, z, i, h, s, r, D = [],
            A = [];

        function x(a) {
            a != -1 && A[a].zd(a == l)
        }

        function v(a) {
            f.l(q.tc, a * n)
        }
        f.$Elmt = a;
        f.Jc = function (a) {
            if (a != t) {
                var d = l,
                    b = c.P(a / n);
                l = b;
                t = a;
                x(d);
                x(b)
            }
        };
        f.Sc = function (c) {
            b.J(a, c)
        };
        var w;
        f.Tc = function (x) {
            if (!w) {
                u = c.I(x / n);
                l = 0;
                var o = s + y,
                    p = r + z,
                    m = c.I(u / j) - 1;
                C = s + o * (!i ? m : j - 1);
                B = r + p * (i ? m : j - 1);
                b.$CssWidth(a, C);
                b.$CssHeight(a, B);
                for (var f = 0; f < u; f++) {
                    var t = b.wg();
                    b.$InnerText(t, f + 1);
                    var k = b.Id(h, "numbertemplate", t, d);
                    b.tb(k, "absolute");
                    var q = f % (m + 1);
                    b.R(k, !i ? o * q : f % j * o);
                    b.U(k, i ? p * q : c.P(f / (m + 1)) * p);
                    b.$AppendChild(a, k);
                    D[f] = k;
                    e.$ActionMode & 1 && b.$AddEvent(k, "click", b.$CreateCallback(g, v, f));
                    e.$ActionMode & 2 && b.$AddEvent(k, "mouseenter", b.$CreateCallback(g, v, f));
                    A[f] = b.lc(k)
                }
                w = d
            }
        };
        f.zc = e = b.B({
            $SpacingX: 10,
            $SpacingY: 10,
            $Orientation: 1,
            $ActionMode: 1
        }, E);
        h = b.$FindChild(a, "prototype");
        s = b.$CssWidth(h);
        r = b.$CssHeight(h);
        b.fc(h, a);
        n = e.$Steps || 1;
        j = e.$Rows || 1;
        y = e.$SpacingX;
        z = e.$SpacingY;
        i = e.$Orientation - 1;
        e.$Scale == k && o.Z(a, m.Sb, 1);
        e.$AutoCenter && o.Z(a, m.Hb, e.$AutoCenter);
        o.kc(a)
    };
    j.$JssorArrowNavigator$ = function (a, e, i, A, z, x) {
        var c = this;
        p.call(c);
        var j, h, f, l;
        b.$CssWidth(a);
        b.$CssHeight(a);
        var s, r;

        function n(a) {
            c.l(q.tc, a, d)
        }

        function v(c) {
            b.J(a, c);
            b.J(e, c)
        }

        function u() {
            s.$Enable(i.$Loop || !j.Le(h));
            r.$Enable(i.$Loop || !j.xe(h))
        }
        c.Jc = function (c, a, b) {
            h = a;
            !b && u()
        };
        c.Sc = v;
        var t;
        c.Tc = function (c) {
            h = 0;
            if (!t) {
                b.$AddEvent(a, "click", b.$CreateCallback(g, n, -l));
                b.$AddEvent(e, "click", b.$CreateCallback(g, n, l));
                s = b.lc(a);
                r = b.lc(e);
                t = d
            }
        };
        c.zc = f = b.B({
            $Steps: 1
        }, i);
        l = f.$Steps;
        j = x;
        if (f.$Scale == k) {
            o.Z(a, m.Sb, 1);
            o.Z(e, m.Sb, 1)
        }
        if (f.$AutoCenter) {
            o.Z(a, m.Hb, f.$AutoCenter);
            o.Z(e, m.Hb, f.$AutoCenter)
        }
        o.kc(a);
        o.kc(e)
    };
    j.$JssorThumbnailNavigator$ = function (f, E) {
        var j = this,
            x, A, s, a, y = [],
            B, z, e, l, n, w, v, r, t, h, u;
        p.call(j);
        f = b.$GetElement(f);

        function D(n, f) {
            var h = this,
                c, m, l;

            function o() {
                m.zd(s == f)
            }

            function i(g) {
                if (g || !t.we()) {
                    var c = e - f % e,
                        a = t.Ud((f + c) / e - 1),
                        b = a * e + e - c;
                    if (a < 0) b += x % e;
                    if (a >= A) b -= x % e;
                    j.l(q.tc, b, k, d)
                }
            }
            h.Cb = f;
            h.md = o;
            l = n.oe || n.qc || b.$CreateDiv();
            h.ec = c = b.Id(u, "thumbnailtemplate", l, d);
            m = b.lc(c);
            a.$ActionMode & 1 && b.$AddEvent(c, "click", b.$CreateCallback(g, i, 0));
            a.$ActionMode & 2 && b.$AddEvent(c, "mouseenter", b.$CreateCallback(g, i, 1))
        }
        j.Jc = function (a, f, d) {
            if (a != s) {
                var b = s;
                s = a;
                b != -1 && y[b].md();
                y[a].md()
            } !d && t.$PlayTo(t.Ud(c.P(a / e)))
        };
        j.Sc = function (a) {
            b.J(f, a)
        };
        var C;
        j.Tc = function (H, I) {
            if (!C) {
                x = H;
                A = c.I(x / e);
                s = -1;
                var g = a.$Orientation & 1,
                    p = w + (w + l) * (e - 1) * (1 - g),
                    o = v + (v + n) * (e - 1) * g,
                    u = (g ? c.k : c.j)(B, p),
                    q = (g ? c.j : c.k)(z, o);
                r = c.I((B - l) / (w + l) * g + (z - n) / (v + n) * (1 - g));
                r = c.j(r, A);
                var F = p + (p + l) * (r - 1) * g,
                    E = o + (o + n) * (r - 1) * (1 - g);
                u = c.j(u, F);
                q = c.j(q, E);
                b.tb(h, "absolute");
                b.Zb(h, "hidden");
                b.$CssWidth(h, u);
                b.$CssHeight(h, q);
                b.Bc(h, 3);
                var m = [];
                b.$Each(I, function (k, f) {
                    var i = new D(k, f),
                        d = i.ec,
                        a = c.P(f / e),
                        j = f % e;
                    b.R(d, (w + l) * j * (1 - g));
                    b.U(d, (v + n) * j * g);
                    if (!m[a]) {
                        m[a] = b.$CreateDiv();
                        b.$AppendChild(h, m[a])
                    }
                    b.$AppendChild(m[a], d);
                    y.push(i)
                });
                var G = b.B({
                    $AutoPlay: 0,
                    $NaviQuitDrag: k,
                    $SlideWidth: p,
                    $SlideHeight: o,
                    $SlideSpacing: l * g + n * (1 - g),
                    $MinDragOffsetToSlide: 12,
                    $SlideDuration: 200,
                    $PauseOnHover: 1,
                    $Cols: r,
                    $PlayOrientation: a.$Orientation,
                    $DragOrientation: a.$NoDrag || a.$DisableDrag ? 0 : a.$Orientation
                }, a);
                t = new i(f, G);
                j.lg = t.lg;
                C = d
            }
        };
        j.zc = a = b.B({
            $SpacingX: 0,
            $SpacingY: 0,
            $Orientation: 1,
            $ActionMode: 1
        }, E);
        B = b.$CssWidth(f);
        z = b.$CssHeight(f);
        h = b.$FindChild(f, "slides", d);
        u = b.$FindChild(h, "prototype");
        w = b.$CssWidth(u);
        v = b.$CssHeight(u);
        b.fc(u, h);
        e = a.$Rows || 1;
        l = a.$SpacingX;
        n = a.$SpacingY;
        a.$Scale == k && o.Z(f, m.Sb, 1);
        a.$AutoCenter &= a.$Orientation;
        a.$AutoCenter && o.Z(f, m.Hb, a.$AutoCenter);
        o.kc(f)
    };

    function s(e, d, c) {
        var a = this;
        l.call(a, 0, c);
        a.Bd = b.ed;
        a.od = 0;
        a.fd = c
    }
    j.$JssorCaptionSlideo$ = function (t, k, B, E) {
        var a = this,
            u, o = {},
            v = k.$Transitions,
            r = k.$Controls,
            m = new l(0, 0),
            p = [],
            h = [],
            D = E,
            e = D ? 1e8 : 0;
        l.call(a, 0, 0);

        function q(d, c) {
            var a = {};
            b.$Each(d, function (d, f) {
                var e = o[f];
                if (e) {
                    if (b.sd(d)) d = q(d, c || f == "e");
                    else if (c)
                        if (b.mc(d)) d = u[d];
                    a[e] = d
                }
            });
            return a
        }

        function s(d, e) {
            var a = [],
                c = b.$Children(d);
            b.$Each(c, function (c) {
                var d = v[b.db(c, "t")];
                d && a.push({
                    $Elmt: c,
                    Xc: d
                });
                a = a.concat(s(c, e + 1))
            });
            return a
        }

        function n(c, e) {
            var a = p[c];
            if (a == g) {
                a = p[c] = {
                    hb: c,
                    bd: [],
                    id: []
                };
                var d = 0;
                !b.$Each(h, function (a, b) {
                    d = b;
                    return a.hb > c
                }) && d++;
                h.splice(d, 0, a)
            }
            return a
        }

        function y(s, t, h) {
            var a, f;
            if (r) {
                var m = r[b.db(s, "c")];
                if (m) {
                    a = n(m.r, 0);
                    a.rg = m.e || 0
                }
            }
            b.$Each(t, function (i) {
                var g = b.B(d, {}, q(i)),
                    j = b.Gc(g.$Easing);
                delete g.$Easing;
                if (g.$Left) {
                    g.G = g.$Left;
                    j.G = j.$Left;
                    delete g.$Left
                }
                if (g.$Top) {
                    g.F = g.$Top;
                    j.F = j.$Top;
                    delete g.$Top
                }
                var o = {
                    $Easing: j,
                    $OriginalWidth: h.D,
                    $OriginalHeight: h.E
                },
                    k = new l(i.b, i.d, o, s, h, g);
                e = c.k(e, i.b + i.d);
                if (a) {
                    if (!f) f = new l(i.b, 0);
                    f.W(k)
                } else {
                    var m = n(i.b, i.b + i.d);
                    m.bd.push(k)
                }
                h = b.ve(h, g)
            });
            if (a && f) {
                f.Ae();
                var i = f,
                    k, j = f.pc(),
                    o = f.yb(),
                    p = c.k(o, a.rg);
                if (a.hb < o) {
                    if (a.hb > j) {
                        i = new l(j, a.hb - j);
                        i.W(f, d)
                    } else i = g;
                    k = new l(a.hb, p - j, {
                        Qc: p - a.hb,
                        se: d
                    });
                    k.W(f, d)
                }
                i && a.bd.push(i);
                k && a.id.push(k)
            }
            return h
        }

        function x(a) {
            b.$Each(a, function (f) {
                var a = f.$Elmt,
                    e = b.$CssWidth(a),
                    d = b.$CssHeight(a),
                    c = {
                        $Left: b.R(a),
                        $Top: b.U(a),
                        G: 0,
                        F: 0,
                        $Opacity: 1,
                        $ZIndex: b.H(a) || 0,
                        $Rotate: 0,
                        $RotateX: 0,
                        $RotateY: 0,
                        $ScaleX: 1,
                        $ScaleY: 1,
                        $TranslateX: 0,
                        $TranslateY: 0,
                        $TranslateZ: 0,
                        $SkewX: 0,
                        $SkewY: 0,
                        D: e,
                        E: d,
                        $Clip: {
                            $Top: 0,
                            $Right: e,
                            $Bottom: d,
                            $Left: 0
                        }
                    };
                c.Kd = c.$Left;
                c.Jd = c.$Top;
                y(a, f.Xc, c)
            })
        }

        function A(f, e, g) {
            var c = f.b - e;
            if (c) {
                var b = new l(e, c);
                b.W(m, d);
                b.$Shift(g);
                a.W(b)
            }
            a.ke(f.d);
            return c
        }

        function z(e) {
            var c = m.pc(),
                d = 0;
            b.$Each(e, function (e, f) {
                e = b.B({
                    d: 3e3
                }, e);
                A(e, c, d);
                c = e.b;
                d += e.d;
                if (!f || e.t == 2) {
                    a.od = c;
                    a.fd = c + e.d
                }
            })
        }

        function j(k, d, f) {
            var g = d.length;
            if (g > 4)
                for (var m = c.I(g / 4), a = 0; a < m; a++) {
                    var h = d.slice(a * 4, c.j(a * 4 + 4, g)),
                        i = new l(h[0].hb, 0);
                    j(i, h, f);
                    k.W(i)
                } else b.$Each(d, function (a) {
                    b.$Each(f ? a.id : a.bd, function (a) {
                        f && a.ke(e - a.yb());
                        k.W(a)
                    })
                })
        }
        a.Bd = function () {
            a.L(-1, d)
        };
        u = [f.$Linear, f.$Swing, f.$InQuad, f.$OutQuad, f.$InOutQuad, f.$InCubic, f.$OutCubic, f.$InOutCubic, f.$InQuart, f.$OutQuart, f.$InOutQuart, f.$InQuint, f.$OutQuint, f.$InOutQuint, f.$InSine, f.$OutSine, f.$InOutSine, f.$InExpo, f.$OutExpo, f.$InOutExpo, f.$InCirc, f.$OutCirc, f.$InOutCirc, f.$InElastic, f.$OutElastic, f.$InOutElastic, f.$InBack, f.$OutBack, f.$InOutBack, f.$InBounce, f.$OutBounce, f.$InOutBounce, f.$Early, f.$Late];
        var C = {
            $Top: "y",
            $Left: "x",
            $Bottom: "m",
            $Right: "t",
            $Rotate: "r",
            $RotateX: "rX",
            $RotateY: "rY",
            $ScaleX: "sX",
            $ScaleY: "sY",
            $TranslateX: "tX",
            $TranslateY: "tY",
            $TranslateZ: "tZ",
            $SkewX: "kX",
            $SkewY: "kY",
            $Opacity: "o",
            $Easing: "e",
            $ZIndex: "i",
            $Clip: "c"
        };
        b.$Each(C, function (b, a) {
            o[b] = a
        });
        x(s(t, 1));
        j(m, h);
        var w = k.$Breaks || [],
            i = w[b.db(t, "b")] || [];
        i = i.concat({
            b: e,
            d: i.length ? 0 : B
        });
        z(i);
        e = c.k(e, a.yb());
        j(a, h, d);
        a.L(-1)
    };
    var i = j.$JssorSlider$ = (j.module || {}).exports = function () {
        var a = this;
        b.Gg(a, p);
        var Gb = "data-jssor-slider",
            Wb = "data-jssor-thumb",
            v, n, ab, lb, eb, qb, db, U, O, M, Qb, lc, pc = 1,
            kc = 1,
            Yb = 1,
            bc = {},
            z, Z, Eb, Sb, Pb, pb, sb, rb, kb, r = -1,
            Jb, o, S, Q, G, yb, zb, w, N, P, bb, y, X, xb, gb = [],
            gc, ic, cc, qc, Lc, u, mb, J, ec, wb, Hb, fc, L, Cb = 0,
            E = 0,
            K = Number.MAX_VALUE,
            H = Number.MIN_VALUE,
            hc, D, nb, V, R = 1,
            cb, B, fb, Kb = 0,
            Lb = 0,
            T, tb, ub, ob, x, ib, A, Mb, hb = [],
            Tb = b.$Device(),
            vb = Tb.Mg,
            C = [],
            F, W, I, Fb, Vb, Y;

        function xc(e, k, o) {
            var l = this,
                h = {
                    $Top: 2,
                    $Right: 1,
                    $Bottom: 2,
                    $Left: 1
                },
                n = {
                    $Top: "top",
                    $Right: "right",
                    $Bottom: "bottom",
                    $Left: "left"
                },
                g, a, f, i, j = {};
            l.$Elmt = e;
            l.$ScaleSize = function (q, p, t) {
                var l, s = q,
                    r = p;
                if (!f) {
                    f = b.ng(e);
                    g = e.parentNode;
                    i = {
                        $Scale: b.db(e, m.Sb, 1),
                        $AutoCenter: b.db(e, m.Hb)
                    };
                    b.$Each(n, function (c, a) {
                        j[a] = b.db(e, "data-scale-" + c, 1)
                    });
                    a = e;
                    if (k) {
                        a = b.$CloneNode(g, d);
                        b.N(a, {
                            $Top: 0,
                            $Left: 0
                        });
                        b.$AppendChild(a, e);
                        b.$AppendChild(g, a)
                    }
                }
                if (o) {
                    l = c.k(q, p);
                    if (k)
                        if (t > 0 && t < 1) {
                            var v = c.j(q, p);
                            l = c.j(l / v, 1 / (1 - t)) * v
                        }
                } else s = r = l = c.q(O < M ? p : q, i.$Scale);
                b.cg(a, l);
                b.$Attribute(a, m.Ic, l);
                b.$CssWidth(g, f.D * s);
                b.$CssHeight(g, f.E * r);
                var u = b.$IsBrowserIE() && b.$BrowserEngineVersion() < 9 || b.$BrowserEngineVersion() < 10 && b.$IsBrowserIeQuirks() ? l : 1,
                    w = (s - u) * f.D / 2,
                    x = (r - u) * f.E / 2;
                b.R(a, w);
                b.U(a, x);
                b.$Each(f, function (d, a) {
                    if (h[a] && d) {
                        var e = (h[a] & 1) * c.q(q, j[a]) * d + (h[a] & 2) * c.q(p, j[a]) * d / 2;
                        b.ze[a](g, e)
                    }
                });
                b.Bc(g, i.$AutoCenter)
            }
        }

        function Kc() {
            var b = this;
            l.call(b, -1e8, 2e8);
            b.tg = function () {
                var a = b.ob();
                a = t(a);
                var d = c.$Round(a),
                    g = d,
                    f = a - c.P(a),
                    e = Xb(a);
                return {
                    Cb: g,
                    Zf: d,
                    sb: f,
                    bc: a,
                    ag: e
                }
            };
            b.Ac = function (e, b) {
                var g = t(b);
                if (c.m(b - e) > 1e-5) {
                    var f = c.P(b);
                    if (f != b && b > e && (D & 1 || b > E)) f++;
                    jc(f, g, d)
                }
                a.l(i.$EVT_POSITION_CHANGE, g, t(e), b, e)
            }
        }

        function Jc() {
            var a = this;
            l.call(a, 0, 0, {
                Qc: o
            });
            b.$Each(C, function (b) {
                D & 1 && b.le(o);
                a.Pc(b);
                b.$Shift(L / w)
            })
        }

        function Ic() {
            var a = this,
                b = Mb.$Elmt;
            l.call(a, -1, 2, {
                $Easing: f.$Linear,
                ye: {
                    sb: oc
                },
                Qc: o
            }, b, {
                    sb: 1
                }, {
                    sb: -2
                });
            a.ec = b
        }

        function Ac(o, m) {
            var b = this,
                e, f, h, j, c;
            l.call(b, -1e8, 2e8, {
                Ld: 100
            });
            b.Sd = function () {
                cb = d;
                fb = g;
                a.l(i.$EVT_SWIPE_START, t(x.jb()), x.jb())
            };
            b.Od = function () {
                cb = k;
                j = k;
                var b = x.tg();
                a.l(i.$EVT_SWIPE_END, t(x.jb()), x.jb());
                if (!B) {
                    Mc(b.Zf, r);
                    (!b.sb || b.ag) && jc(r, b.bc)
                }
            };
            b.Ac = function (g, d) {
                var a;
                if (j) a = c;
                else {
                    a = f;
                    if (h) {
                        var b = d / h;
                        a = n.$SlideEasing(b) * (f - e) + e
                    }
                }
                x.L(a)
            };
            b.uc = function (a, d, c, g) {
                e = a;
                f = d;
                h = c;
                x.L(a);
                b.L(0);
                b.ie(c, g)
            };
            b.eg = function (a) {
                j = d;
                c = a;
                b.$Play(a, g, d)
            };
            b.fg = function (a) {
                c = a
            };
            x = new Kc;
            x.W(o);
            x.W(m)
        }

        function Bc() {
            var c = this,
                a = mc();
            b.H(a, 0);
            b.$Css(a, "pointerEvents", "none");
            c.$Elmt = a;
            c.te = function (c) {
                b.$AppendChild(a, c);
                b.J(a)
            };
            c.Bb = function () {
                b.eb(a);
                b.Dc(a)
            }
        }

        function Hc(m, f) {
            var e = this,
                s, I, w, j, x = [],
                E, y, P, G, M, B, H, h, v, q;
            l.call(e, -N, N + 1, {});

            function z(a) {
                s && s.Bd();
                O(m, a, 0);
                B = d;
                s = new eb.$Class(m, eb, b.db(m, "idle", ec), !u);
                s.L(0)
            }

            function T() {
                s.Hc < eb.Hc && z()
            }

            function K(p, r, o) {
                if (!G) {
                    G = d;
                    if (j && o) {
                        var g = o.width,
                            c = o.height,
                            m = g,
                            l = c;
                        if (g && c && n.$FillMode) {
                            if (n.$FillMode & 3 && (!(n.$FillMode & 4) || g > S || c > Q)) {
                                var h = k,
                                    q = S / Q * c / g;
                                if (n.$FillMode & 1) h = q > 1;
                                else if (n.$FillMode & 2) h = q < 1;
                                m = h ? g * Q / c : S;
                                l = h ? Q : c * S / g
                            }
                            b.$CssWidth(j, m);
                            b.$CssHeight(j, l);
                            b.U(j, (Q - l) / 2);
                            b.R(j, (S - m) / 2)
                        }
                        b.tb(j, "absolute");
                        a.l(i.$EVT_LOAD_END, f)
                    }
                }
                b.eb(r);
                p && p(e)
            }

            function R(g, b, c, d) {
                if (d == fb && r == f && u)
                    if (!Lc) {
                        var a = t(g);
                        F.Ce(a, f, b, e, c, Q / S);
                        b.Hg();
                        ib.$Shift(a - ib.pc() - 1);
                        ib.L(a);
                        A.uc(a, a, 0)
                    }
            }

            function W(b) {
                if (b == fb && r == f) {
                    if (!h) {
                        var a = g;
                        if (F)
                            if (F.Cb == f) a = F.Ie();
                            else F.Bb();
                        T();
                        h = new Gc(m, f, a, s);
                        h.Wd(q)
                    } !h.$IsPlaying() && h.Yc()
                }
            }

            function D(a, d, k) {
                if (a == f) {
                    if (a != d) C[d] && C[d].be();
                    else !k && h && h.Lg();
                    q && q.$Enable();
                    var l = fb = b.cb();
                    e.Pb(b.$CreateCallback(g, W, l))
                } else {
                    var j = c.j(f, a),
                        i = c.k(f, a),
                        p = c.j(i - j, j + o - i),
                        m = N + n.$LazyLoading - 1;
                    (!M || p <= m) && e.Pb()
                }
            }

            function X() {
                if (r == f && h) {
                    h.Db();
                    q && q.$Quit();
                    q && q.$Disable();
                    h.Td()
                }
            }

            function Y() {
                r == f && h && h.Db()
            }

            function U(b) {
                !V && a.l(i.$EVT_CLICK, f, b)
            }

            function L() {
                q = v.pInstance;
                h && h.Wd(q)
            }
            e.Pb = function (e, c) {
                c = c || w;
                if (x.length && !G) {
                    b.J(c);
                    if (!P) {
                        P = d;
                        a.l(i.$EVT_LOAD_START, f);
                        b.$Each(x, function (a) {
                            if (!b.$Attribute(a, "src")) {
                                a.src = b.$AttributeEx(a, "src2") || "";
                                b.ac(a, a["display-origin"])
                            }
                        })
                    }
                    b.qe(x, j, b.$CreateCallback(g, K, e, c))
                } else K(e, c)
            };
            e.hf = function () {
                if (o == 1) {
                    e.be();
                    D(f, f)
                } else {
                    var a;
                    if (F) a = F.ue(o);
                    if (a) {
                        var h = fb = b.cb(),
                            c = f + mb,
                            d = C[t(c)];
                        return d.Pb(b.$CreateCallback(g, R, c, d, a, h), w)
                    } else Ob(mb)
                }
            };
            e.Nc = function () {
                D(f, f, d)
            };
            e.be = function () {
                q && q.$Quit();
                q && q.$Disable();
                e.Md();
                h && h.jf();
                h = g;
                z()
            };
            e.Hg = function () {
                b.eb(m)
            };
            e.Md = function () {
                b.J(m)
            };
            e.nf = function () {
                q && q.$Enable()
            };

            function O(a, f, c, h) {
                if (b.$Attribute(a, Gb)) return;
                if (!B) {
                    if (a.tagName == "IMG") {
                        x.push(a);
                        if (!b.$Attribute(a, "src")) {
                            M = d;
                            a["display-origin"] = b.ac(a);
                            b.eb(a)
                        }
                    }
                    var e = b.pe(a);
                    if (e) {
                        var g = new Image;
                        b.$AttributeEx(g, "src2", e);
                        x.push(g)
                    }
                    c && b.H(a, (b.H(a) || 0) + 1)
                }
                var i = b.$Children(a);
                b.$Each(i, function (a) {
                    var e = a.tagName,
                        g = b.$AttributeEx(a, "u");
                    if (g == "player" && !v) {
                        v = a;
                        if (v.pInstance) L();
                        else b.$AddEvent(v, "dataavailable", L)
                    }
                    if (g == "caption") {
                        if (f) {
                            b.dg(a, b.$AttributeEx(a, "to"));
                            b.gg(a, b.$AttributeEx(a, "bf"));
                            H && b.$AttributeEx(a, "3d") && b.jg(a, "preserve-3d")
                        }
                    } else if (!B && !c && !j) {
                        if (e == "A") {
                            if (b.$AttributeEx(a, "u") == "image") j = b.sg(a, "IMG");
                            else j = b.$FindChild(a, "image", d);
                            if (j) {
                                E = a;
                                b.N(E, kb);
                                y = b.$CloneNode(E, d);
                                b.Fc(y, 0);
                                b.$Css(y, "backgroundColor", "#000")
                            }
                        } else if (e == "IMG" && b.$AttributeEx(a, "u") == "image") j = a;
                        if (j) {
                            j.border = 0;
                            b.N(j, kb)
                        }
                    }
                    O(a, f, c + 1, h)
                })
            }
            e.Lc = function (c, b) {
                var a = N - b;
                oc(I, a)
            };
            e.Cb = f;
            p.call(e);
            H = b.$AttributeEx(m, "p");
            b.ig(m, H);
            b.hg(m, b.$AttributeEx(m, "po"));
            var J = b.$FindChild(m, "thumb", d);
            if (J) {
                e.oe = b.$CloneNode(J);
                b.eb(J)
            }
            b.J(m);
            w = b.$CloneNode(Z);
            b.H(w, 1e3);
            b.$AddEvent(m, "click", U);
            z(d);
            e.qc = j;
            e.Nd = y;
            e.Cd = m;
            e.ec = I = m;
            b.$AppendChild(I, w);
            a.$On(203, D);
            a.$On(28, Y);
            a.$On(24, X)
        }

        function Gc(z, g, p, q) {
            var c = this,
                n = 0,
                v = 0,
                h, j, f, e, m, t, s, o = C[g];
            l.call(c, 0, 0);

            function w() {
                b.Dc(W);
                qc && m && o.Nd && b.$AppendChild(W, o.Nd);
                b.J(W, !m && o.qc)
            }

            function x() {
                c.Yc()
            }

            function y(a) {
                s = a;
                c.Db();
                c.Yc()
            }
            c.Yc = function () {
                var b = c.ob();
                if (!B && !cb && !s && r == g) {
                    if (!b) {
                        if (h && !m) {
                            m = d;
                            c.Td(d);
                            a.l(i.$EVT_SLIDESHOW_START, g, n, v, h, e)
                        }
                        w()
                    }
                    var k, p = i.$EVT_STATE_CHANGE;
                    if (b != e)
                        if (b == f) k = e;
                        else if (b == j) k = f;
                        else if (!b) k = j;
                        else k = c.he();
                    a.l(p, g, b, n, j, f, e);
                    var l = u && (!J || R);
                    if (b == e) (f != e && !(J & 12) || l) && o.hf();
                    else (l || b != f) && c.ie(k, x)
                }
            };
            c.Lg = function () {
                f == e && f == c.ob() && c.L(j)
            };
            c.jf = function () {
                F && F.Cb == g && F.Bb();
                var b = c.ob();
                b < e && a.l(i.$EVT_STATE_CHANGE, g, -b - 1, n, j, f, e)
            };
            c.Td = function (a) {
                p && b.Zb(bb, a && p.Xc.$Outside ? "" : "hidden")
            };
            c.Lc = function (c, b) {
                if (m && b >= h) {
                    m = k;
                    w();
                    o.Md();
                    F.Bb();
                    a.l(i.$EVT_SLIDESHOW_END, g, n, v, h, e)
                }
                a.l(i.$EVT_PROGRESS_CHANGE, g, b, n, j, f, e)
            };
            c.Wd = function (a) {
                if (a && !t) {
                    t = a;
                    a.$On($JssorPlayer$.Re, y)
                }
            };
            p && c.Pc(p);
            h = c.yb();
            c.Pc(q);
            j = h + q.od;
            e = c.yb();
            f = u ? h + q.fd : e
        }

        function Ib(a, c, d) {
            b.R(a, c);
            b.U(a, d)
        }

        function oc(c, b) {
            var a = y > 0 ? y : ab,
                d = (yb * b + Cb) * (a & 1),
                e = (zb * b + Cb) * (a >> 1 & 1);
            Ib(c, d, e)
        }

        function Db(a) {
            if (!(D & 1)) a = c.j(K, c.k(a, H));
            return a
        }

        function Xb(a) {
            return !(D & 1) && (a - H < .0001 || K - a < .0001)
        }

        function dc() {
            Fb = cb;
            Vb = A.he();
            I = x.jb()
        }

        function sc() {
            dc();
            if (B || !R && J & 12) {
                A.Db();
                a.l(i.of)
            }
        }

        function rc(g) {
            if (!B && (R || !(J & 12)) && !A.$IsPlaying()) {
                var b = x.jb(),
                    a = I,
                    e = 0;
                if (g && c.m(T) >= n.$MinDragOffsetToSlide) {
                    a = b;
                    e = ub
                }
                if (Xb(b)) {
                    if (!g || V) a = c.$Round(a)
                } else a = c.I(a);
                a = Db(a + e);
                if (!(D & 1)) {
                    if (K - a < .5) a = K;
                    if (a - H < .5) a = H
                }
                var d = c.m(a - b);
                if (d < 1 && n.$SlideEasing != f.$Linear) d = 1 - c.q(1 - d, 5);
                if (!V && Fb) A.Fe(Vb);
                else if (b == a) {
                    Jb.nf();
                    Jb.Nc()
                } else A.uc(b, a, d * wb)
            }
        }

        function Ub(a) {
            !b.Wb(b.$EvtSrc(a), "nodrag") && b.$CancelEvent(a)
        }

        function Ec(a) {
            nc(a, 1)
        }

        function nc(c, j) {
            var f = b.$EvtSrc(c);
            xb = k;
            var l = b.Wb(f, "1", Wb);
            if ((!l || l === v) && !X && (!j || c.touches.length == 1)) {
                xb = b.Wb(f, "nodrag") || !nb || !Fc();
                var n = b.Wb(f, e, m.Ic);
                if (n) Yb = b.$Attribute(n, m.Ic);
                if (j) {
                    var p = c.touches[0];
                    Kb = p.clientX;
                    Lb = p.clientY
                } else {
                    var o = b.de(c);
                    Kb = o.x;
                    Lb = o.y
                }
                B = d;
                fb = g;
                b.$AddEvent(h, j ? "touchmove" : "mousemove", Rb);
                b.cb();
                V = 0;
                sc();
                if (!Fb) y = 0;
                T = 0;
                tb = 0;
                ub = 0;
                a.l(i.$EVT_DRAG_START, t(I), I, c)
            }
        }

        function Rb(g) {
            if (B) {
                var a;
                if (g.type != "mousemove")
                    if (g.touches.length == 1) {
                        var p = g.touches[0];
                        a = {
                            x: p.clientX,
                            y: p.clientY
                        }
                    } else jb();
                else a = b.de(g);
                if (a) {
                    var e = a.x - Kb,
                        f = a.y - Lb;
                    if (y || c.m(e) > 1.5 || c.m(f) > 1.5) {
                        if (c.P(I) != I) y = y || ab & X;
                        if ((e || f) && !y) {
                            if (X == 3)
                                if (c.m(f) > c.m(e)) y = 2;
                                else y = 1;
                            else y = X;
                            if (vb && y == 1 && c.m(f) > c.m(e) * 2.4) xb = d
                        }
                        var n = f,
                            i = zb;
                        if (y == 1) {
                            n = e;
                            i = yb
                        }
                        if (T - tb < -1.5) ub = 0;
                        else if (T - tb > 1.5) ub = -1;
                        tb = T;
                        T = n;
                        Y = I - T / i / Yb;
                        if (!(D & 1)) {
                            var l = 0,
                                j = [-I + E, 0, I - o + P - G / w - E];
                            b.$Each(j, function (b, d) {
                                if (b > 0) {
                                    var a = c.q(b, 1 / 1.6);
                                    a = c.xd(a * c.C / 2);
                                    l = (a - b) * (d - 1)
                                }
                            });
                            var h = l + Y,
                                m = k;
                            j = [-h + E, 0, h - o + P + G / w - E];
                            b.$Each(j, function (a, b) {
                                if (a > 0) {
                                    a = c.j(a, i);
                                    a = c.Af(a) * 2 / c.C;
                                    a = c.q(a, 1.6);
                                    Y = a * (b - 1) + E;
                                    if (b) Y += o - P - G / w;
                                    m = d
                                }
                            });
                            if (!m) Y = h
                        }
                        if (T && y && !xb) {
                            b.$CancelEvent(g);
                            if (!cb) A.eg(Y);
                            else A.fg(Y)
                        }
                    }
                }
            }
        }

        function jb() {
            Cc();
            if (B) {
                V = T;
                b.cb();
                b.fb(h, "mousemove", Rb);
                b.fb(h, "touchmove", Rb);
                V && u & 8 && (u = 0);
                A.Db();
                B = k;
                var c = x.jb();
                a.l(i.$EVT_DRAG_END, t(c), c, t(I), I);
                J & 12 && dc();
                rc(d)
            }
        }

        function wc(c) {
            var a = b.$EvtSrc(c),
                d = b.Wb(a, "1", Gb);
            if (v === d)
                if (V) {
                    b.$StopEvent(c);
                    while (a && v !== a) {
                        (a.tagName == "A" || b.$Attribute(a, "data-jssor-button")) && b.$CancelEvent(c);
                        a = a.parentNode
                    }
                } else u & 4 && (u = 0)
        }

        function Nc(d) {
            if (d != r) {
                var b = ob.ob(),
                    a = Db(d),
                    e = c.$Round(t(a));
                if (b - a < .5) a = b;
                C[r];
                r = e;
                Jb = C[r];
                x.L(a)
            }
        }

        function Mc(b, c) {
            y = 0;
            Nc(b);
            if (u & 2 && (mb > 0 && r == o - 1 || mb < 0 && !r)) u = 0;
            a.l(i.$EVT_PARK, r, c)
        }

        function jc(a, d, e) {
            if (!(D & 1)) {
                a = c.k(0, a);
                a = c.j(a, o - P + E);
                a = c.$Round(a)
            }
            a = t(a);
            b.$Each(gb, function (b) {
                b.Jc(a, d, e)
            })
        }

        function Fc() {
            var b = i.fe || 0,
                a = nb;
            i.fe |= a;
            return X = a & ~b
        }

        function Cc() {
            if (X) {
                i.fe &= ~nb;
                X = 0
            }
        }

        function mc() {
            var a = b.$CreateDiv();
            b.N(a, kb);
            return a
        }

        function t(b, a) {
            a = a || o || 1;
            return (b % a + a) % a
        }

        function Bb(c, a, b) {
            u & 8 && (u = 0);
            Ab(c, wb, a, b)
        }

        function Nb() {
            b.$Each(gb, function (a) {
                a.Sc(a.zc.$ChanceToShow <= R)
            })
        }

        function uc() {
            if (!R) {
                R = 1;
                Nb();
                if (!B) {
                    J & 12 && rc();
                    J & 3 && C[r] && C[r].Nc()
                }
            }
            a.l(i.$EVT_MOUSE_LEAVE)
        }

        function tc() {
            if (R) {
                R = 0;
                Nb();
                B || !(J & 12) || sc()
            }
            a.l(i.$EVT_MOUSE_ENTER)
        }

        function vc() {
            b.$Each(hb, function (a) {
                b.N(a, kb);
                b.Zb(a, "hidden");
                b.eb(a)
            });
            b.N(Z, kb)
        }

        function Ob(b, a) {
            Ab(b, a, d)
        }

        function Ab(l, g, m, p) {
            if (!B && (R || !(J & 12)) || n.$NaviQuitDrag) {
                cb = d;
                B = k;
                A.Db();
                if (g == e) g = wb;
                var b = t(ob.ob()),
                    f = l;
                if (m) {
                    f = b + l;
                    f = c.$Round(f)
                }
                var a = f;
                if (!(D & 1)) {
                    if (p) a = t(a);
                    else if (D & 2 && (a < 0 && c.m(b - H) < .0001 || a > o - P && c.m(b - K) < .0001)) a = a < 0 ? K : H;
                    a = Db(a);
                    if (K - a < .5) a = K;
                    if (a - H < .5) a = H
                }
                var j = (a - b) % o;
                a = b + j;
                var h = b == a ? 0 : g * c.m(j),
                    i = 1;
                if (N > 1) i = (ab & 1 ? sb : rb) / w;
                h = c.j(h, g * i * 1.5);
                A.uc(b, a, h || 1)
            }
        }
        a.$SlidesCount = function () {
            return hb.length
        };
        a.$CurrentIndex = function () {
            return r
        };
        a.$AutoPlay = function (a) {
            if (a == e) return u;
            if (a != u) {
                u = a;
                u && C[r] && C[r].Nc()
            }
        };
        a.$IsDragging = function () {
            return B
        };
        a.$IsSliding = function () {
            return cb
        };
        a.$IsMouseOver = function () {
            return !R
        };
        a.we = function () {
            return V
        };
        a.$OriginalWidth = function () {
            return O
        };
        a.$OriginalHeight = function () {
            return M
        };
        a.$ScaleHeight = function (b) {
            if (b == e) return lc || M;
            a.$ScaleSize(b / M * O, b)
        };
        a.$ScaleWidth = function (b) {
            if (b == e) return Qb || O;
            a.$ScaleSize(b, b / O * M)
        };
        a.$ScaleSize = function (c, a, d) {
            b.$CssWidth(v, c);
            b.$CssHeight(v, a);
            pc = c / O;
            kc = a / M;
            b.$Each(bc, function (a) {
                a.$ScaleSize(pc, kc, d)
            });
            if (!Qb) {
                b.Nb(bb, z);
                b.U(bb, 0);
                b.R(bb, 0)
            }
            Qb = c;
            lc = a
        };
        a.Le = function (a) {
            return c.m(a - H) < .0001
        };
        a.xe = function (a) {
            return c.m(a - K) < .0001
        };
        a.$PlayTo = Ab;
        a.$GoTo = function (a) {
            A.uc(a, a, 0)
        };
        a.$Next = function () {
            Ob(1)
        };
        a.$Prev = function () {
            Ob(-1)
        };
        a.$Pause = function () {
            u = 0
        };
        a.$Play = function () {
            a.$AutoPlay(u || 1)
        };
        a.$SetSlideshowTransitions = function (a) {
            n.$SlideshowOptions.$Transitions = a
        };
        a.$SetCaptionTransitions = function (a) {
            eb.$Transitions = a;
            eb.Hc = b.cb()
        };
        a.Ud = function (a) {
            a = t(a);
            if (D & 1) {
                var d = L / w,
                    b = t(ob.ob()),
                    e = t(a - b + d),
                    f = t(c.m(a - b));
                if (e >= N) {
                    if (f > o / 2)
                        if (a > b) a -= o;
                        else a += o
                } else if (a > b && e < d) a -= o;
                else if (a < b && e > d) a += o
            }
            return a
        };
        a.jc = function (I, m) {
            a.$Elmt = v = b.$GetElement(I);
            O = b.$CssWidth(v);
            M = b.$CssHeight(v);
            n = b.B({
                $FillMode: 0,
                $LazyLoading: 1,
                $ArrowKeyNavigation: 1,
                $StartIndex: 0,
                $AutoPlay: 0,
                $Loop: 1,
                $HWA: d,
                $NaviQuitDrag: d,
                $AutoPlaySteps: 1,
                $AutoPlayInterval: 3e3,
                $PauseOnHover: 1,
                $SlideDuration: 500,
                $SlideEasing: f.$OutQuad,
                $MinDragOffsetToSlide: 20,
                $SlideSpacing: 0,
                $UISearchMode: 1,
                $PlayOrientation: 1,
                $DragOrientation: 1
            }, m);
            n.$HWA = n.$HWA && b.yg();
            if (n.$Idle != e) n.$AutoPlayInterval = n.$Idle;
            if (n.$DisplayPieces != e) n.$Cols = n.$DisplayPieces;
            if (n.$ParkingPosition != e) n.$Align = n.$ParkingPosition;
            ab = n.$PlayOrientation & 3;
            lb = n.$SlideshowOptions;
            eb = b.B({
                $Class: s
            }, n.$CaptionSliderOptions);
            qb = n.$BulletNavigatorOptions;
            db = n.$ArrowNavigatorOptions;
            U = n.$ThumbnailNavigatorOptions;
            !n.$UISearchMode;
            var p = b.$Children(v);
            b.$Each(p, function (a, d) {
                var c = b.$AttributeEx(a, "u");
                if (c == "loading") Z = a;
                else {
                    if (c == "slides") z = a;
                    if (c == "navigator") Eb = a;
                    if (c == "arrowleft") Sb = a;
                    if (c == "arrowright") Pb = a;
                    if (c == "thumbnavigator") pb = a;
                    if (a.tagName != "STYLE" && a.tagName != "SCRIPT") bc[c || d] = new xc(a, c == "slides", b.dd(["slides", "thumbnavigator"])[c])
                }
            });
            Z = Z || b.$CreateDiv(h);
            sb = b.$CssWidth(z);
            rb = b.$CssHeight(z);
            S = n.$SlideWidth || sb;
            Q = n.$SlideHeight || rb;
            kb = {
                D: S,
                E: Q,
                $Top: 0,
                $Left: 0,
                bc: "block",
                sb: "absolute"
            };
            G = n.$SlideSpacing;
            yb = S + G;
            zb = Q + G;
            w = ab & 1 ? yb : zb;
            var i = ab & 1 ? sb : rb;
            mb = n.$AutoPlaySteps;
            J = n.$PauseOnHover;
            ec = n.$AutoPlayInterval;
            wb = n.$SlideDuration;
            Mb = new Bc;
            if (n.$HWA && (!b.$IsBrowserFireFox() || vb)) Ib = function (a, c, d) {
                b.ic(a, {
                    $TranslateX: c,
                    $TranslateY: d
                })
            };
            u = n.$AutoPlay & 63;
            a.zc = m;
            b.$Attribute(v, Gb, "1");
            b.H(z, b.H(z) || 0);
            b.tb(z, "absolute");
            bb = b.$CloneNode(z, d);
            b.Nb(bb, z);
            ib = new Ic;
            b.$AppendChild(bb, ib.ec);
            b.Zb(z, "hidden");
            J &= vb ? 10 : 5;
            var r = b.$Children(z),
                B = b.dd(["DIV", "A", "LI"]);
            b.$Each(r, function (a) {
                B[a.tagName.toUpperCase()] && !b.$AttributeEx(a, "u") && hb.push(a);
                b.H(a, (b.H(a) || 0) + 1)
            });
            W = mc();
            b.$Css(W, "backgroundColor", "#000");
            b.Fc(W, 0);
            b.H(W, 0);
            b.Nb(W, z.firstChild, z);
            o = hb.length;
            if (o) {
                vc();
                L = n.$Align;
                if (L == e) L = (i - w + G) / 2;
                P = i / w;
                N = c.j(o, n.$Cols || o, c.I(P));
                hc = N < o;
                D = hc ? n.$Loop : 0;
                if (o * w - G <= i) {
                    P = o - G / w;
                    L = (i - w + G) / 2;
                    Cb = (i - w * o + G) / 2
                }
                if (lb) {
                    qc = lb.$ShowLink;
                    Hb = lb.$Class;
                    fc = !L && N == 1 && o > 1 && Hb && (!b.$IsBrowserIE() || b.$BrowserVersion() >= 9)
                }
                if (!(D & 1)) {
                    E = L / w;
                    if (E > o - 1) {
                        E = o - 1;
                        L = E * w
                    }
                    H = E;
                    K = H + o - P - G / w
                }
                nb = (N > 1 || L ? ab : -1) & n.$DragOrientation;
                Tb.Zd && b.$Css(z, Tb.Zd, ([g, "pan-y", "pan-x", "none"])[nb] || "");
                if (fc) F = new Hb(Mb, S, Q, lb, vb, Ib);
                for (var k = 0; k < hb.length; k++) {
                    var x = hb[k],
                        y = new Hc(x, k);
                    C.push(y)
                }
                b.eb(Z);
                ob = new Jc;
                A = new Ac(ob, ib);
                b.$AddEvent(v, "click", wc, d);
                b.$AddEvent(v, "mouseleave", uc);
                b.$AddEvent(v, "mouseenter", tc);
                b.$AddEvent(v, "mousedown", nc);
                b.$AddEvent(v, "touchstart", Ec);
                b.$AddEvent(v, "dragstart", Ub);
                b.$AddEvent(v, "selectstart", Ub);
                b.$AddEvent(j, "mouseup", jb);
                b.$AddEvent(h, "mouseup", jb);
                b.$AddEvent(h, "touchend", jb);
                b.$AddEvent(h, "touchcancel", jb);
                b.$AddEvent(j, "blur", jb);
                if (Eb && qb) {
                    gc = new qb.$Class(Eb, qb, O, M);
                    gb.push(gc)
                }
                if (db && Sb && Pb) {
                    db.$Loop = D;
                    ic = new db.$Class(Sb, Pb, db, O, M, a);
                    gb.push(ic)
                }
                if (pb && U) {
                    U.$StartIndex = n.$StartIndex;
                    U.$ArrowKeyNavigation = U.$ArrowKeyNavigation || 0;
                    cc = new U.$Class(pb, U);
                    !U.$NoDrag && b.$Attribute(pb, Wb, "1");
                    gb.push(cc)
                }
                b.$Each(gb, function (a) {
                    a.Tc(o, C, Z);
                    a.$On(q.tc, Bb)
                });
                b.$Css(v, "visibility", "visible");
                a.$ScaleSize(O, M);
                Nb();
                n.$ArrowKeyNavigation && b.$AddEvent(h, "keydown", function (a) {
                    if (a.keyCode == 37) Bb(-n.$ArrowKeyNavigation, d);
                    else a.keyCode == 39 && Bb(n.$ArrowKeyNavigation, d)
                });
                var l = n.$StartIndex;
                l = t(l);
                Ab(l, 0)
            }
        };
        b.jc(a)
    };
    i.$EVT_CLICK = 21;
    i.$EVT_DRAG_START = 22;
    i.$EVT_DRAG_END = 23;
    i.$EVT_SWIPE_START = 24;
    i.$EVT_SWIPE_END = 25;
    i.$EVT_LOAD_START = 26;
    i.$EVT_LOAD_END = 27;
    i.of = 28;
    i.$EVT_MOUSE_ENTER = 31;
    i.$EVT_MOUSE_LEAVE = 32;
    i.$EVT_POSITION_CHANGE = 202;
    i.$EVT_PARK = 203;
    i.$EVT_SLIDESHOW_START = 206;
    i.$EVT_SLIDESHOW_END = 207;
    i.$EVT_PROGRESS_CHANGE = 208;
    i.$EVT_STATE_CHANGE = 209
}(window, document, Math, null, true, false)
