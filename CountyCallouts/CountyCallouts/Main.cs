using LSPD_First_Response.Mod.API;
using Rage;

namespace CountyCallouts
{
    public class Main : Plugin
    {
        private static string modname = "County Callouts";
        private static string version = "1.1";
        private static string author = "Abel Gaming";

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
        }

        public override void Finally()
        {
            Game.LogTrivial("Plugin has been cleaned up.");
        }

        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                //Display Notification
                Game.DisplayNotification("~b~" + modname + " ~w~has loaded successfully");

                //Log
                Game.LogTrivial(modname + " by " + author + " " + version + " loaded.");

                RegisterCallouts();
            }
        }

        private static void RegisterCallouts()
        {
            //Register Callouts Here
            Functions.RegisterCallout(typeof(Callouts.ATVAccident1));
            Functions.RegisterCallout(typeof(Callouts.ATVAccident2));
            Functions.RegisterCallout(typeof(Callouts.LockedVehicle));
            Functions.RegisterCallout(typeof(Callouts.MissingPerson));
        }
    }
}
