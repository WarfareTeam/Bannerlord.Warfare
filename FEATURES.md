# Warfare Features

## General
- Armies are less likely to travel to siege a settlement already being besieged - weighted by difference in besieger and besieged strength - and will instead try to fight or defend on their own fronts where possible.
- Armies will be redirected to defend a faction settlement instead of besieging a settlement that overpowers them.
- MCM configurable ability to modify maximum battlefield agents with a higher threshold than vanilla allows (max 2000 instead of 1000).
- (EXPERIMENTAL) Decreased chance of war decisions and increased chance of peace decisions in winter. Wars are more likely if the proposer of a decision and the ruler of their kingdom are calculating, and peace is more likely if they are not. Must be enabled in the MCM manually.

## Army Overhaul
- You can manage member parties in an army on demand as the faction leader or as member of the respective army.
- You can change army leaders via the button on the kingdom army page. Relationship penalties won't be incurred when doing so.
- You can split an army via the button on the kingdom army page. It will split into two separate armies with your leader choice for the new army, and the original leader continuing to lead the original army. You choose the parties from the old army to place in the new army.
- You can do both army leader changes and splits at the normal influence cost of calling each member to a new army as the faction or original army leader, or twice the cost otherwise. You cannot do this as a mercenary. Half of the influence will be given to the previous army leader as a refund, unless it is yourself or a member of your clan.
- You can force armies you don't lead to disband even when you're in that army.
- Whenever a party is moved between armies or removed from one, they will become disorganized. When changing leaders, this can be avoided only by changing to a leader already gathered in the army. It is unavoidable for a newly split army, but the army it split from will not be put in a disorganized state. Parties won't be disorganized during army relocation if it had not yet arrived at the old army.
- If any of the new leaders don't have enough influence it will disband after the cohesion falls to zero, so prepare for this. You can add cohesion by joining the army temporarily or as the faction leader.
- The player is not counted in calculations of influence, nor will they be added to any newly created armies automatically. You will need to join the army by finding it on the campaign map like vanilla.
- Strategy can be decided on a per-army basis on the Kingdom army screen. Choose between offensive, defensive and balanced behaviors for your armies. Remembers the last option for an army leader when a new army is created for the same leader. Stacks multiplicatively with the per-war strategy setting on the Diplomacy screen.
- You can automatically maintain cohesion of individual armies you are not leading using your own influence through the Kingdom army screen. When active, cohesion will be automatically boosted to maximum whenever it falls below the daily cohesion change + 30. The lower limit of 30 cohesion is due to armies trying to disband when below this amount. If the player has insufficient influence, the cohesion maintenance will be disabled and a log message will be created. Daily influence change from cohesion maintenance is also shown through a log message.

## Mercenary Overhaul
- Armies tab in the Kingdom UI renamed to Military. Easily see mercenaries that are available, hired by your faction, or hired by another faction. You can also hire and fire mercenaries, view parties, troop counts & costs.
- Mercenaries are now contracted seasonally with a flat gold cost, and can be extended to two total seasons when the remaining contract time is at or below one season.
- Mercenaries do not desert during their contract, but are removed from the kingdom immediately when it expires.
- Mercenaries cost 1.25x their total troop wages, or typically ~50-150k/season on vanilla time settings in the early game.
- Mercenaries do not have troop limitations for individual parties but instead by the sum of all their clan parties normal limits and the amount of weeks since campaign start.
- Mercenaries do not recruit from settlements; They will spawn troops daily to their smallest party when below their troop limit while not in battles, raids or settlements. Limited randomly with a maximum of 10 troops plus the amount of seasons since campaign start.
- Non-player mercenary clans do not change banner colors when joining and leaving kingdoms to distinguish them from vassals.
- There are many more minor factions to be hired as mercenaries.
- Mercenaries are given generic names such as [LEADER]'s Wanderers.
- AI Factions can spend their extra gold on mercenaries during war, which is more or less likely based on opposition strength, if the war is defensive, and how wealthy the hiring clan is.
- Mercenaries do not go to war independently.
- Hired mercenaries can be added to armies by the faction leader with a denar cost instead of an influence cost. The cost type and multiplier is MCM configurable.
- Hiring clan immediately adds a hired mercenary into their army for no cost (one-time), even if they aren't the leader of said army.
- Remaining contract time for a hired mercenary is displayed on the mercenary panel.

## Miscellaneous
- Basic MCM options
- Basic localization support
- Includes two commands to check remaining party food (/campaign.party_food [HeroName]) and hero gold (/campaign.hero_gold [HeroName])
