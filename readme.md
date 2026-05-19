# Space Engineers Day/Night Light Detector - Script Documentation

This *Space Engineers* programmable block script implements an **automatic day/night lighting system** that detects sunlight from solar panel output, switches tagged lights between day and night modes, and displays a clean status dashboard on tagged LCD panels.

---

## Table of Contents

* [Initialization and Configuration](#initialization-and-configuration)
* [Day and Night Detection Logic](#day-and-night-detection-logic)
* [Light Control Logic](#light-control-logic)
* [LCD Display Logic](#lcd-display-logic)
* [Manual Commands](#manual-commands)
* [Troubleshooting](#troubleshooting)

---

## Initialization and Configuration

The system initializes by scanning the current grid for tagged lights, tagged LCD panels, and solar panels. Lights and LCDs are controlled only when their name contains the configured tag.

### **Script Constants**
| Constant | Default Value | Description |
| --- | --- | --- |
| `LIGHT_TAG` | `[DN]` | Any light containing this text in its name is controlled by the script. |
| `LCD_TAG` | `[DN]` | Any LCD panel containing this text in its name is used as a status display. |
| `DAY_THRESHOLD` | `0.01f` | Minimum solar production ratio required to be considered daytime. |
| `NIGHT_INTENSITY` | `0.5f` | Light intensity applied immediately when night mode starts. |
| `RESCAN_EVERY_TICKS` | `30` | Automatic block rescan interval. The script runs on `Update100`. |
| `SWITCH_DELAY_UPDATES` | `12` | Delay before changing day/night mode. Prevents short shadows from triggering night mode. |

### **Required Blocks**
| Block | Required Name / Rule | Description |
| --- | --- | --- |
| Programmable Block | Any name | Runs the script automatically every `Update100`. |
| Solar Panel | Same grid | Used to detect whether it is day or night. Multiple solar panels are supported. |
| Light Block | Contains `[DN]` | Controlled by the script. Works with tagged lights on the same grid. |
| LCD Panel | Contains `[DN]` | Optional status display. Only tagged LCD panels are updated. |

* **Light naming:** Examples: `Hangar Light [DN]`, `Red Alert Lamp [DN]`, `Base Spotlight [DN]`.
* **LCD naming:** Examples: `LCD Day Night [DN]`, `Control Screen [DN]`.
* **Tag behavior:** Set `LIGHT_TAG` or `LCD_TAG` to `""` only if you want the script to control every matching block on the grid.

---

## Day and Night Detection Logic

The script uses solar panel output to determine whether the grid is in day or night mode.

### **Operating Logic**
1.  **Scan solar panels:** Finds solar power producers on the same grid.
2.  **Measure output:** Adds current solar output and maximum solar output.
3.  **Compute ratio:** Divides current output by maximum output.
4.  **Confirm change:** If the detected state changes, the script starts a delay timer.
5.  **Switch mode:** The script changes mode only if the new detected state stays stable for `SWITCH_DELAY_UPDATES`.

If no solar panel is found, the script stays in day mode by default.

This delay prevents short shadows, moving ships, or temporary solar occlusion from immediately switching the lights to night mode.

---

## Light Control Logic

Tagged lights are switched automatically between day and night states.

### **Day Mode**
| Property | Value |
| --- | --- |
| Color | White |
| Intensity | Restored to the value saved before night mode |
| Enabled | `true` |

### **Night Mode**
| Property | Value |
| --- | --- |
| Color | Red |
| Intensity | `0.5` |
| Enabled | `true` |

Before night mode changes a light intensity, the script stores its current intensity in memory. When day mode returns, the original intensity is restored, then the saved values are cleared for the next night cycle.

---

## LCD Display Logic

Tagged LCD panels provide a compact status dashboard for the lighting system.

### **Displayed Information**
* **State:** Shows whether the system is in `JOUR` or `NUIT`.
* **Detection:** Shows the current solar reading before the delay timer is applied.
* **Timer:** Shows whether the state is stable or waiting before switching.
* **Lights:** Shows the active light behavior, such as restored day intensity or red night intensity.
* **Detected Blocks:** Shows how many tagged lights and tagged LCD panels are currently found.

The script automatically configures the LCD to:
* `TEXT_AND_IMAGE` mode
* white font color
* compact font size
* left alignment
* day or night background color

---

## Manual Commands

Commands can be run from the programmable block argument field.

| Command | Description |
| --- | --- |
| `scan` | Rescans lights, LCD panels, and solar panels immediately. |
| `force` | Reapplies the current detected mode immediately. |
| `day` | Forces day mode manually. |
| `night` | Forces night mode manually. |

Manual `day` and `night` commands are useful for testing the lighting behavior without waiting for the solar output to change.

---

## Troubleshooting

| Problem | Fix |
| --- | --- |
| No lights are controlled | Rename the target lights so their name contains `[DN]`, then run `scan`. |
| LCD is not updated | Rename the LCD so its name contains `[DN]`, then run `scan`. |
| Script always stays in day mode | Add at least one solar panel on the same grid, or use the `night` command to test manually. |
| Script switches to night too early or too late | Adjust `DAY_THRESHOLD`. A higher value makes night mode start earlier. |
| Shadows trigger night mode | Increase `SWITCH_DELAY_UPDATES` so the detected state must stay stable longer. |
| Light intensity does not restore as expected | Make sure the light was detected before night mode started. Run `scan` after adding or renaming lights. |
| Too many blocks are controlled | Check `LIGHT_TAG` and `LCD_TAG`. Keep them set to `[DN]` unless you intentionally want full-grid control. |

---

## License & Author

Provided by **soraanoir/sundae https://github.com/soraanoir**.  
Free to use and modify, provided that credit is given to the author.
