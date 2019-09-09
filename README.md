# AtmoUnity3DSDK

SDK that interfaces Atmo with Unity3D. The SDK provides information about game pieces on the table detected by Atmo. 
Furthermore it has an emulator that enables development without the need of an actual Atmo game console.
The emulator triggers the same game piece events that an Atmo would.

## Events

Static game piece events can be referenced from the AtmoTracker class. 
The events return a GamePiece instance with the following fields:

```
int uniqueId;            // unique id of the game piece
int side;                // side of the game piece (dice has 6, token has 2)
Vector2 worldPosition;   // postion of the game piece in world space
float angle;             // rotation of the game piece
```

#### OnDetected event

This event is invoked when a new game piece is detected on the table.

#### OnLost event

This event is invoked when a game piece disappears from the table. 
This can also happen if a game piece is hidden by another object or by someone's hand.

#### OnRedetected event

Lost game pieces are saved in a history for 5 seconds. 
In case the same game piece appears on the same location within this 5 seconds, 
this event is invokeded instead of the onDetected.

## Getting started with the emulator

1. In Unity3D add a new resolution on the Game view with fixed resolution of 1280x800.
2. Open the Example scene from the Scene folder.
3. After hitting play, push and hold button E to show emulator hints:

```
Toggle Chosen - toggles between dice and token game pieces  
Creat         - puts down a game piece on cursor position  
Remove        - removes the game piece under the cursor  
Hide          - hides the game piece, so it can be reused  
Roll/Flip     - changes the side of the game piece that is facing up  
Rotate        - rotates the game piece  
Remove All    - removes all game pieces from the screen
```
