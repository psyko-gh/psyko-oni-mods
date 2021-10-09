using System;
using PeterHan.PLib.UI;
using UnityEngine;

namespace Psyko.MomentarySwitch
{
    /*
     * Inspired from PlayerControlledToggleSideScreen
     */
    public class MomentarySwitchSideScreen : SideScreenContent, IRenderEveryTick
    {
        public MomentarySwitch target;
        private KBatchedAnimController kbac;
        private KButton button;
        private bool currentState;
        private float lastKeyboardShortcutTime;
        private const float KEYBOARD_COOLDOWN = 0.1f;
        private bool keyDown;
        private StatusItem togglePendingStatusItem;
        protected static readonly HashedString[] ON_ANIMS = new HashedString[] { "on_pre", "on" };
        protected static readonly HashedString[] OFF_ANIMS = new HashedString[2] { "off_pre", "off" };
        
        public override bool IsValidForTarget(GameObject target) => target.GetComponent<MomentarySwitch>() != null;
        
        protected override void OnPrefabInit()
        {
            GameObject ui = PUIElements.CreateUI(gameObject, "MomentarySwitchSideScreenButton");
            ui.SetActive(false);
            
            kbac = ui.AddOrGet<KBatchedAnimController>();
            kbac.Offset = new Vector3(0, -50, 0);
            kbac.materialType = KAnimBatchGroup.MaterialType.UI;
            kbac.setScaleFromAnim = false;
            kbac.SetVisiblity(true);
            kbac.AnimFiles = new[] { Assets.GetAnim((HashedString) "button_momentary_ui_kanim") };
            kbac.transform.SetParent(ui.transform);
            kbac.initialAnim = "off";
            kbac.initialMode = KAnim.PlayMode.Loop;
            kbac.visibilityType = KAnimControllerBase.VisibilityType.Always;
            
            KImage bgImage = ui.AddComponent<KImage>();
            bgImage.transform.SetParent(kbac.transform);
            var transparentColor = ScriptableObject.CreateInstance<ColorStyleSetting>();
            transparentColor.Init(new Color32(0, 0, 0, byte.MaxValue));
            bgImage.colorStyleSetting = transparentColor;
            
            button = ui.AddOrGet<KButton>();
            button.additionalKImages = Array.Empty<KImage>();
            ButtonSoundPlayer buttonSoundPlayer = new ButtonSoundPlayer();
            buttonSoundPlayer.Enabled = true;
            button.soundPlayer = buttonSoundPlayer;
            button.onClick += this.RequestToggle;
            
            ui.SetMinUISize(new Vector2(140, 140));
            ui.SetActive(true);

            ContentContainer = gameObject;
            base.OnPrefabInit();
            UpdateVisuals(false, false);
        }
        
        public override void SetTarget(GameObject target)
        {
            if (this.target != null)
            {
                this.target.onStateChange -= OnTargetStateChanged;
            }
            if (target == null)
            {
                Debug.LogError("MomentarySwitchSideScreen.SetTarget : received null target");
            }
            else
            {
                this.target = target.GetComponent<MomentarySwitch>();
                if (this.target == null)
                {
                    Debug.LogError("MomentarySwitchSideScreen.SetTarget : received target is not a MomentarySwitch");
                }
                else
                {
                    this.target.onStateChange += OnTargetStateChanged;
                    this.UpdateVisuals(this.target.ToggleRequested ? !this.target.ToggledOn() : this.target.ToggledOn(), false);
                    this.titleKey = this.target.SideScreenTitleKey;
                }
            }
        }
        
        private void UpdateVisuals(bool state, bool smooth)
        {
            if (state != this.currentState)
            {
                if (smooth)
                    this.kbac.Play(state ? ON_ANIMS : OFF_ANIMS);
                else
                    this.kbac.Play(state ? ON_ANIMS[1] : OFF_ANIMS[1]);
            }
            this.currentState = state;
        }
        
        private void RequestToggle()
        {
            this.target.ToggleRequested = !this.target.ToggleRequested;
            if (this.target.ToggleRequested && SpeedControlScreen.Instance.IsPaused)
                this.target.GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, this.togglePendingStatusItem, (object) this);
            else
                this.target.GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, (StatusItem) null);
            this.UpdateVisuals(this.target.ToggleRequested ? !this.target.ToggledOn() : this.target.ToggledOn(), true);
        }
        
        private void ClickToggle(GameObject go)
        {
            if (SpeedControlScreen.Instance.IsPaused)
                this.RequestToggle();
            else
                this.Toggle();
        }
        
        private void Toggle()
        {
            this.target.ToggledByPlayer();
            this.UpdateVisuals(this.target.ToggledOn(), true);
            this.target.ToggleRequested = false;
        }

        private void OnTargetStateChanged() => UpdateVisuals(target.ToggledOn(), true);

        public void RenderEveryTick(float dt)
        {
            if (!this.isActiveAndEnabled)
                return;
            if (!this.keyDown && Input.GetKeyDown(KeyCode.Return) & Time.unscaledTime - this.lastKeyboardShortcutTime > KEYBOARD_COOLDOWN)
            {
                if (SpeedControlScreen.Instance.IsPaused)
                    this.RequestToggle();
                else
                    this.Toggle();
                this.lastKeyboardShortcutTime = Time.unscaledTime;
                this.keyDown = true;
            }
            if (!this.keyDown || !Input.GetKeyUp(KeyCode.Return))
                return;
            this.keyDown = false;
        }
    }
}