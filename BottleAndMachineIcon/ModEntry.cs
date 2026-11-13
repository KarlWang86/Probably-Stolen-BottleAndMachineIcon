namespace BottleAndMachineIcon
{
    public sealed class ModEntry : IMod
    {
        public void Init(ModManifest manifest) => IconRuntime.Init(manifest);
        public void OnEnable() => IconRuntime.Enable();
        public void OnDisable() => IconRuntime.Disable();
    }
}
