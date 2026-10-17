Ember

Ember is a 3D wave-survival, arena-defense-style game built in Unity. You play as Kael(a dragon), defending a treasure from waves of enemies (Goblins, Trolls and Skinwalkers) using fireball and ultimate abilities.

> Group members: Bibi Zahraa Khan(224036460), Mikayla Pillay(224044200) , Shreya Ramsamooch(224107436) ,Ria Razina Khan(224058492)
> Module: Comp307 Group Project

1. Requirements

- Unity version:`6000.6.0f1` (Unity 6). Install this exact version through Unity Hub to avoid upgrade prompts.
- OS: Windows 10/11 (the project was developed on Windows).
- A GPU that supports DirectX 11 or later.


2. Option A: Run the built game (no Unity needed)

1. Unzip the submission.
2. Open the `Build/` folder.
3. Double-click `Ember.exe` [confirm the exe name].
4. Choose a resolution when prompted and press Play.
5. To exit the build - press Alt + F4.
> If Windows SmartScreen warns about an unknown publisher, click More info → Run anyway.


3. Option B: Open the project in Unity

1. Install Unity 6000.6.0f1 via Unity Hub.
2. In Unity Hub, click Add → Add project from disk and select the unzipped project folder (the one containing `Assets/`, `Packages/` and `ProjectSettings/`).
3. Open the project. The first import can take several minutes, because Unity regenerates the `Library/` folder.
4. In the Project window, open the scene **`Assets/Scenes/[SceneName].unity`** [confirm path and name].
5. Press the Play button at the top of the editor.


4. Controls

- Move: WASD or the arrow keys
- Shoot fireballs: Spacebar
- Camera: Mouse or trackpad to look around
-Ultimate ability in wave 6-press Q
-skip to wave 3-press N
skip to wave 3 -press M


5. How to play

- Survive each wave and protect the treasure. If its health reaches zero, you lose.
- Keep your own hearts above zero. Losing all of them also ends the game.
- Use fireballs to damage enemies. Wait for the cooldown shown in the HUD between uses.
- Charge and use your ultimate ability to clear groups of enemies.
- Stay inside the arena boundary.
-Win & Loss Conditions :

Kael is bound to the treasure by an ancient pact, so the player must **protect the treasure** and **keep Kael alive** across a series of increasingly difficult enemy waves.

## Winning-Survive every wave without any enemy reaching the treasure. Kael's remaining health decides the ending:
1.Flawless Defense- (best ending): Kael finishes with **more than 50% health**
2.Narrow Survival- (standard ending): Kael finishes with **50% health or less**

##Losing
The game ends immediately if either happens:
1. The treasure health reaches 0. Originally starts with 5 in health.
2. Kael's health reaches zero. Originally starts with 10 in health.


6. Project structure

Assets/
  Scripts/
    Player/        Player movement, camera, health, fireball, ultimate, cooldown UI
    Enemies/       EnemyBase, GoblinAI, TrollAI, SkinwalkerAI
    GameSystems/   WaveManager, GameStateManager, UIManager, treasure and wave UI
    Shaders/       Ember Shrine visuals and spawning
    Shared/        Shared interfaces (e.g. IDamageable)
Packages/          Unity package manifest
ProjectSettings/   Unity project settings
Build/             Packaged, runnable build of the game


 7. Third-party assets

- Stylized 3D Animated Chests, FREE Demo (Unity Asset Store)
- Health Heart System
- Skybox demo assets
-Skinwalker and troll Assest pack
-rock and nature Assest pack
-treasure and game audio
-Grass asset pack
-Mountain Asset pack




