using DI;
using Registries;
using Ships;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GameSystems
{
    public class MovementVisualizeSystem : GameSystemBase, ICorePreUpdateTickObserver
    {
        private static readonly int _plumePowerValueID = Shader.PropertyToID("_PowerValue");

        private ShipRegistry _shipRegistry;

        [Inject]
        public void Construct(ShipRegistry shipRegystry)
        {
            _shipRegistry = shipRegystry;
        }

        public void CorePreUpdateTick()
        {
            VisualizePlayerShip();
        }

        private void VisualizePlayerShip()
        {
            var playerShip = _shipRegistry.PlayerShip;
            ref var movementRuntimeData = ref playerShip.MovementRuntimeData;
            ref var view = ref playerShip.View;
            VisualizeMainEngine(movementRuntimeData, ref view);
            VisualizeThrusters(movementRuntimeData, ref view);
            VisualizeBoosters(movementRuntimeData, ref view);
        }

        private void VisualizeMainEngine(in MovementRuntimeData movementRuntimeData, ref MovementView view)
        {
            var mainEnginePower = movementRuntimeData.MainEnginePower;

            if (view.LastMainEnginePowerValue == mainEnginePower) return;

            view.LastMainEnginePowerValue = mainEnginePower;
            SetMainEngines(view.MainEnginesPlumes, mainEnginePower);
        }

        private void VisualizeThrusters(in MovementRuntimeData movementRuntimeData, ref MovementView view)
        {
            var currentThrustersPower = view.ThrustersPower;
            var newThrustersPower = new ThrustersPower();

            var strafePower = movementRuntimeData.StrafePower;
            var absStrafePower = Mathf.Abs(strafePower);

            var rotatePower = movementRuntimeData.RotatePower;
            var absRotatePower = Mathf.Abs(rotatePower);

            if (strafePower > 0)
            {
                newThrustersPower.BackLeft = absStrafePower;
                newThrustersPower.FrontLeft = absStrafePower;
            }
            else if (strafePower < 0)
            {
                newThrustersPower.BackRight = absStrafePower;
                newThrustersPower.FrontRight = absStrafePower;
            }

            if (rotatePower > 0)
            {
                if (newThrustersPower.FrontRight == 0) newThrustersPower.FrontRight = absRotatePower;
                if (newThrustersPower.BackLeft == 0) newThrustersPower.BackLeft = absRotatePower;
            }
            else if (rotatePower < 0)
            {
                if (newThrustersPower.BackRight == 0) newThrustersPower.BackRight = absRotatePower;
                if (newThrustersPower.FrontLeft == 0) newThrustersPower.FrontLeft = absRotatePower;
            }

            if (currentThrustersPower.BackLeft != newThrustersPower.BackLeft) SetTrusters(view.ThrustersBL, newThrustersPower.BackLeft);
            if (currentThrustersPower.FrontLeft != newThrustersPower.FrontLeft) SetTrusters(view.ThrustersFL, newThrustersPower.FrontLeft);
            if (currentThrustersPower.BackRight != newThrustersPower.BackRight) SetTrusters(view.ThrustersBR, newThrustersPower.BackRight);
            if (currentThrustersPower.FrontRight != newThrustersPower.FrontRight) SetTrusters(view.ThrustersFR, newThrustersPower.FrontRight);

            view.ThrustersPower = newThrustersPower;
        }

        private void VisualizeBoosters(in MovementRuntimeData movementRuntimeData, ref MovementView view)
        {
            var state = movementRuntimeData.BoostersIsActive;

            if (view.LastBoostersState == state) return;

            view.LastBoostersState = state;
            var value = state ? 1 : 0;
            SetBusters(view.BoostersPlumes, value);
        }

        private void SetTrusters(ThrusterPlume[] thrusterPlumes, float value)
        {
            foreach (var thruster in thrusterPlumes)
            {
                var matBlock = thruster.MatBlock;
                matBlock.SetFloat(_plumePowerValueID, value);
                thruster.SpriteRenderer.SetPropertyBlock(matBlock);
            }
        }

        private void SetMainEngines(MainEnginePlume[] mainEnginePlumes, float value)
        {
            foreach (var engine in mainEnginePlumes)
            {
                var matBlock = engine.MatBlock;
                matBlock.SetFloat(_plumePowerValueID, value);
                engine.SpriteRenderer.SetPropertyBlock(matBlock);

                var forwardParticles = engine.ForwardParticles;
                var reverseParticles = engine.ReverseParticles;

                if (value == 0)
                {
                    forwardParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    reverseParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    return;
                }

                if (value > 0 && !forwardParticles.isPlaying)
                {
                    forwardParticles.Play();
                    reverseParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                else if (!reverseParticles.isPlaying)
                {
                    reverseParticles.Play();
                    forwardParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

        private void SetBusters(BoosterPlume[] boosterPlumes, float value)
        {
            foreach (var buster in boosterPlumes)
            {
                var matBlock = buster.MatBlock;
                matBlock.SetFloat(_plumePowerValueID, value);
                buster.SpriteRenderer.SetPropertyBlock(matBlock);

                if (value == 0)
                {
                    buster.BoosterParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                else
                {
                    buster.BoosterParticles.Play();
                }
            }
        }
    }
}

public struct ThrustersPower
{
    public float FrontLeft;
    public float FrontRight;
    public float BackLeft;
    public float BackRight;
}

public static class SideEnginesPowerExtensions
{
    public static void Lerp(this ref ThrustersPower current, in ThrustersPower target, float t)
    {
        t = Mathf.Clamp01(t);
        float it = 1f - t;

        current.FrontLeft = current.FrontLeft * it + target.FrontLeft * t;
        current.FrontRight = current.FrontRight * it + target.FrontRight * t;
        current.BackLeft = current.BackLeft * it + target.BackLeft * t;
        current.BackRight = current.BackRight * it + target.BackRight * t;
    }
    public static void MoveTowards(ref this ThrustersPower current, in ThrustersPower target, float maxDelta)
    {
        current.FrontLeft = Mathf.MoveTowards(current.FrontLeft, target.FrontLeft, maxDelta);
        current.FrontRight = Mathf.MoveTowards(current.FrontRight, target.FrontRight, maxDelta);
        current.BackLeft = Mathf.MoveTowards(current.BackLeft, target.BackLeft, maxDelta);
        current.BackRight = Mathf.MoveTowards(current.BackRight, target.BackRight, maxDelta);
    }
}