using BepInEx.Configuration;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace FrankenToilet.Core;

internal class TogglePatchHelper
{

    GameObject UltraButton = null;
    /// <summary>
    /// FOR SYSTEM USE ONLY
    /// </summary>
    public void Init()
    {
        SceneManager.activeSceneChanged += StageManager;

    }
    /// <summary>
    /// Runs on level change. Not for API use
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="ID"></param>
    private void StageManager(Scene scene, Scene ID)
    {
        if (ID.name == "b3e7f2f8052488a45b35549efb98d902")
        {
            HomeMenuCreation();

        }

    }
    /// <summary>
    /// Creates a menu on the home screen by utilizing assets from the base menu
    /// </summary>
    private void HomeMenuCreation()
    {
        GameObject canvas = null;
        Scene scene = SceneManager.GetActiveScene();
        GameObject mainmenu;
        try
        {
            foreach (var rootCanvas in scene.GetRootGameObjects().Where(obj => obj.name == "Canvas"))

            {
                mainmenu = rootCanvas.transform.Find("Main Menu (1)").gameObject;
                GameObject leftside = mainmenu.transform.Find("LeftSide").gameObject;
                GameObject Oppbutton = leftside.transform.Find("Options").gameObject;
                Sprite sprite = Oppbutton.GetComponent<Image>().sprite;
                GameObject Opentoiletbutton = makebutton("ToiletButton",sprite,"TP");
                
                

                Opentoiletbutton.transform.SetParent(leftside.transform);


                Opentoiletbutton.transform.localPosition = new Vector3(450, -405, 0);
                Opentoiletbutton.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                Opentoiletbutton.GetComponent<Image>().pixelsPerUnitMultiplier = 4.05f;
                GameObject ToiletMenu = new GameObject("ToiletMenu");
                ToiletMenu.transform.SetParent(rootCanvas.transform);
                ToiletMenu.AddComponent<RectTransform>();
                Opentoiletbutton.GetComponent<Button>().onClick.AddListener(() => showtoilet(ToiletMenu,mainmenu));
                Opentoiletbutton.SetActive(true);
                GameObject Savetoiletbutton = makebutton("Close", sprite, "Restart The Game");
                GameObject Backtoiletbutton = makebutton("ToiletButton", sprite, "Back");

            }

        }
        catch (Exception ex)
        {
            LogHelper.LogError("it appears something has gone wrong " + ex.ToString());
        }

    }


    
    private GameObject MakeBlock(GameObject parent,string title)
    {
        GameObject block = new GameObject(title+"Block");
        block.transform.SetParent(parent.transform);
        return block;
    }
    private GameObject makebutton(string name, Sprite sprite, string words)
    {
        GameObject thing = new GameObject("ToiletOpenButton");
        thing.AddComponent<RectTransform>();
        Image TI = thing.AddComponent<Image>();
        TI.sprite = sprite;
        Button TB = thing.AddComponent<Button>();
        GameObject textthing = new GameObject("text");
        textthing.AddComponent<RectTransform>();
        textthing.transform.SetParent(thing.transform);
        TextMeshProUGUI tmp = textthing.AddComponent<TextMeshProUGUI>();
        
        tmp.text = words;
        return thing;
    }

    private void showtoilet(GameObject ToiletMenu, GameObject mainmenu)
    {
        mainmenu.SetActive(false);
        ToiletMenu.SetActive(true);
    }
}
