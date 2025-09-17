# 🎯 **Enhanced SetUpHelper Guide**

## **🚀 What's New:**

Your **EscapeRoomSetupHelper** now includes a complete **Terminal System Setup** section that works with **Unity 6** and **Alteruna Networking**.

---

## **📋 Quick Start (Easiest Way):**

### **If you already have puzzles:**
1. **Click "Do Complete Setup"** ✨ - This does everything automatically!

### **If you need to create puzzles first:**
1. **Use Legacy section** to create puzzles (AI Terminal, Cipher Wheel, etc.)
2. **Click "Do Complete Setup"** ✨

---

## **🔧 Manual Step-by-Step Setup:**

### **Step 1: Create Your Puzzles (If Needed)**
Use the **Legacy options** at the bottom:
- ✅ Create Escape Code Manager
- ✅ Create AI Terminal
- ✅ Create Cipher Wheel UI
- ✅ Create Shadow Logic UI
- ✅ Create Frequency UI (optional)

### **Step 2-6: Terminal System Setup**
**Just click each checkbox in order:**
- ✅ Clean up AI Terminal
- ✅ Setup Terminal System
- ✅ Create Puzzle Terminals
- ✅ Style All Terminals
- ✅ Test Terminal System

---

## **🎮 What You Get:**

### **Individual Puzzle Terminals:**
- **Small terminal objects** scattered around your room
- **Walk up and press E** → Full screen puzzle UI opens
- **Player movement locked** while solving puzzles
- **Press ESC or Close** → Back to walking around

### **Visual Features:**
- **Lights that pulse** when player is nearby
- **Different colors** for solved/unsolved puzzles
- **Consistent styling** across all terminals

### **Networking Compatible:**
- **Works with Alteruna multiplayer**
- **Each player can use terminals independently**
- **Shared puzzle states** between players

---

## **🧪 Testing Your Setup:**

1. **Run the game**
2. **Walk up to any terminal** (should see "Press E to access...")
3. **Press E** → UI opens, movement stops
4. **Try moving** → WASD should not work
5. **Press ESC** → Back to normal movement
6. **Test all terminals** to make sure they work

---

## **🎨 Customization:**

### **Terminal Positioning:**
- **Move terminal GameObjects** to desired locations in your room
- **Position them near related puzzle clues** for intuitive gameplay

### **Visual Styling:**
- **Modify colors** in TerminalUIHelper component
- **Add different materials** to terminal objects
- **Adjust light colors** and intensities

### **UI Layout:**
- **Terminals use full screen overlay** - no more tiny world-space UIs!
- **Existing puzzle UIs work unchanged** - just get shown/hidden by terminals

---

## **🐛 Troubleshooting:**

### **"Press E" doesn't work:**
- **Make sure AITerminal layer** is set to "Interactable"
- **Check BoxCollider isTrigger** is set to `false` (important for raycasting!)
- **Run "Clean Up AI Terminal"** to fix common issues

### **Terminals don't open:**
- **Run "Test Terminal System"** to check for missing references
- **Make sure you have a PlayerController** in the scene

### **Puzzles don't show:**
- **Check if puzzle canvases exist** (CipherWheelCanvas, ShadowLogicCanvas, etc.)
- **Create puzzles first** using Legacy options

---

## **💡 Pro Tips:**

- **Use "Test Terminal System"** regularly to catch issues early
- **Position terminals logically** around puzzle-related objects
- **The system is designed to be intuitive** - players discover terminals naturally
- **Each terminal controls one specific puzzle** - clear and focused gameplay

**Ready to create an amazing escape room experience!** 🎉