mergeInto(LibraryManager.library, {

    SendTG_URL:function(str){
window.dispatchReactUnityEvent("SendTG_URL", str)
},
    CallReactClipboard:function(str){
window.dispatchReactUnityEvent("CallReactClipboard", str)
},
    Quit:function(){
        try {
        window.dispatchReactUnityEvent("Quit");
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
    }

  });