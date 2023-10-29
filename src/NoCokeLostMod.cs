using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace NoCokeLost;

[HarmonyPatch]
public class NoCokeLostMod : ModSystem {
    private Harmony? harmony;

    public override bool ShouldLoad(EnumAppSide side) {
        return side.IsServer();
    }

    public override void StartServerSide(ICoreServerAPI capi) {
        harmony = new Harmony(Mod.Info.ModID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public override void Dispose() {
        harmony?.UnpatchAll(Mod.Info.ModID);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(BlockEntityCoalPile), "onBurningTickServer")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static IEnumerable<CodeInstruction> OnBurningTickServer(IEnumerable<CodeInstruction> instructions) {
        var codes = new List<CodeInstruction>(instructions);

        for (int i = 0; i < codes.Count; i++) {
            CodeInstruction code = codes[i];
            if (!code.operand?.ToString()?.Equals("coke") ?? true) {
                continue;
            }

            codes.RemoveRange(i + 8, 5);
            break;
        }

        return codes.AsEnumerable();
    }
}
