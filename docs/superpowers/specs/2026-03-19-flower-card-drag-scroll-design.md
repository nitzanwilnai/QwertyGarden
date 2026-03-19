# Flower Card Drag Scroll

## Summary

Add mouse drag scrolling to the flower card carousel in EditFlowersVisual. On release, snap to the nearest card. Dragging does not select a flower — only clicking does. Fix card spacing from 288px to 256px.

## Changes

All changes are in `Assets/Scripts/EditFlowersVisual.cs`. No Logic changes needed — this is pure UI.

### Bug Fix & Constant

- Extract `const float CARD_SPACING = 256.0f` and use it everywhere: Init card placement (line 126, was 288), snap calculation, and the existing `selectFlowerCommon` (line 376, already 256).
- Remove unused field `m_cardsParentTargetX` (line 41, dead code).

### New State Fields

- `bool m_isDragging` — true while the user is actively dragging
- `bool m_wasDragging` — true if the last interaction was a drag (used to suppress clicks)
- `float m_dragStartMouseX` — mouse screen X when drag began
- `float m_dragStartX` — `m_currentX` when drag began
- `const float DRAG_THRESHOLD = 10.0f` — screen pixel distance before a press becomes a drag

### Drag Logic in Tick

Uses `UnityEngine.InputSystem.Mouse.current`:
- `Mouse.current.leftButton.wasPressedThisFrame` for mouse down
- `Mouse.current.leftButton.isPressed` for mouse held
- `Mouse.current.leftButton.wasReleasedThisFrame` for mouse up
- `Mouse.current.position.ReadValue().x` for mouse X position

Steps:

1. **Mouse down:** Record `m_dragStartMouseX` (screen position) and `m_dragStartX = m_currentX`. Set `m_isDragging = true`, `m_wasDragging = false`.
2. **Mouse held + moved beyond threshold:** Convert screen delta to canvas space: `canvasDelta = screenDelta / m_canvas.scaleFactor`. Update `m_currentX = m_dragStartX + canvasDelta`. Set `m_wasDragging = true`. Also set `m_targetX = m_currentX` so the normal slide logic doesn't fight the drag.
3. **Mouse up:** Set `m_isDragging = false`. If `m_wasDragging`, snap: `m_targetX = Mathf.Round(m_currentX / CARD_SPACING) * CARD_SPACING`, clamped to `[-(numFlowers-1) * CARD_SPACING, 0]`. The existing Tick slide logic animates to the snap point.

### Click Suppression

- In `selectFlower`: early-return if `m_wasDragging` is true
- `m_wasDragging` is reset to false on the next mouse down

### State Reset

Reset `m_isDragging` and `m_wasDragging` to false in `Show()`, alongside the existing `m_currentX`/`m_targetX` reset.
