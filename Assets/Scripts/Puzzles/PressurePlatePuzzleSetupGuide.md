# Pressure Plate Puzzle Setup Guide

This guide will walk you through setting up the pressure plate puzzle system in your Unity project.

## 1. Script Overview

The pressure plate puzzle system consists of four main components:

1. **PressurePlate.cs** - Handles individual pressure plate functionality
2. **CodeDisplay.cs** - Displays the code when all plates are activated
3. **PressurePlatePuzzleManager.cs** - Manages the overall puzzle state
4. **Door.cs** (Optional) - Handles door unlocking with the code

## 2. Setting Up the Pressure Plates

### Create Pressure Plate Objects

1. In your scene, create 3D objects (like cubes) for each pressure plate.
2. Position them where you want them in your level.

### Add Components to Pressure Plates

1. Select each pressure plate GameObject.
2. Add these components:
   - **Box Collider** (make sure "Is Trigger" is checked)
   - **Mesh Renderer** (for visual feedback)
   - **PressurePlate** script (from the Puzzles folder)

### Configure Pressure Plate Settings

1. In the Inspector, set up the PressurePlate component:
   - Assign materials for active/inactive states
   - Set up the OnActivate and OnDeactivate events (we'll connect these later)

## 3. Setting Up the Code Display

### Create the Code Display UI

1. Create a new Canvas (Right-click in Hierarchy > UI > Canvas)
2. Set the Render Mode to "World Space" or "Screen Space - Overlay" depending on your needs
3. Add a Text (TMP) element to display the code
4. Add a GameObject to act as the container for the code display

### Add the CodeDisplay Script

1. Select the Canvas or container GameObject
2. Add the **CodeDisplay** script
3. In the Inspector:
   - Assign the TMP Text component to the `codeText` field
   - Set the `code` value to your desired code (e.g., "1234")
   - Assign the container GameObject to `codeDisplayObject`

## 4. Setting Up the Puzzle Manager

### Create the Puzzle Manager Object

1. Create an empty GameObject in your scene
2. Name it "PressurePlatePuzzleManager"
3. Add the **PressurePlatePuzzleManager** script

### Configure the Puzzle Manager

1. In the Inspector:
   - Add all your pressure plate GameObjects to the `pressurePlates` list
   - Assign your CodeDisplay GameObject to the `codeDisplay` field

## 5. Connecting the Components

### Connect Pressure Plates to the Puzzle Manager

1. Select each pressure plate GameObject
2. In the PressurePlate component:
   - Click the "+" button next to the OnActivate event
   - Drag the PressurePlatePuzzleManager GameObject into the field
   - Select the **PressurePlatePuzzleManager > PlateActivated** method
3. Repeat for the OnDeactivate event, selecting the **PlateDeactivated** method

## 6. (Optional) Setting Up the Door

### Create the Door Object

1. Create a 3D object for your door (or use an existing one)
2. Add a Collider component
3. Add the **Door** script

### Configure the Door

1. In the Inspector:
   - Set the `correctCode` to match your code display code
   - Set up any visual feedback (like an Animator for opening animation)
   - Assign any feedback text components

## 7. Testing the Puzzle

1. Enter Play Mode
2. Step on the pressure plates
3. When all plates are activated, the code should appear
4. (Optional) Enter the code to unlock the door

## Troubleshooting

### Common Issues and Solutions

1. **Code Not Showing**:
   - Check if all pressure plates are properly connected to the puzzle manager
   - Verify the code display is assigned in the puzzle manager
   - Make sure the code display object is active in the hierarchy

2. **Pressure Plates Not Detecting Player**:
   - Ensure the player has the correct tag ("Player" by default)
   - Verify the pressure plates have colliders with "Is Trigger" checked
   - Check the console for any error messages

3. **Door Not Unlocking**:
   - Make sure the correct code is entered
   - Verify the door script is properly connected
   - Check if the door animator is set up correctly

## Additional Enhancements

1. **Visual Feedback**:
   - Add particle effects or sound when plates are activated
   - Change plate colors or play animations

2. **Timer Mechanic**:
   - Add a timer that resets the puzzle if plates aren't held for a certain time

3. **Multiple Codes**:
   - Implement different codes for different scenarios

4. **Puzzle Reset**:
   - Add a way to reset the puzzle if players get stuck

This setup guide should help you implement the pressure plate puzzle system in your game. Adjust the settings as needed to fit your specific requirements.
