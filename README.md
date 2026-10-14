# SubwayGame — Unity walking baseline

Unity 6.3 LTS (6000.3.21f1), Built-in rendering pipeline. Open this folder as a Unity project, then open `Assets/Scenes/SubwayWalkthrough.unity` and press Play. Click inside Game view to lock the mouse; WASD moves; mouse looks; Escape releases the cursor. The temporary CharacterController capsule is 1.7 m tall, radius 0.23 m; eye height is 1.58 m. This milestone has no NPCs, interactions, animation, or other gameplay.

## Assets and source

- `Assets/Models/Subway/SubwayCar.fbx`: normalized Blender FBX, meter scale, Y up in Unity. Full exterior bounds roughly 3.38 × 2.89 × 19.06 m; interior width 3.16 m. Prefab faces the entry player camera; 180° root yaw aligns the one-sided information panels.
- `Assets/Prefabs/SubwayCar.prefab`: independently named visual components and 22 primitive collision shapes. Doorway walls stay closed in this milestone.
- `Assets/Models/Subway/Materials`: 10 native Unity Standard materials, including transparent glass and enabled emission. A reusable fine floor normal map is included. The Blender procedural shading is not directly imported.
- Original Blender source is retained outside the project: `D:/DEEPSEEK/assets/models/subway/subway_interior.blend`.
- Unity-ready source: `D:/DEEPSEEK/assets/models/subway/subway_unity_ready.blend`; export script in the same directory. Rotation/scale applied, normals recalculated, matching geometry shares mesh data. Do not place .blend files inside Assets; FBX is the import authority.

## Object structure

```
SubwayCar (Prefab)
  Visuals
    Floor
    Walls
    Doors
    Windows
    Seats
    Poles
    Handles
    Ceiling
    Displays
  Colliders (floor, side/end walls, ceiling, benches, poles)
Lighting
Indirect_Light_Approximation
Interior_ReflectionProbe
TemporaryPlayer_1p7m
  PlayerCamera_1p58m
```

530 renderers. Repeated normalized mesh data is reused where identical; material instancing is enabled. This is an editable import baseline, not a draw-call-optimized final shipping asset. Collision is intentionally coarse for predictable walking. Transparent glass uses Standard transparency and reflection probes, not Blender refraction. Interior illumination uses real-time spots, bounded indirect fill and a box-projected reflection probe; no lightmap bake is required.

## Actual verification

`Evidence/playmode_validation.json`: actual CharacterController.Move traversal in Unity Play mode, not a geometric-only calculation. Walk from z=-8.65 to z=8.756 passed. Seat blocking (x=0.680), side-wall blocking (x=1.315), and end-wall blocking passed. Player remained above the floor. The representative lateral checks use the right side; symmetric colliders exist on the left. Manual human acceptance remains separate.

`Evidence/unity_player_central.png` and `unity_player_oblique.png` were rendered by the live player Camera during Play mode. `imported_hierarchy.txt` contains all imported objects. `import_report.json` contains the import inventory. The latest run completed without C# or gameplay exceptions. The initial editor startup emitted a Unity Search indexing exception; it did not recur in the final run. A licensing token-refresh warning was nonblocking; the editor opened and ran under the existing license.

Automatic traversal is disabled for normal Play. To repeat, stop Play and choose **Subway > Run Traversal Validation**. Keep the completed Evidence marker files: the one-time editor migration scripts use them to avoid reapplying setup. `SubwaySetup.Build` is an initial setup helper, not a routine command for an edited project.

This phase stops at a stable imported walking scene. Door opening, exterior platforms, NPCs, mechanics, and production rendering optimization remain outside this milestone.
