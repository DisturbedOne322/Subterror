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
    private float bulletAlphaModifierSpeed = 0.05f;

    private Shoot shoot;

    int bulletsShot = 0;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        Shoot.OnSuccessfulShoot += Shoot_OnSuccessfulShoot;
        Shoot.OnSuccessfulReload += Shoot_OnSuccessfulReload;
        shoot = GameManager.Instance.GetPlayerReference().GetComponentInChildren<Shoot>();
        magazineBulletCountText.text = GameManager.Instance.GetPlayerReference().GetComponentInChildren<Shoot>().currentBulletNum.ToString() + "/12";
        bulletsShot = 12 - shoot.currentBulletNum;
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
        for (float i = 1; i > 0; i -= 0.1f)
        {
            Color temp = bulletImages[bulletImageIndex].color;
            temp.a = i;
            bulletImages[bulletImageIndex].color = temp;
            yield return new WaitForSeconds(bulletAlphaModifierSpeed);
        }
    }

    IEnumerator ShowBullet(int bulletImageIndex)
    {
        for (float i = 0; i <= 1; i += 0.1f)
        {
            Color temp = bulletImages[bulletImageIndex].color;
            temp.a = i;
            bulletImages[bulletImageIndex].color = temp;
            yield return new WaitForSeconds(bulletAlphaModifierSpeed);
        }
    }

    private void RepopulateMag(int magSize)
    {
        //enable all the bullets that are left in the mag (reposition them to the bottom of mag)
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
