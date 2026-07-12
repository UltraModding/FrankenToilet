using BepInEx;
using BepInEx.Configuration;
using FrankenToilet.Core;
using FrankenToilet.flazhik.Assets;

using HarmonyLib;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static FrankenToilet.bobthecorn.storebutton;


namespace FrankenToilet.bobthecorn;


[EntryPoint]
class Boot
{
    [EntryPointAttribute]
    static void Enter()
    {
        
        SceneManager.activeSceneChanged += OnSceneLoaded;
        
        LogHelper.LogInfo("Welcome to UltraClicker: FrankenToilet Edition");
        
        

    }
    static void OnSceneLoaded(Scene scene, Scene mode)
    {
        if (mode.name == "b3e7f2f8052488a45b35549efb98d902")
        {
            if (UltraClicker.instance == null)
            {
                LogHelper.LogInfo("Good day for a swell battle!");
                GameObject UCGO = new GameObject("UGCO");

                UCGO.AddComponent<UltraClicker>();

            }
        }

    }
}

public class UltraClicker : MonoBehaviour
{
    ConfigFile CookieFile;
    private ConfigEntry<double> SavedCookieAmount;
    private ConfigEntry<double> SavedCookieAllTime;
    private ConfigEntry<int> SavedFBAmount;
    private ConfigEntry<int> SavedFilthAmount;
    private ConfigEntry<int> SavedStrayAmount;
    private ConfigEntry<int> SavedSchismAmount;
    private ConfigEntry<int> SavedSoldierAmount;
    private ConfigEntry<int> SavedHeadAmount;
    private ConfigEntry<int> SavedSMAmount;
    private ConfigEntry<int> SavedCerbAmount;
    private ConfigEntry<int> SavedHMAmount;
    private ConfigEntry<int> SavedV2Amount;
    private ConfigEntry<int> SavedMFAmount;
    private ConfigEntry<int> SavedGabAmount;
    private ConfigEntry<int> SavedVirtAmount;
    private ConfigEntry<int> SavedInsurAmount;
    private ConfigEntry<int> SavedSimpAmount;
    private ConfigEntry<int> SavedLevAmount;
    private ConfigEntry<int> SavedManAmount;
    private ConfigEntry<int> SavedGMAmount;
    private ConfigEntry<int> SavedSchlattAmount;
    private ConfigEntry<int> SavedLoadStarAmount;
    public double CookieAmount;
    public int FBAmount;
    public int FilthAmount;
    public int StrayAmount;
    public int SchismAmount;
    public int SoldierAmount;
    public int HeadAmount;
    public int SMAmount;
    public int CerbAmount;
    public int HMAmount;
    public int V2Amount;
    public int MFAmount;
    public int GabAmount;
    public int VirtAmount;
    public int InsurAmount;
    public int SimpAmount;
    public int LevAmount;
    public int ManAmount;
    public int GMAmount;
    public int SchlattAmount;
    public int LoadStarAmount;
    public double CPS;
    public static UltraClicker instance { get; private set; }
    private IEnumerator cookieroutine;
    public Dictionary<string, SpawnableObject> enemydata = new Dictionary<string, SpawnableObject>();
    public GameObject CookieUI;
    public GameObject CookieButton;
    public static readonly AccessTools.FieldRef<EnemyInfoPage, SpawnableObjectsDatabase> SpawnObjectDataBase = AccessTools.FieldRefAccess<EnemyInfoPage, SpawnableObjectsDatabase>("objects");
    void Awake()
    {

        if (instance != null && instance != this)
        {
            UnityEngine.Object.Destroy(gameObject);
            LogHelper.LogError("the ultraclicker already exists");

            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        byte[] data = LoadBytesFromDll("FrankenToilet.bobthecorn.ultraclicker_assets_all.bundle");
        AssetBundle bundle = AssetBundle.LoadFromMemory(data);
        CookieUI = bundle.LoadAsset<GameObject>("Assets/Clicker.prefab");
        CookieButton = bundle.LoadAsset<GameObject>("Assets/StoreOption.prefab");

        ConfigInit();
        LoadData();
        calculateCPS();
        SceneManager.activeSceneChanged += OnSceneChange;
        cookieroutine = CookieLoop();

        LogHelper.LogInfo("And begin!");
        StartCoroutine(cookieroutine);
    }
    void OnDestroy()
    {
        LogHelper.LogError("KnockOut!!!, seriously though. this should never be destroyed");
    }
    IEnumerator CookieLoop()
    {
        while (true)
        {
            CookieAmount += CPS;
            UpdateUI?.Invoke();
            yield return new WaitForSecondsRealtime(1);
        }

    }
    public void calculateCPS()
    {
        CPS = FBAmount * 0.1f +
        FilthAmount * 1 +
        StrayAmount * 8 +
        SchismAmount * 47 +
        SoldierAmount * 260 +
        HeadAmount * 1400 +
        SMAmount * 7800 +
        CerbAmount * 44000 +
        HMAmount * 260000 +
        V2Amount * 1600000 +
        MFAmount * 10000000 +
        GabAmount * 65000000 +
        VirtAmount * 430000000 +
        InsurAmount * 2900000000 +
        SimpAmount * 21000000000 +
        LevAmount * 150000000000 +
        ManAmount * 1000000000000 +
        GMAmount * 8300000000000 +
        SchlattAmount * 64000000000000 +
        LoadStarAmount * 510000000000000;
    }


    


    public static byte[] LoadBytesFromDll(string resourceName)
    {
        Assembly asm = typeof(UltraClicker).Assembly; // or DLL type
        using Stream stream = asm.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new Exception($"Resource {resourceName} not found in assembly.");

        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        return bytes;
    }

    void OnSceneChange(Scene scene, Scene mode)
    {
        if (mode.name == "b3e7f2f8052488a45b35549efb98d902")
        {

            SaveData();
            LogHelper.LogDebug("Saved");

        }
        else if (mode.name == "Bootstrap")
        {
            LogHelper.LogDebug("Cant Save cookies, currently straping my boots");
        }
        else if (mode.name == "241a6a8caec7a13438a5ee786040de32")
        {
            LogHelper.LogDebug("Cant Save Cookies, currently watching a movie");
        }
        else
        {
            SaveData();
            LogHelper.LogDebug("Saved");
        }

    }

    void ConfigInit()
    {
        CookieFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "UltraClickerData.cfg"), true);

