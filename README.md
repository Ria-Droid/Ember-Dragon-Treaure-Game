Ember-Dragon Treasure Game 

Ember is a 3D wave-survival, tower-defense-style game built in Unity 6 with the Universal Render Pipeline. 
You play as Kael, the guardian of a treasure under attack. Survive escalating waves of enemies, each with its own AI and behaviour:
Goblins, Trolls and Skinwalkers. Fight back with fireballs, charge your ultimate ability to clear groups, and keep an eye on your hearts and the treasure’s health.
Ember Shrines add to the arena’s atmosphere.

Group members: Bibi Zahraa Khan(224036460), Mikayla Pillay(224044200) , Shreya Ramsamooch(224107436) ,Ria Razina Khan(224058492) Module: Comp307 Group Project

Requirements
Unity version:6000.6.0f1 (Unity 6). Install this exact version through Unity Hub to avoid upgrade prompts.
OS: Windows 10/11 (the project was developed on Windows).
A GPU that supports DirectX 11 or later.

Option A: Run the built game (no Unity needed)
Unzip the submission.
Open the Build/ folder.
Double-click Ember.exe [confirm the exe name].
Choose a resolution when prompted and press Play.
To exit the build - press Alt + F4.
If Windows SmartScreen warns about an unknown publisher, click More info → Run anyway.

Option B: Open the project in Unity
Install Unity 6000.6.0f1 via Unity Hub.
In Unity Hub, click Add → Add project from disk and select the unzipped project folder (the one containing Assets/, Packages/ and ProjectSettings/).
Open the project. The first import can take several minutes, because Unity regenerates the Library/ folder.
In the Project window, open the scene Assets/Scenes/[SceneName].unity [confirm path and name].
Press the Play button at the top of the editor.

Controls
Move: WASD or the arrow keys
Shoot fireballs: Spacebar
Camera: Mouse or trackpad to look around -Ultimate ability in wave 6-press Q -skip to wave 3-press N skip to wave 3 -press M

How to play
Survive each wave and protect the treasure. If its health reaches zero, you lose.
Keep your own hearts above zero. Losing all of them also ends the game.
Use fireballs to damage enemies. Wait for the cooldown shown in the HUD between uses.
Charge and use your ultimate ability to clear groups of enemies.
Stay inside the arena boundary. -Win & Loss Conditions :
Kael is bound to the treasure by an ancient pact, so the player must protect the treasure and keep Kael alive across a series of increasingly difficult enemy waves.
##Winning-Survive every wave without any enemy reaching the treasure. Kael's remaining health decides the ending:
1.Flawless Defense- (best ending): Kael finishes with more than 50% health 2.Narrow Survival- (standard ending): Kael finishes with 50% health or less
##Losing The game ends immediately if either happens:
The treasure health reaches 0. Originally starts with 5 in health.
Kael's health reaches zero. Originally starts with 10 in health.

Project structure
Assets/ Scripts/ Player/ Player movement, camera, health, fireball, ultimate, cooldown UI Enemies/ EnemyBase, GoblinAI, TrollAI, SkinwalkerAI GameSystems/ WaveManager, GameStateManager, UIManager, treasure and wave UI Shaders/ Ember Shrine visuals and spawning Shared/ Shared interfaces (e.g. IDamageable) Packages/ Unity package manifest ProjectSettings/ Unity project settings Build/ Packaged, runnable build of the game

Third-party assets
Stylized 3D Animated Chests, FREE Demo (Unity Asset Store)
Health Heart System
Skybox demo assets -Skinwalker and troll Assest pack -rock and nature Assest pack -treasure and game audio -Grass asset pack -Mountain Asset pack
