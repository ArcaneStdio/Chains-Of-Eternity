<div align="center">
  <img src="https://github.com/user-attachments/assets/421993d3-d36f-4287-955e-68cd6d406916" alt="Arcane Chains of Eternity" width="200">
</div>


<div align="center">

![Unity](https://img.shields.io/badge/Unity-2023.3.0f1-000000?style=for-the-badge&logo=unity&logoColor=white)
![Solidity](https://img.shields.io/badge/Solidity-0.8.20-363636?style=for-the-badge&logo=solidity&logoColor=white)
![FLOW](https://img.shields.io/badge/Flow-E84142?style=for-the-badge&logo=avalanche&logoColor=white)
![Web3Auth](https://img.shields.io/badge/Web3Auth-4F46E5?style=for-the-badge&logo=web3dotjs&logoColor=white)


**A Revolutionary Blockchain-Powered RPG Adventure**


</div>

<div align="center">
  <img src="https://github.com/user-attachments/assets/1c5a9e8a-ec73-4210-bb71-82e45df74928" width="700">
  
  <img src="https://github.com/user-attachments/assets/1868457d-6f3d-4328-a588-9da4176d2871" width="700">
  <img src="https://github.com/user-attachments/assets/492394e8-f1aa-4756-b0eb-d5ef989c40b1" width="700">
</div>

---

# Arcane: Chains of Eternity

## Table of Contents
- [Overview](#overview)
- [Problem Statement](#problem-statement)
- [Solution](#solution)
- [Architecture](#architecture)
- [Core Systems](#core-systems)
- [Technical Stack](#technical-stack)
- [Smart Contract Integration](#smart-contract-integration)
- [Game Economy](#game-economy)
- [Installation & Setup](#installation--setup)

## Overview

Arcane: Chains of Eternity is a blockchain-powered 2D fantasy RPG built with Unity that seamlessly integrates Web3 mechanics into core gameplay. The project demonstrates how decentralized infrastructure can enhance modern gaming without compromising player experience or game balance.

Players venture into a mystical world where they craft custom spells, complete dynamic quests, battle enemies, and trade magical items that exist as on-chain NFTs. Unlike traditional Web3 games that bolt blockchain features on top, Arcane embeds ownership, trading, and progression directly into the game loop.

### Key Differentiators

- **Skill-Based Progression**: No pay-to-win mechanics; advancement depends on player skill and strategic decision-making
- **Custom Spell System**: Over 25 modifiers creating millions of unique spell combinations
- **Player-Driven Economy**: Community-powered marketplace with scarcity mechanics and competitive auctions
- **Automated Quest System**: On-chain verifiable randomness and scheduled transactions for fair, dynamic challenges
- **Data Monetization**: Spell borrowing system that allows players to access powerful abilities without heavy costs

## Problem Statement

Most Web3 games fail within months due to fundamental design flaws:

1. **Token-First Design**: Games prioritize tokenomics over gameplay, resulting in shallow experiences
2. **Pay-to-Win Models**: Economic advantages destroy competitive balance and alienate free-to-play users
3. **Collapsed Economies**: Once token rewards dry up, player retention disappears
4. **Lack of Innovation**: Copy-paste mechanics with blockchain added as an afterthought
5. **Poor Player Retention**: No meaningful reason for players to stay beyond initial hype

### The Core Issue

Web3 games forget the players. They launch with hype, but without sustainable game design, the community moves on when rewards end.

## Solution

Arcane addresses these problems through a three-pillar approach:

### 1. Player-Driven Economy

A self-sustaining marketplace where items created by players are traded by players. Scarcity mechanics and competitive auctions ensure items find their true value while keeping the economy active.

### 2. Custom Spell System

Instead of predefined abilities, players craft unique spells from scratch using 25+ modifiers. This creates millions of possible combinations and gives players true ownership of their gameplay style.

### 3. Skill-Based Quest System

Dynamic quests that adapt difficulty based on player level and use verifiable randomness for fair rewards. Progression feeds back into the economy, creating a continuous engagement loop.

## Architecture

```mermaid
graph TB
    subgraph Client["Client Layer"]
        Unity[Unity Game Client]
        UI[User Interface]
    end
    
    subgraph Relayer["Relayer Service"]
        Express[Express Server]
        FCL[Flow Client Library]
    end
    
    subgraph Blockchain["Flow Blockchain"]
        QuestManager[Quest Manager Contract]
        SpellSystem[Spell System Contract]
        Marketplace[Marketplace Contract]
        AuctionHouse[Auction House Contract]
        ItemManager[Item Manager Contract]
        HeroNFT[Hero NFT Contract]
        Scheduler[Transaction Scheduler]
        VRF[Verifiable Random Function]
    end
    
    subgraph Storage["Decentralized Storage"]
        Lighthouse[Lighthouse SDK]
        IPFS[IPFS Storage]
    end
    
    subgraph Database["Database Layer"]
        PostgreSQL[(PostgreSQL)]
    end
    
    subgraph Identity["Identity Layer"]
        ENS[Ethereum Name Service]
    end
    
    Unity --> Express
    UI --> Express
    Express --> FCL
    FCL --> QuestManager
    FCL --> SpellSystem
    FCL --> Marketplace
    FCL --> AuctionHouse
    FCL --> ItemManager
    FCL --> HeroNFT
    
    QuestManager --> Scheduler
    QuestManager --> VRF
    AuctionHouse --> Scheduler
    
    SpellSystem --> Lighthouse
    Lighthouse --> IPFS
    
    Express --> PostgreSQL
    
    Unity --> ENS
    
    style Unity fill:#4A90E2
    style Express fill:#50C878
    style QuestManager fill:#9B59B6
    style SpellSystem fill:#E74C3C
    style Marketplace fill:#F39C12
    style Lighthouse fill:#1ABC9C
```

## Core Systems

### Quest Manager System

The Quest Manager implements a fully on-chain quest system with automated lifecycle management.

```mermaid
sequenceDiagram
    participant Player
    participant Relayer
    participant VRF as Random Picker
    participant QuestContract as Quest Manager
    participant Scheduler as TX Scheduler
    participant Database
    
    Player->>Relayer: Create Quest (level, rarity)
    Relayer->>VRF: Commit Random Values
    VRF-->>Relayer: Receipt Saved
    
    Relayer->>VRF: Reveal Random Values
    VRF-->>Relayer: Enemy Count 1
    
    Relayer->>QuestContract: Calculate Enemy Distribution
    Note over QuestContract: totalWeight = level * rarityFactor * 100
    Note over QuestContract: enemy2 = (totalWeight - enemy1*weight1) / weight2
    
    QuestContract->>QuestContract: Create Quest
    QuestContract->>Scheduler: Schedule Expiry (172800s)
    
    QuestContract-->>Database: Store Quest Data
    Database-->>Player: Quest ID & Details
    
    Note over Scheduler: Wait 2 days...
    Scheduler->>QuestContract: Execute Cleanup
    QuestContract->>Database: Mark Quest Expired
```

#### Quest Creation Flow

1. **Random Enemy Generation**
   - Player initiates quest creation with level and rarity parameters
   - System commits to VRF for random enemy count generation
   - Receipt stored in player's account storage

2. **Quest Initialization**
   - VRF reveals random value for first enemy type count
   - System calculates remaining weight distribution
   - Second enemy type count determined algorithmically
   - Quest created with unique ID and enemy composition

3. **Automated Expiry**
   - Transaction scheduled for 2 days (172800 seconds) in future
   - Cleanup handler registered with Transaction Scheduler
   - Quest status updated to EXPIRED after designated time

# Quest System Documentation

## Table of Contents
- [Overview](#overview)
- [System Architecture](#system-architecture)
- [Contract Specifications](#contract-specifications)
- [Quest Lifecycle](#quest-lifecycle)
- [Data Models](#data-models)
- [Transaction Flows](#transaction-flows)
- [Reward Mechanics](#reward-mechanics)
- [Security Considerations](#security-considerations)

## Overview

This project implements a comprehensive quest management system on the Flow blockchain using Cadence. The system enables players to participate in time-bound quests with varying difficulty levels and rarities, earning token rewards upon successful completion.

### Core Components

- **QuestManager**: Main contract handling quest creation, assignment, and completion
- **Arcane**: Fungible token contract for reward distribution
- **AuctionHouse**: NFT auction system for trading quest-related items
- **Transaction Handlers**: Automated quest expiration management
- **ItemManager**: NFT collection management for quest rewards

### Key Features

- Dynamic quest generation with multiple rarity tiers (C, B, A, S)
- Level-based difficulty scaling
- Automatic quest expiration and cleanup
- Variable reward calculation based on performance
- Player participation tracking
- Escrowed bid management for auctions

## System Architecture

```mermaid
graph TB
    subgraph "Core Contracts"
        QM[QuestManager]
        ARC[Arcane Token]
        AH[AuctionHouse]
        IM[ItemManager]
    end
    
    subgraph "Transaction Handlers"
        QTQ[QuestTransactionQuestHandler]
        QTU[QuestTransactionUserHandler]
        FTS[FlowTransactionScheduler]
    end
    
    subgraph "Player Resources"
        PC[QuestParticipation]
        QC[QuestCollection]
        AV[Arcane Vault]
    end
    
    QM --> ARC
    QM --> IM
    QM --> PC
    QM --> QC
    AH --> IM
    QTQ --> QM
    QTU --> QM
    FTS --> QTQ
    FTS --> QTU
    
    style QM fill:#4A90E2
    style ARC fill:#7ED321
    style AH fill:#F5A623
```

## Contract Specifications

### QuestManager.cdc

The central contract managing all quest operations.

#### Constants

| Constant | Value | Description |
|----------|-------|-------------|
| UNCLAIMED_TIMEOUT | 172800.0 | 2 days in seconds |
| STATUS_ACTIVE | "ACTIVE" | Quest is available |
| STATUS_COMPLETED | "COMPLETED" | Quest successfully finished |
| STATUS_FAILED | "FAILED" | Quest failed or expired |

#### Rarity Configuration

```mermaid
graph LR
    subgraph "Rarity Tiers"
        C[C: Common]
        B[B: Uncommon]
        A[A: Rare]
        S[S: Legendary]
    end
    
    subgraph "Duration (seconds)"
        C --> |3600| CD[1 hour]
        B --> |21600| BD[6 hours]
        A --> |43200| AD[12 hours]
        S --> |86400| SD[24 hours]
    end
    
    subgraph "Distribution (max concurrent)"
        C -.-> |4| CM[4 quests]
        B -.-> |3| BM[3 quests]
        A -.-> |2| AM[2 quests]
        S -.-> |1| SM[1 quest]
    end
```

#### Enemy Types and Weights

| Enemy | Weight | Relative Difficulty |
|-------|--------|---------------------|
| Slime | 10 | Easiest |
| FireWorm | 20 | Easy |
| Wizard | 30 | Medium |
| BringerOfDeath | 40 | Hard |
| Gorgon | 50 | Hardest |

### Arcane.cdc

Fungible token contract following the Flow FungibleToken standard.

#### Key Features

- Initial supply: 1000.0 tokens
- Minting capability restricted to contract owner
- Standard vault operations (deposit, withdraw, balance)
- Metadata views support for wallets and explorers

#### Storage Paths

```
VaultStoragePath: /storage/ArcaneVault
VaultPublicPath: /public/ArcaneVault
MinterStoragePath: /storage/ArcaneMinter
ReceiverPublicPath: /public/ArcaneReceiver
```


## Quest Lifecycle

### Creation and Assignment

```mermaid
stateDiagram-v2
    [*] --> Pending: Manager creates quest
    Pending --> Active: Quest available in pool
    Active --> Assigned: Player joins quest
    Assigned --> InProgress: Player accepts quest
    
    InProgress --> Completed: All enemies defeated
    InProgress --> Failed: Time expired
    InProgress --> Failed: Player gives up
    
    Completed --> [*]
    Failed --> [*]
```

### Quest Generation Algorithm

```mermaid
flowchart TD
    A[Start Quest Generation] --> B{Check slot availability}
    B -->|Slot available| C[Select level and rarity]
    B -->|No slots| Z[Abort: Slot limit reached]
    
    C --> D[Calculate total weight]
    D --> E[Select enemy types]
    E --> F[Generate enemy counts]
    
    F --> G[Calculate duration]
    G --> H[Create Quest resource]
    H --> I[Store in contract pool]
    I --> J[Emit QuestCreated event]
    J --> K[End]
    
    style A fill:#4A90E2
    style K fill:#7ED321
    style Z fill:#D0021B
```

### Enemy Count Calculation

For a quest with:
- Level: L
- Rarity multiplier: R
- Total weight: W = L × R × 100

Each enemy type is assigned a random count based on:
1. Calculate maximum possible count: `max = W / enemy_weight`
2. Randomly select count from range `[0, max]`
3. Deduct used weight from total
4. Repeat for remaining enemy types

## Data Models

### Quest Resource

```cadence
resource Quest {
    let id: UInt64
    let level: UInt8
    let rarity: String
    var enemies: {String: UInt64}
    var assignedTo: [Address]
    var expiresAt: UFix64
    var status: String
    var createdAt: UFix64
    let createdBy: Address
}
```

### QuestParticipation Resource

```cadence
resource QuestParticipation {
    let questID: UInt64
    var status: String
    let joinedAt: UFix64
    var expiresAt: UFix64
}
```

### Auction Listing Struct

```cadence
struct Listing {
    let id: UInt64
    let seller: Address
    let basePrice: UFix64
    let tokenID: UInt64
    var currentBid: UFix64
    var highestBidder: Address?
    let endTime: UFix64
}
```

## Transaction Flows

### Join Quest Transaction

```mermaid
sequenceDiagram
    participant P as Player
    participant QM as QuestManager
    participant M as Manager
    participant S as Storage
    
    P->>QM: joinQuest(questID, playerLevel)
    QM->>M: Verify level requirements
    
    alt Level mismatch > 1
        M-->>P: Error: Level gap too large
    else Level acceptable
        M->>QM: Remove quest from pool
        M->>S: Create QuestParticipation
        S->>S: Save to player storage
        S->>S: Publish capability
        M->>QM: Return quest to pool
        QM-->>P: Success
    end
```

### Complete Quest Transaction

```mermaid
sequenceDiagram
    participant P as Player
    participant QM as QuestManager
    participant M as Manager
    participant ARC as Arcane Vault
    participant RNG as RandomPicker
    
    P->>QM: completeQuest(questID, enemies_defeated)
    QM->>M: Verify quest status
    M->>M: Check expiration
    M->>M: Validate enemies defeated
    
    M->>RNG: Generate variability factor
    RNG-->>M: Random value
    
    M->>M: Calculate reward
    M->>ARC: Withdraw reward amount
    ARC-->>P: Transfer tokens
    
    M->>M: Update quest status
    M-->>P: Emit QuestCompleted event
```

## Reward Mechanics

### Base Reward Formula

```
reward = baseValue × level × rarityMultiplier × variabilityFactor × levelDeltaModifier
```

### Components

1. **Base Value**: Fixed per rarity tier
   - S: 100.0
   - A: 40.0
   - B: 20.0
   - C: 10.0

2. **Level Multiplier**: Player's quest level

3. **Rarity Multiplier**:
   - S: 10x
   - A: 7x
   - B: 3x
   - C: 1x

4. **Variability Factor**: `1.0 ± (0-20%)` (random)

5. **Level Delta Modifier**:
   - Quest 1 level higher: 1.2x (bonus)
   - Quest 1 level lower: 0.7x (penalty)
   - Quest same level: 1.0x

### Example Calculation

```mermaid
graph LR
    A[Player Level 5] --> B[Quest Level 6]
    B --> C[Rarity: A]
    C --> D[Base: 40.0]
    D --> E[40.0 × 6 × 7 = 1680.0]
    E --> F[Variability: +10%]
    F --> G[1680.0 × 1.1 = 1848.0]
    G --> H[Level Delta: +1]
    H --> I[1848.0 × 1.2 = 2217.6 ARC]
    
    style I fill:#7ED321
```

## Security Considerations

### Access Control

```mermaid
graph TD
    subgraph "Public Access"
        A[createEmptyQuestCollection]
        B[depositArc]
        C[Quest viewing functions]
    end
    
    subgraph "Manager Resource Only"
        D[createQuest]
        E[joinQuest]
        F[completeQuest]
        G[cleanupExpiredQuests]
    end
    
    subgraph "Contract Account Only"
        H[withdrawARCAdmin]
        I[Minter operations]
    end
    
    style D fill:#F5A623
    style E fill:#F5A623
    style F fill:#F5A623
    style H fill:#D0021B
    style I fill:#D0021B
```

### Resource Safety

1. **Vault Escrow**: Bid payments moved into contract storage, preventing loss
2. **Capability-based Access**: QuestParticipation uses published capabilities
3. **Atomic Operations**: Quest completion is all-or-nothing
4. **Expiration Cleanup**: Automated handlers prevent resource leaks

### Validation Checks

- Level requirements (max delta of 1)
- Quest expiration verification
- Enemy defeat confirmation
- Duplicate participation prevention
- Auction bid ordering
- Payment sufficiency checks

## Transaction Handler Integration

### Quest Expiration Handler

```cadence
// QuestTransactionQuestHandler.cdc
executeTransaction(id, data) {
    let questID = data.questID
    managerRef.cleanupExpiredQuests(questID)
}
```

### User Expiration Handler

```cadence
// QuestTransactionUserHandler.cdc
executeTransaction(id, data) {
    let questID = data.questID
    let player = data.player
    managerRef.expireParticipantIfNeeded(player, questID)
}
```

### Scheduling Flow

```mermaid
sequenceDiagram
    participant S as Scheduler
    participant FTS as FlowTransactionScheduler
    participant QTQ as QuestHandler
    participant QTU as UserHandler
    participant QM as QuestManager
    
    S->>FTS: Schedule quest expiration
    FTS->>QTQ: executeTransaction(questID)
    QTQ->>QM: cleanupExpiredQuests(questID)
    
    S->>FTS: Schedule user expiration
    FTS->>QTU: executeTransaction(player, questID)
    QTU->>QM: expireParticipantIfNeeded(player, questID)
```

## API Reference

### Quest Management

#### createQuest()
Creates a new quest in the contract pool.

**Access**: Manager resource only  
**Parameters**: None (derives from rarity configuration)  
**Returns**: None  
**Emits**: `QuestCreated`

#### joinQuest(playerAcct, questID, playerLevel)
Assigns player to an existing quest.

**Access**: Manager resource only  
**Parameters**:
- `playerAcct`: Authorized player account reference
- `questID`: Unique quest identifier
- `playerLevel`: Current level of the player

**Validates**:
- Quest exists
- Level delta ≤ 1
- Player not already assigned

**Emits**: `QuestAssigned`

#### completeQuest(signer, questID, playerLevel, enemies_defeated)
Marks quest as completed and distributes rewards.

**Access**: Manager resource only  
**Parameters**:
- `signer`: Player's authorized account
- `questID`: Quest to complete
- `playerLevel`: Player's current level
- `enemies_defeated`: Map of defeated enemies

**Validates**:
- Quest is active
- Not expired
- All enemies defeated
- Player is assigned

**Emits**: `QuestCompleted` or `QuestFailed`

### Token Operations

#### depositArc(from: @Arcane.Vault)
Deposits ARC tokens into contract vault.

**Access**: Public  
**Emits**: `ARCDeposited`

#### withdrawARCAdmin(amount, to)
Withdraws ARC tokens to specified address.

**Access**: Contract owner only  
**Emits**: `ARCWithdrawn`

### Auction Operations

#### listItem(nft, basePrice, seller, endTime)
Creates new auction listing.

**Returns**: `UInt64` listingID  
**Emits**: `Listed`

---


#### Quest Parameters

```cadence
struct Quest {
    pub let id: UInt64
    pub let level: UInt8
    pub let rarity: String
    pub let enemies: {String: UInt64}
    pub let expiresAt: UFix64
    pub var status: String
}
```

**Rarity System**:
- Common: 2 enemy types, multiplier 1.0
- Rare: 3 enemy types, multiplier 1.5
- Epic: 4 enemy types, multiplier 2.0
- Legendary: 5 enemy types, multiplier 3.0

**Weight Calculation**:
```
totalWeight = level * rarityMultiplier * 100
enemy1Count = VRF_randomValue
remainingWeight = totalWeight - (enemy1Count * enemy1Weight)
enemy2Count = remainingWeight / enemy2Weight
```





## Scheduled Transaction Architecture

### Quest Creation with VRF and Scheduled Expiration

```mermaid
sequenceDiagram
    participant Admin as Admin/Scheduler
    participant FTS as FlowTransactionScheduler
    participant RP as RandomPicker (VRF)
    participant QM as QuestManager
    participant Handler as QuestTransactionQuestHandler
    participant Pool as Contract Quest Pool
    
    Note over Admin,Pool: Quest Creation Phase
    
    Admin->>FTS: Schedule quest creation
    FTS->>QM: Trigger createQuest()
    
    QM->>QM: Check slot availability<br/>(level + rarity limits)
    
    alt Slot Available
        Note over QM,RP: VRF Enemy Generation
        QM->>RP: commit(enemyTypes)
        RP-->>QM: receipt
        QM->>RP: reveal(receipt)
        RP-->>QM: Selected enemy types
        
        QM->>RP: commit(countRange)
        RP-->>QM: receipt
        QM->>RP: reveal(receipt)
        RP-->>QM: Enemy count for type 1
        
        QM->>RP: commit(countRange)
        RP-->>QM: receipt
        QM->>RP: reveal(receipt)
        RP-->>QM: Enemy count for type 2
        
        QM->>QM: Create Quest resource<br/>with generated enemies
        QM->>Pool: Store quest in contract pool
        Pool-->>QM: questID
        
        Note over QM,FTS: Schedule Quest Expiration
        QM->>FTS: scheduleTransaction(<br/>executeAt: now + 2 days,<br/>handler: QuestTransactionQuestHandler,<br/>data: {questID})
        
        QM->>QM: Publish public capability<br/>at /public/QuestAccess_{questID}
        
        QM-->>Admin: Emit QuestCreated(questID, level, rarity, expiresAt)
        
        Note over Pool: Quest available for 2 days
        
        rect rgb(255, 200, 200)
            Note over FTS,Handler: After 2 Days
            FTS->>Handler: executeTransaction(questID)
            Handler->>QM: cleanupExpiredQuests(questID)
            QM->>Pool: Remove quest resource
            QM->>QM: Unpublish capability
            QM->>QM: decrementActiveCount()
            QM-->>FTS: Emit QuestRemoved(questID)
        end
        
    else No Slot
        QM-->>Admin: Error: Slot limit reached
    end
```

### Player Quest Participation with Capability Management

```mermaid
sequenceDiagram
    participant P as Player
    participant QM as QuestManager
    participant Mgr as Manager Resource
    participant FTS as FlowTransactionScheduler
    participant UserHandler as QuestTransactionUserHandler
    participant PStorage as Player Storage
    participant Pool as Contract Quest Pool
    participant RP as RandomPicker (VRF)
    participant ARC as Arcane Vault
    
    Note over P,Pool: Player Joins Quest
    
    P->>QM: joinQuest(questID, playerLevel)
    QM->>Pool: Borrow quest reference
    Pool-->>QM: Quest resource
    
    QM->>QM: Validate level<br/>(delta ≤ 1)
    
    alt Level Valid
        QM->>QM: Add player to assignedTo[]
        QM->>PStorage: Create QuestParticipation resource
        PStorage->>PStorage: Save to /storage/QuestParticipation_{questID}
        
        Note over PStorage: Create Capability
        PStorage->>PStorage: Issue storage capability<br/>QuestParticipationManagerAccess
        PStorage->>PStorage: Publish at /public/QuestParticipation_{questID}
        
        Note over P,FTS: Schedule Player Expiration
        QM->>FTS: scheduleTransaction(<br/>executeAt: now + questDuration,<br/>handler: QuestTransactionUserHandler,<br/>data: {player, questID})
        
        QM->>Pool: Return quest to pool
        QM-->>P: Emit QuestAssigned(questID, player)
        
        Note over P,ARC: Quest Active Period
        
        rect rgb(200, 255, 200)
            Note over P,ARC: Player Completes Quest (Before Expiration)
            
            P->>QM: completeQuest(questID, playerLevel, enemies_defeated)
            QM->>Mgr: Validate completion
            Mgr->>Pool: Borrow quest
            
            Mgr->>Mgr: Check status == ACTIVE
            Mgr->>Mgr: Check not expired
            Mgr->>Mgr: Verify enemies_defeated matches quest.enemies
            Mgr->>Mgr: Confirm player in assignedTo[]
            
            Note over Mgr,RP: VRF Reward Calculation
            
            Mgr->>RP: commit([0, 5, 10, 15, 20])
            RP-->>Mgr: receipt (variability)
            Mgr->>RP: reveal(receipt)
            RP-->>Mgr: randomRange (e.g., 15)
            
            Mgr->>RP: commit([0, 1])
            RP-->>Mgr: receipt (sign)
            Mgr->>RP: reveal(receipt)
            RP-->>Mgr: randomSign (0=negative, 1=positive)
            
            Mgr->>Mgr: Calculate variabilityFactor<br/>= 1.0 ± (randomRange * 0.01)
            
            Mgr->>Mgr: Calculate reward:<br/>baseValue × level × rarityMult<br/>× variabilityFactor × levelDeltaMod
            
            Mgr->>ARC: Load contract vault
            ARC-->>Mgr: Vault reference
            Mgr->>ARC: withdraw(reward)
            ARC-->>Mgr: Reward vault
            
            Mgr->>P: Deposit to player's ARC receiver
            P-->>Mgr: Tokens received
            
            Mgr->>ARC: Save vault back
            Mgr->>Pool: quest.markCompleted()
            
            Note over PStorage: Update Participation
            Mgr->>PStorage: Borrow capability
            PStorage-->>Mgr: Participation reference
            Mgr->>PStorage: markCompleted()
            
            Mgr->>Pool: decrementActiveCount()
            Mgr-->>P: Emit QuestCompleted(questID, player, reward)
            
            Note over FTS: Scheduled expiration still runs<br/>but finds quest already completed
        end
        
        rect rgb(255, 200, 200)
            Note over FTS,PStorage: After Quest Duration (If Not Completed)
            
            FTS->>UserHandler: executeTransaction(player, questID)
            UserHandler->>PStorage: Borrow capability
            PStorage-->>UserHandler: Participation reference
            
            UserHandler->>UserHandler: Check status == ACTIVE
            UserHandler->>UserHandler: Check getCurrentBlock().timestamp >= expiresAt
            
            alt Quest Expired
                UserHandler->>PStorage: markExpired()
                PStorage->>PStorage: status = "EXPIRED"
                UserHandler-->>FTS: Emit QuestFailed(questID, "Expired for player")
                
                Note over PStorage: Capability remains but<br/>status prevents completion
            else Already Completed
                UserHandler-->>FTS: No action needed
            end
        end
        
    else Level Invalid
        QM-->>P: Error: Level gap too large
    end
    
    Note over P,Pool: First Player to Complete Gets Full Reward<br/>Subsequent completions still possible<br/>until quest pool expires (2 days)
```

## Detailed VRF Integration Flow

### Quest Creation VRF Process

```mermaid
flowchart TD
    A[Start: createQuest] --> B{Check Slot<br/>Availability}
    
    B -->|Available| C[Calculate Total Weight<br/>W = level × rarityMult × 100]
    B -->|Full| Z1[Abort: No slots]
    
    C --> D[Determine Enemy Types<br/>Based on Rarity]
    
    D --> E1[VRF: Select Enemy Type 1]
    E1 --> E2[commit enemyTypeIndices]
    E2 --> E3[reveal → enemyType1]
    
    E3 --> F1[Calculate maxCount1<br/>= W / weight enemyType1]
    F1 --> F2[VRF: Select Count for Enemy 1]
    F2 --> F3[commit range 0...maxCount1]
    F3 --> F4[reveal → count1]
    
    F4 --> G[Update Remaining Weight<br/>W -= weight1 × count1]
    
    G --> H1[VRF: Select Enemy Type 2]
    H1 --> H2[commit enemyTypeIndices]
    H2 --> H3[reveal → enemyType2]
    
    H3 --> I1[Calculate maxCount2<br/>= W / weight enemyType2]
    I1 --> I2[VRF: Select Count for Enemy 2]
    I2 --> I3[commit range 0...maxCount2]
    I3 --> I4[reveal → count2]
    
    I4 --> J[Build enemies Dictionary<br/>enemyType1: count1<br/>enemyType2: count2]
    
    J --> K[Create Quest Resource]
    K --> L[Store in Contract Pool]
    L --> M[Schedule Expiration<br/>executeAt: now + 2 days]
    M --> N[Publish Public Capability]
    N --> O[Emit QuestCreated]
    O --> P[End]
    
    style A fill:#4A90E2
    style P fill:#7ED321
    style Z1 fill:#D0021B
    style E2 fill:#F5A623
    style E3 fill:#F5A623
    style F3 fill:#F5A623
    style F4 fill:#F5A623
    style H2 fill:#F5A623
    style H3 fill:#F5A623
    style I3 fill:#F5A623
    style I4 fill:#F5A623
```

### Quest Completion VRF Reward Calculation

```mermaid
flowchart TD
    A[Start: completeQuest] --> B[Validate Quest Status]
    
    B --> C{Status == ACTIVE?}
    C -->|No| Z1[Error: Quest not active]
    C -->|Yes| D{Not Expired?}
    
    D -->|Expired| Z2[markFailed<br/>Emit QuestFailed]
    D -->|Valid| E[Verify Enemies Defeated]
    
    E --> F{All Enemies<br/>Match?}
    F -->|No| Z3[Error: Incomplete]
    F -->|Yes| G{Player in<br/>assignedTo?}
    
    G -->|No| Z4[Error: Not assigned]
    G -->|Yes| H[Get Base Reward<br/>from rarity]
    
    H --> I1[VRF: Generate Variability]
    I1 --> I2[commit randomRange<br/>values: 0, 5, 10, 15, 20]
    I2 --> I3[reveal → vrfOutput<br/>e.g., 15]
    
    I3 --> J1[VRF: Generate Sign]
    J1 --> J2[commit randomSign<br/>values: 0, 1]
    J2 --> J3[reveal → vrfSignOutput<br/>0=negative, 1=positive]
    
    J3 --> K{Sign == 0?}
    K -->|Yes| L1[factor = -vrfOutput]
    K -->|No| L2[factor = +vrfOutput]
    
    L1 --> M[Calculate variabilityFactor<br/>= 1.0 + factor × 0.01]
    L2 --> M
    
    M --> N[Initial Reward = baseValue<br/>× questLevel × rarityMult<br/>× variabilityFactor]
    
    N --> O[Calculate Level Delta<br/>Δ = questLevel - playerLevel]
    
    O --> P{Delta Value?}
    P -->|Δ == 1| Q1[reward × 1.2<br/>+20% bonus]
    P -->|Δ == -1| Q2[reward × 0.7<br/>-30% penalty]
    P -->|Δ == 0| Q3[reward × 1.0<br/>No change]
    
    Q1 --> R[Final Reward Calculated]
    Q2 --> R
    Q3 --> R
    
    R --> S[Load Contract ARC Vault]
    S --> T[Withdraw Reward Amount]
    T --> U[Transfer to Player Receiver]
    U --> V[Save Vault Back]
    
    V --> W[Mark Quest Completed]
    W --> X[Update Participation Status]
    X --> Y[Decrement Active Count]
    Y --> AA[Emit QuestCompleted]
    AA --> AB[End]
    
    style A fill:#4A90E2
    style AB fill:#7ED321
    style Z1 fill:#D0021B
    style Z2 fill:#D0021B
    style Z3 fill:#D0021B
    style Z4 fill:#D0021B
    style I2 fill:#F5A623
    style I3 fill:#F5A623
    style J2 fill:#F5A623
    style J3 fill:#F5A623
    style R fill:#BD10E0
```

## VRF Commit-Reveal Pattern Details

### How Flow VRF Works in Quest System

The system uses Flow's VRF (Verifiable Random Function) through the `RandomPicker` contract to ensure fair, unpredictable, and verifiable randomness on-chain.

#### Commit Phase
```cadence
let receipt <- RandomPicker.commit(values: [0, 5, 10, 15, 20])
```
- Submits values to VRF system
- Returns a receipt resource
- Cannot be manipulated after commit

#### Reveal Phase
```cadence
let result: UInt64 = RandomPicker.reveal(receipt: <-receipt)
```
- Consumes the receipt
- Returns provably random value from submitted array
- Result is deterministic but unpredictable before reveal

### VRF Usage in Quest Creation

**Enemy Type Selection:**
```
Input: [0, 1, 2, 3, 4] (enemy indices)
Output: Random index → Select enemy type
```

**Enemy Count Selection:**
```
Input: [0, 1, 2, ..., maxCount]
Output: Random count within weight budget
```

### VRF Usage in Reward Calculation

**Variability Factor (±20%):**
```
Step 1: commit([0, 5, 10, 15, 20])
        reveal → 15 (represents 15%)

Step 2: commit([0, 1])
        reveal → 0 (negative) or 1 (positive)

Result: variabilityFactor = 1.0 - 0.15 = 0.85
        OR
        variabilityFactor = 1.0 + 0.15 = 1.15
```

This ensures rewards vary by ±20% unpredictably, making quest completion exciting while maintaining fairness.

## Race Condition Handling

### First-to-Complete Reward Distribution

```mermaid
gantt
    title Quest Completion Timeline (Multiple Players)
    dateFormat HH:mm:ss
    axisFormat %H:%M:%S
    
    section Quest Active
    Quest Created               :milestone, m1, 00:00:00, 0m
    Player A Joins              :done, p1, 00:05:00, 10m
    Player B Joins              :done, p2, 00:08:00, 10m
    Player C Joins              :done, p3, 00:12:00, 10m
    
    section Completion Race
    Player A Completes (WINNER) :crit, done, c1, 01:30:00, 2m
    Player B Completes          :done, c2, 01:35:00, 2m
    Player C Completes          :done, c3, 01:40:00, 2m
    
    section Quest Lifecycle
    Quest Expires (2 days)      :milestone, m2, 48:00:00, 0m
```

**Key Points:**
- All players who joined can attempt completion
- Each player has their own expiration timer from join time
- First successful completion gets the VRF-calculated reward
- Quest remains in pool until 2-day expiration
- Subsequent completions can still succeed (but quest is already marked completed)
- Quest cleanup happens after 2 days via scheduled transaction

---

## Capability Lifecycle Management

### Storage and Capability Flow

```mermaid
graph TB
    subgraph "Quest Creation"
        A[QuestManager creates Quest] --> B[Store in contract pool]
        B --> C[Publish public capability<br/>/public/QuestAccess_{questID}]
    end
    
    subgraph "Player Joins"
        D[Player calls joinQuest] --> E[Create QuestParticipation resource]
        E --> F[Save to player storage<br/>/storage/QuestParticipation_{questID}]
        F --> G[Issue storage capability]
        G --> H[Publish capability<br/>/public/QuestParticipation_{questID}]
    end
    
    subgraph "Completion or Expiration"
        I{Quest Status?} -->|Completed| J[markCompleted via capability]
        I -->|Expired| K[markExpired via capability]
        J --> L[Capability remains<br/>for historical record]
        K --> L
    end
    
    subgraph "Final Cleanup (2 days)"
        M[Scheduled transaction fires] --> N[cleanupExpiredQuests]
        N --> O[Remove quest from pool]
        O --> P[Unpublish quest capability]
    end
    
    C -.-> D
    H -.-> I
    L -.-> M
    
    style A fill:#4A90E2
    style E fill:#4A90E2
    style J fill:#7ED321
    style K fill:#F5A623
    style O fill:#D0021B
```













### AuctionHouse.cdc

NFT auction system with English auction mechanics.

#### Auction Flow

```mermaid
sequenceDiagram
    participant S as Seller
    participant AH as AuctionHouse
    participant B1 as Bidder 1
    participant B2 as Bidder 2
    participant W as Winner
    
    S->>AH: listItem(NFT, basePrice, endTime)
    AH-->>S: listingID
    
    B1->>AH: placeBid(listingID, payment)
    AH-->>B1: Bid accepted
    
    B2->>AH: placeBid(listingID, higher payment)
    AH->>B1: Refund previous bid
    AH-->>B2: New highest bidder
    
    Note over AH: Auction ends
    
    AH->>AH: completeAuction(listingID)
    AH->>S: Transfer payment (minus fee)
    AH->>W: Transfer NFT
```


### Spell System

The Spell System combines on-chain ownership with off-chain data monetization through Lighthouse SDK.

```mermaid
graph LR
    subgraph Creation["Spell Creation"]
        A[Player Designs Spell] --> B[Spend Arcane Tokens]
        B --> C[Generate Spell Data]
        C --> D[Encrypt with Lighthouse]
        D --> E[Store Hash On-Chain]
    end
    
    subgraph Monetization["Data Monetization"]
        F[Spell Listed] --> G[Demand Increases]
        G --> H[DataCoin Value Grows]
        H --> I[Access Fee Increases]
    end
    
    subgraph Borrowing["Spell Borrowing"]
        J[Player Discovers Spell] --> K[Pay Flow Tokens]
        K --> L[Lighthouse Access Granted]
        L --> M[Decrypt Spell Data]
        M --> N[Use in Game]
    end
    
    E --> F
    I --> J
```

#### Spell Architecture

**Spell Attributes** (25+ modifiers):
- Element Type: Fire, Ice, Lightning, Earth, Wind, Dark, Light
- Damage Type: Physical, Magical, True
- Effect Type: Instant, Duration, Periodic
- Area of Effect: Single Target, Line, Circle, Cone
- Status Effects: Burn, Freeze, Stun, Slow, Poison
- Scaling: Attack, Magic, Level, Intelligence
- Cooldown: Short, Medium, Long
- Energy Cost: Low, Medium, High
- Critical Modifiers: Rate, Damage multiplier
- Range: Melee, Short, Medium, Long

**Spell Combinations**:
With 25+ attributes, the system generates over 1 million unique spell combinations, ensuring no two players have identical loadouts.

#### Data Monetization Flow

1. **Initial Creation**
   - Spell starts with base value equal to creation cost
   - Data encrypted and stored via Lighthouse
   - On-chain hash reference created

2. **Demand-Driven Growth**
   - As more players borrow/view spell, DataCoin value increases
   - Access fee adjusts dynamically based on popularity
   - Original creator receives royalties on each borrow

3. **Borrowing Mechanism**
   - Players pay Flow tokens to access encrypted spell data
   - Lighthouse SDK verifies payment and grants decryption access
   - Borrowed spell available for limited duration or number of uses

#### Credit System

Players purchase spell credits to create new spells, with level-based caps preventing pay-to-win scenarios:

```
Level 1-10: Max 100 credits per spell
Level 11-25: Max 250 credits per spell
Level 26-50: Max 500 credits per spell
Level 51+: Max 1000 credits per spell
```

Free-to-play players earn credits through quest completion and achievements.

### Marketplace & Auction House

```mermaid
stateDiagram-v2
    [*] --> ItemCreated: Quest Reward / Crafting
    ItemCreated --> Listed: Player Lists (3 max)
    Listed --> ActiveListing: Marketplace
    Listed --> ActiveAuction: Auction House
    
    ActiveListing --> Sold: Direct Purchase
    ActiveListing --> Delisted: Cancel Listing
    
    ActiveAuction --> BidPlaced: Player Bids
    BidPlaced --> HigherBid: Outbid
    HigherBid --> BidPlaced
    BidPlaced --> ScheduledEnd: Time Expires
    ScheduledEnd --> Sold: Winner Determined
    
    Sold --> TransactionFee: Platform Fee (2.5%)
    TransactionFee --> OwnershipTransfer
    OwnershipTransfer --> [*]
    
    Delisted --> [*]
```

#### Marketplace Features

**Scarcity Mechanics**:
- Maximum 3 active listings per player
- Encourages strategic trading decisions
- Prevents market flooding

**Listing Types**:
1. **Direct Sale**: Fixed price, instant purchase
2. **Auction**: Competitive bidding with scheduled end time

**Transaction Flow**:
```cadence
transaction(tokenID: UInt64, price: UFix64) {
    let withdrawRef: auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}
    
    prepare(signer: auth(Storage, Capabilities) &Account) {
        // Withdraw NFT from seller's collection
        let nft <- self.withdrawRef.withdraw(withdrawID: tokenID)
        
        // List on marketplace
        MarketPlace.listItem(
            nft: <- nft,
            price: price,
            seller: signer.address
        )
    }
}
```

#### Auction House Mechanics

**Scheduled Auctions**:
```mermaid
sequenceDiagram
    participant Seller
    participant AuctionContract
    participant Scheduler
    participant Bidder
    participant Winner
    
    Seller->>AuctionContract: List Item (base price, duration)
    AuctionContract->>Scheduler: Schedule End (future timestamp)
    
    loop Auction Active
        Bidder->>AuctionContract: Place Bid
        AuctionContract->>Bidder: Refund Previous Bidder
    end
    
    Note over Scheduler: End Time Reached
    Scheduler->>AuctionContract: Execute Settlement
    AuctionContract->>Seller: Transfer Flow Tokens (- fees)
    AuctionContract->>Winner: Transfer NFT
```

**Automated Settlement**:
- Flow Transaction Scheduler handles auction completion
- No manual intervention required
- Fair, trustless process

**Fee Structure**:
- Marketplace listings: 2.5% transaction fee
- Auction house: 2.5% + 0.1 Flow scheduling fee
- Fees fund ongoing development and server costs

### Item Manager (NFT System)

```mermaid
classDiagram
    class ItemNFT {
        +UInt64 id
        +String name
        +String description
        +ItemType itemType
        +Rarity rarity
        +Bool stackable
        +WeaponData? weapon
        +ArmourData? armour
        +ConsumableData? consumable
        +AccessoryData? accessory
    }
    
    class WeaponData {
        +Int damage
        +Int attackSpeed
        +Int criticalRate
        +Int criticalDamage
    }
    
    class ArmourData {
        +Int defense
        +Int maxHealth
        +Int healthRegen
        +[Int] resistances
    }
    
    class ConsumableData {
        +String effectType
        +Int effectValue
        +Int duration
    }
    
    class AccessoryData {
        +[String] statBoosts
        +[Int] values
    }
    
    ItemNFT --> WeaponData
    ItemNFT --> ArmourData
    ItemNFT --> ConsumableData
    ItemNFT --> AccessoryData
```

#### Item Types

**Weapons**:
- Swords, Axes, Bows, Staves, Daggers
- Stats: Damage, Attack Speed, Critical Rate, Critical Damage

**Armour**:
- Helmets, Chest plates, Leggings, Boots, Shields
- Stats: Defense, Max Health, Health Regeneration, Elemental Resistances

**Consumables**:
- Potions, Scrolls, Food, Elixirs
- Effects: Healing, Buffs, Debuff Removal, Stat Boosts

**Accessories**:
- Rings, Amulets, Trinkets
- Effects: Stat Boosts, Special Abilities, Set Bonuses

#### NFT Minting Process

```javascript
// Mint NFT transaction
const mintNFT = async (recipientAddr) => {
    const txId = await fcl.mutate({
        cadence: mintTransaction,
        args: (arg, t) => [arg(recipientAddr, t.Address)],
        proposer: authz,
        payer: authz,
        authorizations: [authz],
        limit: 9999,
    });
    
    const status = await fcl.tx(txId).onceSealed();
    return status.events[0].data.id; // Return minted token ID
};
```

### Hero NFT System

Each player's character exists as an NFT with comprehensive stat tracking and progression.

```cadence
pub struct Stats {
    pub let offensiveStats: OffensiveStats
    pub let defensiveStats: DefensiveStats
    pub let specialStats: SpecialStats
    pub let statPointsAssigned: StatPointsAssigned
}

pub struct OffensiveStats {
    pub var damage: Int
    pub var attackSpeed: Int
    pub var criticalRate: Int
    pub var criticalDamage: Int
}

pub struct DefensiveStats {
    pub var maxHealth: Int
    pub var defense: Int
    pub var healthRegeneration: Int
    pub var resistances: [Int]
}

pub struct SpecialStats {
    pub var maxEnergy: Int
    pub var energyRegeneration: Int
    pub var maxMana: Int
    pub var manaRegeneration: Int
}

pub struct StatPointsAssigned {
    pub var constitution: Int
    pub var strength: Int
    pub var dexterity: Int
    pub var intelligence: Int
    pub var stamina: Int
    pub var agility: Int
    pub var remainingPoints: Int
}
```

#### Character Progression

**Stat Allocation**:
- Constitution: Increases max health and health regeneration
- Strength: Increases physical damage
- Dexterity: Increases attack speed and critical rate
- Intelligence: Increases magical damage and mana pool
- Stamina: Increases energy pool and regeneration
- Agility: Increases movement speed and dodge chance

**Level-Up System**:
- Players earn experience through quest completion and combat
- Each level grants stat points for allocation
- No level cap, encouraging long-term progression

## Technical Stack

### Frontend
- **Unity 2022.3 LTS**: Game engine
- **C#**: Game logic and client-side scripting
- **Unity State Machine**: Player and enemy AI architecture

### Backend
- **Node.js + Express**: Relayer service for transaction signing
- **PostgreSQL**: Quest and spell data persistence
- **FCL (Flow Client Library)**: Blockchain interaction layer

### Blockchain
- **Flow Blockchain**: High-performance blockchain for NFTs and smart contracts
- **Cadence**: Resource-oriented smart contract language
- **Flow Transaction Scheduler**: Automated on-chain cron jobs
- **Random Picker Contract**: Verifiable random function for fair quest generation

### Storage & Identity
- **Lighthouse SDK**: Encrypted data storage and access control
- **IPFS**: Decentralized file storage
- **ENS (Ethereum Name Service)**: Human-readable addresses for NFTs and players

### Development Tools
- **Thirdweb**: Wallet connection and authentication
- **dotenv**: Environment variable management
- **elliptic**: ECDSA signature generation for Flow transactions

## Smart Contract Integration

### Contract Addresses (Flow Testnet)

```
ItemManager: 0x0095f13a82f1a835
MarketPlace2: 0x0095f13a82f1a835
AuctionHouse: 0x0095f13a82f1a835
QuestManager: 0x0095f13a82f1a835
HeroNFT: 0x0095f13a82f1a835
FlowTransactionScheduler: 0x11a3131ddbaa0917
RandomPicker: 0x2f52190177ec174b
NonFungibleToken: 0x631e88ae7f1d7c20
FungibleToken: 0x9a0766d93b6608b7
FlowToken: 0x7e60df042a9c0868
```

### Transaction Signing

The relayer service signs transactions on behalf of the game server using ECDSA P-256:

```javascript
const signWithKey = async (privateKeyHex, msgHex) => {
    // Hash with SHA3-256
    const sha = new SHA3(256);
    sha.update(Buffer.from(msgHex, "hex"));
    const msgHash = sha.digest();
    
    // Load private key and sign
    const key = ec.keyFromPrivate(Buffer.from(privateKeyHex, "hex"));
    const sig = key.sign(msgHash);
    
    // Return 64-byte signature (r + s)
    const r = sig.r.toArrayLike(Buffer, "be", 32);
    const s = sig.s.toArrayLike(Buffer, "be", 32);
    return Buffer.concat([r, s]).toString("hex");
};
```

### Authorization Function

```javascript
const authz = async (account) => {
    return {
        ...account,
        tempId: `${ACCOUNT_ADDRESS}-${KEY_ID}`,
        addr: fcl.withPrefix(ACCOUNT_ADDRESS),
        keyId: KEY_ID,
        signingFunction: async ({ message }) => ({
            addr: fcl.withPrefix(ACCOUNT_ADDRESS),
            keyId: KEY_ID,
            signature: await signWithKey(PRIVATE_KEY, message),
        }),
    };
};
```

## Game Economy

### Economic Loop

```mermaid
graph TB
    A[Player Completes Quest] --> B{Reward Type}
    B -->|NFT Item| C[Equip or Trade]
    B -->|Arcane Tokens| D[Create Spells]
    
    C --> E[List on Marketplace]
    E --> F[Other Players Purchase]
    F --> G[Seller Receives Flow Tokens]
    
    D --> H[Spell Becomes Available]
    H --> I[Players Borrow Spell]
    I --> J[Creator Earns Royalties]
    
    G --> K[Platform Takes 2.5% Fee]
    J --> K
    K --> L[Funds Development]
    
    L --> M[New Content Released]
    M --> N[Multiplayer Features]
    N --> O[Increased Player Engagement]
    O --> A
```

### Revenue Model

**1. Marketplace Transaction Fees**
- 2.5% fee on all direct sales
- 2.5% fee on auction house settlements
- Minimal impact on player economy while ensuring sustainability

**2. Spell Creation Credits**
- Players purchase credits with fiat or crypto
- Level-based caps prevent pay-to-win mechanics
- Free-to-play users earn credits through gameplay

**3. Spell Borrowing (Data Monetization)**
- Access fees paid in Flow tokens
- Original creators receive majority of fees
- Platform takes small percentage for infrastructure costs

**4. Future Revenue Streams**
- Cosmetic items (skins, emotes, particle effects)
- Battle pass system with free and premium tracks
- Guild features and tournament entry fees

### Token Economics

**Arcane Token (Fungible)**:
- In-game currency earned through quests
- Used for spell creation and upgrades
- Not tradeable for real currency (prevents RMT)

**Flow Token (External)**:
- Used for marketplace transactions
- Required for spell borrowing
- Handles gas fees for on-chain operations

**NFT Items**:
- Quest rewards and crafted gear
- Fully tradeable on marketplace
- Scarcity determined by drop rates and rarity tiers

## Installation & Setup

### Prerequisites

- Node.js v18+ and npm
- PostgreSQL 14+
- Unity 2022.3 LTS
- Flow CLI
- Flow testnet account with funded balance

### Backend Setup

1. Clone the repository:
```bash
git clone https://github.com/your-repo/arcane-chains-eternity.git
cd arcane-chains-eternity/backend
```

2. Install dependencies:
```bash
npm install
```

3. Configure environment variables:
```bash
cp .env.example .env
```

Edit `.env` with your credentials:
```
ACCOUNT_ADDRESS=0xYourFlowAddress
PRIVATE_KEY=your_private_key_hex
KEY_ID=0

USER_ACCOUNT_ADDRESS=0xUserFlowAddress
USER_PRIVATE_KEY=user_private_key_hex

DATABASE_URL=postgresql://user:password@localhost:5432/arcane_db
```

4. Initialize database:
```bash
npm run db:migrate
```

5. Start the relayer service:
```bash
npm start
```

The server will run on `http://localhost:3000`.

### Unity Client Setup

1. Open the Unity project:
```bash
cd ../unity-client
```

2. Open with Unity Hub (Unity 2022.3 LTS)

3. Configure Flow connection:
   - Navigate to `Assets/Scripts/Config`
   - Update `FlowConfig.cs` with relayer endpoint
   - Set testnet RPC URL

4. Build and run:
   - File → Build Settings
   - Select target platform
   - Build and Run


   
*Join us in revolutionizing the gaming industry!*

---

**Star this repository if you believe in the future of blockchain gaming!**

</div>
