using FrankenToilet.Core;
using HarmonyLib;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TMPro;
using UnityEngine;

namespace FrankenToilet.prideunique;

public static class TextHelper
{
    private static string Prefix = "FRANKEN";
    private static string Postfix = "TOILET";

    private static string RemoveTags(string src)
    {
        StringBuilder sb = new StringBuilder();
        bool isTag = false;
        for (int i = 0; i < src.Length; i++)
        {
            char c = src[i];
            bool isCurrentlyTag = isTag;
            if (c == '<')
                isTag = true;
            else if (c == '>')
                isTag = false;
            isCurrentlyTag |= isTag;

            if (isCurrentlyTag)
                continue;

            sb.Append(src[i]);
        }

        return sb.ToString();
    }

    private static bool IsVisibleChar(char c)
    {
        var cat = CharUnicodeInfo.GetUnicodeCategory(c);
        return cat != UnicodeCategory.Format
               && cat != UnicodeCategory.Control
               && cat != UnicodeCategory.Surrogate
               && cat != UnicodeCategory.OtherNotAssigned
               && c != ' ';
    }

    private static bool ReplaceFirstPattern(ref string src, string pattern, string replacement)
    {
        bool isTag = false;
        int startOfVisibility = -1;

        string currentPattern = "";
        bool matches = false;

        for (int i = 0; i < src.Length; i++)
        {
            char c = src[i];

            bool isCurrentlyTag = isTag;
            if (c == '<')
                isTag = true;
            else if (c == '>')
                isTag = false;
            isCurrentlyTag |= isTag;

            if (!isCurrentlyTag && IsVisibleChar(c) && startOfVisibility == -1)
                startOfVisibility = i;

            if (!isCurrentlyTag && startOfVisibility != -1)
            {
                currentPattern += c;
                if (currentPattern == pattern)
                {
                    matches = true;
                    break;
                }
                // No chance
                if (currentPattern.Length > pattern.Length)
                    break;
            }
        }

        if (matches)
        {
            string a = src.Substring(0, startOfVisibility);
            string b = src.Substring(startOfVisibility + pattern.Length);

            src = a + replacement + b;
            return true;
        }

        return false;
    }
    private static bool ReplaceLastPattern(ref string src, string pattern, string replacement)
    {
        bool isTag = false;
        int startOfVisibility = -1;

        string currentPattern = "";
        bool matches = false;

        for (int i = src.Length - 1; i >= 0; i--)
        {
            char c = src[i];

            bool isCurrentlyTag = isTag;
            if (c == '<')
                isTag = false;
            else if (c == '>')
                isTag = true;
            isCurrentlyTag |= isTag;

            if (!isCurrentlyTag && IsVisibleChar(c) && startOfVisibility == -1)
                startOfVisibility = i;

            if (!isCurrentlyTag && startOfVisibility != -1)
            {
                currentPattern = c + currentPattern;
                if (currentPattern == pattern)
                {
                    matches = true;
                    break;
                }
                // No chance
                if (currentPattern.Length > pattern.Length)
                    break;
            }
        }

        if (matches)
        {
            string a = src.Substring(startOfVisibility + 1);
            string b = src.Substring(0, startOfVisibility - pattern.Length + 1);

            src = b + replacement + a;
            return true;
        }

        return false;
    }

    public static string Transformy(string src, bool isPrefix)
    {
        string previousSource = src;
        string previousSourceNoTags = RemoveTags(src);

        if (isPrefix)
        {
            if (ReplaceFirstPattern(ref src, "[ ", "[ " + Prefix + "-"))
            {
                return src;
            }
            if (ReplaceFirstPattern(ref src, "-- ", "-- " + Prefix + "-"))
            {
                return src;
            }
            if (ReplaceFirstPattern(ref src, "[", "[" + Prefix + "-"))
            {
                return src;
            }
            if (ReplaceFirstPattern(ref src, "--", "--" + Prefix + "-"))
            {
                return src;
            }
            if (ReplaceFirstPattern(ref src, "-> ", "--" + Prefix + "-"))
            {
                return src;
            }
            if (ReplaceFirstPattern(ref src, "> ", "--" + Prefix + "-"))
            {
                return src;
            }
            if (ReplaceFirstPattern(ref src, "->", "--" + Prefix + "-"))
            {
                return src;
            }
            if (ReplaceFirstPattern(ref src, ">", "--" + Prefix + "-"))
            {
                return src;
            }


            src = src.TrimStart();
            string trailing = "";
            if (previousSource.Length > src.Length)
                trailing = previousSource.Substring(0, previousSource.Length - src.Length);

            src = Prefix + "-" + src;
            src = trailing + src;
            return src;
        }
        else
        {
            if (ReplaceLastPattern(ref src, " ]", "-" + Postfix +" ]"))
            {
                return src;
            }
            if (ReplaceLastPattern(ref src, " --", "-" + Postfix + " --"))
            {
                return src;
            }
            if (ReplaceLastPattern(ref src, "]", "-" + Postfix + "]"))
            {
                return src;
            }
            if (ReplaceLastPattern(ref src, "--", "-" + Postfix + "--"))
            {
                return src;
            }


            src = src.TrimEnd();
            string trailing = "";
            if (previousSource.Length > src.Length)
                trailing = previousSource.Substring(src.Length, previousSource.Length - src.Length);

            src += "-" + Postfix;
            src += trailing;
            return src;
        }
    }

    internal static int ParseText(ref string src, object target)
    {
        if (string.IsNullOrWhiteSpace(src))
            return 0;

        string previousSource = src;
        string previousSourceNoTags = RemoveTags(src);

        // It is probably a rank icon.
        if (previousSourceNoTags.Length == 1)
        {
            return 0;
        }

        var unityTarget = target as UnityEngine.Object;
        int id = 0;
        if (unityTarget != null)
            id = Mathf.Abs(unityTarget.GetInstanceID());

        string[] lines = src.Split('\n');
        StringBuilder newSrc = new StringBuilder();

        System.Random r = new System.Random(id);
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(RemoveTags(lines[i])))
            {
                newSrc.AppendLine(lines[i]);
                continue;
            }

            bool isPrefix = ((id + RandomForMe.Next(int.MaxValue) * i) % 100) < 50;
            newSrc.AppendLine(Transformy(lines[i], isPrefix));
        }

        src = newSrc.ToString();
        return src.Length - previousSource.Length;
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(TMP_Text))]
[HarmonyPatch("PopulateTextBackingArray", typeof(string), typeof(int), typeof(int))]
public class TextReplacements
{
    private static string fodder = "";
    private static readonly MethodInfo ParseText = SymbolExtensions.GetMethodInfo(() => TextHelper.ParseText(ref fodder, ""));

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_3);
        yield return new CodeInstruction(OpCodes.Ldarga_S, 1);
        yield return new CodeInstruction(OpCodes.Ldarg_0);
        yield return new CodeInstruction(OpCodes.Call, ParseText);
        yield return new CodeInstruction(OpCodes.Add);
        yield return new CodeInstruction(OpCodes.Starg_S, 3);

        foreach (var instruction in instructions)
            yield return instruction;
    }
}