mergeInto(LibraryManager.library, {
    SendMessageToJS: function(eventName) {
        var eventNameStr = UTF8ToString(eventName);
        window.dispatchEvent(new CustomEvent('unity-message', { detail: eventNameStr }));
    },

    UnityLogin: function(sessionKey) {
        var sessionKeyStr = UTF8ToString(sessionKey);
        window.dispatchEvent(new CustomEvent('UnityLoginRequest', { detail: sessionKeyStr }));
    },

    SendSessionKey: function(sessionKey) {
        var sessionKeyStr = UTF8ToString(sessionKey);
        window.dispatchEvent(new CustomEvent('SendSessionKey', { detail: sessionKeyStr }));
    }
}); 