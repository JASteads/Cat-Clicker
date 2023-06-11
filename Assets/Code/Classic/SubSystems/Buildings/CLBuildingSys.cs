using System.Collections.Generic;
using UnityEngine;

public class CLBuildingSys : CLSubSys
{
    public List<CLBuilding> database;


    public CLBuildingSys(CLSystem sys, Transform parent)
        : base(sys, parent)
    {

    }

    protected override void Init(CLSystem sys, Transform parent)
    {
        database = new List<CLBuilding>()
        {
            // Primary Characters
            new CLBuilding(
                "Cello", "Your personal bit cat companion! Train" +
                " him to be active, and he'll produce more bits.",
                0.1f, 15),
            new CLBuilding(
                "Dig-Dig", "The robot cat drill finds bits in" +
                " the ground. It ain't no trick to get rich quick!",
                1, 85),
            new CLBuilding(
                "Smithy", "Smithy likes to take raw bits and turn" +
                "them into useful tools. For making more bits" +
                " of course!", 6, 700),
            new CLBuilding(
                "Magic Orb", "The orb somehow creates bits out of" +
                " thin air! There seems to be a mysterious cat" +
                " inside the orb...", 20, 5000),

            // Bit Cats
            new CLBuilding(
                "Work Cat", "", 100, 25000),
            new CLBuilding(
                "Engy Cat", "", 500, 200000),
            new CLBuilding(
                "Magic Cat", "", 1500, 420690),
            new CLBuilding(
                "Science Cat", "", 3400, 1431100),

            // Bases
            new CLBuilding(
                "Mine", "", 10000, 5000000),
            new CLBuilding(
                "Factory", "", 25000, 20000000),
            new CLBuilding(
                "Tower", "", 500000, 100000000),
            new CLBuilding(
                "Laboratory", "", 1000000, 500000000)
        };

        panel = new CLBuildingPanel(sys, parent);

        onCalcCP = (cp) =>
        {
            database.ForEach(b =>
            { cp += b.GetCPModifier(); });
        };
    }
}