        // wow this is really stinky
        SavedCookieAmount = CookieFile.Bind(
     "SaveData",
     "SavedCookieAmount",
     0d,
     "Total amount of cookies saved."
 );

        SavedFBAmount = CookieFile.Bind(
            "SaveData",
            "SavedFBAmount",
            0,
            "Saved FB amount."
        );

        SavedFilthAmount = CookieFile.Bind(
            "SaveData",
            "SavedFilthAmount",
            0,
            "Saved Filth amount."
        );

        SavedStrayAmount = CookieFile.Bind(
            "SaveData",
            "SavedStrayAmount",
            0,
            "Saved Stray amount."
        );

        SavedSchismAmount = CookieFile.Bind(
            "SaveData",
            "SavedSchismAmount",
            0,
            "Saved Schism amount."
        );

        SavedSoldierAmount = CookieFile.Bind(
            "SaveData",
            "SavedSoldierAmount",
            0,
            "Saved Soldier amount."
        );

        SavedHeadAmount = CookieFile.Bind(
            "SaveData",
            "SavedHeadAmount",
            0,
            "Saved Head amount."
        );

        SavedSMAmount = CookieFile.Bind(
            "SaveData",
            "SavedSMAmount",
            0,
            "Saved SM amount."
        );

        SavedCerbAmount = CookieFile.Bind(
            "SaveData",
            "SavedCerbAmount",
            0,
            "Saved Cerberus amount."
        );

        SavedHMAmount = CookieFile.Bind(
            "SaveData",
            "SavedHMAmount",
            0,
            "Saved HM amount."
        );

        SavedV2Amount = CookieFile.Bind(
            "SaveData",
            "SavedV2Amount",
            0,
            "Saved V2 amount."
        );

        SavedMFAmount = CookieFile.Bind(
            "SaveData",
            "SavedMFAmount",
            0,
            "Saved MF amount."
        );

        SavedGabAmount = CookieFile.Bind(
            "SaveData",
            "SavedGabAmount",
            0,
            "Saved Gabriel amount."
        );

        SavedVirtAmount = CookieFile.Bind(
            "SaveData",
            "SavedVirtAmount",
            0,
            "Saved Virtue amount."
        );

