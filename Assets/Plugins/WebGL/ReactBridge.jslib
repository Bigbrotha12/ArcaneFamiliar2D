mergeInto(LibraryManager.library, {
    Web3Authentication: function () {
      try {
        window.dispatchReactUnityEvent("RequestAuth");
      } catch (e) {
        console.warn("Failed to dispatch event");
      }
    },
    Web3InitialData: function () {
      try {
        window.dispatchReactUnityEvent("RequestInitialData");
      } catch (e) {
        console.warn("Failed to dispatch event");
      }
    },
    Web3PlayerPreferences: function () {
      try {
        window.dispatchReactUnityEvent("GetPlayerPreferences");
      } catch (e) {
        console.warn("Failed to dispatch event");
      }
    },
    UpdateRegistration: function () {
      try {
        window.dispatchReactUnityEvent("UpdateRegistration");
      } catch (e) {
        console.warn("Failed to dispatch event");
      }
    },
    UpdatePlayerPreferences: function (preferences) {
      try {
        window.dispatchReactUnityEvent("UpdatePlayerPreferences", UTF8ToString(preferences));
      } catch (e) {
        console.warn("Failed to dispatch event");
      }
    }
  });