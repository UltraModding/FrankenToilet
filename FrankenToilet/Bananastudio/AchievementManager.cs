using FrankenToilet.Core;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrankenToilet.Bananastudio;

public class AchievementManager
{
    public static void ExecuteAchievement(string name, string description)
    {
        if (MainThingy.frankenCanvas == null) return;

        GameObject AchievementTemplate = MainThingy.frankenCanvas.transform.Find("AchievementStuff/" +
            "Achievements/AchievementTemp").gameObject;

        if(AchievementTemplate == null)
        {
            LogHelper.LogError("Cannot find achievement template.");
            return;
        }

        GameObject ach = Object.Instantiate(AchievementTemplate, AchievementTemplate.transform.parent);
        ach.transform.Find("FullThing/Name").GetComponent<TMP_Text>().text = name;
        ach.transform.Find("FullThing/Description").GetComponent<TMP_Text>().text = description;
        //ach.GetComponent<Animator>().speed = 0.1666666666666667f;
        ach.SetActive(true);
        Object.Destroy(ach, 6f);
    }
}
