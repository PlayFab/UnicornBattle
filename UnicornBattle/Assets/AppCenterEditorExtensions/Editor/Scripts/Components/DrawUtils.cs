using UnityEngine;
using System;

public class DrawUtils {

    private static Color32[] rotateSquare(Color32[] arr, double phi, Texture2D originTexture)
    {
        int x;
        int y;
        int i;
        int j;
        double sn = Math.Sin(phi);
        double cs = Math.Cos(phi);
        Color32[] arr2 = originTexture.GetPixels32();
        int W = originTexture.width;
        int H = originTexture.height;
        int xc = W / 2;
        int yc = H / 2;
        for (j = 0; j < H; j++)
        {
            for (i = 0; i < W; i++)
            {
                arr2[j * W + i] = new Color32(0, 0, 0, 0);
                x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);
                if ((x > -1) && (x < W) && (y > -1) && (y < H))
                {
                    arr2[j * W + i] = arr[y * W + x];
                }
            }
        }
        return arr2;
    }

    public static Texture2D RotateImage(Texture2D originTexture, int angle)
    {
        Texture2D result;
        result = new Texture2D(originTexture.width, originTexture.height);
        Color32[] pix1 = result.GetPixels32();
        Color32[] pix2 = originTexture.GetPixels32();
        int W = originTexture.width;
        int H = originTexture.height;
        int x = 0;
        int y = 0;
        Color32[] pix3 = rotateSquare(pix2, (Math.PI / 180 * (double)angle), originTexture);
        for (int j = 0; j < H; j++)
        {
            for (var i = 0; i < W; i++)
            {
                pix1[result.width / 2 - W / 2 + x + i + result.width * (result.height / 2 - H / 2 + j + y)] = pix3[i + j * W];
            }
        }
        result.SetPixels32(pix1);
        result.Apply();
        return result;
    }
}
