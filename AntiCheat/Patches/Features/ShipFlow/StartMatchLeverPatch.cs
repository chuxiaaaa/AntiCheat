using HarmonyLib;

using Unity.Netcode;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(StartMatchLever))]
    [HarmonyWrapSafe]
    public static class StartMatchLeverPatch
    {
        [HarmonyPatch("__rpc_handler_2406447821")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool PlayLeverPullEffectsServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                if (UnityEngine.Object.FindAnyObjectByType<StartMatchLever>().leverHasBeenPulled &&
                    !StartOfRound.Instance.shipHasLanded)
                {
                    return StartOfRoundShipFlowPatch.StartGameServerRpc(target, reader, rpcParams);
                }

                return StartOfRoundShipFlowPatch.EndGameServerRpc(target, reader, rpcParams);
            }
            else if (player == null)
            {
                UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript.interactable = true;
                return false;
            }
            return true;
        }
    }
}
