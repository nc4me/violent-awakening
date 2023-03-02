using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using XRL;
using XRL.World;


namespace ViolentAwakening.HarmonyPatches {
    [HarmonyPatch(typeof(XRL.World.Effects.Asleep))]
    [HarmonyPatch(nameof(XRL.World.Effects.Asleep.HandleEvent))]
    [HarmonyPatch(new Type[] { typeof(InventoryActionEvent) })]
    public static class Patch_XRL_World_Effects_Asleep {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var found = false;
            foreach (var instruction in instructions) {
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand != null && ((string)instruction.operand).Contains("gently")) {
                    found = true;
                    var newInstruction = new CodeInstruction(instruction); // very important --> this constructor copies labels, the direct initialization ctor does not! <-- very important
                    newInstruction.operand = ((string)newInstruction.operand).Replace("gently", "violently");
                    yield return newInstruction;
                } else {
                    yield return instruction;
                }
            }
            if (!found) {
                UnityEngine.Debug.LogError($"ViolentAwakening: failed to replace awakening text. Mod activation failed :( sorry, I would fix it if I were feeling up to it");
            } else {
                UnityEngine.Debug.Log($"ViolentAwakening: sleeping NPCs are now in danger. (Mod successfully activated)");
            }
        }
    }
}
