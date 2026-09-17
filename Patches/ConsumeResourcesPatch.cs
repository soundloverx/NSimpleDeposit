using HarmonyLib;

namespace NSimpleDeposit.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.ConsumeResources))]
    internal static class ConsumeResourcesPatch
    {
        private static bool Prefix(Player __instance, Piece.Requirement[] requirements, int qualityLevel, int multiplier)
        {
            BuildCraftService.ConsumeRequirements(__instance, requirements, qualityLevel, multiplier);

            return false;
        }
    }
}