        SavedInsurAmount = CookieFile.Bind(
            "SaveData",
            "SavedInsurAmount",
            0,
            "Saved Insurrectionist amount."
        );

        SavedSimpAmount = CookieFile.Bind(
            "SaveData",
            "SavedSimpAmount",
            0,
            "Saved Simp amount."
        );

        SavedLevAmount = CookieFile.Bind(
            "SaveData",
            "SavedLevAmount",
            0,
            "Saved Leviathan amount."
        );

        SavedManAmount = CookieFile.Bind(
            "SaveData",
            "SavedManAmount",
            0,
            "Saved Man amount."
        );

        SavedGMAmount = CookieFile.Bind(
            "SaveData",
            "SavedGMAmount",
            0,
            "Saved GM amount."
        );

        SavedSchlattAmount = CookieFile.Bind(
            "SaveData",
            "SavedSchlattAmount",
            0,
            "Saved Schlatt amount."
        );

        SavedLoadStarAmount = CookieFile.Bind(
            "SaveData",
            "SavedLoadStarAmount",
            0,
            "Saved LoadStar amount."
        );

    }
    void LoadData()
    {
        CookieAmount = SavedCookieAmount.Value;


        FBAmount = SavedFBAmount.Value;
        FilthAmount = SavedFilthAmount.Value;
        StrayAmount = SavedStrayAmount.Value;
        SchismAmount = SavedSchismAmount.Value;
        SoldierAmount = SavedSoldierAmount.Value;
        HeadAmount = SavedHeadAmount.Value;
        SMAmount = SavedSMAmount.Value;
        CerbAmount = SavedCerbAmount.Value;
        HMAmount = SavedHMAmount.Value;
        V2Amount = SavedV2Amount.Value;
        MFAmount = SavedMFAmount.Value;
        GabAmount = SavedGabAmount.Value;
        VirtAmount = SavedVirtAmount.Value;
        InsurAmount = SavedInsurAmount.Value;
        SimpAmount = SavedSimpAmount.Value;
        LevAmount = SavedLevAmount.Value;
        ManAmount = SavedManAmount.Value;
        GMAmount = SavedGMAmount.Value;
        SchlattAmount = SavedSchlattAmount.Value;
        LoadStarAmount = SavedLoadStarAmount.Value;
    }
    void SaveData()
    {
        SavedCookieAmount.Value = CookieAmount;

        SavedFBAmount.Value = FBAmount;
        SavedFilthAmount.Value = FilthAmount;
        SavedStrayAmount.Value = StrayAmount;
        SavedSchismAmount.Value = SchismAmount;
        SavedSoldierAmount.Value = SoldierAmount;
        SavedHeadAmount.Value = HeadAmount;
        SavedSMAmount.Value = SMAmount;
        SavedCerbAmount.Value = CerbAmount;
        SavedHMAmount.Value = HMAmount;
        SavedV2Amount.Value = V2Amount;
        SavedMFAmount.Value = MFAmount;
        SavedGabAmount.Value = GabAmount;
        SavedVirtAmount.Value = VirtAmount;
        SavedInsurAmount.Value = InsurAmount;
        SavedSimpAmount.Value = SimpAmount;
        SavedLevAmount.Value = LevAmount;
        SavedManAmount.Value = ManAmount;
        SavedGMAmount.Value = GMAmount;
        SavedSchlattAmount.Value = SchlattAmount;
        SavedLoadStarAmount.Value = LoadStarAmount;

        CookieFile.Save();
    }


    public double calculatePrice(Building building)
    {
        double basePrice = BuildingBasePrices.ContainsKey(building) ? BuildingBasePrices[building] : 0;
        int amount = building switch
        {
            Building.FB => UltraClicker.instance.FBAmount,
            Building.Filth => UltraClicker.instance.FilthAmount,
            Building.Stray => UltraClicker.instance.StrayAmount,
            Building.Schism => UltraClicker.instance.SchismAmount,
            Building.Soldier => UltraClicker.instance.SoldierAmount,
            Building.Head => UltraClicker.instance.HeadAmount,
            Building.SM => UltraClicker.instance.SMAmount,
            Building.Cerb => UltraClicker.instance.CerbAmount,
            Building.HM => UltraClicker.instance.HMAmount,
            Building.V2 => UltraClicker.instance.V2Amount,
            Building.MF => UltraClicker.instance.MFAmount,
            Building.Gab => UltraClicker.instance.GabAmount,
            Building.Virt => UltraClicker.instance.VirtAmount,
            Building.Insur => UltraClicker.instance.InsurAmount,
            Building.Simp => UltraClicker.instance.SimpAmount,
            Building.Lev => UltraClicker.instance.LevAmount,
            Building.Man => UltraClicker.instance.ManAmount,
            Building.GM => UltraClicker.instance.GMAmount,
            Building.Schlatt => UltraClicker.instance.SchlattAmount,
            Building.LodeStar => UltraClicker.instance.LoadStarAmount,
            _ => 0
        };
        return Math.Ceiling(basePrice * Math.Pow(1.15, amount));
    }


    public int getAmountByBuilding(Building building)
    {
        int amount = building switch
        {
            Building.FB => UltraClicker.instance.FBAmount,
            Building.Filth => UltraClicker.instance.FilthAmount,
            Building.Stray => UltraClicker.instance.StrayAmount,
            Building.Schism => UltraClicker.instance.SchismAmount,
            Building.Soldier => UltraClicker.instance.SoldierAmount,
            Building.Head => UltraClicker.instance.HeadAmount,
            Building.SM => UltraClicker.instance.SMAmount,
            Building.Cerb => UltraClicker.instance.CerbAmount,
            Building.HM => UltraClicker.instance.HMAmount,
            Building.V2 => UltraClicker.instance.V2Amount,
            Building.MF => UltraClicker.instance.MFAmount,
            Building.Gab => UltraClicker.instance.GabAmount,
            Building.Virt => UltraClicker.instance.VirtAmount,
            Building.Insur => UltraClicker.instance.InsurAmount,
            Building.Simp => UltraClicker.instance.SimpAmount,
            Building.Lev => UltraClicker.instance.LevAmount,
            Building.Man => UltraClicker.instance.ManAmount,
            Building.GM => UltraClicker.instance.GMAmount,
            Building.Schlatt => UltraClicker.instance.SchlattAmount,
            Building.LodeStar => UltraClicker.instance.LoadStarAmount,
            _ => 0
        };

        return amount;
    }


    public void populateshopbuttons(Transform trans)
    {
        
        for (int i = trans.childCount - 1; i >= 0; i--){
            Destroy(trans.GetChild(i).gameObject);
        }
        
        foreach (Building building in Enum.GetValues(typeof(Building)))
        {
            makeshopbutton(trans, building); // prints enum names: FB, Filth, Stray, etc.
        }



    }
    
    private void makeshopbutton(Transform trans, storebutton.Building building)
    {
        GameObject sb = Instantiate(CookieButton, trans);
        storebutton ssb = sb.AddComponent<storebutton>();
        sb.AddComponent<ControllerPointer>();
        ssb.init(building);
       
    }

    public event Action UpdateUI;
    public static Transform FindDeepChildByPathIncludeInactive(Transform root, string path)
    {
        string[] parts = path.Split('/');
        Transform current = root;

        foreach (string part in parts)
        {
            bool found = false;
            foreach (Transform child in current)
            {
                if (child.name == part)
                {
                    current = child;
                    found = true;
                    break;
                }
            }

            if (!found)
                return null; // Path is invalid
        }

        return current;
    }
    private static readonly Dictionary<Building, double> BuildingBasePrices = new Dictionary<Building, double>
    {
    { Building.FB, 15 },
    { Building.Filth, 100 },
    { Building.Stray, 1100 },
    { Building.Schism, 12000 },
    { Building.Soldier, 130000 },
    { Building.Head, 1400000 },
    { Building.SM, 20000000 },
    { Building.Cerb, 330000000 },
    { Building.HM, 5100000000d },
    { Building.V2, 75000000000d },
    { Building.MF, 1000000000000d },
    { Building.Gab, 14000000000000d },
    { Building.Virt, 170000000000000d },
    { Building.Insur, 2100000000000000d },
    { Building.Simp, 26000000000000000d },
    { Building.Lev, 310000000000000000d },
    { Building.Man, 71000000000000000000d },
    { Building.GM, 12000000000000000000000d },
    { Building.Schlatt, 1900000000000000000000000d },
    { Building.LodeStar, 540000000000000000000000000d }
};
}

