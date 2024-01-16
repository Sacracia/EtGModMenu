using Dungeonator;
using UnityEngine;

namespace EtGModMenu
{
    internal class LevelMenu : MonoBehaviour
    {
        private bool _visible = true;
        private Rect _window = new Rect(345f, 10f, 150f, 100f);
        private bool _doorsUnlocked = false;

        static LevelMenu s_instance;

        public static LevelMenu Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = (LevelMenu)UnityEngine.Object.FindObjectOfType(typeof(LevelMenu));
                }
                if (s_instance == null)
                {
                    s_instance = Loader.Load.AddComponent<LevelMenu>();
                }
                return s_instance;
            }
        }

        void KillEveryone()
        {
            var localPlayer = GameManager.Instance?.PrimaryPlayer;
            if (localPlayer != null)
            {
                Dungeonator.RoomHandler currentRoom = localPlayer.CurrentRoom;
                foreach (var enemy in currentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All))
                {
                    enemy.EraseFromExistenceWithRewards();
                }
            }
        }

        void SpawnChest(PickupObject.ItemQuality quality, bool rainbow = false)
        {
            Pathfinding.CellValidator cellValidator = delegate (IntVector2 c)
            {
                for (int n = 0; n < 5; n++)
                {
                    for (int num2 = 0; num2 < 5; num2++)
                    {
                        if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(c.x + n, c.y + num2) || GameManager.Instance.Dungeon.data[c.x + n, c.y + num2].type == CellType.PIT || GameManager.Instance.Dungeon.data[c.x + n, c.y + num2].isOccupied)
                        {
                            return false;
                        }
                    }
                }
                return true;
            };

            GameManager gameManager = GameManager.Instance;
            PlayerController localPlayer = gameManager?.PrimaryPlayer;
            if (gameManager == null || localPlayer == null)
                return;
            RoomHandler currentRoom = localPlayer.CurrentRoom;
            IntVector2? randomAvailableCell = currentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 5), new CellTypes?(CellTypes.FLOOR), false, cellValidator);
            if (randomAvailableCell == null)
                return;
            IntVector2? intVector = new IntVector2?(randomAvailableCell.GetValueOrDefault() + IntVector2.One);
            if (!rainbow)
            {
                gameManager.RewardManager.SpawnRewardChestAt(intVector.Value, -1, quality);
                return;
            }
            RewardManager rewardManager = gameManager.RewardManager;
            Chest.Spawn(rewardManager.Rainbow_Chest, intVector.Value);
        }

        void DrawElements()
        {
            bool flag = GUILayout.Button("Reveal map", new GUILayoutOption[0]);
            if (flag)
            {
                Minimap.Instance?.RevealAllRooms(true);
            }
            flag = GUILayout.Button("Kill everyone", new GUILayoutOption[0]);
            if (flag)
            {
                KillEveryone();
            }
            flag = GUILayout.Button("Go next level", new GUILayoutOption[0]);
            if (flag)
            {
                GameManager.Instance?.LoadNextLevel();
            }
            flag = GUILayout.Toggle(_doorsUnlocked, "Doors in rooms opened", new GUILayoutOption[0]);
            if (flag != _doorsUnlocked)
            {
                _doorsUnlocked = flag;
                Patches.ApplyPatch(typeof(RoomHandler), "SealRoom", () => Patches.SealRoom(), flag, true);
            }
            for (int i = 5; i > 0; i--)
                if (GUILayout.Button($"Spawn chest tier {(PickupObject.ItemQuality)i}"))
                    SpawnChest((PickupObject.ItemQuality)i, false);
            if (GUILayout.Button($"Spawn chest tier SPECIAL"))
                SpawnChest(PickupObject.ItemQuality.SPECIAL, false);
            if (GUILayout.Button($"Spawn chest tier RAINBOW"))
                SpawnChest(PickupObject.ItemQuality.S, true);
        }

        void OnWindow(int id)
        {
            DrawElements();
            GUI.DragWindow();
        }

        void OnGUI()
        {
            if (_visible)
            {
                _window = GUILayout.Window(3, _window, OnWindow, "Level", new GUILayoutOption[0]);
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
