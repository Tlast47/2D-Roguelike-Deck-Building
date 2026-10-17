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

## Card collection windows

Shop removal, rest upgrades, and battle pile inspection share CardCollectionUI,
CardTileView, and ResponsiveCardGrid. Cards keep a 2:3 ratio in a centered grid of
up to four columns. The viewport includes a scrollbar and comfortable spacing.

- Shop: a dedicated gold balance is placed beside the SHOP heading and updates
  after purchases/removal. Selecting a card outlines that exact deck copy in gold;
  the separate confirmation button displays the card name and removal price.
- Rest: only eligible cards are listed. Selecting one opens a side-by-side view
  of the current and upgraded CardData. Confirm applies one upgrade; choosing
  another card or returning to rest does not use the rest action.
- Battle: Deck shows the persistent run deck, Draw shows the current draw pile,
  and Discard shows the current discard pile. Buttons display live counts.
  Browsers use sorted copies and preserve duplicate cards; they never reorder
  the live piles or consume random numbers. Draw order remains hidden.
- Battle keyboard shortcuts are ignored while a card browser is open.

These windows are created at runtime; their appearance is defined by the shared
UI helpers rather than the legacy RestScene upgrade panel.

## Unified card style

CardVisualStyle applies the shop reference appearance to hand cards, shop offers,
reward cards, upgrade/removal cards, comparison previews, and deck/pile browsers.
The serialized card prefabs use the same anchors and background for editor previews.

- Solid gray background with white text and a centered title.
- Gold titles for upgraded cards in every view.
- Top-left description beneath the title; shared font sizes and fitting limits.
- Footer region: Gold price in shops; Energy cost in battle and card browsers.
- White action buttons with dark text, positioned at the bottom where applicable.
- Only the selected card receives a gold outline in selection windows.
- Preserve the existing hand sizing, grid dimensions, selection actions and prices.

Future card layout changes should be made in CardVisualStyle rather than adding
separate appearance rules to each screen.
