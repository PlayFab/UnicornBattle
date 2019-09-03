using System;
using System.Collections;
using System.Collections.Generic;
using UnicornBattle.Controllers;
using UnityEngine;
using UnityEngine.UI;

public class FX_Placement : MonoBehaviour
{
    public Camera FX_Cam;
    public Vector2 clickPos = Vector2.zero;
    public Text txtPos;
    public List<GameObject> effects = new List<GameObject>();

    public static Dictionary<string, GameObject> fx_lookup = new Dictionary<string, GameObject>();

    void Start()
    {
        foreach (var fx in effects)
            if (!fx_lookup.ContainsKey(fx.name))
                fx_lookup.Add(fx.name, fx);
    }

    public static IEnumerator FxHereAndFire(Vector3 here, GameObject prefab, float delay = 0)
    {
        if (prefab == null || here == Vector3.zero)
            yield break;

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        var fx = CFX_SpawnSystem.GetNextObject(prefab, false);

        fx.transform.position = here;
        fx.gameObject.SetActive(true);
    }

    public void TestFx(GameplayEnemyController enemy, string fxName = null)
    {
        Vector3 center = enemy.myRT.rect.center;

        int rng = Mathf.FloorToInt(UnityEngine.Random.Range(5, 25));
        //Vector3 worldPos = FX_Cam.ScreenToWorldPoint(new Vector3(center.x, center.y, rng));

        var worldPos = enemy.myRT.TransformPoint(new Vector3(center.x, center.y + (enemy.myRT.offsetMin.y * .40f), rng));

        Func<GameObject> GetRadomFX = () =>
        {
            int rnd = UnityEngine.Random.Range(0, effects.Count);
            return CFX_SpawnSystem.GetNextObject(effects[rnd], false);
        };

        GameObject fx;
        if (fxName == null)
            fx = GetRadomFX();
        else if (fx_lookup.ContainsKey(fxName))
            fx = CFX_SpawnSystem.GetNextObject(fx_lookup[fxName], false);
        else
            fx = GetRadomFX();
        fx.transform.position = FX_Cam.transform.position + new Vector3(worldPos.x, worldPos.y, 5f);

        fx.SetActive(true);
    }

    // will eventually need spell sounds?
    // will need combo or not effects.
    public void PlayerTakesDamage(PlayerUIEffectsController player, string fxName = null)
    {
        var lifeRect = player.LifeBar.fillBG.gameObject.GetComponent<RectTransform>();
        var center = lifeRect.rect.center;
        var worldPos = lifeRect.TransformPoint(new Vector3(lifeRect.rect.xMax, center.y, 0));
        GameObject fx;

        Func<GameObject> GetRadomFX = () =>
        {
            int rnd = UnityEngine.Random.Range(0, effects.Count);
            //Debug.Log(""+rnd);
            return CFX_SpawnSystem.GetNextObject(effects[rnd], false);
        };

        if (fxName == null)
            fx = GetRadomFX();
        else if (fx_lookup.ContainsKey(fxName))
            fx = CFX_SpawnSystem.GetNextObject(fx_lookup[fxName], false);
        else
            fx = GetRadomFX();

        fx.transform.position = new Vector3(FX_Cam.transform.position.x + worldPos.x, FX_Cam.transform.position.y + worldPos.y, -23f);
        fx.SetActive(true);
    }
}