public class storebutton : MonoBehaviour
{
    Building type;
    double price;
    bool unlocked;
    public enum Building {
        FB,
        Filth,
        Stray,
        Schism,
        Soldier,
        Head,
        SM,
        Cerb,
        HM,
        V2,
        MF,
        Gab,
        Virt,
        Insur,
        Simp,
        Lev,
        Man,
        GM,
        Schlatt,
        LodeStar,


    }
    public void init(Building building)
    {
        type = building;
        UltraClicker.instance.enemydata.TryGetValue(BuildingNames[building], out SpawnableObject SO);
        if (SO != null)
        {
            if (MonoSingleton<BestiaryData>.Instance.GetEnemy(SO.enemyType) >= 1 || type == Building.FB)
            {
                transform.GetChild(0).GetComponent<Image>().sprite = SO.gridIcon;
                transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = SO.objectName;
                unlocked = true;
            }
            else
            {
                gameObject.GetComponent<Button>().interactable = false;
                transform.GetChild(0).GetComponent<Image>().sprite = SO.gridIcon;
                transform.GetChild(0).GetComponent<Image>().color = Color.black;
                transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "???";
                unlocked = false;

            }



        }
        else if (type == Building.FB)
        {
            unlocked = true;
        }
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = UltraClicker.instance.getAmountByBuilding(type).ToString();
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = UltraClicker.instance.calculatePrice(type).ToString() + " oz";
        price = UltraClicker.instance.calculatePrice(type);

        gameObject.GetComponent<Button>().onClick.AddListener(() => AttemptPurchase());



    }


