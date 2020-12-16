(important)Network files:

ExtendeHUD.cs defines the network buttons and text (notably intro text)

ExtendedManager.cs defines certain specific network behaviours, and handles the overall network logic. Important part is the player prefab, which is what it spawns when a player joins the game.

FullMRlinObj is prefab containing all the interaction testing 1 stuff that Yufei provided. For the purposes of the network, the imporant thing in this one is main_player_1, and all the other stuff has been deactivated.

GameState keeps track of the netIds of players in the game, and also serves as a debug window.

MainGame is the main game that endusers will play

MirrorSphere bounces back spells towards the player, serves as the "tutorial" when a player is hosting a game and no one has joined yet.

Occupied.cs is to keep track of which position is occupied when a player joins the game.

PlayerCanvas defines the phone screen. Behaviour given from PlayerScript.

PlayerScript is the main file syncronizing the player behaviour between instances of the game. It has a special property, isLocalPlayer, that defines wheter this instance of the player belongs to this client.

pos1 is position for player 1, pos2 is position for player 2, posPhone is where the phone players go when joining the game.

SpellScript is attached to the SpellSphere, and defines the color logic and the collision behaviour.

SpellSphere is the object spawned by the player, that goes either towards the mirrorSphere, or the other player if it has joined.

targetCollision is just a debug script for the MirrorSphere.

IN THE SCENE

ColorTRacker is used to track a specific color in the real world, and PlayerScript uses the target posiiton from it to move the player.

Two camPos for the camera positions for the different players (main camera is moved).

----------What was changed in Yufei's stuff

MageControllerNew

A reference to the PlayerScript was attached, and checks for isLocalPlayer was added. Some of the original rotation and translation stuff was commented out, and some key presses conflicted with networking keys. Some of the phases were commented out, and phone button hold was added.

WandGyroController

Switched out input.gyro.attitude, for the one transmitted over network from an attached phone.


Current problems

Har to draw anything with Gyro Controls from phone
Health bars don't update
casting spell vfx not fixed
Tracking only works on first player
Shield spell can be cast, but doesn't exist in network setting






