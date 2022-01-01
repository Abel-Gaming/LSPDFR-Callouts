using System;
using System.Drawing;
using Rage;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using System.Collections.Generic;

namespace CountyCallouts.Callouts
{
    [CalloutInfo("ATV Accident", CalloutProbability.Low)]
    public class ATVAccident1 : Callout
    {
        //References
        private Vector3 SpawnPoint;
        private Vector3 VehicleSpawn;
        private float ATVHeading;
        private float PedHeading;
        private Ped Suspect;
        private Blip SuspectBlip;
        private Vehicle ATV;

        private bool OnScene = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            //SpawnPoints
            var randomspawn = new Random();
            var spawnlist = new List<string>
            {
                "1", "2", "3"
            };
            int index = randomspawn.Next(spawnlist.Count);

            //Select Spawn
            if (spawnlist[index] == "1")
            {
                //Create ATV Spawn
                VehicleSpawn = new Vector3(2543.46f, 4728.28f, 29.44f);

                //Create Ped SpawnPoint
                SpawnPoint = new Vector3(2540.34f, 4725.32f, 29.92f);

                //Create ATV Heading
                ATVHeading = 78.65f;

                //Create Ped Heading
                PedHeading = 305.32f;
            }

            if (spawnlist[index] == "2")
            {
                //Create ATV Spawn
                VehicleSpawn = new Vector3(1862.20f, 4590.98f, 33.61f);

                //Create ATV Heading
                ATVHeading = 127.47f;

                //Create Ped Spawn
                SpawnPoint = new Vector3(1845.20f, 4589.40f, 30.68f);

                //Create Ped Heading
                PedHeading = 98.79f;
            }

            if (spawnlist[index] == "3")
            {
                //Create ATV Spawn
                VehicleSpawn = new Vector3(2067.43f, 3935.18f, 30.84f);

                //Create ATV Heading
                ATVHeading = 333.75f;

                //Create Ped Spawn
                SpawnPoint = new Vector3(2056.86f, 3930.88f, 33.08f);

                //Create Ped Heading
                PedHeading = 105.21f;
            }

            //Create ATV
            ATV = new Vehicle("BLAZER", VehicleSpawn);
            ATV.Heading = ATVHeading;
            ATV.MakePersistent();
            ATV.SetRotationRoll(180f);

            //Create Ped
            Suspect = new Ped(SpawnPoint, PedHeading);
            Suspect.BlockPermanentEvents = true;
            Suspect.MakePersistent();

            this.ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 15f);

            //Set the callout message(displayed in the notification), and the position(also shown in the notification)
            this.CalloutMessage = "ATV Accident";
            this.CalloutPosition = SpawnPoint;

            //Set the callout friendly name
            this.FriendlyName = "atv accident";

            //Play the scanner audio
            Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_03 CRIME_AMBULANCE_REQUESTED_01 IN_OR_ON_POSITION", this.SpawnPoint);

            //Last Line
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Suspect Actions
            Suspect.Tasks.PlayAnimation("amb@world_human_sunbathe@male@back@base", "base", 1f, AnimationFlags.Loop);

            //Suspect Blip
            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.IsFriendly = true;

            //Draw route
            SuspectBlip.EnableRoute(Color.Orange);

            //Last Line
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            //First Line
            base.OnCalloutNotAccepted();

            if (Suspect.Exists())
            {
                Suspect.Dismiss();
            }

            if (SuspectBlip.Exists())
            {
                SuspectBlip.Delete();
            }

            if (ATV.Exists())
            {
                ATV.Dismiss();
            }
        }

        public override void Process()
        {
            //First Line
            base.Process();

            if (!OnScene & Game.LocalPlayer.Character.Position.DistanceTo(Suspect) <= 15f)
            {
                //Disable Route
                SuspectBlip.DisableRoute();

                //Display Instructions
                Game.DisplayHelp("Perform your investigation for the incident. Press ~r~END ~w~to end the call when completed.");

                //Set onscene to true
                OnScene = true;
            }

            if (Game.IsKeyDown(System.Windows.Forms.Keys.End))
            {
                this.End();
            }
        }

        public override void End()
        {
            //First Line
            base.End();

            //Display Code 4
            Game.DisplayNotification("Call is ~g~Code 4");

            if (Suspect.Exists())
            {
                Suspect.Dismiss();
            }

            if (SuspectBlip.Exists())
            {
                SuspectBlip.Delete();
            }

            if (ATV.Exists())
            {
                ATV.Dismiss();
            }
        }
    }
}
