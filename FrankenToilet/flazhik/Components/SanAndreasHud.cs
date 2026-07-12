using FrankenToilet.flazhik.Assets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GameObject;

namespace FrankenToilet.flazhik.Components;

public sealed class SanAndreasHud : MonoBehaviour
{
    [ExternalAsset("Assets/UltrakillSanAndreas/Textures/PiercerIcon.png", typeof(Sprite))]
    private static Sprite piercerIcon;

    [ExternalAsset("Assets/UltrakillSanAndreas/Textures/ShotgunIcon.png", typeof(Sprite))]
    private static Sprite shotgunIcon;

    [ExternalAsset("Assets/UltrakillSanAndreas/Textures/NailgunIcon.png", typeof(Sprite))]
    private static Sprite nailgunIcon;

    [ExternalAsset("Assets/UltrakillSanAndreas/Textures/RailgunIcon.png", typeof(Sprite))]
    private static Sprite railgunIcon;

    [ExternalAsset("Assets/UltrakillSanAndreas/Textures/RocketLauncherIcon.png", typeof(Sprite))]
    private static Sprite rocketLauncherIcon;

    private StatsManager _statsManager;
    private NewMovement _newMovement;
    private GunControl _gunControl;

    private int maxHp;

    private Image hpBar;
    private Image staminaBar;
    private TMP_Text time;
    private TMP_Text lePee;
    private Image weaponIcon;

    private int currentSlotIndex = 1;

    private void Start()
    {
        _statsManager = StatsManager.Instance;
        _newMovement = NewMovement.Instance;
        _gunControl = GunControl.Instance;

        maxHp = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty") == 0 ? 200 : 100;

        hpBar = Find("HealthBar/Fill").GetComponent<Image>();
        staminaBar = Find("StaminaBar/Fill").GetComponent<Image>();
        time = Find("Time").GetComponent<TMP_Text>();
        lePee = Find("Moneh").GetComponent<TMP_Text>();
        weaponIcon = Find("WeaponIcon").GetComponent<Image>();
    }

    private void LateUpdate()
    {
        time.SetCharArray(SecondsToTimeString(_statsManager.seconds).ToCharArray());
        lePee.SetCharArray(EightDigits(_statsManager.stylePoints).ToCharArray());
        hpBar.fillAmount = (float)_newMovement.hp / maxHp;
        staminaBar.fillAmount = _newMovement.boostCharge / 300;

        if (_gunControl.currentSlotIndex == currentSlotIndex)
            return;

        currentSlotIndex = _gunControl.currentSlotIndex;
        weaponIcon.sprite = GetWeaponSprite(currentSlotIndex);
    }

    private Sprite GetWeaponSprite(int index)
    {
        return index switch
        {
            1 => piercerIcon,
            2 => shotgunIcon,
            3 => nailgunIcon,
            4 => railgunIcon,
            5 => rocketLauncherIcon,
            _ => piercerIcon
        };
    }

    private static string SecondsToTimeString(float seconds)
    {
        var totalSeconds = Mathf.FloorToInt(seconds);
        var minutes = totalSeconds / 60;
        var secs = totalSeconds % 60;

        return $"{minutes:00}:{secs:00}";
    }

    private static string EightDigits(int points) => $"P{points:00000000}";
}