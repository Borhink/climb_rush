# ClimbRush - Unity 2D Climbing Game

## Project Structure

```
Assets/
├── Scripts/
│   ├── Manager/
│   │   └── GameManager.cs        # Entry point
│   ├── Input/
│   │   └── InputHandler.cs       # Mouse/touch input
│   ├── Gameplay/
│   │   ├── Player/
│   │   │   └── PlayerController.cs # Body + hands + climbing
│   │   └── World/
│   │       └── ClimbHold.cs      # Grabbable hold
│   ├── UI/                       # (empty - for future)
│   └── Utilities/                # (empty - for future)
├── Prefabs/                      # (empty)
├── Art/Sprites, Materials, Animations/
├── Audio/
└── Scenes/
```

## Classes

### Manager

#### GameManager
Simple entry point. Add to scene and link PlayerController and InputHandler.

### Input

#### InputHandler  
Unified mouse (editor) and touch (mobile) input.
- `OnDragStart(Vector2)` - touch/click started
- `OnDragUpdate(Vector2)` - dragging
- `OnDragEnd(Vector2)` - touch/click released
- `OnTap(Vector2)` - quick tap

### Gameplay

#### PlayerController
Main player class - handles everything:
- Body Rigidbody2D with gravity
- Two hand Rigidbody2Ds connected via DistanceJoint2D
- Alternating hand mechanic:
  1. Drag start → active hand moves
  2. Drag end → try grip with active hand
  3. If grip succeeds → release opposite hand
  4. Switch to other hand

**Setup in Editor:**
1. Create empty GameObject with PlayerController
2. Add 3 Rigidbody2D children: body, left hand, right hand
3. Assign Rigidbody2D references
4. Assign InputHandler reference
5. Set hold layer in layer mask

#### ClimbHold
Simple climbable hold:
- Place on hold GameObject
- Assign to "ClimbHold" layer
- Set grip radius

## Physics Setup

1. **Layers**: Add "ClimbHold" layer in Physics 2D settings
2. **Body**: Rigidbody2D with Gravity Scale = 1
3. **Hands**: Rigidbody2D with Gravity Scale = 0, Body Type = Dynamic
4. **Joints**: PlayerController creates DistanceJoint2D at runtime

## Usage

1. Create scene with Camera, Light
2. Add GameManager object
3. Add PlayerController with body + 2 hand children (Rigidbody2D)
4. Add InputHandler to PlayerController
5. Create ClimbHold prefabs on "ClimbHold" layer
6. Run - drag hands to grab holds
