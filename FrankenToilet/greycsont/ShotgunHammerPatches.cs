using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

using FrankenToilet.Core;


namespace FrankenToilet.greycsont;

[PatchOnEntry]
[HarmonyPatch(typeof(ShotgunHammer))]
public static class ShotgunHammerPatch
{
    
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ShotgunHammer.DeliverDamage))]
    public static IEnumerable<CodeInstruction> OnTriggerEnterTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        DirectionRandomizer.Reset(); 
        
        var negate = AccessTools.Method(typeof(Vector3), "op_UnaryNegation");
        
        var random4 = AccessTools.Method(typeof(DirectionRandomizer), nameof(DirectionRandomizer.Randomize4Dir));
        
        var randomReset = AccessTools.Method(typeof(DirectionRandomizer), nameof(DirectionRandomizer.Reset));
        
        var matcher = new CodeMatcher(instructions);
        

        matcher
            .Start()
            .Insert(new CodeInstruction(OpCodes.Call, randomReset))
            .MatchForward(false, new CodeMatch(OpCodes.Call, negate))
            .Set(OpCodes.Call, random4)
            .MatchForward(false, new CodeMatch(OpCodes.Call, negate))
            .Set(OpCodes.Call, random4);
        
        return matcher.InstructionEnumeration();
    }
}