    void OnEnable()
    {
        UltraClicker.instance.UpdateUI += refreshUI;
    }

    private void refreshUI() {
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = UltraClicker.instance.getAmountByBuilding(type).ToString();
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = UltraClicker.instance.calculatePrice(type).ToString() + " oz";
        price = UltraClicker.instance.calculatePrice(type);
        if (price > UltraClicker.instance.CookieAmount || unlocked == false)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
    }
    void OnDisable()
    {
        UltraClicker.instance.UpdateUI -= refreshUI;
    }

    public void AttemptPurchase()
    {
        if (UltraClicker.instance.CookieAmount >= price)
        {

            bool fail = false;
            switch (type)
            {
                case Building.FB:
                    UltraClicker.instance.FBAmount += 1;
                    break;
                case Building.Filth:
                    UltraClicker.instance.FilthAmount += 1;
                    break;
                case Building.Stray:
                    UltraClicker.instance.StrayAmount += 1;
                    break;
                case Building.Schism:
                    UltraClicker.instance.SchismAmount += 1;
                    break;
                case Building.Soldier:
                    UltraClicker.instance.SoldierAmount += 1;
                    break;
                case Building.Head:
                    UltraClicker.instance.HeadAmount += 1;
                    break;
                case Building.SM:
                    UltraClicker.instance.SMAmount += 1;
                    break;
                case Building.Cerb:
                    UltraClicker.instance.CerbAmount += 1;
                    break;
                case Building.HM:
                    UltraClicker.instance.HMAmount += 1;
                    break;
                case Building.V2:
                    UltraClicker.instance.V2Amount += 1;
                    break;
                case Building.MF:
                    UltraClicker.instance.MFAmount += 1;
                    break;
                case Building.Gab:
                    UltraClicker.instance.GabAmount += 1;
                    break;
                case Building.Virt:
                    UltraClicker.instance.VirtAmount += 1;
                    break;
                case Building.Insur:
                    UltraClicker.instance.InsurAmount += 1;
                    break;
                case Building.Simp:
                    UltraClicker.instance.SimpAmount += 1;
                    break;
                case Building.Lev:
                   UltraClicker.instance.LevAmount += 1;
                    break;
                case Building.Man:
                    UltraClicker.instance.ManAmount += 1;
                    break;
                case Building.GM:
                    UltraClicker.instance.GMAmount += 1;
                    break;
                case Building.Schlatt:
                    UltraClicker.instance.SchlattAmount += 1;
                    break;
                case Building.LodeStar:
                    UltraClicker.instance.LoadStarAmount += 1;
                    break;
                default:
                    LogHelper.LogError("no building type was set");
                    fail = true;
                    break;
            }
            if (!fail)
            {
              UltraClicker.instance.CookieAmount -= price;
              UltraClicker.instance.calculateCPS();
                refreshUI();
            }
            
        }
    }

