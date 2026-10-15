# 🪙 COIN COLLECTOR — JEU VIDÉO 2D/2.5D SOUS UNITY & C#

Bienvenue dans le projet complet **COIN COLLECTOR**, développé sous **Unity 6 (6000.3.24f1)** et **C#**, directement issu du design Figma original et enrichi avec de véritables **assets graphiques 2D professionnels**.

Ce projet répond rigoureusement à l'ensemble des critères d'évaluation et de validation académique du module :
1. ✅ **Menu principal complet avec consultation des instructions & règles**.
2. ✅ **3 Niveaux jouables et progressifs** (Green Valley, Crystal Cave, Sky Temple).
3. ✅ **Interfaces complètes de Victoire (Win) et de Défaite (Game Over)**.
4. ✅ **Véritables assets 2D cartoon professionnels intégrés** (aucun cube/sphère/capsule primitive).
5. ✅ **5 skins de personnage sélectionnables** (Classic, Aventurier, Ninja, Mage, Roi).
6. ✅ **Animations 2D dynamiques** (cycle de course, saut, dégât, rotation 4-frames des pièces, rebond gélatineux des slimes, vol des chauves-souris).
7. ✅ **Gameplay réactif et responsive** (commandes PC au clavier + boutons tactiles Mobile à l'écran).
8. ✅ **Build PC Windows autonome 64-bit compilé et testé** (`Builds/Windows/CoinCollector.exe`).

---

## 🎨 1. Bibliothèque d'Assets Graphiques 2D Intégrés (`Assets/Sprites/`)

Tous les visuels provisoires ont été remplacés par une banque d'assets 2D de haute résolution au style cartoon cohérent :

### A. Personnages & Skins (`Assets/Sprites/Characters/`)
- `Player_Classic_Idle.png` : Héros bleu en posture d'attente (avec respiration subtile).
- `Player_Classic_Run1.png` & `Run2.png` : Cycle de course fluide à 2 phases avec orientation dynamique (`flipX`).
- `Player_Classic_Jump.png` : Pose de saut aérienne dynamique.
- `Player_Classic_Hurt.png` : Expression d'impact lors de la perte d'un cœur.
- **4 Skins alternatifs déblocables / équipables** :
  - `Player_Adventurer_Idle.png` : Aventurier avec chapeau d'explorateur, sacoche et boussole.
  - `Player_Ninja_Idle.png` : Ninja Shinobi furtif avec bandeau rouge et tenue d'ombre.
  - `Player_Mage_Idle.png` : Mage arcaniste avec chapeau pointu et gemmes mystiques.
  - `Player_King_Idle.png` : Roi des pièces avec couronne dorée et cape d'or.

### B. Pièces & Collectibles (`Assets/Sprites/Items/`)
- `Coin_Frame0.png`, `Coin_Frame1.png`, `Coin_Frame2.png`, `Coin_Frame3.png` : Cycle complet de rotation 360° en 4 frames, avec reflet métallique étincelant.
- `Coin_Idle.png` : Pièce d'or statique haute définition pour les interfaces.

### C. Ennemis & Monstres (`Assets/Sprites/Enemies/`)
- `Slime_Green_Idle.png` & `Slime_Green_Squash.png` : Slime gélatineux vert avec déformation d'écrasement/étirement.
- `Slime_Purple_Idle.png` : Slime toxique violet des grottes de cristal.
- `Slime_Fire_Idle.png` : Slime ardent flamboyant du temple céleste.
- `Bat_Fly1.png` & `Bat_Fly2.png` : Chauve-souris violette avec battement d'ailes et trajectoire sinusoïdale en l'air.

### D. Environnements & Plateformes (`Assets/Sprites/Environment/`)
- `Platform_Grass.png` : Plateforme herbeuse avec liseré végétal et terre texturée.
- `Platform_Wood.png` : Plateforme en madriers de bois assemblés.
- `Platform_Crystal.png` : Plateforme rocheuse ornée d'améthystes néon scintillantes.
- `Spikes_Hazard.png` : Rangée de pointes métalliques et cristallines acérées.
- `Cloud_Fluffy.png` : Nuages volumétriques flottants avec transparence douce.
- `Exit_Flag.png` : Mât d'arrivée à damier de course avec étoile d'or.
- `Exit_Portal.png` : Vortex mystique cosmique à runes violettes lumineuses.
- `Exit_GoldenGate.png` : Arche sacrée dorée ornée de rubis divins.

### E. Éléments d'Interface (`Assets/Sprites/UI/`)
- `UI_Heart_Full.png` & `UI_Heart_Empty.png` : Cœurs de santé éclatants.
- `UI_Star_Gold.png` & `UI_Star_Empty.png` : Étoiles de notation (1 à 3 étoiles).
- `UI_Coin_Icon.png` : Badge doré pour le compteur de pièces.

---

## 🎮 2. Structure des Menus et Scènes

### A. Écran d'Accueil / Menu Principal (`MainMenu.unity`)
- **Fidélité au design Figma** :
  - Fond bleu nuit (`#100E2C`) avec ciel étoilé et collines au premier plan.
  - Personnage cartoon 2D au centre animé en respiration et flottement doux.
  - Pièces dorées en rotation 4-frames en arrière-plan.
  - Titre **COIN COLLECTOR** en lettrage doré et blanc relief.
  - Bouton **🎮 JOUER** : Lance immédiatement le Niveau 1.
  - Bouton **🗺️ NIVEAUX** : Ouvre l'écran de sélection des 3 mondes.
  - Bouton **🧍 PERSO** : Ouvre le vestiaire pour équiper instantanément les 5 skins !
  - Bouton **⚙️ OPTIONS** : Réglage du volume sonore SFX, Musique et réinitialisation.
  - Bouton **📖 RÈGLES & COMMANDES** : Ouvre le modal complet d'instructions.
  - Version en bas d'écran : `v2.0 — ÉDITION 2D PROFESSIONNELLE`.

### B. Écran de Sélection des Niveaux (`LevelSelect.unity`)
- 3 Cartes illustrées pour chaque monde :
  - **Niveau 1 : Green Valley** (Facile) — Débloqué dès le départ.
  - **Niveau 2 : Crystal Cave** (Moyen) — Débloqué en terminant le Niveau 1.
  - **Niveau 3 : Sky Temple** (Difficile) — Débloqué en terminant le Niveau 2.
- Sauvegarde de la progression : Meilleur score, nombre de pièces et étoiles ⭐⭐⭐ obtenues.
- Bouton `← RETOUR` vers l'accueil.

### C. 3 Niveaux Jouables et Progressifs
1. **Niveau 1 — Green Valley (`Level_01_GreenValley.unity`)** :
   - Ciel bleu éclatant, nuages flottants, collines et plateformes d'herbe.
   - 30 Pièces dorées réparties sur le parcours.
   - Slimes verts au sol (sauter sur leur tête pour les éliminer avec +200 pts !).
   - Drapeau à damier de sortie.
2. **Niveau 2 — Crystal Cave (`Level_02_CrystalCave.unity`)** :
   - Atmosphère de grotte sombre violette avec plateformes en cristal d'améthyste.
   - 35 Pièces dorées, fosses de pics acérés (`Spikes_Hazard`).
   - Slimes violets au sol et chauves-souris en vol ondulant.
   - Portail cosmique magique de sortie.
3. **Niveau 3 — Sky Temple (`Level_03_SkyTemple.unity`)** :
   - Ciel crépusculaire doré, îles célestes suspendues au-dessus du vide.
   - 40 Pièces dorées, sauts de précision millimétrés.
   - Slimes de feu ardents et essaim de chauves-souris.
   - Arche royale dorée de fin d'aventure.

### D. Interfaces de Victoire et de Défaite
- **Victoire (`LevelCompleteUI.cs`)** :
  - Animation séquentielle des 1, 2 ou 3 étoiles ⭐⭐⭐ calculées selon les pièces collectées (≥50% = 2★, ≥85% = 3★).
  - Bilan complet : Pièces / Total, Score accumulé, Chronomètre précis.
  - Boutons : **NIVEAU SUIVANT →**, **🔄 REJOUER**, **🏠 MENU**.
- **Game Over (`GameOverUI.cs`)** :
  - Déclenchée si les 3 cœurs sont perdus.
  - Affichage des pièces et du score avant la défaite.
  - Boutons : **🔄 RÉESSAYER** (relance immédiate), **🏠 MENU PRINCIPAL**.

---

## 🕹️ 3. Commandes de Jeu Responsive

| Action | Clavier PC | Mobile / Tactile |
|---|---|---|
| **Aller à gauche** | `Flèche Gauche` ou `Q` / `A` | Bouton virtuel `←` |
| **Aller à droite** | `Flèche Droite` ou `D` | Bouton virtuel `→` |
| **Sauter** | `Espace` ou `Flèche Haut` / `Z` | Bouton tactile `↑ SAUT` |
| **Pause** | `Échap` | Bouton `⏸️` en haut à droite |
| **Instructions** | Bouton `📖` | Bouton `📖` |

---

## 🚀 4. Comment Lancer et Tester le Jeu

### Option 1 : Lancer directement le Build Windows (Idéal pour la démo)
Le jeu est compilé et prêt à l'emploi :
1. Rendez-vous dans :
   ```
   C:\Users\lenovo\Downloads\Gold rush\Builds\Windows\
   ```
2. Double-cliquez sur :
   ```
   CoinCollector.exe
   ```
3. Le jeu se lance immédiatement en 60 FPS plein écran avec tous les sprites 2D, sons, animations et niveaux !

### Option 2 : Ouvrir dans l'éditeur Unity
1. Ouvrez **Unity Hub** et ouvrez le projet :
   ```
   C:\Users\lenovo\Downloads\Gold rush
   ```
2. Chargez la scène :
   ```
   Assets/Scenes/MainMenu.unity
   ```
3. Cliquez sur le bouton **Play ▶**.

---

## 🛠️ 5. Régénération Automatique (Master Builder)

Si besoin de regénérer l'ensemble des scènes ou le binaire Windows depuis Unity :
- **Gold Rush > 🚀 GÉNÉRER LE PROJET COMPLET (Tous les Niveaux & Menus 2D)**
- Le script `Assets/Editor/GoldRushProjectMasterBuilder.cs` reconstruit toutes les scènes et configure automatiquement les sprites 2D.

---

## 📁 6. Architecture du Code C#

```
Assets/
├── Editor/
│   └── GoldRushProjectMasterBuilder.cs   # Script d'automatisation des scènes & du Build
├── Scenes/
│   ├── MainMenu.unity                     # Menu principal (Figma style)
│   ├── LevelSelect.unity                  # Sélection des 3 mondes
│   ├── Level_01_GreenValley.unity         # Niveau 1 (Prairie)
│   ├── Level_02_CrystalCave.unity         # Niveau 2 (Grotte de cristal)
│   └── Level_03_SkyTemple.unity           # Niveau 3 (Temple céleste)
├── Scripts/
│   ├── Gameplay/
│   │   ├── BatSpriteAnimator.cs           # Animation battement d'ailes et vol ondulant
│   │   ├── CameraFollow25D.cs             # Caméra de suivi 2.5D fluide
│   │   ├── Coin.cs                        # Collecte, audio et score
│   │   ├── CoinSpriteAnimator.cs          # Animation rotation 4-frames des pièces
│   │   ├── GameManager.cs                 # Sauvegarde, PlayerPrefs et progression
│   │   ├── LevelExit.cs                   # Déclenchement de la fin de niveau
│   │   ├── SimpleEnemy.cs                 # IA de patrouille et mécanique d'écrasement
│   │   ├── SlimeSpriteAnimator.cs         # Rebond gélatineux cartoon des slimes
│   │   ├── SoundManager.cs                # Synthèse audio & bruitages
│   │   └── SpikeHazard.cs                 # Pièges mortels de pics
│   ├── Player/
│   │   ├── CharacterSpriteAnimator.cs     # Animateur 2D du joueur (Idle, Run, Jump, Hurt, Skins)
│   │   ├── PlayerController25D.cs         # Contrôleur physique 2D/2.5D
│   │   └── PlayerHealth.cs                # Gestion des 3 cœurs et invincibilité
│   └── UI/
│       ├── CharacterCustomizationMenu.cs  # Sélection des 5 skins de personnage
│       ├── FloatingCharacter.cs           # Animation de flottement du menu
│       ├── GameOverUI.cs                  # Interface de Défaite
│       ├── HUDManager.cs                  # HUD en jeu (Pièces, Score, Cœurs, Progrès)
│       ├── InstructionsMenu.cs            # Fenêtre des règles et commandes
│       ├── LevelCompleteUI.cs             # Interface de Victoire & étoiles
│       ├── LevelSelectMenu.cs             # Gestionnaire des cartes de niveaux
│       ├── MainMenuManager.cs             # Contrôleur de l'écran d'accueil
│       ├── MobileTouchControls.cs         # Boutons tactiles virtuels
│       └── OptionsSettingsMenu.cs         # Panneau de réglages sonores
└── Sprites/
    ├── Characters/                        # Sprites du joueur et des 5 skins
    ├── Enemies/                           # Sprites slimes verts/violets/feu et chauves-souris
    ├── Environment/                       # Plateformes herbe/bois/cristal, nuages, pics, sorties
    ├── Items/                             # Frames de rotation de la pièce dorée
    └── UI/                                # Cœurs, étoiles et icônes d'interface
```
