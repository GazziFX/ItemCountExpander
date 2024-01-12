using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace ItemCountExpander
{
    public class Module : IModuleNexus
    {
        private readonly Harmony HarmonyInstance = new Harmony(nameof(ItemCountExpander));

        public void initialize()
        {
            var transpiler = new HarmonyMethod(typeof(Module), nameof(Transpiler));
            HarmonyInstance.Patch(typeof(PlayerInventory).GetMethod("ReceiveDragItem"), transpiler: transpiler);
            HarmonyInstance.Patch(typeof(PlayerInventory).GetMethod("tryAddItem", new Type[] { typeof(Item), typeof(byte), typeof(byte), typeof(byte), typeof(byte) }), transpiler: transpiler);
            HarmonyInstance.Patch(typeof(Items).GetMethod("tryAddItem", new Type[] { typeof(Item), typeof(bool) }), transpiler: transpiler);
        }

        public void shutdown()
        {
            HarmonyInstance.UnpatchAll(HarmonyInstance.Id);
        }

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
