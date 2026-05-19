// SE Day/Night Lights
// Mets [DN] dans le nom des lumieres et LCD a controler.

string LIGHT_TAG = "[DN]";    // Mets "" pour controler toutes les lumieres.
string LCD_TAG = "[DN]";      // Mets "" pour controler tous les LCD de la grille.
float DAY_THRESHOLD = 0.01f;  // Ratio solaire minimum pour le jour.
float NIGHT_INTENSITY = 0.5f; // Intensite forcee en mode nuit.
int RESCAN_EVERY_TICKS = 30;
int SWITCH_DELAY_UPDATES = 12; // 12 x Update100 = environ 20 secondes.

Color DAY_COLOR = new Color(255, 255, 255);
Color NIGHT_COLOR = new Color(255, 0, 0);
Color LCD_DAY_BACKGROUND = new Color(20, 35, 55);
Color LCD_NIGHT_BACKGROUND = new Color(70, 0, 0);

List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
List<IMyTextPanel> panels = new List<IMyTextPanel>();
List<IMyPowerProducer> solars = new List<IMyPowerProducer>();
Dictionary<long, float> savedIntensity = new Dictionary<long, float>();

bool hasAppliedState = false;
bool lastNight = false;
bool waitingForSwitch = false;
bool pendingNight = false;
int tick = 0;
int switchWaitTicks = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    ScanBlocks();
}

public void Main(string argument, UpdateType updateSource)
{
    string command = "";

    if (argument != null)
        command = argument.Trim().ToLower();

    tick++;

    if (tick >= RESCAN_EVERY_TICKS || command == "scan")
    {
        tick = 0;
        ScanBlocks();
    }

    bool detectedNight = IsNightFromSolarOutput();
    bool requestedNight = detectedNight;
    bool manualCommand = false;

    if (command == "day")
    {
        requestedNight = false;
        manualCommand = true;
    }

    if (command == "night")
    {
        requestedNight = true;
        manualCommand = true;
    }

    if (!hasAppliedState || manualCommand || command == "force")
    {
        ApplyMode(requestedNight);
        lastNight = requestedNight;
        hasAppliedState = true;
        ResetSwitchTimer();
    }
    else
    {
        UpdateTimedSwitch(detectedNight);
    }

    UpdatePanels(lastNight, detectedNight);
    DrawStatus(lastNight, detectedNight, command);
}

void ScanBlocks()
{
    lights.Clear();
    panels.Clear();
    solars.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(lights, delegate(IMyLightingBlock block)
    {
        return block.CubeGrid == Me.CubeGrid && MatchesLightTag(block);
    });

    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(panels, delegate(IMyTextPanel block)
    {
        return block.CubeGrid == Me.CubeGrid && MatchesLcdTag(block);
    });

    GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(solars, delegate(IMyPowerProducer block)
    {
        return block.CubeGrid == Me.CubeGrid &&
            block.BlockDefinition.SubtypeName.ToLower().Contains("solar");
    });

    CleanSavedIntensity();
}

bool MatchesLightTag(IMyTerminalBlock block)
{
    return LIGHT_TAG == "" || block.CustomName.Contains(LIGHT_TAG);
}

bool MatchesLcdTag(IMyTerminalBlock block)
{
    return LCD_TAG == "" || block.CustomName.Contains(LCD_TAG);
}

void CleanSavedIntensity()
{
    List<long> removedIds = new List<long>();

    foreach (KeyValuePair<long, float> pair in savedIntensity)
    {
        bool found = false;

        foreach (IMyLightingBlock light in lights)
        {
            if (light.EntityId == pair.Key)
            {
                found = true;
                break;
            }
        }

        if (!found)
            removedIds.Add(pair.Key);
    }

    foreach (long id in removedIds)
        savedIntensity.Remove(id);
}

bool IsNightFromSolarOutput()
{
    if (solars.Count == 0)
        return false;

    float current = 0f;
    float max = 0f;

    foreach (IMyPowerProducer solar in solars)
    {
        current += solar.CurrentOutput;
        max += solar.MaxOutput;
    }

    if (max <= 0f)
        return true;

    return (current / max) < DAY_THRESHOLD;
}

void ApplyMode(bool isNight)
{
    if (isNight)
    {
        ApplyNightMode();
    }
    else
    {
        ApplyDayMode();
    }
}

