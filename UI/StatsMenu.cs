using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace EtGModMenu
{
    internal class StatsMenu : MonoBehaviour
    {
        private bool _visible = true;
        private Rect _window = new Rect(10f, 170f, 150f, 100f);
        private float _currentSpeed = 1f;
        private float _currentZoom = 1f;
        private bool _infBlanks = false;
        private bool _infAbility = false;
        private bool _nocdAbility = false;

        private static StatsMenu s_instance;

        public static StatsMenu Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = (StatsMenu)UnityEngine.Object.FindObjectOfType(typeof(StatsMenu));
                }
                if (s_instance == null)
                {
                    s_instance = Loader.Load.AddComponent<StatsMenu>();
                }
                return s_instance;
            }
        }

        public int Currency
        {
            get
            {
                PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
                return localPlayer == null ? 0 : localPlayer.carriedConsumables.Currency;
            }
            set
            {
                PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
                int money = Mathf.Min(value, 999999);
                if (localPlayer != null)
                {
                    localPlayer.carriedConsumables.Currency = money;
                }
            }
        }

        public int Keys
        {
            get
            {
                PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
                return localPlayer == null ? 0 : localPlayer.carriedConsumables.KeyBullets;
            }
            set
            {
                PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
                int keys = Mathf.Min(value, 999);
                if (localPlayer != null)
                {
                    localPlayer.carriedConsumables.KeyBullets = keys;
                }
            }
        }

        public int RatKeys
        {
            get
            {
                PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
                return localPlayer == null ? 0 : localPlayer.carriedConsumables.ResourcefulRatKeys;
            }
            set
            {
                PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
                if (localPlayer != null)
                {
                    int currentKeys = localPlayer.carriedConsumables.ResourcefulRatKeys;
                    int keys = Mathf.Min(1, currentKeys);
                    keys++;
                    localPlayer.carriedConsumables.ResourcefulRatKeys = keys;
                }
            }
        }

        public float Speed
        {
            get
            {
                PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
                if (localPlayer != null)
                {
                    PlayerStats stats = localPlayer.stats;
                    if (stats != null)
                    {
                        return stats.GetStatModifier(PlayerStats.StatType.MovementSpeed);
                    }
                }
                return 1f;
            }
            set
            {
                PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
                if (localPlayer != null)
                {
                    PlayerStats stats = localPlayer.stats;
                    if (stats != null)
                    {
                        float baseSpeed = stats.BaseStatValues[0];
                        value = Mathf.Max(1f, value);
                        value = Mathf.Min(4f, value);
                        var StatValues = (List<float>)Traverse.Create(stats).Field("StatValues").GetValue();
                        if (StatValues != null)
                        {
                            StatValues[0] = baseSpeed * value;
                        }
                    }
                }
            }
        }

        public float Zoom
        {
            get
            {
                float? zoom = GameManager.Instance?.MainCameraController?.OverrideZoomScale;
                return zoom == null ? 1f : zoom.Value;
            }
            set
            {
                CameraController cameraController = GameManager.Instance?.MainCameraController;
                if (cameraController != null)
                {
                    value = Mathf.Min(1f, value);
                    value = Mathf.Max(value, 0.5f);
                    cameraController.OverrideZoomScale = value;
                    cameraController.CurrentZoomScale = value;
                }
            }
        }

        void DrawElements()
        {
            bool flag = GUILayout.Button("Add 1000 currency", new GUILayoutOption[0]);
            if (flag)
            {
                this.Currency += 1000;
            }
            flag = GUILayout.Button("Max currency", new GUILayoutOption[0]);
            if (flag)
            {
                this.Currency = 999999;
            }
            flag = GUILayout.Button("Add key", new GUILayoutOption[0]);
            if (flag)
            {
                this.Keys++;
            }
            flag = GUILayout.Button("Max keys", new GUILayoutOption[0]);
            if (flag)
            {
                this.Keys = 999;
            }
            flag = GUILayout.Button("Add rat key (2 - max)", new GUILayoutOption[0]);
            if (flag)
            {
                this.RatKeys++;
            }
            flag = GUILayout.Toggle(_infBlanks, "Infinite blanks", new GUILayoutOption[0]);
            if (flag != _infBlanks)
            {
                _infBlanks = flag;
                Patches.ApplyPatch(typeof(PlayerController), "DoConsumableBlank", () => Patches.DoConsumableBlank(null), flag, true);
            }
            flag = GUILayout.Toggle(_infAbility, "Infinite ability", new GUILayoutOption[0]);
            if (flag != _infAbility)
            {
                _infAbility = flag;
                Patches.ApplyPatch(typeof(PlayerItem), "UseConsumableStack", () => Patches.UseConsumableStack(), flag, true);
            }
            flag = GUILayout.Toggle(_nocdAbility, "No cooldown ability", new GUILayoutOption[0]);
            if (flag != _nocdAbility)
            {
                _nocdAbility = flag;
                Patches.ApplyPatch(typeof(PlayerItem), "ApplyCooldown", () => Patches.ApplyCooldown(null), flag, true);
            }
            GUILayout.Label("Movespeed", new GUILayoutOption[0]);
            _currentSpeed = GUILayout.HorizontalSlider(_currentSpeed, 1f, 4f, new GUILayoutOption[0]);
            GUILayout.Label("Camera distance", new GUILayoutOption[0]);
            _currentZoom = GUILayout.HorizontalSlider(_currentZoom, 0.5f, 1f, new GUILayoutOption[0]);
        }

        void OnWindow(int windowId)
        {
            DrawElements();
            GUI.DragWindow();
        }

        public void OnGUI()
        {
            if (_visible)
            {
                _window = GUILayout.Window(1, _window, OnWindow, "Stats", new GUILayoutOption[0]);
            }
        }

        public void Update()
        {
            if (_currentSpeed != this.Speed)
            {
                this.Speed = _currentSpeed;
            }
            if (_currentZoom != this.Zoom)
            {
                this.Zoom = _currentZoom;
            }
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                _visible = !_visible;
            }
        }
    }
}
