# 🪙 GOLD RUSH — Guide de Création & Configuration Unity

Ce guide vous explique pas à pas comment configurer la **1ère page (Menu Principal)** et l'**écran de jeu (Gameplay HUD 2.5D)** dans Unity.

---

## 📁 1. Structure des Scripts Créés dans votre Projet

Tous les scripts C# prêts à l'emploi ont été générés dans `Assets/Scripts/` :

- **`Assets/Scripts/UI/FloatingCharacter.cs`** : Fait flotter / respirer doucement le personnage sur l'écran d'accueil de haut en bas (~2 mm d'amplitude).
- **`Assets/Scripts/Gameplay/CoinSpinner.cs`** : Fait tourner les pièces sur elles-mêmes avec un axe 3D et un léger flottement.
- **`Assets/Scripts/UI/MainMenuManager.cs`** : Gère les clics sur **JOUER**, **NIVEAUX**, **PERSO**, **OPTIONS**.
- **`Assets/Scripts/Player/PlayerController25D.cs`** : Contrôleur 2.5D avec déplacement gauche/droite, saut, gravité fluide et gestion des animations (Idle, Course, Saut, Chute, Victoire).
- **`Assets/Scripts/Gameplay/Coin.cs`** : Détection de collision de la pièce, gain de score, apparition du popup `+10` et effet de glow.
- **`Assets/Scripts/UI/FloatingFeedbackText.cs`** : Animation du texte flottant `+10` vers le haut.
- **`Assets/Scripts/UI/HUDManager.cs`** : Affichage en haut à gauche (Pièces : 12 / 30, Score), au centre (Niveau 1 & Barre de progression), et à droite (Pause & Paramètres).
- **`Assets/Scripts/UI/MobileTouchControls.cs`** : Boutons tactiles pour smartphone (Gauche, Droite, Gros bouton Saut).
- **`Assets/Scripts/Gameplay/CameraFollow25D.cs`** : Suivi fluide de la caméra 2.5D.
- **`Assets/Scripts/Gameplay/LevelExit.cs`** : Porte de sortie du niveau avec déclenchement de la victoire.

---

## 🎨 2. Mise en place de la 1ère Page (Menu Principal "GOLD RUSH")

### A. Décor et Caméra
1. **Couleur d'arrière-plan** : Dans la `Main Camera`, mettez le *Clear Flags* sur **Solid Color** avec un bleu nuit profond (`#100E2C`).
2. **Collines vertes en bas** :
   - Ajoutez des Sprites ou des formes 3D arrondies (ex: Sphères étirées en bas de l'écran) avec des couleurs vert gazon (`#2D8A4E` et vert clair `#44B365`).
3. **Pièces dorées flottantes dans le fond** :
   - Créez plusieurs cylindres ou sphères dorées (matériau jaune brillant métallisé).
   - Ajoutez le composant **`CoinSpinner`** sur chacune d'elles.
   - Réglez `rotationSpeed` à `100` et cochez `randomizeOffset`.

### B. Personnage au Centre
1. Placez votre modèle 3D de personnage au centre de l'écran face à la caméra.
2. Ajoutez-lui le script **`FloatingCharacter`** :
   - `Vertical Amplitude` : `0.08` à `0.15` (léger mouvement de haut en bas).
   - `Vertical Speed` : `2.5`.
   - `Enable Breathing Scale` : `true`.

### C. Interface Utilisateur (Canvas UI)
1. Créez un **Canvas** (`Render Mode: Screen Space - Overlay`, `UI Scale Mode: Scale With Screen Size` en résolution `1920 x 1080`).
2. **Titre "GOLD RUSH"** :
   - TextMeshProUGUI avec police grasse et arrondie (ex: *Fredoka One*, *Titan One*, *Lilita One* ou *Baloo*).
   - Couleur dorée/jaune vif (`#FFCC00`) avec contour sombre ou ombre portée (Shadow/Outline).
3. **Sous-titre** :
   - "3 NIVEAUX • COLLECTE • AVENTURE" en blanc majuscule avec un léger espacement.
4. **Boutons** :
   - **🎮 JOUER** : Bouton jaune orangé vif, coins arrondis.
   - **🗺️ NIVEAUX** : Bouton violet dégradé.
   - **🧍 PERSO** et **⚙️ OPTIONS** : Boutons vert sombre / translucides arrondis placés côte à côte.
   - Associez les boutons au script **`MainMenuManager`**.
5. **Pied de page** :
   - Badge "v1.0 — ÉDITION PREMIUM" en bas avec un panneau semi-transparent.

---

## 🕹️ 3. Mise en place de l'Écran de Jeu (HUD & Contrôles Tactiles)

1. **En haut à gauche** :
   - Icône de pièce dorée 🪙 + Texte `12 / 30`.
   - Texte Score `1 250`.
2. **En haut au centre** :
   - Texte `NIVEAU 1`.
   - Slider UI pour la barre de progression vers la sortie.
3. **En haut à droite** :
   - Bouton Pause `⏸️` et Bouton Paramètres `⚙️`.
4. **Contrôles tactiles (En bas)** :
   - En bas à gauche : 2 boutons ronds transparents Flèche Gauche `◀` et Flèche Droite `▶` (connectés à `MobileTouchControls`).
   - En bas à droite : 1 gros bouton rond `SAUT ⬆` (connecté à `MobileTouchControls.OnJumpClicked`).
