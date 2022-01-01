using System;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Collections.Generic;

namespace CountyCallouts.Callouts
{
    [CalloutInfo("Missing Person", CalloutProbability.Low)]
    public class MissingPerson : Callout
    {
        //Private References
        private Vector3 HikerSpawn;
        private Vector3 VehicleSpawn;
        private Vector3 FamilySpawn;
        private float HikerHeading;
        private string HikerName;
        private string HikerAge;
        private Ped Hiker;
        private Ped Relative;
        private Vehicle PickupCar;
        private Blip SearchArea;
        private Blip RelativeBlip;

        private bool TalkToFamily = false;
        private bool OnScene = false;
        private bool FinalInstructions = false;
        private bool HikerFound = false;


        public override bool OnBeforeCalloutDisplayed()
        {
            //Generate Hiker Spawns
            var randomspawn = new Random();
            var spawnlist = new List<string> { "1", "2", "3", "4" };
            int index = randomspawn.Next(spawnlist.Count);

            if (spawnlist[index] == "1")
            {
                HikerSpawn = new Vector3(-1546.16f, 4437.69f, 10.77f);
                HikerHeading = 172.44f;
            }

            if (spawnlist[index] == "2")
            {
                HikerSpawn = new Vector3(-1354.74f, 4211.31f, 18.34f);
                HikerHeading = 337.49f;
            }

            if (spawnlist[index] == "3")
            {
                HikerSpawn = new Vector3(-1018.05f, 4405.71f, 14.91f);
                HikerHeading = 207.11f;
            }

            if (spawnlist[index] == "4")
            {
                HikerSpawn = new Vector3(-982.51f, 4556.74f, 129.46f);
                HikerHeading = 163.81f;
            }

            //Create Hiker
            Hiker = new Ped(HikerSpawn, HikerHeading);

            //Make Hiker Persistent
            Hiker.MakePersistent();

            //Set Family Member Spawns
            VehicleSpawn = new Vector3(-1569.34f, 4492.03f, 21.44f);
            FamilySpawn = new Vector3(-1569.61f, 4495.72f, 21.54f);

            //Create Family Member Vehicle
            PickupCar = new Vehicle("PATRIOT", VehicleSpawn, 283.11f);
            PickupCar.MakePersistent();

            //Create Family Member
            Relative = new Ped(FamilySpawn, 320.45f);
            Relative.MakePersistent();

            //Create the callout message
            CalloutMessage = "Missing Hiker";

            //Set the callout position
            CalloutPosition = HikerSpawn;

            this.ShowCalloutAreaBlipBeforeAccepting(HikerSpawn, 30f);

            //LCPDFR.com Friendly Name
            FriendlyName = "missing hiker";

            //Last Line
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Create RelativeBlip
            RelativeBlip = Relative.AttachBlip();
            RelativeBlip.Sprite = BlipSprite.Friend;
            RelativeBlip.Color = System.Drawing.Color.Yellow;

            //Draw Route
            RelativeBlip.EnableRoute(System.Drawing.Color.Orange);

            //Display Help Message
            Game.DisplayHelp("Please make your way to the family member of the hiker to gather information.");

            //Make Hiker Wonder
            Hiker.Tasks.Wander();

            //Last Line
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            //First Line
            base.OnCalloutNotAccepted();

            //Delete everything if it exist
            if (Hiker.Exists())
            {
                Hiker.Dismiss();
            }

            if (Relative.Exists())
            {
                Relative.Dismiss();
            }

            if (PickupCar.Exists())
            {
                PickupCar.Dismiss();
            }

            if (SearchArea.Exists())
            {
                SearchArea.Delete();
            }

            if (RelativeBlip.Exists())
            {
                RelativeBlip.Delete();
            }
        }

        public override void Process()
        {
            //First Line
            base.Process();

            //Get Hiker Name
            HikerName = Functions.GetPersonaForPed(Hiker).Forename.ToString();
            HikerAge = Functions.GetPersonaForPed(Hiker).ModelAge.ToString();

            if (!OnScene & Game.LocalPlayer.Character.Position.DistanceTo(Relative) <= 15f)
            {
                //Tell player to talk to the family member
                Game.DisplayHelp("Go talk to the family member of the hiker. Press ~b~Y~w~ to talk.");

                //Disable Route
                RelativeBlip.DisableRoute();

                //Set OnScene to True
                OnScene = true;
            }

            if (Game.IsKeyDown(System.Windows.Forms.Keys.Y) & OnScene & !TalkToFamily)
            {
                //You
                Game.DisplaySubtitle("~b~You: ~w~We are here to look for your missing family member. Can you give me some information?");

                //Wait
                GameFiber.Sleep(6000);

                //Family Member
                Game.DisplaySubtitle("Their name is " + HikerName + ". I dropped them off earlier and was to meet them in 3 hours. When I came to pick them up, they were not here. It has been six hours now.");

                //Wait
                GameFiber.Sleep(6000);

                //Set TalktoFamily to true
                TalkToFamily = true;
            }

            if (TalkToFamily & !FinalInstructions)
            {
                //Create Search Area Blip
                SearchArea = new Blip(Hiker.Position, 50f);
                SearchArea.Alpha = 0.5f;
                SearchArea.Color = System.Drawing.Color.Yellow;

                //Give Instructions
                Game.DisplayHelp("Search the last known area of the hiker");

                //Set FinalInstructions to true
                FinalInstructions = true;
            }

            if (!HikerFound & Game.LocalPlayer.Character.Position.DistanceTo(Hiker) <= 10f)
            {
                HikerFound = true;
            }

            if (HikerFound)
            {
                GameFiber.Sleep(15000);
                this.End();
            }
        }

        public override void End()
        {
            //First Line
            base.End();

            //Delete everything if it exist
            if (Hiker.Exists())
            {
                Hiker.Dismiss();
            }

            if (Relative.Exists())
            {
                Relative.Dismiss();
            }

            if (PickupCar.Exists())
            {
                PickupCar.Dismiss();
            }

            if (SearchArea.Exists())
            {
                SearchArea.Delete();
            }

            if (RelativeBlip.Exists())
            {
                RelativeBlip.Delete();
            }
        }
    }
}
