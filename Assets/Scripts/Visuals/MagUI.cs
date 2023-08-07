using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI magazineBulletCountText;
    [SerializeField]
    private Image[] bulletImages;
    private float bulletAlphaSpeedMultiplier = 4f;

    int bulletsShot = 0;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.25f);
        Shoot.OnSuccessfulShoot += Shoot_OnSuccessfulShoot;
        Shoot.OnSuccessfulReload += Shoot_OnSuccessfulReload;

        int bullets = GameManager.Instance.GetPlayerReference().GetComponentInChildren<Shoot>().currentBulletNum;
        magazineBulletCountText.text = bullets.ToString() + "/12";
        bulletsShot = 12 - bullets;
        HideShotBulletsOnGameLoad();
    }

    private void OnDestroy()
    {
        Shoot.OnSuccessfulShoot -= Shoot_OnSuccessfulShoot;
        Shoot.OnSuccessfulReload -= Shoot_OnSuccessfulReload;
    }

    private void HideShotBulletsOnGameLoad()
    {
        for (int i = 0; i < bulletsShot; i++)
        {
            Color temp = bulletImages[i].color;
            temp.a = 0;
            bulletImages[i].color = temp;
        }
    }


    private void Shoot_OnSuccessfulReload(int magSize)
    {
        magazineBulletCountText.text = magSize + "/" + magSize;

        StopAllCoroutines();

        RepopulateMag(magSize);
        bulletsShot = 0;
    }

    private void Shoot_OnSuccessfulShoot(int bullets, int magSize)
    {
        magazineBulletCountText.text = bullets + "/" + magSize;
        StartCoroutine(HideBullet(magSize - bullets - 1));
        bulletsShot++;
    }

    IEnumerator HideBullet(int bulletImageIndex)
    {
        Color temp = bulletImages[bulletImageIndex].color;
        for (float i = 1; i > 0; i -= Time.deltaTime * bulletAlphaSpeedMultiplier)
        {
            temp = bulletImages[bulletImageIndex].color;
            temp.a = i;
            bulletImages[bulletImageIndex].color = temp;
            yield return null;
        }
        temp.a = 0;
        bulletImages[bulletImageIndex].color = temp;
    }

    IEnumerator ShowBullet(int bulletImageIndex)
    {
        Color temp = bulletImages[bulletImageIndex].color;
        for (float i = 0; i <= 1; i += Time.deltaTime * bulletAlphaSpeedMultiplier)
        {
            temp = bulletImages[bulletImageIndex].color;
            temp.a = i;
            bulletImages[bulletImageIndex].color = temp;
            yield return null;
        }
        temp.a = 1;
        bulletImages[bulletImageIndex].color = temp;
    }

    private void RepopulateMag(int magSize)
    {
        for (int i = 0; i < magSize - bulletsShot; i++)
        {
            Color temp = bulletImages[i].color;
            temp.a = 1;
            bulletImages[i].color = temp;
        }
        for (int i = magSize - bulletsShot; i < magSize; i++)
        {
            StartCoroutine(ShowBullet(i));
        }
    }
}
