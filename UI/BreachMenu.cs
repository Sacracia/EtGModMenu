using UnityEngine;

namespace EtGModMenu
{
    internal class BreachMenu : MonoBehaviour
    {
        private bool _visible = true;
        private Rect _window = new Rect(550f, 10f, 150f, 100f);
        private bool _unlockHeroes = false;

        private static BreachMenu s_instance;

        public static BreachMenu Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = (BreachMenu)UnityEngine.Object.FindObjectOfType(typeof(BreachMenu));
                }
                if (s_instance == null)
                {
                    s_instance = Loader.Load.AddComponent<BreachMenu>();
                }
                return s_instance;
            }
        }

        void SpawnCurrency(int amount)
        {
            PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
            LootEngine.SpawnCurrency(localPlayer.CenterPosition, amount, true, new Vector2?(Vector2.down), new float?(45f), 0.5f, 0.25f);
        }

        /*void RefreshStock(BaseShopController.AdditionalShopType type)
        {
            BaseShopController ourShop = StaticReferenceManager.AllShops.Where(shop => shop.baseShopType == type).FirstOrDefault();
            if (ourShop != null)
            {
                ourShop.GetType().GetMethod("DoSetup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .Invoke(ourShop, new object[0]);
            }
        }*/

        void DrawElements()
        {
            bool flag = GUILayout.Toggle(_unlockHeroes, "Unlock all characters", new GUILayoutOption[0]);
            if (flag != _unlockHeroes)
            {
                _unlockHeroes = flag;
                bool temp = false;
                Patches.ApplyPatch(typeof(FoyerCharacterSelectFlag), "PrerequisitesFulfilled", () => Patches.PrerequisitesFulfilled(ref temp), flag, true);
            }
            flag = GUILayout.Button("+100 Hegemony Credits", new GUILayoutOption[0]);
            if (flag)
            {
                SpawnCurrency(100);
            }
            flag = GUILayout.Button("Statue stage 1", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.BLACKSMITH_ELEMENT1, true);
                GameStatsManager.Instance?.SetFlag(GungeonFlags.BOSSKILLED_HIGHDRAGUN, true);
            }
            flag = GUILayout.Button("Statue stage 2", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.BLACKSMITH_ELEMENT2, true);
                GameStatsManager.Instance?.SetFlag(GungeonFlags.BOSSKILLED_HIGHDRAGUN, true);
            }
            flag = GUILayout.Button("Statue stage 3", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.BLACKSMITH_ELEMENT3, true);
                GameStatsManager.Instance?.SetFlag(GungeonFlags.BOSSKILLED_HIGHDRAGUN, true);
            }
            flag = GUILayout.Button("Statue stage 4", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.BLACKSMITH_ELEMENT4, true);
                GameStatsManager.Instance?.SetFlag(GungeonFlags.BOSSKILLED_HIGHDRAGUN, true);
            }
            flag = GUILayout.Button("Change to alt costume", new GUILayoutOption[0]);
            if (flag)
            {
                GameManager.Instance?.PrimaryPlayer.SwapToAlternateCostume();
            }
            flag = GUILayout.Button("Unlock Sorceress", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.SORCERESS_ACTIVE_IN_FOYER, true);
            }
            flag = GUILayout.Button("Unlock Daisuke", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.DAISUKE_ACTIVE_IN_FOYER, true);
            }
            flag = GUILayout.Button("Unlock Tonic", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.TONIC_ACTIVE_IN_FOYER, true);
            }
            flag = GUILayout.Button("Unlock Bowler", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.BOWLER_ACTIVE_IN_FOYER, true);
            }
            flag = GUILayout.Button("Unlock Meta Shop", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.META_SHOP_ACTIVE_IN_FOYER, true);
            }
            flag = GUILayout.Button("Unlock Frifle", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.FRIFLE_ACTIVE_IN_FOYER, true);
            }
            flag = GUILayout.Button("Unlock Ledge Goblin", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.LEDGEGOBLIN_ACTIVE_IN_FOYER, true);
            }
            flag = GUILayout.Button("Unlock Gunsling King", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.GUNSLING_KING_ACTIVE_IN_FOYER, true);
            }
            flag = GUILayout.Button("Unlock Lost Adventurer", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.LOST_ADVENTURER_ACTIVE_IN_FOYER, true);
            }
            flag = GUILayout.Button("Unlock Boss-Rush", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.SHERPA_HAS_UNLOCKED_BOSSRUSH, true);
                GameStatsManager.Instance?.SetFlag(GungeonFlags.SHERPA_ACTIVE_IN_ELEVATOR_ROOM, true);
                GameStatsManager.Instance?.SetFlag(GungeonFlags.SHERPA_UNLOCK4_COMPLETE, true);
                GameStatsManager.Instance?.SetFlag(GungeonFlags.SHERPA_UNLOCK3_COMPLETE, true);
                GameStatsManager.Instance?.SetFlag(GungeonFlags.SHERPA_UNLOCK2_COMPLETE, true);
                GameStatsManager.Instance?.SetFlag(GungeonFlags.SHERPA_UNLOCK1_COMPLETE, true);
            }
            flag = GUILayout.Button("Unlock Super-Boss-Rush", new GUILayoutOption[0]);
            if (flag)
            {
                GameStatsManager.Instance?.SetFlag(GungeonFlags.SHERPA_HAS_UNLOCKED_SUPERBOSSRUSH, true);
            }
        }

        void OnWindow(int windowID)
        {
            DrawElements();
            GUI.DragWindow();
        }

        void OnGUI()
        {
            if (_visible)
            {
                _window = GUILayout.Window(4, _window, OnWindow, "Breach", new GUILayoutOption[0]);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                _visible = !_visible;
            }
        }
    }
}
