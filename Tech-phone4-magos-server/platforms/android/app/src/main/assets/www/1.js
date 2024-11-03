//-----------------------------------------------------------------------
		var GetViewportScrollY = function() {
			if (document.documentElement && document.documentElement.scrollTop)
				return document.documentElement.scrollTop;
			else if (document.body && document.body.scrollTop)
				return document.body.scrollTop;
			else if (window.pageYOffset)
				return window.pageYOffset;
			else if (window.scrollY)
				return window.scrollY;
			else
				return 0;
		}; // GetViewportScrollY

		//-----------------------------------------------------------------------
		var GetViewportScrollX = function() {
			if (document.documentElement && document.documentElement.scrollLeft)
				return document.documentElement.scrollLeft;
			else if (document.body && document.body.scrollLeft)
				return document.body.scrollLeft;
			else if (window.pageXOffset)
				return window.pageXOffset;
			else if (window.scrollX)
				return window.scrollX;
			else
				return 0;
		}; // GetViewportScrollX

		//-----------------------------------------------------------------------
		function GetElementSize(oElement) {
			if (!oElement)
				return;

			var nElementWidth = typeof(oElement.clientWidth) == "number" ? oElement.clientWidth : oElement.offsetWidth;
			var nElementHeight = typeof(oElement.clientHeight) == "number" ? oElement.clientHeight : oElement.offsetHeight;

			return { width: nElementWidth, height: nElementHeight };
		} // GetElementSize

		//-----------------------------------------------------------------------
		function GetWindowSize() {
			var nWinWidth = 0, nWinHeight = 0;
			if (typeof(window.innerWidth) == "number") {
				// Non-IE
				nWinWidth = window.innerWidth;
				nWinHeight = window.innerHeight;
			} else if (document.documentElement &&
			(document.documentElement.clientWidth || document.documentElement.clientHeight)) {
				// IE 6+ in "standards compliant mode".
				nWinWidth = document.documentElement.clientWidth;
				nWinHeight = document.documentElement.clientHeight;
			} else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
				// IE 4 compatible.
				nWinWidth = document.body.clientWidth;
				nWinHeight = document.body.clientHeight;
			}

			// alert( "Browser Window Width = " + nWinWidth + " Browser Window Height = " + nWinHeight);
			return { width: nWinWidth, height: nWinHeight };
		} // GetWindowSize

//-----------------------------------------------------------------------
		function CenterElement(oElement) {

			if (!oElement)
				return;

			var oWinSize = GetWindowSize();
			var oElementSize = GetElementSize(oElement);

			g_scrollX = GetViewportScrollX();
			oElement.style.left = Math.max(0, /*GetViewportScrollX() +*/ ((oWinSize.width - oElementSize.width) >> 1)) + "px";

			g_scrollY = GetViewportScrollY();
			oElement.style.top = Math.max(0, /*GetViewportScrollY() +*/ ((oWinSize.height - oElementSize.height) >> 1)) + "px";


		} // CenterElement

//-------------------------------------------------------
function GetDocumentSize() {
			var oDB = document.body;
			var oDE = document.documentElement;

			var nDocWidth = Math.max(oDB.scrollWidth,
				oDE.scrollWidth,
				oDB.offsetWidth,
				oDE.offsetWidth,
				oDB.clientWidth,
				oDE.clientWidth);
			var nDocHeight = Math.max(oDB.scrollHeight,
				oDE.scrollHeight,
				oDB.offsetHeight,
				oDE.offsetHeight,
				oDB.clientHeight,
				oDE.clientHeight);

			// alert( "Browser Document Width = " + nWinWidth + " Browser Document Height = " + nDocHeight);
			return { width: nDocWidth, height: nDocHeight };
} // GetDocumentSize
