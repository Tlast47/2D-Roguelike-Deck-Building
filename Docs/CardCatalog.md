# Card catalog

Single-character first-game card set. 14 base cards and 14 upgrades (28 CardData assets).

Starter deck: 4 Strike, 1 Heavy Slash, 5 Defend, fixed for every new run.
Rewards: choose 1 of 3 distinct cards drawn from the 12 new base cards.
Shop: 3 distinct cards sampled from the same 12-card pool; 50 gold each.
All base cards upgrade once at Rest; upgraded cards are not in reward/shop pools.

Burn deals damage equal to stacks at the end of the affected character turn; Block absorbs it.
Poison deals damage equal to stacks at the start of the affected character turn and bypasses Block.
Each status loses 1 stack after triggering. Stacks add together and reset between battles.

| Card | Energy | Rarity | Base | Upgrade |
| --- | ---: | --- | --- | --- |
| Strike | 1 | Common | Deal 6 damage. | Deal 9 damage. |
| Defend | 1 | Common | Gain 5 Block. | Gain 8 Block. |
| Heavy Slash | 2 | Common | Deal 14 damage. | Deal 20 damage. |
| Cleave | 1 | Common | Deal 5 damage to ALL enemies. | Deal 8 damage to ALL enemies. |
| Guarded Strike | 1 | Common | Deal 5 damage. Gain 4 Block. | Deal 8 damage. Gain 6 Block. |
| Quick Guard | 0 | Common | Gain 3 Block. | Gain 5 Block. |
| Fortify | 2 | Common | Gain 13 Block. | Gain 18 Block. |
| Ember Strike | 1 | Common | Deal 5 damage. Apply 2 Burn. | Deal 7 damage. Apply 3 Burn. |
| Venom Stab | 1 | Common | Deal 4 damage. Apply 3 Poison. | Deal 6 damage. Apply 4 Poison. |
| Ignite | 1 | Uncommon | Apply 5 Burn. | Apply 7 Burn. |
| Toxic Mist | 2 | Uncommon | Apply 3 Poison to ALL enemies. | Apply 4 Poison to ALL enemies. |
| Flame Wave | 2 | Uncommon | Deal 5 damage to ALL enemies. Apply 2 Burn to ALL enemies. | Deal 8 damage to ALL enemies. Apply 3 Burn to ALL enemies. |
| Blazing Guard | 2 | Uncommon | Gain 7 Block. Apply 3 Burn to ALL enemies. | Gain 10 Block. Apply 4 Burn to ALL enemies. |
| Deadly Dose | 2 | Rare | Apply 7 Poison. | Apply 10 Poison. |

## Editing the set

Cards: Assets/ScriptableObjects/Cards. Shared effect assets: Assets/ScriptableObjects/Effects.
Add effects to the CardData Effects list in execution order; set Target on each effect.
Assign the upgraded CardData to Upgraded Card; upgraded cards have no further upgrade.
Assign Strike, Heavy Slash and Defend on RunManager in MapScene. Configure rewardCardPool in RewardScene and shopCards/shopCardCount in ShopScene.
Rarity is currently metadata; pool selection keeps the existing uniform random behavior.

## System integration

RunManager owns RunData, PlayerDeck and PlayerRunData across scenes.
DeckManager creates draw/hand/discard piles from PlayerDeck per battle.
BattleManager spends Energy, handles target selection, resolves effects and advances turns.
CardEffectManager executes CardData effects through CardEffectContext.
RewardManager and ShopManager add chosen CardData assets to the run deck.
RestManager upgrades a deck entry by replacing it with UpgradedCard.

Strike, Strike+, Defend and Defend+ have Available In Card Pools disabled; both shop and reward generation filter them out.
Character UI shows only active Block, Burn and Poison and removes empty status rows.

Missing starter-card Inspector references are resolved from Assets/Resources/StarterDeck.asset. A run cannot start with an incomplete starter deck.
