# Character baseline

Open `Assets/Scenes/SubwayWalkthrough.unity` and Play. The original character is preserved: 19 input meshes, 21 prepared meshes after two knee cuts, original UVs and atlas, uniform height 1.6 m including hat. Hands, feet and leg pieces are independent transforms. No Animator, Humanoid skeleton, NPC AI or networking.

- WASD: walk; mouse: look/turn after clicking Game view.
- Hold left/right mouse button: independently raise the corresponding hand toward the view target. Release: smooth return.
- E within 1.4 m of a seat anchor: sit; E again: stand into the aisle.
- Ctrl or C: crouch. Escape releases the cursor.

`CharacterMotionController` implements all Idle, Walk, Turn, Sit, StandUp and Crouch motion for both `PlayerCharacter_1p6m` and `NPC_MotionTest_NoAI`. The NPC is stationary without commands, not AI-driven. `CharacterPlayerInput` only supplies player commands; the NPC uses the same public API. Character height includes the tall hat, so eye height is 1.135 m, matching the supplied face position.

Prefab: `Assets/Prefabs/LowPolyCharacter.prefab`. Original FBX: `Assets/Characters/Source/CharacterOriginal.fbx`. Prepared FBX: `Assets/Characters/CharacterPrepared.fbx`. Blender source: `D:/DEEPSEEK/assets/models/subway/character_unity_ready.blend`.

Runtime scripts added: CharacterMotionController.cs, CharacterPlayerInput.cs, CharacterMotionValidation.cs (optional scripted demonstration; disabled in normal Play). Editor helpers added: CharacterSetup.cs, CharacterFinish.cs. Keep the Evidence completion markers so one-time setup does not rerun.

Actual Play screenshots and full character hierarchy are under Evidence. Initial motion run reached all six states and exercised each hand independently and together. The old hand_return check sampled a moving crouch transition; its sampling delay was corrected, but not rerun after the user requested skipping optional checks. Proportions, animation appearance, seated contact and input feel await human review. No claim of complete visual acceptance is made.
