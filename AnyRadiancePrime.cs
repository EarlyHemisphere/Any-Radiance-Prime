using System.Diagnostics;
using System.Reflection;
using Modding;
using UnityEngine;
using GlobalEnums;

public class AnyRadiancePrime : Mod, ITogglableMod {
    public static AnyRadiancePrime instance;

    public AnyRadiancePrime() : base("Any Radiance Prime") { }

    public override void Initialize() {
        instance = this;

        Log("Initalizing.");
        ModHooks.AfterSavegameLoadHook += AfterSaveGameLoad;
        ModHooks.NewGameHook += AddComponent;
        ModHooks.LanguageGetHook += LangGet;
        ModHooks.AfterTakeDamageHook += CreateRespawn;
    }


    public override string GetVersion(){
        return FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(AnyRadiancePrime)).Location).FileVersion;
    }

    private static string LangGet(string key, string sheettitle, string orig) {
        if (key != null) {
            switch (key) {
                case "ABSOLUTE_RADIANCE_SUPER": return "Any";
                case "GG_S_RADIANCE": return "God of meme.";
                case "GODSEEKER_RADIANCE_STATUE": return "k"; // i hope bob fred is okay with this
                default: return Language.Language.GetInternal(key, sheettitle);
            }
        }
        return orig;
    }

    private static void AfterSaveGameLoad(SaveGameData data) {
        AddComponent();
    }

    private static void AddComponent() {
        GameManager.instance.gameObject.AddComponent<AbsFinder>();
    }

    private static int CreateRespawn(int hazardType, int damage) {
        GameObject absRad = GameObject.Find("Absolute Radiance");
        if ((HazardType)hazardType == HazardType.ACID && absRad != null) {
            if (absRad.transform.GetPositionY() > 150f) {
                // respawn final phase platform on death to abyss pit
                GameObject.Find("Radiant Plat Small (11)").LocateMyFSM("radiant_plat").SendEvent("APPEAR");
            } else if (GameObject.Find("Phase2 Detector") != null && GameObject.Find("Phase2 Detector").activeSelf) {
                // respawn platform phase platform on death to abyss pit
                GameObject.Find("Hazard Plat/Radiant Plat Wide (4)").LocateMyFSM("radiant_plat").SendEvent("APPEAR");
            }
        }
        return damage;
    }

    public void Unload() {
        ModHooks.AfterSavegameLoadHook -= AfterSaveGameLoad;
        ModHooks.NewGameHook -= AddComponent;
        ModHooks.LanguageGetHook -= LangGet;
        GameManager instance = GameManager.instance;
        AbsFinder absFinder = ((instance != null) ? instance.gameObject.GetComponent<AbsFinder>() : null);
        if (!(absFinder == null))
        {
            Object.Destroy(absFinder);
        }
    }
}
