Note: Install Mirror from package managaer

------------------------

--NetCubeTest

Contains a network manager, two start positions, and a cube with network transform properties

	-- Manager

Sets up the network and client/server functionality.Currently using default settings. Needs to be told
what player prefab to use for spawning players, currently using CubePlayer.

Uses NetworkManager (and Telepathy Transport automatically added), and uses NetworkManagerHud.

	-- StartPos

Uses NetworkStartPosition for telling manager where to spawn players

	-- SharedCube

Contains a NetworkIdentity for finding in networked game, and NetworkTransform for moving around in a 
networked game.

--CubePlayer and CubePlayerScript

CubePlayer is the prefab that gets spawned by Manager when a new player joins the game (on set start positions). It's just a cylinder with a NetworkIdentity, and most importantly, a script.

CubePlayerScript defines what the player/client can do in the game world. For now, the player can move the shared cube around use gyro controls if android, and keyboard controls otherwise. See comments on code for more info.

--------How to run---------

Build the game. If building to laptop/PC, simply build and run the game and spawn two games. Host a server/client on one and a client on other. If same computer, the whole thing should work.

If different computers, but same LAN, find out the ip address of server computer (using eg ipconfig on Windows command line) and input it instead of "localhost". Don't need to specify port.

If building to android, first enable developer mode and usb debugging using this guide https://docs.unity3d.com/Manual/android-sdksetup.html

Use a usb, and build and run from Unity to Android. It should start automatically once done. Use the same scheme as above for joining a LAN game.



