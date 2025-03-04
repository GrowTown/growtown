mergeInto(LibraryManager.library, {
  UnityLogin: function (sessionKeyPtr) {
    var sessionKey = UTF8ToString(sessionKeyPtr);
    console.log("🔄 Dispatching UnityLoginRequest event with session key:", sessionKey);
    var event = new CustomEvent("UnityLoginRequest", { detail: sessionKey });
    window.dispatchEvent(event);
  },

  SendSessionKey: function (sessionKeyPtr) {
    var sessionKey = UTF8ToString(sessionKeyPtr);
    console.log("📢 Sending Session Key to React at / route:", sessionKey);
    if (typeof Module !== "undefined" && Module.canvas) {
      Module.canvas.dispatchEvent(new CustomEvent("SendSessionKey", { detail: sessionKey }));
      window.sessionKeyFromUnity = sessionKey; // Fallback
    } else {
      console.error("❌ Cannot dispatch SendSessionKey event: Module or canvas not ready.");
      window.sessionKeyFromUnity = sessionKey; // Fallback
    }
  },

  SendMessageToJS: function (eventNamePtr) {
    var eventName = UTF8ToString(eventNamePtr);
    console.log("📢 WebGL Dispatching Event:", eventName);
    window.dispatchEvent(new CustomEvent(eventName));
  }
});