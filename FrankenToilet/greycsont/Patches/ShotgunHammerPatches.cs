using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;

using FrankenToilet.Core;


namespace FrankenToilet.greycsont;

[PatchOnEntry]
[HarmonyPatch(typeof(ShotgunHammer))]
public static class ShotgunHammerPatch
{
    public static ShotgunHammer lastActiveHammer;

    private static readonly MethodInfo negate = AccessTools.Method(typeof(Vector3), "op_UnaryNegation");

    private static readonly MethodInfo random4 = AccessTools.Method(typeof(DirectionRandomizer), nameof(DirectionRandomizer.Randomize4Dir));

    private static readonly MethodInfo TrueStop = AccessTools.Method(typeof(TimeController), nameof(TimeController.TrueStop));

    private static readonly MethodInfo GenerateArrowImage =
        AccessTools.Method(typeof(ArrowController), nameof(ArrowController.GenerateImage));


    [HarmonyPrefix]
    [HarmonyPatch(nameof(ShotgunHammer.Impact))]
    public static void ImpactPatch(ShotgunHammer __instance)
    {
        lastActiveHammer = __instance;
        DirectionRandomizer.GenerateRandomDirection();
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ShotgunHammer.ImpactRoutine), MethodType.Enumerator)]
    public static IEnumerable<CodeInstruction> ImpactRoutineTranspiler
        (IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher.MatchForward(false,
            new CodeMatch(i => i.IsLdloc()),
            new CodeMatch(OpCodes.Callvirt, TrueStop)
        );

        var num6 = matcher.Instruction;

        matcher.InsertAndAdvance(
            new CodeInstruction(num6.opcode, num6.operand),
            new CodeInstruction(OpCodes.Call, GenerateArrowImage)
        );

        return matcher.InstructionEnumeration();
    }


    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ShotgunHammer.DeliverDamage))]
    public static IEnumerable<CodeInstruction> DeliverDamageTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions);

        matcher
            .MatchForward(false, new CodeMatch(OpCodes.Call, negate))
            .Set(OpCodes.Call, random4)
            .MatchForward(false, new CodeMatch(OpCodes.Call, negate))
            .Set(OpCodes.Call, random4);

        return matcher.InstructionEnumeration();
    }
}



