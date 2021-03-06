﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class savebtn : MonoBehaviour
{

    public com_pixelEdit edit;
    // Use this for
    public UnityEngine.UI.InputField input;
    static System.Security.Cryptography.SHA1 sha1 = new System.Security.Cryptography.SHA1Managed();
    void Start()
    {
        this.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
            // () =>
            //{
            //    List<byte> bytes = new List<byte>();
            //    var pc = edit.palette.GetPixels32(0);
            //    var ic = edit.edit.GetPixels32(0);
            //    Dictionary<byte, Color32> usep = new Dictionary<byte, Color32>();
            //    bytes.Add((byte)edit.edit.width);
            //    bytes.Add((byte)edit.edit.height);

           //    for (int i = 0; i < ic.Length; i++)
            //    {
            //        byte ind= ic[i].a;
            //        bytes.Add(ind);

           //        if(ind>0)
            //        {
            //            if(usep.ContainsKey(ind)==false)
            //            {
            //                usep.Add(ind,pc[ind]);
            //            }
            //        }
            //    }
            //    bytes.Add((byte)usep.Count);
            //    foreach(var up in usep)
            //    {
            //        bytes.Add(up.Key);
            //        bytes.Add(up.Value.r);
            //        bytes.Add(up.Value.g);
            //        bytes.Add(up.Value.b);
            //    }

           //    var s =           LZMAHelper.Compress(new System.IO.MemoryStream(bytes.ToArray()), (uint)bytes.Count);
            //    byte[] nb = new byte[s.Length];
            //    s.Read(nb, 0, nb.Length);
            //    string str = System.Convert.ToBase64String(nb);
            //    string str2 = System.Uri.EscapeDataString(str);
            //    input.value = str2;
            //    Debug.Log(input.value.Length);
            //}
           () =>
           {
               var pc = edit.palette.GetPixels32(0);
               var ic = edit.edit.GetPixels32(0);
               Texture2D tout = new Texture2D(edit.edit.width, edit.edit.height, TextureFormat.ARGB32, false);
               Color32[] cout = new Color32[ic.Length];
               for (int i = 0; i < ic.Length; i++)
               {
                   cout[i] = pc[ic[i].a];
               }
               tout.SetPixels32(cout, 0);
               tout.Apply();
               byte[] src = tout.EncodeToPNG();
               string hash = System.Uri.EscapeDataString(System.Convert.ToBase64String(sha1.ComputeHash(src)));
               string file = System.Uri.EscapeDataString("test.png");
               string resp = System.Uri.EscapeDataString("game01");
               string len = src.Length.ToString();
               string url = "http://localhost:25080/filepost?d=" + resp + "&f=" + file + "&h=" + hash + "&l=" + len;
               www = new WWW(url, src);
               Debug.Log("url="+url);
           }
           );
        input.onSubmit.AddListener((str) =>
        {
            ReadByte(str, edit);

        });
    }
    WWW www;
    public static void ReadByte(string scode, com_pixelEdit edit)
    {
        try
        {
            var src = edit.edit;
            var p = edit.palette;
            string strbase64 = System.Uri.UnescapeDataString(scode);
            byte[] bb = System.Convert.FromBase64String(strbase64);
            var s = LZMAHelper.DeCompress(new System.IO.MemoryStream(bb), (uint)bb.Length);
            bb = new byte[s.Length];
            s.Read(bb, 0, bb.Length);

            int seek = 0;
            int width = bb[seek]; seek++;
            if (width == 0) width = 256;
            int height = bb[seek]; seek++;
            if (height == 0) height = 256;
            Debug.Log("w=" + width + ",h=" + height);
            edit.Resize(width, height);
            Color32[] pp = src.GetPixels32(0);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte ind = bb[seek]; seek++;
                    pp[y * width + x].a = ind;
                }
            }
            src.SetPixels32(pp, 0);
            src.Apply();
            Color32[] ppp = p.GetPixels32(0);
            int c = bb[seek]; seek++;
            for (int i = 0; i < c; i++)
            {
                byte inde = bb[seek]; seek++;
                byte r = bb[seek]; seek++;
                byte g = bb[seek]; seek++;
                byte b = bb[seek]; seek++;
                ppp[inde].r = r;
                ppp[inde].g = g;
                ppp[inde].b = b;

            }
            p.SetPixels32(ppp);
            p.Apply();
        }
        catch
        {

        }
    }
    // Update is called once per frame
    void Update()
    {
        if (www != null)
        {
            if (www.isDone)
            {
                www = null;
                Debug.Log("done.");
            }
        }
    }
}
