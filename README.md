# LoRSideTracker
Legends of Runeterra Deck Tracker

This deck tracker is built on the notification system described in https://developer.riotgames.com/docs/lor

Latest Release:

<a href="https://github.com/ronbos/LoRSideTracker/releases/tag/0.8">LoR Side Tracker 0.8</a> (<a href="https://github.com/ronbos/LoRSideTracker/releases/download/0.8/LoRSideTracker.zip">download</a>)

A near full-featured release:
- Track your deck, your and opponent graveyard, and all cards drawn/generated
- Full card art for each card tracked
- Identify generated cards (green text), accurate in most cases
- Per-deck history for each constructed deck and expedition (double-click to rename deck)
- Automated expedition overall record display
- Full game history
- Action log, which can also be accessed from game history

Existing issues:
- Tossed cards not tracked due to limitations of Riot API
- Champion cards transformed to spells in hand (and vice versa) tracked as generated cards
- Listening port cannot be changed currently
- Unable to accurately mark all opponent cards as generated
- Each set update requires a one-time large download from Riot servers
- Some cards get logged twice, especially cards that offer a choice of action.

Future planned work:
- Game recording and playback
- Expedition draft progression review (selected sets and swaps only)

Screenshots:
- <a href="https://github.com/ronbos/LoRSideTracker/releases/download/0.8/Decks.jpg">Decks History</a>
- <a href="https://github.com/ronbos/LoRSideTracker/releases/download/0.8/Expeditions.jpg">Expeditions History</a>
- <a href="https://github.com/ronbos/LoRSideTracker/releases/download/0.8/Tracking.jpg">Tracking screen</a>

