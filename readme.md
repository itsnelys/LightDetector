# Space Engineers Day/Night Light Detector

## FR

Script de Programmable Block pour *Space Engineers* qui detecte le jour et la nuit via la production des panneaux solaires, puis adapte automatiquement les lumieres taguees.

Les lumieres et LCD controles doivent contenir le tag `[DN]` dans leur nom, sauf si les reglages sont modifies pour tout controler.

### Installation

1. Place un `Programmable Block`.
2. Copie `Script_DayNightLights.cs` dans le Programmable Block.
3. Ajoute au moins un panneau solaire sur la meme grille.
4. Ajoute `[DN]` dans le nom des lumieres a controler.
5. Ajoute `[DN]` dans le nom des LCD de statut optionnels.
6. Compile et lance le script.

### Blocs requis

| Bloc | Nom / Regle | Description |
| --- | --- | --- |
| Programmable Block | N'importe quel nom | Execute le script. |
| Panneau solaire | Meme grille | Sert a detecter le jour ou la nuit. |
| Lumiere | Contient `[DN]` | Lumiere controlee automatiquement. |
| LCD | Contient `[DN]` | Affichage optionnel de l'etat. |

Exemples : `Hangar Light [DN]`, `Base Spotlight [DN]`, `LCD Day Night [DN]`.

### Reglages

| Reglage | Valeur par defaut | Description |
| --- | --- | --- |
| `LIGHT_TAG` | `[DN]` | Tag cherche dans les noms des lumieres. |
| `LCD_TAG` | `[DN]` | Tag cherche dans les noms des LCD. |
| `DAY_THRESHOLD` | `0.01f` | Ratio solaire minimum pour le mode jour. |
| `NIGHT_INTENSITY` | `0.5f` | Intensite forcee en mode nuit. |
| `RESCAN_EVERY_TICKS` | `30` | Frequence de recherche des blocs. |
| `SWITCH_DELAY_UPDATES` | `12` | Delai avant changement de mode. |

### Comportement

| Mode | Couleur | Intensite | Etat |
| --- | --- | --- | --- |
| Jour | Blanc | Intensite restauree | Active |
| Nuit | Rouge | `0.5` | Active |

Avant de passer en mode nuit, le script memorise l'intensite des lumieres. Au retour du jour, il restaure ces valeurs.

### Commandes

| Commande | Action |
| --- | --- |
| `scan` | Relance la detection des lumieres, LCD et panneaux solaires. |
| `force` | Reapplique immediatement le mode detecte. |
| `day` | Force le mode jour pour test. |
| `night` | Force le mode nuit pour test. |

### Depannage

| Probleme | Solution |
| --- | --- |
| Aucune lumiere ne reagit | Ajoute `[DN]` au nom des lumieres, puis lance `scan`. |
| Le LCD ne s'actualise pas | Ajoute `[DN]` au nom du LCD, puis lance `scan`. |
| Le script reste en jour | Ajoute un panneau solaire sur la meme grille ou teste avec `night`. |
| Les ombres declenchent la nuit trop vite | Augmente `SWITCH_DELAY_UPDATES`. |

### Note de patch 0.1

- Premiere version documentee.
- Detection jour/nuit par ratio de production solaire.
- Controle automatique des lumieres taguees `[DN]`.
- LCD de statut et commandes manuelles de test.

---

## EN

*Space Engineers* Programmable Block script that detects day and night from solar panel output, then automatically updates tagged lights.

Controlled lights and LCDs must contain the `[DN]` tag in their name unless the settings are changed to control every matching block.

### Installation

1. Place a `Programmable Block`.
2. Copy `Script_DayNightLights.cs` into the Programmable Block.
3. Add at least one solar panel on the same grid.
4. Add `[DN]` to the name of lights you want to control.
5. Add `[DN]` to optional status LCD names.
6. Compile and run the script.

### Required Blocks

| Block | Name / Rule | Description |
| --- | --- | --- |
| Programmable Block | Any name | Runs the script. |
| Solar panel | Same grid | Used to detect day or night. |
| Light | Contains `[DN]` | Automatically controlled light. |
| LCD | Contains `[DN]` | Optional status display. |

Examples: `Hangar Light [DN]`, `Base Spotlight [DN]`, `LCD Day Night [DN]`.

### Settings

| Setting | Default Value | Description |
| --- | --- | --- |
| `LIGHT_TAG` | `[DN]` | Tag searched in light names. |
| `LCD_TAG` | `[DN]` | Tag searched in LCD names. |
| `DAY_THRESHOLD` | `0.01f` | Minimum solar ratio for day mode. |
| `NIGHT_INTENSITY` | `0.5f` | Forced night mode intensity. |
| `RESCAN_EVERY_TICKS` | `30` | Block rescan interval. |
| `SWITCH_DELAY_UPDATES` | `12` | Delay before switching mode. |

### Behavior

| Mode | Color | Intensity | State |
| --- | --- | --- | --- |
| Day | White | Restored intensity | Enabled |
| Night | Red | `0.5` | Enabled |

Before switching to night mode, the script stores each light intensity. When day returns, it restores those values.

### Commands

| Command | Action |
| --- | --- |
| `scan` | Rescans lights, LCDs, and solar panels. |
| `force` | Immediately reapplies the detected mode. |
| `day` | Forces day mode for testing. |
| `night` | Forces night mode for testing. |

### Troubleshooting

| Problem | Fix |
| --- | --- |
| No lights react | Add `[DN]` to light names, then run `scan`. |
| LCD is not updated | Add `[DN]` to the LCD name, then run `scan`. |
| Script stays in day mode | Add a solar panel on the same grid or test with `night`. |
| Shadows trigger night too quickly | Increase `SWITCH_DELAY_UPDATES`. |

### Patch Note 0.1

- First documented release.
- Day/night detection from solar production ratio.
- Automatic control for `[DN]` tagged lights.
- Status LCD and manual test commands.

---

## License & Author

Provided by **soraanoir/sundae https://github.com/soraanoir**.  
Free to use and modify, provided that credit is given to the author.
