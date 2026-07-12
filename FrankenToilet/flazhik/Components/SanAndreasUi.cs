using UnityEngine;
using static UnityEngine.GameObject;

namespace FrankenToilet.flazhik.Components;

public sealed class SanAndreasUi : MonoBehaviour
{
    private void Awake()
    {
        var levelName = Find("LevelNameScreen").AddComponent<SanAndreasLevelName>();
        var missionPassed = Find("MissionPassedScreen").AddComponent<SanAndreasMissionPassedScreen>();
        levelName.gameObject.SetActive(false);
        missionPassed.gameObject.SetActive(false);
    }
}