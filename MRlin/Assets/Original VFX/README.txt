TO HELP YOU NAVIGATE THIS FOLDER.

----------------

Folder SIMPLE FIRE contains the VFX graphs for a simple, unmoving fire VFX. The MAGICfIRE prefab in main folder shows this effect.

Folder ADAPTIVE PROJECTILE is a particle projectile that is based on Unity's collision detection (trigger). The PROJECTILE prefab uses this.

The STATIC-PROJECTILE prefab is a distance-based particle projectile and uses the VFX03 effect graph.

The FIREpROJECTILE prefab is a magical fire projectile that is distance-based. It uses the VFX04 effect graph.

The MATERIALS are for the walls, shooter and target, just to make the scene nicer to look at.

The SHOOTER prefab uses the SHOOTERmANAGER script and takes an "effect" input. The "effect" should be a projectile-type prefab. The script will instantiate the "effect" when mouse is clicked.

There are two projectile scripts. PROJECTILEmANAGER in adaptive projectile folder is geared towards collision detection based projectiles. It gives the projectile speed, will instantiate an explosion effect and self-destruct upon collision.
The PARTIALpROJECTILEmANAGER is geared towards distance-based projectiles. It has no speed/movement, and simply self-destructs after 3s of existance.