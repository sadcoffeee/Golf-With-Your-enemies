# Golf With Your Enemies 
This repository hosts the full project folder for the Unity game Golf With Your Enemies, my exam project for the "\[blank] but worse" exercise. Following is a paste of the majority of my brief project report to give a general overview of how the game is built. 

Download the built version of the game from the [itch page](https://sadcoffeee.itch.io/golf-with-your-enemies) or view the [demo video](https://youtu.be/e0rkbefSzts)

## Overview of the Game:
The game is very loosely inspired by Golf With Your Friends, featuring 4 small mini-golf levels connected by an open area for the player to test mechanics. The player controls a golf-ball, with the primary ability to shoot the golf ball in a direction with a variable amount of force, controlled by how long the player holds the shooting button. Halfway through the game, the player unlocks a secondary fire, which is used to access the final levels by destroying enemies. The game features a simple tip/tutorial system to assist the player, as well as a UI overlay to keep track of player scores. 
The main parts of the game are:
-	**Player** – golf ball that is moved by clicking ‘mouse1’ to enter aim-mode and holding space to shoot. Has an unlockable secondary catapult fire for destroying enemies.
-	**Camera** – Pivots around player based on mouse movement. Distance to player adjusted with scroll-wheel, freelook unlockable with ‘mouse3’. 
-	**Golf Levels** – Levels automatically start when entering a starting area, they count your shots during the level and have a flag for indicating the goal position.
-	**Enemies** – Enemies are large walls-on-wheels that move back and forth, blocking the player.
-	**Play field** – the entire play field is made up of static mesh models imported from a model-pack
-	**UI** – Gradual tutorial tips are shown as the player progresses, and all previous tips as well as a scorecard are shown with the ‘Tab’ button

Game features:
-	Levels automatically start and end when you enter and leave their areas. Level is automatically reset if you get out of bounds.
-	The game keeps track of your best scores on each level and display them in a menu.
-	The game uses line-renderers to show a predicted shot path.

## How were the Different Parts of the Course Utilized:
The player consists of a sphere collider & mesh with a rigidbody. All movement is done by applying force vectors to the player rigidbody using standard Unity physics. Camera position is controlled by calculating spherical coordinates in relation to the player position and applying these directly to the camera transform. The player uses collisions with invisible trigger box-colliders to trigger functions that start and stop levels, either in the case of leaving or in the case of entering the goal. Level flags are controlled by script to appear or disappear depending on distance to player. The enemy model was built using ProBuilder to assemble primitives, and animated to move back and forth at variable width and speed. The GUI is controlled by scripts to show tooltips at specific moments, as well as dynamically update with previously shown tips & player high scores. The scene is lit with a global directional light, and all ground uses a custom physics material for improved golf feel. 
## Project Parts:
### Scripts:
-	**Golfer** – main player script. Gets mouse-input to control camera, activate aiming guide objects (arrow marker + catapult + their line renderers), perform shooting with coroutines, as well as collision checks for things like killplane and level activation and deactivation triggers. 
-	**GolfLevel** – Keeps track of whether the level is currently played and how many shots the player has taken through functions called by the Golfer script. Has public variables for par and max amount of shots 
-	**Flag** – Toggles flag visibility if player is currently playing the appropriate level and gets close enough to the flag
-	**WallOnWheels** – Collision check for player projectile and sets driving width and speed variables for animator
-	**keepTrackOfPlayer** – Keeps track of player stats like highscore, current respawn point, as well as holding the function to populate and show UI overlay
-	**tutorialHandler** – Responsible for keeping track of and displaying tooltips. Gets called by fx. GolfLevel & keepTrackOfPlayer
### Models & Prefabs:
-	Golf assets acquired from https://www.kenney.nl/assets/minigolf-kit
-	WallsOnWheels constructed with primitives using ProBuilder
-	Aiming arrow & catapult made in MagicaVoxel
### Animations:
-	WallsOnWheels – Back and forth driving at three different widths, controlled by an animation controller with variables for driving width and speed
-	Tooltip fade in and out 
### Materials:
-	Regular opaque Unity materials for levels, includes 3 colour variations for golf assets
-	Slightly transparent material for line renderers
-	Adjusted physic material for ground to improve ball rolling
### Scenes:
-	Game consists of one scene
### Testing:
-	Game was tested on Windows, game cannot be played on a mobile platform

## Time Management
| Task | Time it Took (in hours) |
| ------------- | ------------- |
| Setting up Unity, making a project in GitHub  | 0.5 |
| Research and conceptualization of game idea  | 3 |
| Searching for 3D models - environment  | 0.5 |
| Writing camera and shooting logic | 2  |
| Writing level and goal logic | 1.5  |
| Building level 1 | 0.5  |
| Modelling shooting marker | 0.5  |
| Implementing shooting indicators (marker + line), killplane & golf flags | 2  |
| Building level 2 | 0.5  |
| Modelling catapult  | 0.5  |
| Implementing catapult shot & catapult line  | 2  |
| Building WallsOnWheels from primitives  | 0.5  |
| Animating WallsOnWheels  | 0.5  |
| Designing/placing level 3&4  | 1  |
| Implementing tooltip logic  | 2  |
| Implementing score/tip overlay  | 2  |
| **All**  | **19.5**  |


## Used Resources
-	[**Skybox**](https://assetstore.unity.com/packages/2d/textures-materials/sky/customizable-skybox-174576)
- [**Golf models**](https://www.kenney.nl/assets/minigolf-kit)

## Future work
*(these are just my own ideas for improvements I'm considering making if I come back to this project)*
- Impact particle effect and fall animation for when catapult shot hits walls
- Splitting levels into their own scenes, using noise to generate killplane terrain for visual variety
- Stationary sniper enemy to perform area denial (paired with a dash-like speed boost ability?)
- Different ground materials (slippery, bouncy, sticky)
- Remake golf models as probuilder assets for improved modularity 



