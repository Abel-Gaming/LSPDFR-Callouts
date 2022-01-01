using System;
using System.Drawing;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Engine.Scripting.Entities;

namespace CountyCallouts.Callouts
{
    [CalloutInfo("Locked Vehicle", CalloutProbability.Medium)]
    public class LockedVehicle : Callout
    {
        //Private References
        private Vector3 VehicleSpawn;
        private Vector3 PedSpawn;
        private Blip PedBlip;
        private Vehicle Car;
        private Ped Ped;
        private bool OnScene = false;
        private bool TalkToDriver = false;
        private bool Unlocked = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            //Create Vehicle Spawn
            VehicleSpawn = new Vector3(-1616.15f, 4741.96f, 52.50f);

            //Create Ped Spawn
            PedSpawn = new Vector3(-1615.86f, 4745.25f, 52.87f);

            ShowCalloutAreaBlipBeforeAccepting(PedSpawn, 30f);

            //Set callout position
            this.CalloutPosition = PedSpawn;

            //Create Callout message
            CalloutMessage = "Keys locked in a vehicle";

            //Create friendly name
            FriendlyName = "keys locked in a vehicle";

            //Last Line
            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            //Create Vehicle
            Car = new Vehicle(VehicleSpawn, 329.60f);
            Car.MakePersistent();
            Car.LockStatus = VehicleLockStatus.LockedButCanBeBrokenInto;

            //Create Ped
            Ped = new Ped(PedSpawn, 27.09f);
            Ped.MakePersistent();
            Ped.BlockPermanentEvents = true;
            Ped.Tasks.StandStill(-1);

            //Create Blip
            PedBlip = Ped.AttachBlip();
            PedBlip.Sprite = BlipSprite.Friend;
            PedBlip.Color = Color.Blue;

            //Draw Route
            PedBlip.EnableRoute(Color.Blue);

            //Last Line
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            //First Line
            base.OnCalloutNotAccepted();
            if (Ped.Exists()) { Ped.Dismiss(); }
            if (Car.Exists()) { Car.Dismiss(); }
            if (PedBlip.Exists()) { PedBlip.Delete(); }
        }

        public override void Process()
        {
            //First Line
            base.Process();

            if (!OnScene & !TalkToDriver & !Unlocked & Game.LocalPlayer.Character.Position.DistanceTo(Ped) <= 15f)
            {
                OnScene = true;
                PedBlip.DisableRoute();

                Game.DisplayHelp("Speak to the owner of the vehicle. Press ~b~Y~w~ to talk.");
            }

            if (!TalkToDriver & !Unlocked & Game.IsKeyDown(System.Windows.Forms.Keys.Y))
            {
                Game.LocalPlayer.HasControl = false;

                Game.DisplaySubtitle("~b~You:~w~ Hello! Are you the one who called about locking your keys in your car?");

                GameFiber.Sleep(5000);

                Game.DisplaySubtitle("Yes, that would be me. I feel so dumb. I went on a walk and came back when I realized I locked my keys in.");

                GameFiber.Sleep(5000);

                Game.DisplaySubtitle("~b~You:~w~ Alright! No problem! Let me see if I can help you!");

                GameFiber.Sleep(5000);

                Game.LocalPlayer.Character.Tasks.GoStraightToPosition(Car.GetBonePosition(1), 1f, 1f, 1f, -1).WaitForCompletion();

                GameFiber.Sleep(5000);

                Game.DisplayHelp("Unlocking car door...");

                Car.LockStatus = VehicleLockStatus.Unlocked;

                GameFiber.Sleep(5000);

                Game.DisplaySubtitle("~b~You:~w~ Alright! You should be good to go!");

                GameFiber.Sleep(5000);

                Game.DisplaySubtitle("Thank you so much officer!");

                Game.LocalPlayer.HasControl = true;

                TalkToDriver = true;
                Unlocked = true;
            }

            if (Unlocked)
            {
                Ped.Tasks.EnterVehicle(Car, -1).WaitForCompletion();
                Ped.Tasks.CruiseWithVehicle(15f);

                GameFiber.Sleep(5000);

                this.End();
            }
        }

        public override void End()
        {
            //First Line
            base.End();
            if (Ped.Exists()) { Ped.Dismiss(); }
            if (Car.Exists()) { Car.Dismiss(); }
            if (PedBlip.Exists()) { PedBlip.Delete(); }
        }
    }
}