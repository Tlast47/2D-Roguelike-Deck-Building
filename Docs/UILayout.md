# Shared UI layout

All six game scenes use CanvasScaler / Scale With Screen Size, 1600 x 900, Expand.
Expand preserves the minimum design area on other aspect ratios without clipping UI.
All positions are canvas units; the canvas applies a uniform screen scale.

- Card prefabs: 220 x 330. Names, descriptions, price/cost, and action buttons have separate anchored regions and bounded auto-sizing text.
- Battle: player status and End Turn occupy the left side; the hand occupies the lower right; three enemy slots occupy the upper right.
- ResponsiveCardRow: fits and centers active cards in the hand, shop, and rewards. Reuses existing cards and recalculates on child or area changes.
- Enemy targets stretch over the actual enemy transform. The enemy's own UI supplies its status labels.
- Shop/rewards: centered card choices with a centered Leave button near the bottom.
- Rest: centered actions, an anchored upgrade scroll view, and a grid of the same card prefabs with vertical scrolling.
- Map: preserves generated node positions and recomputes their display coordinates when the canvas size changes. Connections follow their node transforms.

Changes are serialized in scenes and prefabs, so reload the scene after editing during Play mode.