    private static readonly Dictionary<Building, string> BuildingNames = new Dictionary<Building, string>
    {
        { Building.FB, "Feedbacker" },
        { Building.Filth, "ultrakill.flith" },
        { Building.Stray, "ultrakill.stray" },
        { Building.Schism, "ultrakill.schism" },
        { Building.Soldier, "ultrakill.soldier" },
        { Building.Head, "ultrakill.malicious-face" },
        { Building.SM, "ultrakill.swordsmachine" },
        { Building.Cerb, "ultrakill.cerberus" },
        { Building.HM, "ultrakill.mass" },
        { Building.V2, "ultrakill.v2" },
        { Building.MF, "ultrakill.mindflayer" },
        { Building.Gab, "ultrakill.gabriel" },
        { Building.Virt, "ultrakill.virtue" },
        { Building.Insur, "ultrakill.insurrectionist" },
        { Building.Simp, "ultrakill.ferryman" },
        { Building.Lev, "ultrakill.leviathan" },
        { Building.Man, "ultrakill.mannequin" },
        { Building.GM, "ultrakill.gutterman" },
        { Building.Schlatt, "ultrakill.minos-prime" },
        { Building.LodeStar, "ultrakill.sisyphus-prime" }
    };

}

public class Cookie : MonoBehaviour
{
    void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => UltraClicker.instance.CookieAmount += 1);
    }
}
public class banner : MonoBehaviour
{
    void OnEnable()
    {
        UltraClicker.instance.UpdateUI += refreshUI;
    }

   private void refreshUI() {
        gameObject.GetComponent<TextMeshProUGUI>().text = Math.Ceiling(UltraClicker.instance.CookieAmount).ToString() + " oz";
    }

    void OnDisable()
    {
        UltraClicker.instance.UpdateUI -= refreshUI;
    }
}



[PatchOnEntry]
public class Patches
{
    
    [HarmonyPatch(typeof(ShopZone), "Start")]
    [HarmonyPostfix]
    public static void Start(ShopZone __instance)
    {
        Transform trans = UltraClicker.FindDeepChildByPathIncludeInactive(__instance.transform, "Canvas/Background");
        Transform yoinkenemyObject = UltraClicker.FindDeepChildByPathIncludeInactive(trans, "Main Panel/Enemies");

        if (UltraClicker.instance.enemydata.Count == 0)
        {
            EnemyInfoPage eip = yoinkenemyObject.gameObject.GetComponent<EnemyInfoPage>();
            SpawnableObjectsDatabase SODB = UltraClicker.SpawnObjectDataBase(eip);
            foreach (SpawnableObject SO in SODB.enemies)
            {
                UltraClicker.instance.enemydata.TryAdd(SO.identifier, SO);
            }
            
        }
        if (yoinkenemyObject != null)
        {
            
            
            GameObject clickerUI = GameObject.Instantiate(UltraClicker.instance.CookieUI, trans);
            Transform buttoncontainer = UltraClicker.FindDeepChildByPathIncludeInactive(clickerUI.transform, "Right/Scroll View/Viewport/Content");
            Transform Scrollview = UltraClicker.FindDeepChildByPathIncludeInactive(clickerUI.transform, "Right/Scroll View");
            Scrollview.gameObject.AddComponent<ControllerPointer>();
            Transform banner = UltraClicker.FindDeepChildByPathIncludeInactive(clickerUI.transform, "PopOut/Left/Banner/text");
            Transform cookie = UltraClicker.FindDeepChildByPathIncludeInactive(clickerUI.transform, "PopOut/Left/BloodDrop");
            Transform close = UltraClicker.FindDeepChildByPathIncludeInactive(clickerUI.transform, "PopOut/Left/Button");
            UltraClicker.instance.populateshopbuttons(buttoncontainer);
            banner.gameObject.AddComponent<banner>();
            cookie.gameObject.AddComponent<Cookie>();
            cookie.gameObject.AddComponent<ControllerPointer>();
            close.gameObject.AddComponent<ControllerPointer>();
            close.gameObject.GetComponent<Button>().onClick.AddListener(() => clickerUI.SetActive(false));
            
        }



    }
}
