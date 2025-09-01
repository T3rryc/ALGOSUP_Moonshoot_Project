# VR Conversation Training – Test Plan
## Objective

Ensure the VR conversation training app works correctly across all supported scenes (MiniMarket, Café/Restaurant, TrainStation), with smooth interaction, stable VR locomotion, correct audio playback, and proper NPC responses (OpenAI).

##  Test Categories
### 1. General VR Setup

- ✅ Headset is detected correctly on app start.

- ✅ Player spawns at correct position in each scene (no clipping into floor/walls).

- ✅ Controllers tracked and inputs registered.

### 2. Locomotion & Teleportation

- ✅ Teleport ray appears when joystick pressed.

- ✅ Valid surfaces (Teleport Areas) is highlighted.

- ✅ Invalid surfaces highlight in red on the ray cast.

- ✅ Player can teleport to multiple areas of the map.


### 3. NPC Conversation Logic

- ✅ NPC stays in same language as player’s first utterance.

- ✅ NPC remembers past answers (does not ask for same info twice).

- ✅ NPC follows conversation stages:

- - ✅ Greeting (only if player greets)

- - ✅ Product/service inquiry

- - ✅ Confirmation (price, choice, etc.)

- - ✅ Thanks & exit (only after goodbye)

- - ✅ NPC does not grade until conversation ends.

- - ✅ NPC evaluation mentions skipped/missed stages correctly.

- - ✅ NPC evaluation is contextual (no literal “goodbye” check — must detect actual conversation end).



### 4. Scene-Specific Checks
#### MiniMarket


- ✅ Products correctly textured (no popping materials).

- ✅ NPC conversation follows “shopkeeper” persona.

### Café/Restaurant

- ✅ NPC behaves like waiter/shopkeeper.

- ✅ Tables, chairs, and food assets are solid (no clipping).

- ✅ Ambient audio plays if included.

### TrainStation

- ✅ Floor has working mesh collider (no falling through).


### 5. UI / Menu

- ✅ Main Menu loads first.

- ✅ Buttons highlight on hover (Market = green, Reset = orange, Quit = red).

- ✅ Scene selection loads correct scene.

- ✅ Reset button reloads current scene.

- ✅ Quit button exits app in .exe build.

### 6. Performance & Stability

- ✅ FPS stable (>72fps Quest / >90fps PCVR).

- ✅ No memory leaks (watch logs during long playtest).

- ✅ Scenes load without crashing.



###  Acceptance Criteria

All tests marked ✅ pass on both Unity Editor Play Mode and Windows Build (.exe).

Audio works in build (not only editor).

NPC logic consistent across all scenes.

No scene-breaking collisions or missing textures.