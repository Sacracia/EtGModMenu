using UnityEngine;

namespace EtGModMenu
{
    internal class WeaponMenu : MonoBehaviour
    {
        private bool _visible = true;
        private Rect _window = new Rect(185f, 10f, 150f, 100f);
        private bool _infAmmo = false;
        private bool _infClip = false;
        private bool _highAccuracy = false;
        private bool _autoAim = false;
        private bool _oneHitKill = false;
        private bool _instCharge = false;
        private string _filter;

        private static WeaponMenu s_instance;

        public static WeaponMenu Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = (WeaponMenu)UnityEngine.Object.FindObjectOfType(typeof(WeaponMenu));
                }
                if (s_instance == null)
                {
                    s_instance = Loader.Load.AddComponent<WeaponMenu>();
                }
                return s_instance;
            }
        }

        void DrawElements()
        {
            bool flag = GUILayout.Toggle(_oneHitKill, "High damage", new GUILayoutOption[0]);
            if (flag != _oneHitKill)
            {
                _oneHitKill = flag;
                float dmg = 0;
                CoreDamageTypes type = CoreDamageTypes.None;
                DamageCategory dc = DamageCategory.Normal;
                bool b = false;
                Patches.ApplyPatch(typeof(HealthHaver), "ApplyDamage",
                    () => Patches.ApplyDamage(null, ref dmg, Vector2.left, null, ref type, ref dc, ref b), flag, true);
            }
            flag = GUILayout.Toggle(_infAmmo, "Infinite ammo", new GUILayoutOption[0]);
            if (flag != _infAmmo)
            {
                _infAmmo = flag;
                Patches.ApplyPatch(typeof(Gun), "DecrementAmmoCost",
                    () => Patches.DecrementAmmoCost(null), flag, true);
            }
            flag = GUILayout.Toggle(_infClip, "Infinite clip", new GUILayoutOption[0]);
            if (flag != _infClip)
            {
                _infClip = flag;
                Patches.ApplyPatch(typeof(Gun), "IncrementModuleFireCountAndMarkReload",
                    () => Patches.IncrementModuleFireCountAndMarkReload(null), flag, true);
            }
            flag = GUILayout.Toggle(_instCharge, "Instant charge", new GUILayoutOption[0]);
            if (flag != _instCharge)
            {
                _instCharge = flag;
                float temp = 0;
                Patches.ApplyPatch(typeof(ProjectileModule), "GetChargeProjectile",
                    () => Patches.GetChargeProjectile(ref temp), flag, true);
            }
            flag = GUILayout.Toggle(_highAccuracy, "High accuracy", new GUILayoutOption[0]);
            if (flag != _highAccuracy)
            {
                _highAccuracy = flag;
                PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
                if (localPlayer != null)
                {
                    localPlayer.HighAccuracyAimMode = flag;
                }
            }
            _autoAim = GUILayout.Toggle(_autoAim, "Auto aim", new GUILayoutOption[0]);
            GUILayout.BeginVertical();
            GUILayout.Label("Give weapon", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal();
            GUILayout.Label("ID", new GUILayoutOption[0]);
            _filter = GUILayout.TextField(_filter, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Gain", new GUILayoutOption[0]))
                if (int.TryParse(_filter, out int id))
                    GiveWeapon(id);
            GUILayout.EndVertical();
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
                _window = GUILayout.Window(2, _window, OnWindow, "Weapon", new GUILayoutOption[0]);
            }
        }

        public void AutoAim(PlayerController playerController)
        {
            Projectile projectile = null;
            float distance;
            AIActor nearestEnemy = playerController.CurrentRoom.GetNearestEnemy(playerController.CenterPosition, out distance, true, true);
            if (nearestEnemy == null || nearestEnemy.healthHaver.IsDead)
            {
                playerController.forceAimPoint = null;
                return;
            }
            Vector2 enemyPosition = nearestEnemy.specRigidbody.GetUnitCenter(ColliderType.HitBox);
            if (playerController.CurrentGun != null && playerController.CurrentGun.DefaultModule != null)
            {
                projectile = playerController.CurrentGun.DefaultModule.GetCurrentProjectile();
            }
            float projectileSpeed = (!projectile) ? float.MaxValue : projectile.baseData.speed;
            Vector2 offset = enemyPosition + nearestEnemy.Velocity * (distance / projectileSpeed);
            playerController.forceAimPoint = offset;
        }

        void GiveWeapon(int id)
        {
            PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
            if (localPlayer != null)
            {
                PickupObject pickup = PickupObjectDatabase.GetById(id);
                if (pickup != null)
                    LootEngine.GivePrefabToPlayer(pickup.gameObject, localPlayer);
            }
        }

        void Update()
        {
            PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
            if (localPlayer != null)
            {
                if (_autoAim)
                {
                    AutoAim(localPlayer);
                }
            }
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                _visible = !_visible;
            }
        }
    }
}