void UpdateTimedSwitch(bool detectedNight)
{
    if (detectedNight == lastNight)
    {
        ResetSwitchTimer();
        return;
    }

    if (!waitingForSwitch || pendingNight != detectedNight)
    {
        waitingForSwitch = true;
        pendingNight = detectedNight;
        switchWaitTicks = 1;
        return;
    }

    switchWaitTicks++;

    if (switchWaitTicks >= SWITCH_DELAY_UPDATES)
    {
        ApplyMode(detectedNight);
        lastNight = detectedNight;
        ResetSwitchTimer();
    }
}

void ResetSwitchTimer()
{
    waitingForSwitch = false;
    pendingNight = false;
    switchWaitTicks = 0;
}

void ApplyNightMode()
{
    foreach (IMyLightingBlock light in lights)
    {
        if (!savedIntensity.ContainsKey(light.EntityId))
            savedIntensity.Add(light.EntityId, light.Intensity);

        light.Enabled = true;
        light.Color = NIGHT_COLOR;
        light.Intensity = NIGHT_INTENSITY;
    }

    UpdatePanels(true, true);
}

void ApplyDayMode()
{
    foreach (IMyLightingBlock light in lights)
    {
        float intensity = light.Intensity;

        if (savedIntensity.ContainsKey(light.EntityId))
            intensity = savedIntensity[light.EntityId];

        light.Enabled = true;
        light.Color = DAY_COLOR;
        light.Intensity = intensity;
    }

    savedIntensity.Clear();
    UpdatePanels(false, false);
}

void UpdatePanels(bool isNight, bool detectedNight)
{
    Color background = LCD_DAY_BACKGROUND;
    string stateText = "JOUR";
    string lightText = "Blanc / intensite restauree";

    if (isNight)
    {
        background = LCD_NIGHT_BACKGROUND;
        stateText = "NUIT";
        lightText = "Rouge / intensite 0.5";
    }

    string detectedText = "JOUR";

    if (detectedNight)
        detectedText = "NUIT";

    string timerText = "Stable";

    if (waitingForSwitch)
        timerText = switchWaitTicks.ToString() + " / " + SWITCH_DELAY_UPDATES.ToString();

    float current = 0f;
    float max = 0f;
    float ratio = GetSolarRatio(out current, out max);
    string solarPercent = (ratio * 100f).ToString("0.0") + "%";

    string text = "";
    text += "=== DAY / NIGHT SYSTEM ===\n";
    text += "\n";
    text += "Etat        : " + stateText + "\n";
    text += "Detection   : " + detectedText + "\n";
    text += "Timer       : " + timerText + "\n";
    text += "Lumieres    : " + lightText + "\n";
    text += "Solaire     : " + solarPercent + "\n";
    text += "Production  : " + current.ToString("0.00") + " / " + max.ToString("0.00") + " MW\n";
    text += "\n";
    text += "Blocs trouves\n";
    text += "Lumieres    : " + lights.Count.ToString() + "\n";
    text += "LCD         : " + panels.Count.ToString() + "\n";
    text += "Solaires    : " + solars.Count.ToString() + "\n";

    foreach (IMyTextPanel panel in panels)
    {
        panel.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        panel.BackgroundColor = background;
        panel.FontColor = Color.White;
        panel.FontSize = 0.85f;
        panel.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
        panel.WriteText(text);
    }
}

float GetSolarRatio(out float current, out float max)
{
    current = 0f;
    max = 0f;

    foreach (IMyPowerProducer solar in solars)
    {
        current += solar.CurrentOutput;
        max += solar.MaxOutput;
    }

    if (max <= 0f)
        return 0f;

    return current / max;
}

void DrawStatus(bool isNight, bool detectedNight, string command)
{
    string stateText = "JOUR - Blanc - intensite restauree";
    string detectedText = "JOUR";

    if (isNight)
        stateText = "NUIT - Rouge - intensite 0.5";

    if (detectedNight)
        detectedText = "NUIT";

    Echo("=== SE Day/Night Lights ===");
    Echo("Etat: " + stateText);
    Echo("Detection solaire: " + detectedText);
    Echo("Timer: " + switchWaitTicks.ToString() + " / " + SWITCH_DELAY_UPDATES.ToString());
    Echo("Lumieres: " + lights.Count.ToString());
    Echo("Panneaux LCD: " + panels.Count.ToString());
    Echo("Panneaux solaires: " + solars.Count.ToString());
    Echo("Intensites memorisees: " + savedIntensity.Count.ToString());
    Echo("");
    Echo("Commandes: scan / force / day / night");

    if (command != "")
        Echo("Derniere commande: " + command);
}
