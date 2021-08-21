using UnityEngine;
using  static Pacman.Direction;
namespace Pacman
{
    public class EnvManager : MonoBehaviour
    {

        public PlayerMove playerMove;
        public GameObject cookiePrefab;
        public Transform tileLayer;
        public Transform cookieParent;
        public int totalCookie;

        public EnemyManager enemyManager;

        // Start is called before the first frame update
        void Start()
        {
            enemyManager = GetComponent<EnemyManager>();

            /*  Academy.Instance.OnEnvironmentReset+= delegate
              {
                  Destroy(cookieParent.gameObject);
                  cookieParent= Instantiate(cookiePrefab,tileLayer).transform;
                  totalCookie = cookieParent.childCount;
                  enemyManager.Initialize();
              };*/
        }

        public void Init()
        {
            Destroy(cookieParent.gameObject);
            cookieParent = Instantiate(cookiePrefab, transform).transform;
            totalCookie = cookieParent.childCount;

            enemyManager.Initialize();
        }
    }
}
