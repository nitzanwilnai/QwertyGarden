# Flower Card Drag Scroll Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add mouse drag scrolling to the flower card carousel with snap-to-nearest on release, and fix card spacing bug.

**Architecture:** All changes in EditFlowersVisual.cs. Drag state tracked in new fields. Drag detection in Tick using Mouse.current from InputSystem. Click suppression via m_wasDragging flag checked in selectFlower.

**Tech Stack:** Unity 6, C#, UnityEngine.InputSystem (Mouse.current)

**Spec:** `docs/superpowers/specs/2026-03-19-flower-card-drag-scroll-design.md`

---

### Task 1: Extract CARD_SPACING constant and fix spacing bug

**Files:**
- Modify: `Assets/Scripts/EditFlowersVisual.cs`

- [ ] **Step 1: Add constant and fix Init spacing**

Replace the magic numbers with a constant. In the field declarations area (around line 33), add:

```csharp
const float CARD_SPACING = 256.0f;
```

In `Init` line 126, change `288.0f` to `CARD_SPACING`:

```csharp
m_flowerPopupGUI[flowerType].GO.transform.localPosition = new Vector3(CARD_SPACING * flowerType, 0.0f, 0.0f);
```

In `selectFlowerCommon` line 376, replace `-256.0f` with `-CARD_SPACING`:

```csharp
m_targetX = -CARD_SPACING * newFlowerType;
```

- [ ] **Step 2: Remove dead field**

Delete line 41:

```csharp
float m_cardsParentTargetX;
```

- [ ] **Step 3: Commit**

```
fix: extract CARD_SPACING constant and fix 288->256 spacing bug
```

---

### Task 2: Add drag state fields and reset in Show

**Files:**
- Modify: `Assets/Scripts/EditFlowersVisual.cs`

- [ ] **Step 1: Add drag state fields**

Add these fields near the other state fields (after `m_flowerType`, around line 59):

```csharp
bool m_isDragging;
bool m_wasDragging;
float m_dragStartMouseX;
float m_dragStartX;
const float DRAG_THRESHOLD = 10.0f;
```

- [ ] **Step 2: Reset drag state in Show**

In `Show()`, after the existing `m_targetX = 0.0f;` (line 205), add:

```csharp
m_isDragging = false;
m_wasDragging = false;
```

- [ ] **Step 3: Commit**

```
feat: add drag scroll state fields to EditFlowersVisual
```

---

### Task 3: Implement drag logic in Tick

**Files:**
- Modify: `Assets/Scripts/EditFlowersVisual.cs`

- [ ] **Step 1: Add handleDragInput method**

Add a new private method before `Tick`:

```csharp
void handleDragInput()
{
    Mouse mouse = Mouse.current;
    if (mouse == null)
        return;

    float mouseX = mouse.position.ReadValue().x;

    if (mouse.leftButton.wasPressedThisFrame)
    {
        m_isDragging = true;
        m_wasDragging = false;
        m_dragStartMouseX = mouseX;
        m_dragStartX = m_currentX;
    }

    if (m_isDragging && mouse.leftButton.isPressed)
    {
        float screenDelta = mouseX - m_dragStartMouseX;
        if (Mathf.Abs(screenDelta) > DRAG_THRESHOLD)
        {
            m_wasDragging = true;
            float canvasDelta = screenDelta / m_canvas.scaleFactor;
            m_currentX = m_dragStartX + canvasDelta;
            m_targetX = m_currentX;
        }
    }

    if (mouse.leftButton.wasReleasedThisFrame && m_isDragging)
    {
        m_isDragging = false;
        if (m_wasDragging)
        {
            float snapped = Mathf.Round(m_currentX / CARD_SPACING) * CARD_SPACING;
            float minX = -(balance.NumFlowers - 1) * CARD_SPACING;
            m_targetX = Mathf.Clamp(snapped, minX, 0.0f);
        }
    }
}
```

- [ ] **Step 2: Call handleDragInput at the top of Tick**

At the beginning of `Tick(float dt)`, before the existing slide logic (line 263), add:

```csharp
handleDragInput();
```

- [ ] **Step 3: Commit**

```
feat: implement mouse drag scrolling for flower cards
```

---

### Task 4: Add click suppression in selectFlower

**Files:**
- Modify: `Assets/Scripts/EditFlowersVisual.cs`

- [ ] **Step 1: Add early return in selectFlower**

At the very top of `selectFlower(int newFlowerType)` (line 335), before any existing code, add:

```csharp
if (m_wasDragging)
    return;
```

- [ ] **Step 2: Commit**

```
feat: suppress flower selection during drag scroll
```

---

### Task 5: Manual testing in Unity Editor

- [ ] **Step 1: Open Unity and enter the Edit Flowers screen**

Navigate to: play the game → select a keyboard → select a key on the keyboard to open the flower cards.

- [ ] **Step 2: Verify card spacing**

Confirm flower cards are evenly spaced at 256px apart (no gap change from the old 288px bug).

- [ ] **Step 3: Verify click-to-select still works**

Click a flower card. It should scroll to center on that card AND select it (outline, scale, receipt update).

- [ ] **Step 4: Verify drag scrolling**

Press and drag left/right on the cards area. Cards should follow the mouse. On release, they should snap to the nearest card. No flower should be selected.

- [ ] **Step 5: Verify drag-then-release doesn't select**

Press on a flower card, drag past the threshold, release on the same or different card. No selection should occur.

- [ ] **Step 6: Verify edge clamping**

Drag past the first card (rightward) or past the last card (leftward). On release, it should snap to the first or last card respectively, not go out of bounds.
