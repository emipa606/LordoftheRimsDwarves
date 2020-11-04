using RimWorld;
using Verse;

namespace Dwarves
{
    class ThingWithComps_Glower : ThingWithComps
    {
        public Building_StreetLamp master = null;

        public override void Draw()
        {
        }

        public override void Tick()
        {
            base.Tick();
            CheckNeedsDestruction();
            CheckNeedsFlick();
        }

        public void CheckNeedsDestruction()
        {
            if (master != null && Spawned)
            {
                if (!master.Spawned)
                {
                    Destroy(0);
                    return;
                }

            }
        }

        public void CheckNeedsFlick()
        {
            if (master == null)
            {
                return;
            }

            CompFlickable masterflickable = master.TryGetComp<CompFlickable>();
            CompFlickable flickable = this.TryGetComp<CompFlickable>();

            if (masterflickable.SwitchIsOn != flickable.SwitchIsOn)
            {
                flickable.DoFlick();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref master, "master", false);
        }
    }
}
