# Multiplayer_VR_Physics_Sim
A Multiplayer VR lobby with a Custom Physics Simulation 

![Unity Physics](https://img.shields.io/badge/Physics-Simulation-blue.svg)

## 📌 Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Physics Model](#physics-model)
- [Multiplayer Interaction with PlayerCanvas](#multiplayer-interaction-with-playercanvas)
- [Assumptions](#assumptions)
- [Interacting with the Simulation](#interacting-with-the-simulation)
- [Using the Chat System](#using-the-chat-system)

---

## Overview
This project simulates physics objects within a **multiplayer VR environment** using Unity Netcode. The simulation includes:
- Bouncing objects
- Impulse forces
- Custom gravity
- Multiplayer synchronization
- Multiplayer chat

---

## Features
 - **Custom Gravity Simulation** - Objects experience gravity and move accordingly.
 - **Collision Detection** - Objects detect collisions using `OverlapSphere`.
 - **Bounce Mechanics** - Objects bounce based on a configurable efficiency factor.
 - **Impulse Forces** - Objects can receive impulses along the **X and Z axes**.
 - **Network Synchronization** - Position and velocity are synced across clients.
 - **Reset Mechanism** - Objects reset after bouncing or leaving the bounds.
 - **Multiplayer UI and Chat System** - PlayerCanvas handles network interactions, client tracking, and an in-game chat system with an on-screen keyboard.

---

## Physics Model

### **Gravity**
Each object experiences gravity, continuously modifying its velocity:
```csharp
Velocity += new Vector3(0, Gravity * Time.deltaTime, 0);
```

### **Collision Detection**
Collisions are detected using `Physics.OverlapSphere`:
```csharp
Collider[] hitColliders = Physics.OverlapSphere(Position, Radius, groundLayer);
```

### **Bounce Mechanics**
Upon collision with the ground, bouncing objects update their velocity:
```csharp
Velocity = new Vector3(Velocity.x, Mathf.Abs(Velocity.y) * BounceEfficiency, Velocity.z);
```

### **Impulse Forces**
Impulse forces can be applied in **X** and **Z** directions:
```csharp
Velocity = new Vector3(impulseForce / Mass, Velocity.y, Velocity.z);
```

---

## Multiplayer Interaction with PlayerCanvas

- Network Clients Tracking: The PlayerCanvas tracks the number of connected players and updates the UI accordingly.
- Each player's position and movements are synced across all clients in real-time.
- Chat System: Players can open the chat panel by clicking the Chat button.
The chat system allows sending and receiving messages among networked clients.
An on-screen keyboard is implemented to enable text input for VR users.
- Syncing Position and Velocity: Every player's position and velocity updates are sent and received via Netcode RPCs.
- When a new player joins, they receive the latest state of all objects in the environment.


## Assumptions
- Objects start from predefined positions.
- Network synchronization is server-authoritative.
- Gravity and impulse forces are consistent across all clients.
- Reset conditions include collisions, reaching low velocity, or going out of bounds.


## Interacting with the Simulation

- Running the Simulation
- Launch Unity and start the scene.
- Objects fall due to gravity and interact based on their physics properties.
- When hitting the ground, bouncing objects bounce, while others reset.

- **Applying Impulse**

- Press the on-screen button to apply an impulse.
- The object will move in the corresponding direction.

## Using the Chat System

- Click the Chat button to open the chat panel.
- Type messages using the on-screen keyboard.
- Messages are broadcasted to all players in the session.


## Unit Testing Performed
![image](https://github.com/user-attachments/assets/b12bd18f-73f6-4c5a-8926-0a8d1d0fbe75)

