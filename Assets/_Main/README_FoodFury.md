# Steps To Follow for PRODUCTION build

## GAME SIDE
* Game version must match Asset Project's version

* LoginScene
    * GAMEDATA must be ENABLED
    * Check for Maintenance must be ON in GAMEDATA
    * ServerMode in GAMEDATA-> MainNet
    * ReleaseMode in GAMEDATA -> Release
    * Platform in GAMEDATA -> Set Build Platform
    * Set Analytics Environment (GameData/Managers)-> Production
    * LunarConsole/QuantumConsole must be DISABLED
    * REMOVE Player ID from LoginManager in Inspector
    * LoginPanelCanvasGroup - Alpha to 0, Blocs Raycasts to false

* HomeScene & Driving Scene
    * GameData must be DISABLED

* Addressables
    * Profile -> Production
    * PlayModeScript -> Use Existing Build


* REMOVE Splash screen
* Update Game Version in GAMEDATA
* Set Production Keys for AES, HMAC, Fusion 

## APIs SIDE
* Update Game Version in GeneralSettings if Product version is changed
* Update any change in APIs on Production
