using AntiCheat.Utils;

using BepInEx;
using BepInEx.Configuration;

using GameNetcodeStuff;

using HarmonyLib;

using Netcode.Transports.Facepunch;

using Steamworks;
using Steamworks.Data;
using Steamworks.ServerList;

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TMPro;

using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.Events;

using static UnityEngine.GraphicsBuffer;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(ShipBuildModeManager))]
    [HarmonyWrapSafe]
    public static class ShipBuildModeManagerPatch
    {

        /// <summary>
        /// 放置飞船装饰事件(检测)
        /// Prefix ShipBuildModeManager.PlaceShipObjectServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_861494715")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_861494715(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || StartOfRound.Instance.IsHost)
            {
                if (PluginConfig.ShipBuild.Value)
                {
                    Vector3 newPosition;
                    reader.ReadValueSafe(out newPosition);
                    Vector3 newRotation;
                    reader.ReadValueSafe(out newRotation);
                    reader.ReadValueSafe(out NetworkObjectReference objectRef, default);
                    reader.Seek(0);
                    if (objectRef.TryGet(out var networkObject, null))
                    {
                        PlaceableShipObject placingObject = networkObject.gameObject.GetComponentInChildren<PlaceableShipObject>();
                        LogInfo(p, "ShipBuildModeManager.PlaceShipObjectServerRpc", $"object:{placingObject.parentObject.name}", $"newPosition:{newPosition.ToString()}", $"newRotation:{newRotation}");
                        //LogInfo($"newRotation:{newRotation}|mainMesh:{placingObject.mainMesh.transform.eulerAngles.ToString()}");
                        if (Math.Floor(newRotation.x) != Math.Floor(placingObject.mainMesh.transform.eulerAngles.x) || Math.Floor(newRotation.z) != Math.Floor(placingObject.mainMesh.transform.eulerAngles.z))
                        {
                            ShowMessage(locale.Msg_GetString("ShipBuild", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{position}",newRotation.ToString() },
                                    { "{object}",placingObject.parentObject.name }
                            }), locale.Msg_GetString("ShipBuild"));
                            if (PluginConfig.ShipBuild2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                        var ShipBuildModeManager = (ShipBuildModeManager)target;
                        //LogInfo($"{p.playerUsername}|ShipBuildModeManager.PlaceShipObjectServerRpc|placingObject:{placingObject.parentObject.name},{placingObject.unlockableID}|newPosition:{newPosition}|newRotation:{newRotation}");
                        if (!StartOfRound.Instance.shipInnerRoomBounds.bounds.Contains(newPosition))
                        {
                            ShowMessage(locale.Msg_GetString("ShipBuild", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername },
                                { "{position}",newPosition.ToString() }
                            }), locale.Msg_GetString("ShipBuild"));
                            if (PluginConfig.ShipBuild2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                    }
                }
                return true;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }
    }
}
