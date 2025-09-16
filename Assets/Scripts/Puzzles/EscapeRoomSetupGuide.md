# Escape Room Puzzle Setup Guide

## Overview
This guide will help you set up the three new escape room puzzles with the AI terminal system.

## Required GameObjects Setup

### 1. EscapeCodeManager Setup
**Purpose**: Central manager that collects puzzle completion numbers and generates the final escape code.

**GameObject Structure**:
```
EscapeCodeManager (Empty GameObject)
├── EscapeCodeManager.cs script
```

**Inspector Settings**:
- **Collected Numbers**: Leave empty (auto-populated)
- **Escape Door**: Drag your Door object here
- **Win Screen**: Drag your WinScreen GameObject here
- **Required Puzzle Count**: Set to 3
- **Code Method**: Choose "LastTwoDigits" (recommended)

**UI Elements Needed**:
- CollectedNumbersDisplay: TextMeshPro showing collected numbers
- FinalCodeDisplay: TextMeshPro showing final escape code
- ProgressDisplay: TextMeshPro showing progress (X/3 puzzles completed)

---

### 2. AI Terminal Setup
**Purpose**: Provides cryptic hints for puzzles.

**GameObject Structure**:
```
AITerminal (3D Object - Cube or custom model)
├── TerminalControllerNew.cs script
├── InteractableAITerminal.cs script
├── BoxCollider (IsTrigger = true)
├── Canvas (World Space)
│   └── InteractionPrompt (TextMeshPro)
```

**Inspector Settings for TerminalControllerNew**:
- **Terminal UI**: Create a Canvas with terminal interface
- **Hint Text**: TextMeshPro for displaying hints
- **Button 1, 2, 3**: Buttons for each puzzle
- **Button Text 1, 2, 3**: TextMeshPro for button labels
- **Close Button**: Button to close terminal
- **Terminal Header**: TextMeshPro for header text
- **Instruction Text**: TextMeshPro for instructions

**Inspector Settings for InteractableAITerminal**:
- **Terminal Controller**: Drag the TerminalControllerNew component
- **Interaction Prompt**: Drag the TextMeshPro from Canvas
- **Hover Text**: "Press E to access AI Terminal"

---

### 3. Cipher Wheel Puzzle Setup
**Purpose**: Players decode symbols using rotating wheels.

**GameObject Structure**:
```
CipherWheelPuzzle (Empty GameObject)
├── CipherWheelPuzzle.cs script
├── Canvas (Screen Space - Overlay)
│   ├── PuzzlePanel (Panel)
│   │   ├── OuterRingButton (Button)
│   │   ├── InnerRingButton (Button)
│   │   ├── SymbolDisplay (TextMeshPro)
│   │   ├── LetterDisplay (TextMeshPro)
│   │   ├── CodeInputField (TMP_InputField)
│   │   ├── SubmitButton (Button)
│   │   ├── ResetButton (Button)
│   │   ├── SymbolSequenceDisplay (TextMeshPro)
│   │   ├── DescriptionText (TextMeshPro)
│   │   └── FeedbackText (TextMeshPro)
```

**Inspector Settings**:
- **Symbol Sequences**: ["△○□", "●◇▽", "◆⬟⬢"]
- **Correct Password**: "CODE"
- Connect all UI references

**Visual Elements Needed**:
- Place symbol clues around the room (3D Text or UI panels)
- Create visual cipher wheel representation

---

### 4. Frequency Resonance Puzzle Setup
**Purpose**: Players match audio frequencies and sequences.

**GameObject Structure**:
```
FrequencyResonancePuzzle (Empty GameObject)
├── FrequencyResonancePuzzle.cs script
├── Canvas (Screen Space - Overlay)
│   ├── PuzzlePanel (Panel)
│   │   ├── FrequencySlider (Slider)
│   │   ├── FrequencyDisplay (TextMeshPro)
│   │   ├── FrequencyButton1-5 (Buttons)
│   │   ├── SubmitSequenceButton (Button)
│   │   ├── ClearSequenceButton (Button)
│   │   ├── SequenceDisplay (TextMeshPro)
│   │   ├── WaveformIndicator1-5 (Images)
│   │   ├── DescriptionText (TextMeshPro)
│   │   └── FeedbackText (TextMeshPro)
├── FrequencySource1-5 (Empty GameObjects with AudioSource)
```

**Inspector Settings**:
- **Target Frequencies**: [220, 440, 659, 880, 1100]
- **Correct Sequence**: [1, 3, 0, 4, 2]
- **Frequency Tolerance**: 20
- Connect all UI and audio references

**Audio Setup**:
- Create 5 AudioSource components
- Set them to play on awake = false
- Volume = 0.5

---

### 5. Shadow Logic Puzzle Setup
**Purpose**: Players manipulate lights and objects to cast correct shadows.

