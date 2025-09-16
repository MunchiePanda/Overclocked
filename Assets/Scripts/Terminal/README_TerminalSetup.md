# 🎮 **Terminal System Setup Guide**

## **What You Have Now:**
✅ **Main AI Terminal** - Working! Gives cryptic puzzle hints  
✅ **Individual Puzzle Terminals** - Ready to create! Each controls one puzzle  
✅ **Full Screen Lockdown** - When accessing a terminal, player movement stops and UI takes over  

## **The Concept:**
Players walk around the room and find **small terminals** scattered around. Each terminal controls one specific puzzle:

- **Press E** → Terminal opens in **full screen mode**
- **Player movement locked** → Can only interact with that puzzle UI
- **Press ESC or Close** → Back to normal movement

---

## **🔧 Quick Setup Steps:**

### **Step 1: Clean Up Main Terminal**
1. **Select your `/SetUpHelper` GameObject**
2. **Add `TerminalUIHelper` component**
3. **Click "Clean Up Duplicate Components"** (removes old script from AITerminal)

### **Step 2: Create Puzzle Terminals**
1. **In TerminalUIHelper, click "Setup Existing Puzzle UIs"**
2. **This automatically creates:**
   - `CipherWheelTerminal` → Controls your Cipher Wheel puzzle
   - `ShadowLogicTerminal` → Controls your Shadow Logic puzzle
3. **Move these terminals** to desired positions in your room (they spawn at origin)

### **Step 3: Link Puzzle UIs**
The script automatically links your existing puzzle canvases to the terminals:
- **CipherWheelCanvas** → CipherWheelTerminal
- **ShadowLogicCanvas** → ShadowLogicTerminal

---

## **🎯 How It Works:**

### **Player Experience:**
1. **Walk to a terminal** → See "Press E to access [Terminal Name]"
2. **Press E** → Full screen UI opens, movement locked
3. **Solve the puzzle** → Interact with familiar puzzle UI
4. **Press ESC or Close** → Back to walking around

### **Technical Details:**
- **Full Screen Overlay** - ScreenSpaceOverlay canvas covers entire screen
- **Player Lock** - PlayerController.canMove = false while terminal is open
- **Cursor Management** - Automatically unlocks cursor for UI interaction
- **Visual Feedback** - Terminals light up when player is nearby, change color when solved

---

## **🎨 Customization:**

### **Terminal Appearance:**
- Terminals are simple cubes by default
- Change materials, meshes, add lights/particles as desired
- Set `hoverMaterial` for interaction feedback

### **UI Styling:**
- Modify `panelBackgroundColor`, `textColor`, `buttonNormalColor` in TerminalUIHelper
- Use "Style All Terminal UIs" to apply changes to all terminals

### **Positioning:**
- Move terminal GameObjects to desired locations around your room
- Each terminal should be near related puzzle objects/clues

---

## **🔄 Integration with Existing Puzzles:**

Your existing puzzle scripts (CipherWheelPuzzle, ShadowLogicPuzzle) don't need changes!
The terminals just show/hide your existing UI canvases.

**Current Flow:**
1. Terminal opens → Shows your existing puzzle UI
2. Player solves puzzle → Your existing puzzle logic runs
3. Terminal closes → Hides UI, enables movement

---

## **🧪 Testing:**

1. **Walk to a terminal** (should see interaction prompt)
2. **Press E** (UI should open, movement should stop)
3. **Try moving** (WASD shouldn't work)
4. **Press ESC** (should close and restore movement)
5. **Test all terminals** and puzzles

---

## **📝 Next Steps:**

1. **Position terminals** around your room logically
2. **Add visual polish** (materials, lights, effects)
3. **Test the complete flow** from hints → terminals → puzzles → escape codes
4. **Add more terminals** for additional puzzles if needed

**Ready to create an immersive escape room experience!** 🚀