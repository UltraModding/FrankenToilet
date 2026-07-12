#pragma warning disable CS8618
using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine.SceneManagement;
using System.Linq;


namespace FrankenToilet.somebilly {
    // --------------------
    // ----- MACHINES -----
    // --------------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class V1Patch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NewMovement), "Start")]
        public static void StartPostfix(NewMovement __instance) {
            string[] TwoScenes = {"Level 2-1", "Level 2-2", "Level 2-3", "Level 2-4"};
            if (TwoScenes.Contains(SceneHelper.CurrentScene)) {
                __instance.gameObject.AddComponent<TwoSpawner>();
            }

            if (SceneHelper.CurrentScene == "Level 2-S") {
                return;
            }
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["V1"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceSwordsmachinePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SwordsMachine), "Start")]
        public static void Postfix(SwordsMachine __instance) {
            if (__instance.name.Contains("Agony")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SWORDSMACHINE_AGONY"]);
                return;
            } else if (__instance.name.Contains("Tundra")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SWORDSMACHINE_TUNDRA"]);
                return;
            }
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SWORDSMACHINE"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceDronePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Drone), "Start")]
        public static void Postfix(Drone __instance, ref EnemyIdentifier ___eid) {
            if (__instance.name.Contains("FleshCamera")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["BIG_EYE"]);
                return;
            } else if (__instance.name.Contains("Flesh")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["EYE"]);
                return;
            } else if (__instance.name.Contains("Skull")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MINI_MAURICE"]);
                return;
            }
            switch (___eid.enemyType) {
                case EnemyType.Drone:
                    Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["DRONE"]);
                    break;
                case EnemyType.Virtue:
                    Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["VIRTUE"]);
                    break;
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceStreetcleanerPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Streetcleaner), "Start")]
        public static void Postfix(Streetcleaner __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["STREETCLEANER"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceV2Patch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(V2), "Start")]
        public static void Postfix(V2 __instance) {
            if (__instance.name.Contains("Green")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["V2_2"]);
            } else if (__instance.name.Contains("John")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["BIG_JOHNINATOR"]);
            } else {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["V2"]);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMindflayerPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mindflayer), "Start")]
        public static void Postfix(Mindflayer __instance) {
            // p-2
            if (SceneManager.GetActiveScene().name == "1f290c2101e628540bf9c6d1d2140750") {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MINDFLAYER_IDOLED"], 0.8f);
                return;
            }
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MINDFLAYER"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceSentryPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Turret), "Start")]
        public static void Postfix(Turret __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SENTRY"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceGuttermanPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Gutterman), "Start")]
        public static void Postfix(Gutterman __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["GUTTERMAN"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceGuttertankPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Guttertank), "Start")]
        public static void Postfix(Guttertank __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["GUTTERTANK"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceLandminePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Landmine), "Start")]
        public static void Postfix(Landmine __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["LANDMINE"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceEarthmoverPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneHelper), "OnSceneLoaded")]
        public static void Postfix(SceneHelper __instance) {
            if (SceneHelper.CurrentScene == "Level 7-4") {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["EARTHMOVER"]);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceEarthmoverBrainPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Enemy), "Start")]
        public static void Postfix(Enemy __instance) {
            if (__instance.name == "Brain") {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["EARTHMOVER_BRAIN"]);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceRocketLauncherPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DroneFlesh), "Start")]
        public static void Postfix(DroneFlesh __instance) {
            if (__instance.transform.parent.name.Contains("Rocket")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["ROCKET_LAUNCHER"]);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMortarTowerPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MortarLauncher), "Start")]
        public static void Postfix(MortarLauncher __instance) {
            if (__instance.mortar.name.Contains("HH")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MORTAR"]);
                return;
            }
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["TOWER"]);
        }
    }


    // -----------------
    // ----- HUSKS -----
    // -----------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceFilthPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ZombieMelee), "Start")]
        public static void Postfix(ZombieMelee __instance) {
            if (SceneHelper.CurrentScene == "CreditsMuseum2") {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["GIANNI_MATRAGRANO"], 0.35f);
                return;
            }
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["FILTH"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceZombiesPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ZombieProjectiles), "Start")]
        public static void Postfix(ZombieProjectiles __instance, ref EnemyIdentifier ___eid) {
            switch (___eid.enemyType) {
                case EnemyType.Stray:
                    Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["STRAY"]);
                    break;
                case EnemyType.Schism:
                    Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SCHISM"]);
                    break;
                case EnemyType.Soldier:
                    Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SOLDIER"]);
                    break;
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinosCorpsePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneHelper), "OnSceneLoaded")]
        public static void Postfix(SceneHelper __instance) {
            string[] MinosScenes = {"Level 2-1", "Level 2-2", "Level 2-3"};
            if (MinosScenes.Contains(SceneHelper.CurrentScene)) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MINOS_CORPSE"]);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinosCorpseHandPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MinosArm), "Start")]
        public static void Postfix(MinosArm __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MINOS_CORPSE_HAND"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinosCorpseLongPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MinosBoss), "Start")]
        public static void Postfix(MinosBoss __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MINOS_CORPSE_LONG"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinosCorpseParasitePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Parasite), "Start")]
        public static void Postfix(Parasite __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MINOS_CORPSE_PARASITE"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceStalkerPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Stalker), "Start")]
        public static void Postfix(Stalker __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["STALKER"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceSisypheanInsurrectionistPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Sisyphus), "Start")]
        public static void Postfix(Sisyphus __instance) {
            if (__instance.name.Contains("Angry")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SISYPHEAN_INSURRECTIONIST_ANGRY"]);
                return;
            } else if (__instance.name.Contains("Rude")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SISYPHEAN_INSURRECTIONIST_RUDE"]);
                return;
            }
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SISYPHEAN_INSURRECTIONIST"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceFerrymanPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ferryman), "Start")]
        public static void Postfix(Ferryman __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["FERRYMAN"], 0.75f);
        }
    }


    // ------------------
    // ----- DEMONS -----
    // ------------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMaliciousFacePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MaliciousFace), "Start")]
        public static void Postfix(MaliciousFace __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MALICIOUS_FACE"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceCerberusPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(StatueBoss), "Start")]
        public static void Postfix(StatueBoss __instance) {
            if (SceneHelper.CurrentScene == "Level 0-5" || (SceneHelper.CurrentScene == "Level 7-1" && __instance.transform.parent.name == "Awakened")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["CERBERUS_BIG"]);
                return;
            }
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["CERBERUS"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMassPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mass), "Start")]
        public static void Postfix(Mass __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["HIDEOUS_MASS"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceIdolPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Idol), "Awake")]
        public static void Postfix(Idol __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["IDOL"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceLeviathan5_3Patch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ComplexSplasher), "Awake")]
        public static void Postfix(ComplexSplasher __instance) {
            if (SceneHelper.CurrentScene == "Level 5-3") {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["LEVIATHAN_5-3"]);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceLeviathanIntroPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LightningStrikeDecorative), "FlashStart")]
        public static void Postfix(LightningStrikeDecorative __instance) {
            if (__instance.transform.parent.name.Contains("Tease") && SceneHelper.CurrentScene == "Level 5-4") {
                Bib.Instance.AddAndPlayVoice(__instance.transform.root.gameObject, Bib.Voices["LEVIATHAN_INTRO"], 0.8f);
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceLeviathanPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LeviathanHead), "Start")]
        public static void Postfix(LeviathanHead __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["LEVIATHAN"], 0.85f);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMannequinPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mannequin), "Start")]
        public static void Postfix(Mannequin __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MANNEQUIN"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinotaurChasePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MinotaurChase), "Start")]
        public static void Postfix(MinotaurChase __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MINOTAUR"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinotaurPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Minotaur), "Start")]
        public static void Postfix(Minotaur __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MINOTAUR"], 0.85f);
        }
    }


    // ------------------
    // ----- ANGELS -----
    // ------------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceGabrielPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Gabriel), "Start")]
        public static void Postfix(Gabriel __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["GABRIEL"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceGagabrielPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GabrielSecond), "Awake")]
        public static void Postfix(GabrielSecond __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["GAGABRIEL"], 0.85f);
        }
    }


    // -----------------
    // ----- OTHER -----
    // -----------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceSomethingWickedPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Wicked), "Start")]
        public static void Postfix(Wicked __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SOMETHING_WICKED"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceCancerousRodentPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CancerousRodent), "Start")]
        public static void Postfix(CancerousRodent __instance) {
            if (__instance.name.Contains("Very")) {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["VERY_CANCEROUS_RODENT"]);
                return;
            }
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["CANCEROUS_RODENT"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMysteriousDruidKnightAndOwlPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mandalore), "Start")]
        public static void Postfix(Mandalore __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MYSTERIOUS_DRUID_KNIGHT_AND_OWL"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceFleshPrisonPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FleshPrison), "Start")]
        public static void Postfix(FleshPrison __instance, ref EnemyIdentifier ___eid) {
            switch (___eid.enemyType) {
                case EnemyType.FleshPrison:
                    Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["FLESH_PRISON"]);
                    break;
                case EnemyType.FleshPanopticon:
                    Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["FLESH_PANOPTICON"]);
                    break;
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMinosPrimePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MinosPrime), "Start")]
        public static void Postfix(MinosPrime __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MINOS_PRIME"], 0.85f);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceSisyphusPrimePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SisyphusPrime), "Start")]
        public static void Postfix(SisyphusPrime __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["SISYPHUS_PRIME"], 0.85f);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoicePuppetPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Puppet), "Start")]
        public static void Postfix(Puppet __instance) {
            Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["PUPPET"]);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class VoiceMiragePatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneHelper), "OnSceneLoaded")]
        public static void Postfix(SceneHelper __instance) {
            if (SceneHelper.CurrentScene == "Level 2-S") {
                Bib.Instance.AddAndPlayVoice(__instance.gameObject, Bib.Voices["MIRAGE"]);
            }
        }
    }
}
