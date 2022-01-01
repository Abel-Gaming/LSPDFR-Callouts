using Rage;
using LSPD_First_Response.Mod.Callouts;


namespace CountyCallouts.Callouts
{
    [CalloutInfo("ATV Accident", CalloutProbability.Low)]
    public class ATVAccident2 : Callout
    {
        //Private References
        private Vector3 ATVSpawn;
        private Vector3 PedSpawn;
        private Ped Ped;
        private Vehicle ATV;
        private Blip PedBlip;
        private bool OnScene = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            //Create ATV Spawn
            ATVSpawn = new Vector3(-1384.18f, 4307.26f, 2.59f);

            //Create Ped Spawn
            PedSpawn = new Vector3(-1385.87f, 4308.71f, 2.22f);

            this.ShowCalloutAreaBlipBeforeAccepting(PedSpawn, 30f);

            //Set callout position
            this.CalloutPosition = PedSpawn;

            //Create callout message
            this.CalloutMessage = "ATV Accident";

            //Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_GRAND_THEFT_AUTO IN_OR_ON_POSITION", SpawnPoint);

            this.FriendlyName = "ATV accident";

            //Last Line
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Create ATV
            ATV = new Vehicle("BLAZER", ATVSpawn);
            ATV.Heading = 27.10f;
            ATV.MakePersistent();
            ATV.SetRotationRoll(90f);

            //Create Ped
            Ped = new Ped(PedSpawn, 18.97f);
            Ped.MakePersistent();
            Ped.Tasks.PlayAnimation("amb@world_human_sunbathe@male@back@base", "base", 1f, AnimationFlags.Loop);

            //Create Ped Blip
            PedBlip = Ped.AttachBlip();
            PedBlip.Sprite = BlipSprite.GetawayCar;
            PedBlip.Color = System.Drawing.Color.Green;

            //Draw Route
            PedBlip.EnableRoute(System.Drawing.Color.Red);

            //Display Information
            Game.DisplayHelp("Make your way to the injured rider");

            //Last Line
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            //First Line
            base.OnCalloutNotAccepted();
            if (ATV.Exists()) { ATV.Dismiss(); }
            if (Ped.Exists()) { Ped.Dismiss(); }
            if (PedBlip.Exists()) { PedBlip.Delete(); }
        }

        public override void Process()
        {
            //First Line
            base.Process();

            if (!OnScene & Game.LocalPlayer.Character.Position.DistanceTo(Ped) <= 15f)
            {
                PedBlip.DisableRoute();
                Game.DisplayHelp("Make your way to the rider. Call an ambulance if needed. Press ~b~END~w~ to end the call.");
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
            if (ATV.Exists()) { ATV.Dismiss(); }
            if (Ped.Exists()) { Ped.Dismiss(); }
            if (PedBlip.Exists()) { PedBlip.Delete(); }
        }
    }
}