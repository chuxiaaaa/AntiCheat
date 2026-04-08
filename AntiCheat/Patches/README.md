Patch folders are organized by anti-cheat function first.

- Infrastructure: shared helpers and cross-feature state.
- Features/ConnectionIdentity: transport identity, Steam ID checks, and disconnect cleanup.
- Features/SessionLifecycle: round start/end resets and host session bootstrap.
- Features/RoundSync: duplicate sync guards and loading-progress coordination.
- Features/TerminalAccess: who is allowed to use terminal-driven actions.
- Features/TerminalTransactions: credits, purchases, route changes, and buy validation.
- Features/TerminalValidation: terminal log and scanner RPC validation.
- Features/ChatValidation: chat spoofing and formatted-message abuse checks.
- Features/PlayerCombat: player damage, enemy hit, and kill-player validation.
- Features/EnemyControl: enemy ownership and state-change validation.
- Features/GrabObject: inventory-slot, two-hand, and distance grab checks.
- Features/PlayerMovement: position and invisibility-style movement checks.
- Features/WeaponValidation: shovel, knife, and shotgun attack/ammo checks.
- Features/ItemUse: item battery, gift, mask, jetpack, and despawn rules.
- Features/ShipFlow: start round, leave early, vote, and lever flow.
- Features/ShipSystems: ship build, ship lights, and teleporter rules.
- Features/WorldHazards: turrets, landmines, and boss-desk hazards.
- Features/OperationLog: host-facing action logging hooks.
- Features/LobbyMetadata: lobby name and host metadata patches.
- Features/KillEnemy: enemy kill authorization and bypass bookkeeping.

If a patch touches more than one config flag, it lives in the folder that matches
the action being validated, not the patched game class.