**GameObject Structure**:
```
ShadowLogicPuzzle (Empty GameObject)
├── ShadowLogicPuzzle.cs script
├── Canvas (Screen Space - Overlay)
│   ├── PuzzlePanel (Panel)
│   │   ├── LightRotationSlider1-3 (Sliders)
│   │   ├── LightIntensitySlider1-3 (Sliders)
│   │   ├── LightToggleButton1-3 (Buttons)
│   │   ├── MoveObjectButton1-3 (Buttons)
│   │   ├── ShadowCodeDisplay (TextMeshPro)
│   │   ├── AlignmentStatusDisplay (TextMeshPro)
│   │   ├── SubmitButton (Button)
│   │   ├── DescriptionText (TextMeshPro)
│   │   └── FeedbackText (TextMeshPro)
├── AdjustableLight1-3 (Light objects)
├── ShadowObject1-3 (3D Objects that cast shadows)
├── ShadowObjectTarget1-3 (Empty GameObjects for positioning)
├── ShadowCamera (Camera for shadow detection)
```

**Inspector Settings**:
- **Target Rotations**: [45, 135, 225]
- **Target Intensities**: [1.0, 0.8, 1.2]
- **Rotation Tolerance**: 15
- **Intensity Tolerance**: 0.2
- Connect all lights, objects, and UI references

**Lighting Setup**:
- Create 3 spot lights or directional lights
- Position them to cast shadows on a wall
- Create 3 simple geometric objects (cubes, cylinders)
- Set up target positions for the objects

---

### 6. Win Screen Setup
**Purpose**: Displays when player successfully escapes.

**GameObject Structure**:
```
WinScreen (Canvas - Screen Space - Overlay)
├── WinScreen.cs script
├── WinPanel (Panel)
│   ├── CongratsText (TextMeshPro)
│   ├── EscapeTimeText (TextMeshPro)
│   ├── CollectedNumbersText (TextMeshPro)
│   ├── FinalCodeText (TextMeshPro)
│   ├── RestartButton (Button)
│   ├── QuitButton (Button)
│   └── MainMenuButton (Button)
```

**Inspector Settings**:
- **Restart Scene Name**: "SampleScene"
- **Main Menu Scene Name**: "MainMenu"
- Connect all UI references

---

## Setup Order

1. **Create EscapeCodeManager first** - This manages everything
2. **Set up the AI Terminal** - Players need hints
3. **Create each puzzle in order** (start with Cipher Wheel - it's simplest)
4. **Set up Win Screen** - For when they complete everything
5. **Connect everything together** through references

## Integration with Existing Systems

### PuzzleManager Integration
Add the new puzzles to your existing PuzzleManager:
```csharp
// In PuzzleManager, add to puzzles list:
puzzles.Add(cipherWheelPuzzle);
puzzles.Add(frequencyResonancePuzzle);
puzzles.Add(shadowLogicPuzzle);
```

### Door Integration
Your existing Door script has been updated to work with EscapeCodeManager automatically.

## Visual Requirements

### Cipher Wheel
- Place 3 symbol sequences around the room as 3D text or UI panels
- Use symbols: △○□●◇▽◆⬟⬢
- Make them discoverable but not obvious

### Frequency Resonance
- Create 5 interactive objects that make sounds when clicked
- Visual feedback for frequency matching (green = correct, red = wrong)
- Waveform visualization (optional but helpful)

### Shadow Logic
- Wall or surface for shadow projection
- Moveable objects that cast distinct shadows
- Multiple light sources players can adjust
- Clear visual feedback for alignment

## Testing Checklist

- [ ] EscapeCodeManager collects numbers from all 3 puzzles
- [ ] AI Terminal shows cryptic hints for each puzzle
- [ ] Each puzzle generates a random number when completed
- [ ] Final escape code is generated from collected numbers
- [ ] Door unlocks with correct final code
- [ ] Win screen appears when door is unlocked
- [ ] All UI elements are properly connected
- [ ] Audio works for frequency puzzle
- [ ] Lighting works for shadow puzzle

## Tips

1. **Start Simple**: Set up basic UI first, add polish later
2. **Test Each Puzzle Individually**: Make sure they work before connecting
3. **Use Debug.Log**: Check console for puzzle completion messages
4. **Visual Feedback**: Make sure players know when they're making progress
5. **Clear Instructions**: The AI terminal hints should guide but not spoil

## Troubleshooting

- **No random numbers collected**: Check that puzzles call `FindObjectOfType<EscapeCodeManager>().OnPuzzleCompleted(number)`
- **Door won't unlock**: Verify EscapeCodeManager is setting the door's correctCode
- **Terminal not working**: Check that TerminalControllerNew is attached and UI references are set
- **Win screen not showing**: Make sure Door script calls EscapeCodeManager.OnEscapeSuccessful()