using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZenvaVR;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour
{
    public Player player;
    public Boss boss;
    public GameObject bossPrefab;
    private bool isOnBossScene = false;
    public GameObject enemyPrefab;
    public GameObject enemyHPrefab;
    public float enemySpawnInterval = 1f;
    public float horizontalLimit = 2.8f;
    private float enemySpawnTimer;
    private int score;
    private float fuel = 100f;
    private int bossHealth;
    public GameObject gameCamera;
    public Text scoreText;
    public Text fuelText;
    public Text BossHealthText;
    public float fuelDecreaseSpeed = 5f;
    public GameObject fuelPrefab;
    public float fuelSpawnInterval = 9f;
    private float fuelSpawnTimer;
    private float restartTimer = 1f;
    public AudioSource gameMusic;
    public AudioSource fuelAlertSource;
    public AudioClip fuelAlertSound;
    public float BossDist = 2;
    public float BossSpeed;
    private float angle = 0;
    private float BossAproach = 5;

    // Start is called before the first frame update
    void Start()
    {
        enemySpawnTimer = enemySpawnInterval;
        player.OnFuel += OnFuel;
        fuelSpawnTimer = Random.Range(0f, fuelSpawnInterval);
        fuelAlertSource = GetComponent<AudioSource>();
        BossHealthText.text = ""; // inicializa o texto da vida do boss como vazio
        gameMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // loop principal do jogo
        if (player != null)
        {
            enemySpawnTimer -= Time.deltaTime;
            if (enemySpawnTimer <= 0 && !isOnBossScene)
            {
                enemySpawnTimer = enemySpawnInterval;
                GameObject enemyInstance = Instantiate(enemyPrefab);
                enemyInstance.transform.SetParent(transform);
                enemyInstance.transform.position = new Vector2(Random.Range(-horizontalLimit, horizontalLimit), player.transform.position.y + Screen.height / 100f);
                enemyInstance.GetComponent<Enemy>().OnAddScore += OnEnemyDestroy; // atualiza a pontuação do player ao destruir um inimigo

                GameObject enemyHInstance = Instantiate(enemyHPrefab);
                enemyHInstance.transform.SetParent(transform);
                enemyHInstance.transform.position = new Vector2(Random.Range(-horizontalLimit, horizontalLimit), player.transform.position.y + Screen.height / 100f);
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
            }
            else if (fuel >= 20 && fuel < 50)
            {
                fuelText.color = Color.Lerp(Color.red, Color.yellow, fuel / 30f); // muda a cor de amarelo para vermelho 
            }
            if (fuel < 20)
            {
                if ((int)fuel % 2 == 0)
                {
                    fuelText.color = Color.red;
                    fuelAlertSource.PlayOneShot(fuelAlertSound);
                }
                else
                {
                    fuelText.color = Color.yellow;
                }
            }

            if (fuel <= 0)
            {
                fuelText.text = "Fuel: 0";
                Destroy(player.gameObject);
            }
            //Ativa a cena do boss
            if (score >= 500)
            {
                bossHealth = boss.GetBossHealth();
                BossHealthText.text = "Boss: " + ((int)bossHealth * 10);
                BossHealthText.color = Color.Lerp(Color.blue, Color.red, bossHealth / 10f);
                if (boss != null)
                {
                    //gameMusic.Stop();
                    boss.gameObject.SetActive(true);
                    if (isOnBossScene)
                    { // introdução do boss na cena
                        if(player.transform.position.y + BossAproach > player.transform.position.y + BossDist){
                            boss.transform.position = new Vector2(Mathf.Sin(angle) * horizontalLimit, player.transform.position.y + BossAproach);
                            BossAproach = BossAproach - 0.01f;    
                            angle += BossSpeed;
                        }
                        else{// movimenta o boss
                            boss.transform.position = new Vector2(Mathf.Sin(angle) * horizontalLimit, player.transform.position.y + BossDist);
                            angle += BossSpeed;
                        }
                    }
                    isOnBossScene = true; // para de instanciar os inimigos
                } else { // acionado quando o boss morre
                    restartTimer -= Time.deltaTime;
                    if (restartTimer <= 0f)
                    {
                        gameMusic.Stop();
                        SceneManager.LoadScene("GameEnding");
                    }
                }

            }
        }
        else // quando o player morre, carrega a cena de game over
        {
            restartTimer -= Time.deltaTime;
            if (restartTimer <= 0f)
            {
                SceneManager.LoadScene("GameOver");
            }
        }
        // Delete enemies.
        foreach (Enemy enemy in GetComponentsInChildren<Enemy>())
        {
            if (gameCamera.transform.position.y - enemy.transform.position.y > Screen.height / 100f)
            {
                enemy.gameObject.SetActive(false);
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

    // Enche o tanque de combustivel ao pegar o combustivel
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
