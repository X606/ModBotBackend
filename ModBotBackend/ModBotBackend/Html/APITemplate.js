function setCookie(cname, cvalue, exmins) {
	var d = new Date();
	d.setTime(d.getTime() + (exmins * 60 * 1000));
	var expires = "expires=" + d.toUTCString();
	document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/;SameSite=Strict";
}
function getCookie(cname) {
	var name = cname + "=";
	var decodedCookie = decodeURIComponent(document.cookie);
	var ca = decodedCookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') {
			c = c.substring(1);
		}
		if (c.indexOf(name) == 0) {
			return c.substring(name.length, c.length);
		}
	}
	return "";
}

function Post(url, data) {
	var xhr = new XMLHttpRequest();
	xhr.open("POST", url, true);
	xhr.setRequestHeader('Content-Type', 'application/json');
	xhr.send(JSON.stringify(data));

	return new Promise(resolve => {
		xhr.onload = function () {
			resolve(xhr.responseText);
		}
	});	
}

const sessionIdKey = "SessionID";
function setCurrentSessionId(sessionId) {
	setCookie(sessionIdKey, sessionId, 60);
}
const API = {};

API.getCurrentSessionId = () => {
	return getCookie(sessionIdKey);
}
