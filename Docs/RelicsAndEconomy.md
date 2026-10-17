# Relics and the first-act economy

## Relics

Definitions live in `Assets/Resources/Relics`. Each is a RelicData ScriptableObject;
create more with Create > Relics > Relic. The catalog loads these assets in builds.
Owned relics are stored in RunData.Relics and reset on a new run, like gold/deck/HP.
A relic ID can only be owned once. Drops and shop stock exclude owned IDs.

| Relic | Effect | Shop price |
| --- | --- | ---: |
| Iron Shell | First turn: +8 Block | 90 |
| Traveler Satchel | Opening hand: draw 2 extra cards | 110 |
| Amber Lantern | First turn: +1 Energy | 130 |
| Ember Stone | Battle start: apply 2 Burn to every enemy | 110 |
| Venom Vial | Battle start: apply 2 Poison to every enemy | 140 |
| Guardian Seal | Every player turn: +2 Block | 150 |
| Crimson Vial | Victory: heal 4 HP before saving battle HP | 120 |
| Golden Idol | Battle rewards: +20 Gold | 75 |
| Heart Locket | Pickup: +10 Max HP and heal 10 HP | 100 |
| Thunder Charm | Battle start: deal 5 damage to every enemy | 130 |

Opening Block/Energy trigger after the first resource reset. Guardian Seal also
works on turn one. Opening draw adds to the initial five cards rather than replacing
or redrawing them. Relic inventory details are available from the Relics button.
The battle ignores combat keyboard shortcuts while this inventory is open.

## Battle rewards

| Room | Base gold | Chance of one relic |
| --- | ---: | ---: |
| Normal battle | 25 | 25% |
| Elite | 45 | 75% |
| Boss | 80 | 100% |

A winning battle still offers three eligible base cards (choose one). A relic is
an additional reward, claimed independently with Claim relic. Gold is granted once.
The rolled rewards and claim flags are cached per map node for the current run,
so reopening RewardScene cannot reroll the drop or grant the same reward again.
If all relics are owned, no duplicate is substituted. Leaving can skip unclaimed items.

## Shop cards

Each standard shop sells three different card families: at least one base and one
upgraded card where the configured pool provides both. The remaining slot has a
25% upgrade chance. Upgrades display their + name in gold. Strike/Strike+ and
Defend/Defend+ remain excluded from the shop and battle reward pools.

| Card family | Base price | Upgraded price |
| --- | ---: | ---: |
| Quick Guard | 35 | 60 |
| Heavy Slash | 45 | 70 |
| Fortify | 45 | 70 |
| Cleave | 50 | 75 |
| Ember Strike | 50 | 75 |
| Guarded Strike | 55 | 80 |
| Venom Stab | 55 | 80 |
| Ignite | 70 | 95 |
| Toxic Mist | 80 | 105 |
| Flame Wave | 85 | 110 |
| Blazing Guard | 90 | 115 |
| Deadly Dose | 115 | 150 |

Prices vary by +/-10%, rounded to 5 Gold. Equal prices within the same stock are
separated by 5 Gold. Prices and sold flags are cached per shop node and never
rerolled when a purchase is made. Tune per-card Base Shop Price on CardData;
Is Upgraded explicitly identifies upgrade assets instead of guessing from names.

## Relic shop and removal

- Each shop stocks up to two different unowned relics, at the prices above.
- The Gold HUD and all purchase buttons update after any transaction.
- Removal starts at 50 Gold and increases by 20 for each removal during the run.
- Removal is limited to once per shop and cannot empty the deck.
- Select a specific indexed deck entry, then confirm the displayed card and price.
  Canceling is free. Index validation avoids removing a different copy/card.
- Shops and rewards retain their state across scene changes for the current run.
  This extends the existing in-memory run model; it does not add save-to-disk.

Starting gold remains 100. The short first-act budget makes early common cards,
one permanent relic, and deck thinning competing purchases. These are initial
balance values; combat/playthrough balance has not been measured.
