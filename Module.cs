using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace ItemCountExpander
{
    [HarmonyPatch]
    public class Module : IModuleNexus
    {
        private readonly Harmony HarmonyInstance = new Harmony(nameof(ItemCountExpander));

        public void initialize()
        {
            HarmonyInstance.PatchAll(typeof(Module).Assembly);
        }

        public void shutdown()
        {
            HarmonyInstance.UnpatchAll(HarmonyInstance.Id);
        }

        [HarmonyPatch(typeof(PlayerInventory), "ReceiveDragItem")]
        [HarmonyPatch(typeof(Items), "tryAddItem", typeof(Item), typeof(bool))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var inst in instructions)
            {
                if (inst.opcode == OpCodes.Ldc_I4 && (int)inst.operand == 200)
                    inst.operand = 255;

                yield return inst;
            }
        }
    }
}
