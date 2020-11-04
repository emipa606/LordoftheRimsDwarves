using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Dwarves
{
	public class JobDriver_FillFermentingBarrel : JobDriver
	{		
		private const TargetIndex BarrelInd = TargetIndex.A;
		private const TargetIndex WortInd = TargetIndex.B;
		private const int Duration = 200;
		
		protected Building_FermentingMeadBarrel Barrel => (Building_FermentingMeadBarrel)job.GetTarget(BarrelInd).Thing;
		protected Thing Wort => job.GetTarget(WortInd).Thing;

        public override bool TryMakePreToilReservations(bool yeaa)
        {
            return pawn.Reserve(Barrel, job, 1, -1, null) && pawn.Reserve(Wort, job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(BarrelInd);
			this.FailOnBurningImmobile(BarrelInd);
            AddEndCondition(() => (Barrel.SpaceLeftForWort > 0) ? JobCondition.Ongoing : JobCondition.Succeeded);
			yield return Toils_General.DoAtomic(delegate
			{
				job.count = Barrel.SpaceLeftForWort;
			});
			Toil reserveWort = Toils_Reserve.Reserve(WortInd, 1, -1, null);
			yield return reserveWort;
			yield return Toils_Goto.GotoThing(WortInd, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(WortInd).FailOnSomeonePhysicallyInteracting(WortInd);
			yield return Toils_Haul.StartCarryThing(WortInd, false, true, false).FailOnDestroyedNullOrForbidden(WortInd);
			yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveWort, WortInd, TargetIndex.None, true, null);
			yield return Toils_Goto.GotoThing(BarrelInd, PathEndMode.Touch);
			yield return Toils_General.Wait(Duration).FailOnDestroyedNullOrForbidden(WortInd).FailOnDestroyedNullOrForbidden(BarrelInd).FailOnCannotTouch(BarrelInd, PathEndMode.Touch).WithProgressBarToilDelay(BarrelInd, false, -0.5f);
			yield return new Toil
			{
				initAction = delegate
				{
					Barrel.AddWort(Wort);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield break;
		}

	}
}
