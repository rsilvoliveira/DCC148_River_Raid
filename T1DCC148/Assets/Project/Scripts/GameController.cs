using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZenvaVR;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Player player;
    public GameObject enemyPrefab;
    public GameObject enemyHPrefab;
    public float enemySpawnInterval = 1f;
    public float horizontalLimit = 2.8f;
    private float enemySpawnTimer;
    private int score;
    private float fuel = 100f;
    public GameObject gameCamera;
    public Text scoreText;
    public Text fuelText;
    public float fuelDecreaseSpeed = 5f;
    public GameObject fuelPrefab;
    public float fuelSpawnInterval = 9f;
    private float fuelSpawnTimer;
    private float restartTimer = 3f;
    //public ObjectPool enemyPool;
    //public ObjectPoolH enemyPoolH;
    public AudioSource gameMusic;


    // Start is called before the first frame update
    void Start()
    {
        enemySpawnTimer = enemySpawnInterval;
        player.OnFuel += OnFuel;
        fuelSpawnTimer = Random.Range(0f, fuelSpawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            enemySpawnTimer -= Time.deltaTime;
            if (enemySpawnTimer <= 0)
            {
                enemySpawnTimer = enemySpawnInterval;
                //GameObject enemyInstance = enemyPool.GetObj ();
                GameObject enemyInstance = Instantiate(enemyPrefab);
                enemyInstance.transform.SetParent(transform);
                enemyInstance.transform.position = new Vector2(Random.Range(-horizontalLimit, horizontalLimit), player.transform.position.y + Screen.height / 100f);
                enemyInstance.GetComponent<Enemy>().OnKill += OnEnemyKill;
                enemyInstance.GetComponent<Enemy>().OnAddScore += OnEnemyDestroy; // atualiza a pontuação do player ao destruir um inimigo

                //GameObject enemyHInstance = enemyPoolH.GetObj ();
                GameObject enemyHInstance = Instantiate(enemyHPrefab);
                enemyHInstance.transform.SetParent(transform);
                enemyHInstance.transform.position = new Vector2(Random.Range(-horizontalLimit, horizontalLimit), player.transform.position.y + Screen.height / 100f);
                enemyHInstance.GetComponent<EnemyH>().OnKill += OnEnemyKill;
                enemyHInstance.GetComponent<EnemyH>().OnAddScore += OnEnemyDestroy; // atualiza a pontuação do player ao destruir um inimigo

            }
            fuelSpawnTimer -= Time.deltaTime;
            if (fuelSpawnTimer <= 0)
            {
                fuelSpawnTimer = fuelSpawnInterval;
                GameObject fuelInstance = Instantiate(fuelPrefab);
                fuelInstance.transform.SetParent(transform);
                fuelInstance.transform.position = new Vector2(
                Random.Range(-horizontalLimit, horizontalLimit),
                player.transform.position.y + Screen.height / 100f);
            }

            fuel -= Time.deltaTime * fuelDecreaseSpeed;
            fuelText.text = "Fuel: " + (int)fuel;
            // Controla a cor do medidor de combustivel de acordo com o nível
            if (fuel >= 50 && fuel <= 100)
            {
                fuelText.color = Color.Lerp(Color.yellow, Color.green, (fuel - 50) / 50f); // muda a cor de verde para amarelo 
            } else if (fuel >= 20 && fuel < 50)
            {
                fuelText.color = Color.Lerp(Color.red, Color.yellow, fuel / 30f); // muda a cor de amarelo para vermelho 
            }
            if(fuel < 20)
            {
                if ((int)fuel % 2 == 0){
                    fuelText.color = Color.red;
                } else {
                    fuelText.color = Color.yellow;
                }
            }
            
            if (fuel <= 0)
            {
                fuelText.text = "Fuel: 0";
                Destroy(player.gameObject);
            }
            //Ativa a cena do boss
            /*if (score >= 200)
            {
                //gameMusic.Stop();
                SceneManager.LoadScene("Boss");
            }*/
        }
        else
        {
            restartTimer -= Time.deltaTime;
            if (restartTimer <= 0f)
            {
                //SceneManager.LoadScene("Game");
                SceneManager.LoadScene("GameOver");
            }
        }
        // Delete enemies.
        foreach (Enemy enemy in GetComponentsInChildren<Enemy>())
        {
            if (gameCamera.transform.position.y - enemy.transform.position.y > Screen.height / 100f)
            {
                enemy.gameObject.SetActive(false);
                //Destroy(enemy.gameObject);
            }

        }

        foreach (EnemyH enemyH in GetComponentsInChildren<EnemyH>())
        {
            if (gameCamera.transform.position.y - enemyH.transform.position.y > Screen.height / 100f)
            {
                enemyH.gameObject.SetActive(false);
            }
        }
    }

    void OnEnemyKill()
    {
        //score += 25;
        scoreText.text = "Score: " + score;
    }

    void OnFuel()
    {
        fuel = 100f;
    }

    // Atualiza o score quando o player destrói um inimigo
    void OnEnemyDestroy()
    {
        score += 25;
        scoreText.text = "Score: " + score;
    }
}
